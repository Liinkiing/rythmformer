using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Serializable]
    public enum PlayerActions
    {
        Jump,
        Dash
    }

    [Serializable]
    private struct PlayerFlags
    {
        public bool CanDash;
        public bool ActionAvailable;
    }

    [Serializable]
    private struct ScoreState
    {
        public SongSynchronizer.EventScore Score;

        public ScoreState(SongSynchronizer.EventScore score)
        {
            Score = score;
        }
    }

    [Serializable]
    private struct Ray
    {
        [SerializeField] public Transform start;
        [SerializeField] public Vector2 direction;
    }

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

    [SerializeField, Tooltip("Horizontal speed on wall jump")]
    private float wallJumpSpeed = 12;

    [SerializeField, Tooltip("Time given to give direction after Dash")]
    private float dashBuffer = 0.1f;

    [SerializeField, Range(0, 2f), Tooltip("Horizontal to Vertical speed transformation speed modifier")]
    private float horizontalSpeedTransfer = 0.5f;

    [Space(), Header("Dash")] [SerializeField]
    private float dashDuration = 0.15f;

    [SerializeField] private float dashSpeed = 60f;
    [SerializeField] private float dashAcceleration = 500f;

    [Space(), Header("Wall Riding")] [SerializeField]
    private bool drawDebugRays = true;

    [SerializeField, Range(0.1f, 3f)] private float surfaceRayLength = 0.6f;

    [SerializeField, Range(0, 1f), Tooltip("Deceleration applied when character is wall riding")]
    private float wallDeceleration = 0.8f;

    [SerializeField] private List<Ray> leftWallCheck;
    [SerializeField] private List<Ray> rightWallCheck;
    [SerializeField] private List<Ray> groundCheck;
    [SerializeField] private LayerMask wallsLayerMask;

    [Space(), Header("Action restrictions")] [SerializeField]
    private SongSynchronizer.ThresholdBeatEvents restrictActionOn;

    [Space(), Header("Events")] public UnityEvent OnJump;
    public UnityEvent OnDash;

    private BoxCollider2D _boxCollider;
    private float _dashTime;
    private bool _dashing;
    private PlayerInput _input;
    private Vector2 _velocity;
    private bool _grounded;
    private bool _wallRiding;
    private PlayerFlags _flags;
    private SongSynchronizer _synchronizer;
    private Rigidbody2D _rigidbody;
    private Vector3 _upVect;
    private int _direction = 1;
    private int _wall;
    private bool _groundBuffer;
    private float _dashBuffer;
    private readonly Collider2D[] _hitsBuffer = new Collider2D[16];
    private LevelManager _levelManager;
    private Vector3 _initialPosition = Vector3.zero;

    private ScoreState _scoreState = new ScoreState(score: SongSynchronizer.EventScore.Ok);
    [SerializeField] private ParticleSystem _trailPS;
    [SerializeField] private ParticleSystem _dustPS;
    [SerializeField] private ParticleSystem _leaves;

    #endregion

    #region Unity Events

#if UNITY_EDITOR
    private void OnValidate()
    {
        ResetThresholdBeatEvents();
    }
#endif

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _dashTime = dashDuration;
        _dashing = false;
        _rigidbody = GetComponent<Rigidbody2D>();
        _input = new PlayerInput();
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _trailPS = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _initialPosition = transform.position;
    }

    private void OnEnable()
    {
        _input?.Enable();
        _levelManager?.OnLevelReset.AddListener(OnLevelReset);
        AddThresholdedBeatEvents();
    }


    private void OnDisable()
    {
        _input?.Disable();
        _levelManager?.OnLevelReset.RemoveListener(OnLevelReset);
        RemoveThresholdedBeatEvents();
    }

    private void Update()
    {
        Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();

        if (moveInput.x < 0)
        {
            _direction = -1;
        }
        else if (moveInput.x > 0)
        {
            _direction = 1;
        }
        else
        {
            _direction = 0;
        }

        SurfaceDetection();
        HandleMovement(moveInput.x);
        HandleRythmAction(moveInput);
        ResolveDash();
        ResolveTimeBuffers(moveInput);
    }

    private void SurfaceDetection()
    {
        if (leftWallCheck.Any(ray =>
            null != Physics2D.Raycast(ray.start.position, ray.direction, surfaceRayLength, wallsLayerMask).collider))
        {
            _wall = 1;
            _flags.CanDash = true;
        }
        else if (rightWallCheck.Any(ray =>
            null != Physics2D.Raycast(ray.start.position, ray.direction, surfaceRayLength, wallsLayerMask).collider))
        {
            _wall = -1;
            _flags.CanDash = true;
        }
        else
        {
            _wall = 0;
        }

        if (groundCheck.Any(ray =>
            null != Physics2D.Raycast(ray.start.position, ray.direction, surfaceRayLength, wallsLayerMask).collider))
        {
            _groundBuffer = true;
            _flags.CanDash = true;
        }
        else
        {
            _groundBuffer = false;
        }
    }

    private void HandleRythmAction(Vector2 moveInput)
    {
        if (!_flags.ActionAvailable)
        {
            if (_input.Player.Jump.triggered || _input.Player.Dash.triggered)
            {
                var action = _input.Player.Jump.triggered ? PlayerActions.Jump : PlayerActions.Dash;
                OnActionPerformed(this,
                    new OnActionEventArgs() {Move = action, Score = SongSynchronizer.EventScore.Failed});
            }

            return;
        }

        if (_input.Player.Jump.triggered)
        {
            _flags.ActionAvailable = false;
            if (_groundBuffer || _wall != 0 || !_groundBuffer && _wall == 0 && _flags.CanDash)
            {
                if (!_groundBuffer && _wall == 0 && _flags.CanDash)
                {
                    _flags.CanDash = false;
                }

                Jump();
            }
        }
        else if (_input.Player.Dash.triggered && _flags.CanDash)
        {
            _flags.ActionAvailable = false;
            if (Mathf.Abs(moveInput.x) > 0)
            {
                Dash(moveInput);
            }
            else
            {
                _dashBuffer = dashBuffer;
            }
        }
    }

    private void ResolveDash()
    {
        if (_dashing)
        {
            if (_dashTime <= 0)
            {
                _dashTime = dashDuration;
                _velocity = Vector2.zero;
                _dashing = false;
            }
            else
            {
                _dashTime -= Time.deltaTime;
            }
        }
    }

    private void ResolveTimeBuffers(Vector2 moveInput)
    {
        if (_dashBuffer > 0)
        {
            if (Mathf.Abs(moveInput.x) > 0)
            {
                _dashBuffer = 0;
                Dash(moveInput);
                return;
            }

            _dashBuffer -= Time.deltaTime;
        }
    }

    private void HandleMovement(float moveInput)
    {
        if (_wallRiding && !_grounded && _velocity.y < 0)
        {
            _velocity.y *= wallDeceleration;
        }

        var acceleration = _grounded || _wallRiding ? walkAcceleration : airAcceleration;
        var deceleration = _grounded ? groundDeceleration : 0;
        
        if (Mathf.Abs(moveInput) > 0)
        {
            if (!_leaves.isEmitting)
            {
                _leaves.Play();    
            }
            
            _velocity.x = Mathf.MoveTowards(_velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            _leaves.Stop();
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
        }

        if (!_dashing && !_grounded)
        {
            _velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        var raycastGround = Physics2D.Raycast(_rigidbody.position, -Vector2.up, _boxCollider.size.y, wallsLayerMask);

        var slopeAngle = Vector2.Angle(raycastGround.normal, Vector2.up);

        if (raycastGround)
        {
            // We handle sticky physics for slope smaller than 45 deg 
            if (slopeAngle < 45)
            {
                _upVect = raycastGround.normal;
            }
        }

        // Sticky physic is only use if the player is grounded and not on flat ground
        // because it cause grounded check issue otherwise
        if (_grounded && !_dashing && slopeAngle > 0)
        {
            _velocity = Vector3.Cross(_upVect, Vector3.forward) * (moveInput * speed);
        }

        transform.Translate(_velocity * Time.deltaTime);

        // Retrieve all colliders we have intersected after velocity has been applied.
        var count = Physics2D.OverlapBoxNonAlloc(transform.position, _boxCollider.size, 0, _hitsBuffer);
        if (count == 1)
        {
            _grounded = false;
            _wallRiding = false;
        }

        for (var i = 0; i < count; i++)
        {
            // Ignore our own collider.
            if (_hitsBuffer[i] == _boxCollider || _hitsBuffer[i].isTrigger)
                continue;

            ColliderDistance2D colliderDistance = _hitsBuffer[i].Distance(_boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90)
                {
                    _grounded = true;
                    _velocity.y = 0;
                }

                // If we intersect an object above us, we push down the play. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 180 && !_grounded)
                {
                    _velocity.y += Physics2D.gravity.y * 10f * Time.deltaTime;
                }

                // If we intersect an object in our sides, we are wall riding. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 90 && !_grounded)
                {
                    if (Math.Abs(_velocity.x) > 0 && _velocity.y > 0 && !_wallRiding)
                    {
                        _velocity.y *= (1 + Mathf.Abs(_velocity.x) * horizontalSpeedTransfer);
                    }

                    _velocity.x = 0;
                    _wallRiding = true;
                }
                else
                {
                    _wallRiding = false;
                }
            }
        }

        if (drawDebugRays)
        {
            var rayLists = new List<List<Ray>> {leftWallCheck, rightWallCheck, groundCheck};
            foreach (var rayList in rayLists)
            {
                foreach (var ray in rayList)
                {
                    Debug.DrawRay(ray.start.position, ray.direction * surfaceRayLength, Color.magenta);
                }
            }
        }
    }

    #endregion

    #region Events

    public struct OnActionEventArgs
    {
        public SongSynchronizer.EventScore Score;
        public PlayerActions Move;
    }

    public event Action<CharacterController2D, OnActionEventArgs> ActionPerformed;

    private void OnLevelReset()
    {
        transform.position = _initialPosition;
    }

    #endregion

    #region Methods

    private void AddThresholdedBeatEvents()
    {
        if (_synchronizer == null) return;
        switch (restrictActionOn)
        {
            case SongSynchronizer.ThresholdBeatEvents.Step:
                _synchronizer.StepThresholded += OnTresholdedAction;
                break;
            case SongSynchronizer.ThresholdBeatEvents.HalfBeat:
                _synchronizer.HalfBeatThresholded += OnTresholdedAction;
                break;
            case SongSynchronizer.ThresholdBeatEvents.Beat:
                _synchronizer.BeatThresholded += OnTresholdedAction;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RemoveThresholdedBeatEvents()
    {
        if (_synchronizer == null) return;
        _synchronizer.StepThresholded -= OnTresholdedAction;
        _synchronizer.HalfBeatThresholded -= OnTresholdedAction;
        _synchronizer.BeatThresholded -= OnTresholdedAction;
    }

    private void ResetThresholdBeatEvents()
    {
        RemoveThresholdedBeatEvents();
        AddThresholdedBeatEvents();
    }

    private void OnTresholdedAction(SongSynchronizer sender, SongSynchronizer.EventState state)
    {
        switch (state)
        {
            case SongSynchronizer.EventState.Start:
                _flags.ActionAvailable = true;
                _scoreState.Score = SongSynchronizer.EventScore.Ok;
                break;
            case SongSynchronizer.EventState.Mid:
                _scoreState.Score = SongSynchronizer.EventScore.Perfect;
                break;
            case SongSynchronizer.EventState.End:
                _flags.ActionAvailable = false;
                _scoreState.Score = SongSynchronizer.EventScore.Ok;
                break;
        }
    }

    private void Jump()
    {
        OnActionPerformed(this, new OnActionEventArgs() {Move = PlayerActions.Jump, Score = _scoreState.Score});
        // Calculate the velocity required to achieve the target jump height.

        _velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        if (_wall != 0 && !_grounded)
        {
            _velocity.x = _wall * wallJumpSpeed;
        }

        _grounded = false;
        OnJump?.Invoke();
    }

    private void Dash(Vector2 moveInput)
    {
        OnActionPerformed(this, new OnActionEventArgs() {Move = PlayerActions.Dash, Score = _scoreState.Score});
        OnDash?.Invoke();
        _dashing = true;
        _velocity.x = Mathf.MoveTowards(_velocity.x, dashSpeed * _direction, dashAcceleration);
        _flags.CanDash = false;
        _trailPS.Play();
    }

    #endregion

    #region Public Methods

    public void ChangeActionTresholdBeat(SongSynchronizer.ThresholdBeatEvents restrictOn)
    {
        restrictActionOn = restrictOn;
        ResetThresholdBeatEvents();
    }

    #endregion

    protected virtual void OnActionPerformed(CharacterController2D sender, OnActionEventArgs action)
    {
        ActionPerformed?.Invoke(sender, action);
    }
}