using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rythmformer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = System.Random;

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
        public bool CanJump;
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

    [Space(), Header("Wall Riding")]
    [SerializeField, Range(0.1f, 3f)] private float surfaceRayLength = 0.6f;

    [SerializeField, Range(0, 1f), Tooltip("Deceleration applied when character is wall riding")]
    private float wallDeceleration = 0.8f;

    [Space(), Header("Speed incrementation")] [SerializeField, Tooltip("Number of times maximum speed can increase")]
    private int numberOfSteps = 1;

    [SerializeField, Tooltip("Maximum additional speed to gain by performing rythm actions")]
    private float maxAdditionalSpeed = 5;

    [SerializeField] private LayerMask wallsLayerMask;

    [Space(), Header("Action restrictions")] [SerializeField]
    private SongSynchronizer.ThresholdBeatEvents restrictActionOn;

    [Space(), Header("Events")] public UnityEvent OnJump;
    public UnityEvent OnDash;

    [Space(), Header("Variables")] [SerializeField]
    private Transform art;

    private BoxCollider2D _boxCollider;
    private DoubleCast _upCast;
    private DoubleCast _rightCast;
    private DoubleCast _downCast;
    private DoubleCast _leftCast;
    private DoubleCast _yCast;
    private DoubleCast _xCast;
    private Vector3 _initialLocalScale;
    private float _dashTime;
    private bool _dashing;
    private bool _isFlipped;
    private PlayerInput _input;
    private Vector2 _velocity;
    private bool _grounded;
    private bool _roofed;
    private bool _wallRiding;
    private PlayerFlags _flags;
    private SongSynchronizer _synchronizer;
    private Rigidbody2D _rigidbody;
    private Vector3 _upVect;
    private int _direction = 1;
    private int _wall;
    private bool _groundBuffer;
    private float _dashBuffer;
    private int _additionalSpeed;
    private readonly Collider2D[] _hitsBuffer = new Collider2D[16];
    private LevelManager _levelManager;
    private Vector3 _initialPosition = Vector3.zero;
    private Animator _artAnimator;

    private ScoreState _scoreState = new ScoreState(score: SongSynchronizer.EventScore.Ok);
    [SerializeField] private ParticleSystem _trailPS;
    [SerializeField] private ParticleSystem _dustPS;
    [SerializeField] private ParticleSystem _leaves;
    private RippleController _rippleController;

    public enum FootstepFX
    {
        Dust,
        Leaves
    }

    public FootstepFX selectedFootstepFx;
    private static readonly int JumpAnimatorTrigger = Animator.StringToHash("Jump");
    private static readonly int GroundedAnimatorTrigger = Animator.StringToHash("Grounded");
    private static readonly int SpeedFloat = Animator.StringToHash("Speed");
    private static readonly int VelocityYFloat = Animator.StringToHash("VelocityY");
    private static readonly int SpeedMultiplierFloat = Animator.StringToHash("SpeedMultiplier");
    private static readonly int IdleMultiplierFloat = Animator.StringToHash("IdleMultiplier");
    private static readonly int DashAnimatorTrigger = Animator.StringToHash("Dash");
    private static readonly int WallridingAnimatorBool = Animator.StringToHash("Wallriding");

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
        _initialLocalScale = art.localScale;
        _artAnimator = art.GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _dashTime = dashDuration;
        _dashing = false;
        _rigidbody = GetComponent<Rigidbody2D>();
        _input = new PlayerInput();
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _artAnimator.SetFloat(IdleMultiplierFloat, _synchronizer.song.Informations.bpm / 120.00f);
        _rippleController = GameObject.Find("PostProcessing").GetComponent<RippleController>();
        _upCast = new DoubleCast(_boxCollider, Vector2.up, surfaceRayLength, new Vector2(0.5f, 1), wallsLayerMask);
        _rightCast = new DoubleCast(_boxCollider, Vector2.right, surfaceRayLength, new Vector2(1, 0.5f), wallsLayerMask);
        _downCast = new DoubleCast(_boxCollider, Vector2.down, surfaceRayLength, new Vector2(0.5f, 1), wallsLayerMask);
        _leftCast = new DoubleCast(_boxCollider, Vector2.left, surfaceRayLength, new Vector2(1, 0.5f), wallsLayerMask);
        _yCast = new DoubleCast(_boxCollider, Vector2.zero, 0, new Vector2(1, 0.5f), wallsLayerMask);
        _xCast = new DoubleCast(_boxCollider, Vector2.zero, 0, new Vector2(0.5f, 1), wallsLayerMask);
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

        UpdateScale(moveInput.x);
        SurfaceDetection();
        HandleRythmAction(moveInput);
        HandleMovement(moveInput.x);
        ResolveDash(moveInput.x);
        ResolveTimeBuffers(moveInput);
        HandleAnimations(moveInput, _velocity.y);
    }

    private void HandleAnimations(Vector2 input, float velocityY)
    {
        _artAnimator.SetFloat(SpeedFloat, Mathf.Abs(input.x));
        _artAnimator.SetFloat(VelocityYFloat, velocityY);
        _artAnimator.SetBool(GroundedAnimatorTrigger, _grounded);
        _artAnimator.SetFloat(SpeedMultiplierFloat, _additionalSpeed == 0 ? 1f : 1f + (_additionalSpeed * 0.15f));
    }

    private void UpdateScale(float direction)
    {
        if (_wallRiding && !_grounded)
        {
            return;
        }
        if (direction > 0 && _isFlipped)
        {
            _isFlipped = false;
            art.localScale = _initialLocalScale;
        }
        else if (direction < 0 && !_isFlipped)
        {
            _isFlipped = true;
            art.localScale = new Vector3(art.localScale.x * -1, art.localScale.y, art.localScale.z);
        }
    }
    
    private void SurfaceDetection()
    {
        _grounded = _downCast.Check(_downCast.Down);
        _roofed = _upCast.Check(_upCast.Up);
        _wallRiding = _rightCast.Check(_rightCast.Right) || _leftCast.Check(_leftCast.Left);

        Vector2 s = _boxCollider.size;
        Vector2 centerPosition = transform.position;
        
        var downBufferSize = new Vector2(s.x + 2 * surfaceRayLength, s.y);
        var downBuffer = Physics2D.BoxCast(centerPosition, downBufferSize, 0, Vector2.down, Mathf.Infinity, wallsLayerMask);
        
        if (downBuffer.distance <= surfaceRayLength && downBuffer.normal.y > 0)
        {
            _groundBuffer = true;
            _flags.CanDash = true;
            _flags.CanJump = true;
        }
        else
        {
            _groundBuffer = false;
        }
        
        var sideBufferSize = new Vector2(s.x, s.y - 2 * surfaceRayLength);
        var rightBuffer = Physics2D.BoxCast(centerPosition, sideBufferSize, 0, Vector2.right, Mathf.Infinity, wallsLayerMask);
        var leftBuffer = Physics2D.BoxCast(centerPosition, sideBufferSize, 0, Vector2.left, Mathf.Infinity, wallsLayerMask);
        
        if (rightBuffer.distance <= surfaceRayLength && rightBuffer.normal.x < 0)
        {
            _wall = 1;
            _flags.CanDash = true;
            _flags.CanJump = true;
        }
        else if (leftBuffer.distance <= surfaceRayLength && rightBuffer.normal.x > 0)
        {
            _wall = -1;
            _flags.CanDash = true;
            _flags.CanJump = true;
        }
        else
        {
            _wall = 0;
        }
    }

    private void HandleRythmAction(Vector2 moveInput)
    {
        _flags.CanDash = !(moveInput.x < 0 && _wall == -1 || moveInput.x > 0 && _wall == 1);
        
        if (!_flags.ActionAvailable)
        {
            if (_input.Player.Jump.triggered || _input.Player.Dash.triggered)
            {
                var action = _input.Player.Jump.triggered ? PlayerActions.Jump : PlayerActions.Dash;
                if (action == PlayerActions.Dash && !_flags.CanDash)
                {
                    return;
                }
                OnActionPerformed(this,
                    new OnActionEventArgs() {Move = action, Score = SongSynchronizer.EventScore.Failed});
                _additionalSpeed -= numberOfSteps / 2;
                if (_additionalSpeed < 0) _additionalSpeed = 0;
            }

            return;
        }

        if (_input.Player.Jump.triggered)
        {
            _flags.ActionAvailable = false;
            if (_groundBuffer || _wall != 0 || _flags.CanJump)
            {
                if (!_groundBuffer && _wall == 0 && _flags.CanJump)
                {
                    _flags.CanJump = false;
                }

                Jump();
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
            }
        }
        else if (_input.Player.Dash.triggered && _flags.CanDash)
        {
            _flags.ActionAvailable = false;
            if (Mathf.Abs(moveInput.x) > 0)
            {
                Dash(moveInput);
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
            }
            else
            {
                _dashBuffer = dashBuffer;
            }
        }
    }

    private void ResolveDash(float moveInput)
    {
        if (_dashing)
        {
            if (_dashTime <= 0)
            {
                _dashTime = dashDuration;
                _velocity.y = 0;
                _velocity.x = (speed + _additionalSpeed * maxAdditionalSpeed / numberOfSteps) * moveInput;
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
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
                return;
            }

            _dashBuffer -= Time.deltaTime;
        }
    }

    private void HandleMovement(float moveInput)
    {
        //TODO: Move VFX to a dedicated script
        // Footstep VFX
        if (_grounded && moveInput != 0)
        {
            HandleFootstepVfx(selectedFootstepFx);
        }
        else
        {
            _leaves.Stop();
            _dustPS.Stop();
        }
        
        // Decelerate Y on wall downward
        if (_wallRiding && !_grounded && _velocity.y < 0)
        {
            _velocity.y *= wallDeceleration;
        }
        
        // Define surface resistance
        var acceleration = _grounded || _wallRiding ? walkAcceleration : airAcceleration;
        var deceleration = _grounded ? groundDeceleration : 0;

        // Apply horizontal speed depending on user input
        if (Mathf.Abs(moveInput) > 0)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x,
                (speed + _additionalSpeed * maxAdditionalSpeed / numberOfSteps) * moveInput,
                acceleration * Time.deltaTime);
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
        }

        // Apply gravity
        if (!_dashing && !_grounded)
        {
            _velocity.y += Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
        
        // Define ending point
        var pos = _velocity * Time.deltaTime;
        
        // Check if collision is expected
        var minDistance = new float[2];
        
        _yCast.Distance = pos.magnitude;
        _yCast.Direction = _velocity.normalized;
        minDistance[0] = _yCast.MinDistance();
        
        _xCast.Distance = pos.magnitude;
        _xCast.Direction = _velocity.normalized;
        minDistance[1] = _xCast.MinDistance();

        pos = _velocity.normalized * minDistance.Min();
        
        Debug.DrawRay(transform.position, pos * 15, Color.green);
        
        if (_grounded || _dashing || _roofed && pos.y > 0)
        {
            pos.y = 0;
            _velocity.y = 0;
        }

        if (_wallRiding && (_wall < 0 && pos.x < 0 || _wall > 0 && pos.x > 0))
        {
            pos.x = 0;
            _velocity.x = 0;
        }
        _artAnimator.SetBool(WallridingAnimatorBool, _wallRiding);

        if (moveInput > 0 && _wall == -1 || moveInput < 0 && _wall == 1)
        {
            _flags.CanDash = false;
        }
        
        transform.Translate(pos);
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
        // We stop ParticleSystem to reset the emitting one with new properties
        _leaves.Stop();
        _dustPS.Stop();
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
        _artAnimator.SetTrigger(JumpAnimatorTrigger);
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
        _artAnimator.SetTrigger(DashAnimatorTrigger);
        OnActionPerformed(this, new OnActionEventArgs() {Move = PlayerActions.Dash, Score = _scoreState.Score});
        OnDash?.Invoke();
        _dashing = true;
        _velocity.y = 0;
        _velocity.x = Mathf.MoveTowards(_velocity.x,
            (dashSpeed + _additionalSpeed * maxAdditionalSpeed / numberOfSteps) * moveInput.x, dashAcceleration);
        _flags.CanDash = false;
        
        _rippleController.startRipple();
        /*_trailPS.Play();*/
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

    #region VFX

    private void HandleFootstepVfx(FootstepFX selectedFootstepVfx)
    {
        if (selectedFootstepVfx == FootstepFX.Leaves && !_leaves.isEmitting)
        {
            ParticleSystem.EmissionModule leavesPsEmission = _leaves.emission;
            ParticleSystem.ShapeModule leavesPsShape = _leaves.shape;
            ParticleSystem.MainModule main = _leaves.main;

            float absoluteVelocity = Mathf.Abs(_velocity.x);

            // Change leaves amount depending on player velocity
            leavesPsEmission.rateOverTime = absoluteVelocity < speed ? 10 : absoluteVelocity.Remap(speed, 24, 10, 20);

            // Change leaves rotation emitter depending on player velocity
            float leavesPsComputedRotationY = absoluteVelocity < speed ? 0 : absoluteVelocity.Remap(speed, 24, 0, 70);
            leavesPsShape.rotation = new Vector3(0f, leavesPsComputedRotationY * Mathf.Sign(_velocity.x), 0f);

            // Change leaves speed depending on player velocity
            main.startSpeed = absoluteVelocity < speed ? 0 : absoluteVelocity.Remap(speed, 24, 3, 6);

            _leaves.Play();
        }
        else if (selectedFootstepVfx == FootstepFX.Dust && !_dustPS.isEmitting)
        {
            ParticleSystem.ShapeModule dustPsShape = _dustPS.shape;
            dustPsShape.rotation = new Vector3(0f, Mathf.Sign(_velocity.x) * -70, 0f);

            _dustPS.Play();
        }
    }

    #endregion
}