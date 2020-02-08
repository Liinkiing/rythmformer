using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D)), ExecuteInEditMode()]
public class CharacterController2D : MonoBehaviour
{
    [SerializeField, Tooltip("Max speed, in units per second, that the character moves.")]
    float speed = 9;

    [SerializeField, Tooltip("Acceleration while grounded.")]
    float walkAcceleration = 75;

    [SerializeField, Tooltip("Acceleration while in the air.")]
    float airAcceleration = 30;

    [SerializeField, Tooltip("Deceleration applied when character is grounded and not attempting to move.")]
    float groundDeceleration = 70;

    [SerializeField, Tooltip("Max height the character will jump regardless of gravity")]
    float jumpHeight = 4;

    [SerializeField, Range(0, 1f), Tooltip("Deceleration applied when character is wall riding")]
    float wallDeceleration = 0.8f;

    [Space(), Header("Checks")] public bool DrawDebugRays = true;
    [Range(0.1f, 1f)] public float WallsRayLength = 0.6f;
    public Transform LeftWallCheck;
    public Transform RightWallCheck;
    public LayerMask WallsLayerMask;

    [Space(), Header("Events")] public UnityEvent OnJump;

    private BoxCollider2D boxCollider;

    private const float WALLS_RAY_LENGTH = 1.3f;

    private Vector2 velocity;

    /// <summary>
    /// Set to true when the character intersects a collider beneath
    /// them in the previous frame.
    /// </summary>
    private bool grounded;

    /// <summary>
    /// Set to true when the character intersects a collider (a wall)
    /// in his left or right
    /// </summary>
    private bool wallRiding;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Jump()
    {
        // Calculate the velocity required to achieve the target jump height.
        velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        OnJump?.Invoke();
    }
    
    private void Update()
    {
        // Use GetAxisRaw to ensure our input is either 0, 1 or -1.
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (grounded)
        {
            velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
        else if (
            !grounded &&
            moveInput != 0
        )
        {
            if (Physics2D.Raycast(LeftWallCheck.position, Vector2.left, WallsRayLength, WallsLayerMask).collider !=
                null && Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            if (Physics2D.Raycast(RightWallCheck.position, Vector2.right, WallsRayLength, WallsLayerMask)
                                 .collider != null && Input.GetButtonDown("Jump"))
            {
                Jump();
            }

        }


        if (wallRiding && !grounded)
        {
            velocity.y *= wallDeceleration;
        }

        var multiplier = wallRiding ? 10 : 1;
        float acceleration = (grounded || wallRiding) ? walkAcceleration * multiplier : airAcceleration;
        float deceleration = grounded ? groundDeceleration : 0;

        if (moveInput != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, speed * moveInput, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.deltaTime);
        }

        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        grounded = false;
        wallRiding = false;

        // Retrieve all colliders we have intersected after velocity has been applied.
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);

        foreach (Collider2D hit in hits)
        {
            // Ignore our own collider.
            if (hit == boxCollider)
                continue;

            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);

            // Ensure that we are still overlapping this collider.
            // The overlap may no longer exist due to another intersected collider
            // pushing us out of this one.
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);

                // If we intersect an object beneath us, set grounded to true. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && velocity.y < 0)
                {
                    grounded = true;
                }

                // If we intersect an object above us, we push down the play. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 180 && !grounded)
                {
                    velocity.y += (Physics2D.gravity.y * 10f) * Time.deltaTime;
                }

                // If we intersect an object in our sides, we are wall riding. 
                if (Vector2.Angle(colliderDistance.normal, Vector2.up) == 90 && moveInput != 0)
                {
                    wallRiding = true;
                }
            }
        }

        if (DrawDebugRays)
        {
            Debug.DrawRay(LeftWallCheck.position, Vector3.left * WallsRayLength, Color.magenta);
            Debug.DrawRay(RightWallCheck.position, Vector3.right * WallsRayLength, Color.magenta);
        }
    }
}