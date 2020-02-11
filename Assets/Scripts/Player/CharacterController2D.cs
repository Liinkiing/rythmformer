using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour

{
    #region Fields

    [Header("Movements"), SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    private float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    private float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    private float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    private float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    private float jumpHeight = 4;

    [Space(), Header("Dash")] [SerializeField]
    private float dashDuration = 0.15f;

    [SerializeField] private float dashSpeed = 60f;
    [SerializeField] private float dashAcceleration = 500f;

    [Space(), Header("Wall Riding")] [SerializeField]
    private bool drawDebugRays = true;

    [SerializeField, Range(0.1f, 3f)] private float wallsRayLength = 0.6f;

    [SerializeField, Range(0, 1f), Tooltip("Deceleration applied when character is wall riding")]
    private float wallDeceleration = 0.8f;


    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private LayerMask wallsLayerMask;

    [Space(), Header("Events")] public UnityEvent OnJump;

    private const float WallsRayLength = 1.3f;

    private BoxCollider2D _boxCollider;
    private float _dashTime;
    private bool _dashing;
    private PlayerInput _input;
    private Vector2 _velocity;
    private bool _grounded;
    private bool _wallRiding;
    private Rigidbody2D _rigidbody;
    private Vector3 _upVect;
    private bool _jumping;

    #endregion

    #region Unity Events

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _dashTime = dashDuration;
        _dashing = false;
        _input = new PlayerInput();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _input?.Enable();
    }

    private void OnDisable()
    {
        _input?.Disable();
    }

    private void Update()
    {
        float moveInput = _input.Player.Move.ReadValue<Vector2>().x;

        HandleJump();
        HandleWallJump(moveInput);
        HandleDash(moveInput);
        HandleMovement(moveInput);
    }

    private void HandleWallJump(float direction)
    {
        if (_grounded || direction == 0) return;
        if (Physics2D.Raycast(leftWallCheck.position, Vector2.left, WallsRayLength, wallsLayerMask).collider !=
            null && _input.Player.Jump.triggered)
        {
            Jump();
        }

        if (Physics2D.Raycast(rightWallCheck.position, Vector2.right, WallsRayLength, wallsLayerMask)
                .collider != null && _input.Player.Jump.triggered)
        {
            Jump();
        }
    }

    private void HandleJump()
    {
        if (_grounded)
        {
            _velocity.y = 0;
            if (_input.Player.Jump.triggered)
            {
                Jump();
            }
        }
    }

    private void HandleDash(float direction)
    {
        if (_dashTime <= 0 && _dashing)
        {
            _dashTime = dashDuration;
            _velocity = Vector2.zero;
            _dashing = false;
        }

        if (_dashing)
        {
            _dashTime -= Time.deltaTime;
        }

        if (_input.Player.Dash.triggered)
        {
            Dash(direction);
        }
    }

    private void HandleMovement(float moveInput)
    {
        if (_wallRiding && !_grounded)
        {
            _velocity.y *= wallDeceleration;
        }

        var multiplier = _wallRiding ? 10 : 1;
        var acceleration = (_grounded || _wallRiding) ? walkAcceleration * multiplier : airAcceleration;
        var deceleration = _grounded ? groundDeceleration : 0;

        if (moveInput != 0)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (!_dashing)
        {
            _velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        var raycastGround = Physics2D.Raycast(_rigidbody.position, -Vector2.up, _boxCollider.size.y, wallsLayerMask);

        if (raycastGround)
        {
            var slopeAngle = Vector2.Angle(raycastGround.normal, Vector2.up);

            // We handle sticky physics for slope smaller than 45 deg 
            if (slopeAngle < 45)
            {
                _upVect = raycastGround.normal;
            }
        }

        if (_grounded && !_jumping && !_dashing)
        {
            _velocity = Vector3.Cross(_upVect, Vector3.forward) * (moveInput * speed);
        }
        
        transform.Translate(_velocity * Time.deltaTime);

        _grounded = false;
        _wallRiding = false;

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, _boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == _boxCollider || hit.isTrigger)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(_boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && _velocity.y < 0)
                {
                    _jumping = false;
                    _grounded = true;
                }

                // If we intersect an object above us, we push down the play. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 180 && !_grounded)
                {
                    _velocity.y += (Physics2D.gravity.y * 10f) * Time.deltaTime;
                }

                // If we intersect an object in our sides, we are wall riding. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 90 && moveInput != 0)
                {
                    _jumping = false;
                    _wallRiding = true;
                }
            }
        }

        if (drawDebugRays)
        {
            Debug.DrawRay(leftWallCheck.position, Vector3.left * wallsRayLength, Color.magenta);
            Debug.DrawRay(rightWallCheck.position, Vector3.right * wallsRayLength, Color.magenta);
        }
    }

    #endregion

    #region Methods

    private void Jump()
    {
        _jumping = true;
        // Calculate the velocity required to achieve the target jump height.
        _velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        OnJump?.Invoke();
    }

    private void Dash(float direction)
    {
        _dashing = true;
        _velocity.x = Mathf.MoveTowards(_velocity.x, dashSpeed * Mathf.Sign(direction), dashAcceleration);
        _velocity.y = 0;
    }

    #endregion
}