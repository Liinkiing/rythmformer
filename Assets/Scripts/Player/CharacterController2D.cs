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
    
    [SerializeField, Tooltip("Time during which movement keys are locked after wall jumping")]
    private float wallJumpLock;

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

    [Space(), Header("Speed incrementation")] [SerializeField, Tooltip("Number of actions to attain super speed")]
    private int numberOfSteps = 1;

    [SerializeField, Tooltip("Maximum additional speed to gain by performing rythm actions")]
    private float superSpeed = 5;

    [SerializeField] private LayerMask wallsLayerMask;

    [Space(), Header("Action restrictions")] [SerializeField]
    private SongSynchronizer.ThresholdBeatEvents restrictActionOn;

    [Space(), Header("Events")] public UnityEvent OnJump;
    public UnityEvent OnDash;

    [Space(), Header("Variables")] [SerializeField]
    private Transform art;

    private BoxCollider2D _boxCollider;
    private RaycastGroup _collisionX;
    private RaycastGroup _collisionY;
    private RaycastGroup _upCast;
    private RaycastGroup _downCast;
    private RaycastGroup _rightCast;
    private RaycastGroup _leftCast;
    private RaycastGroup _downBuffer;
    private RaycastGroup _rightBuffer;
    private RaycastGroup _leftBuffer;
    private Vector3 _initialLocalScale;
    private float _dashTime;
    private bool _dashing;
    private bool _isFlipped;
    private PlayerInput _input;
    private Vector2 _velocity;
    private bool _grounded;
    private bool _roofed;
    private bool _wallRiding;
    private bool _moveLocked;
    private PlayerFlags _flags;
    private bool _needsReset;
    private SongSynchronizer _synchronizer;
    private Vector3 _upVect;
    private int _direction;
    private int _wall;
    private bool _groundBuffer;
    private float _dashBuffer;
    private int _additionalSpeed;
    private float _superSpeedValue;
    private LevelManager _levelManager;
    private Vector3 _initialPosition = Vector3.zero;
    private Animator _artAnimator;

    private ScoreState _scoreState = new ScoreState(score: SongSynchronizer.EventScore.Nice);
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
        _input = new PlayerInput();
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _artAnimator.SetFloat(IdleMultiplierFloat, _synchronizer.song.Informations.bpm / 120.00f);
        _rippleController = GameObject.Find("PostProcessing").GetComponent<RippleController>();
        _collisionX = new RaycastGroup(_boxCollider, Vector2.zero, 0, 7, wallsLayerMask, new Vector2(1, 1));
        _collisionY = new RaycastGroup(_boxCollider, Vector2.zero, 0, 7, wallsLayerMask, new Vector2(1, 1));
        _upCast = new RaycastGroup(_boxCollider, Vector2.up, 0.1f, 3, wallsLayerMask, new Vector2(1, 1));
        _downCast = new RaycastGroup(_boxCollider, Vector2.down, 0.1f, 3, wallsLayerMask, new Vector2(1, 1));
        _rightCast = new RaycastGroup(_boxCollider, Vector2.right, 0.1f, 7, wallsLayerMask, new Vector2(1, 1));
        _leftCast = new RaycastGroup(_boxCollider, Vector2.left, 0.1f, 7, wallsLayerMask, new Vector2(1, 1));
        _downBuffer = new RaycastGroup(_boxCollider, Vector2.down, surfaceRayLength, 3, wallsLayerMask, new Vector2(1.2f, 1));
        _rightBuffer = new RaycastGroup(_boxCollider, Vector2.right, surfaceRayLength, 7, wallsLayerMask, new Vector2(1, 1));
        _leftBuffer = new RaycastGroup(_boxCollider, Vector2.left, surfaceRayLength, 7, wallsLayerMask, new Vector2(1, 1));
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
        if (!_levelManager.isGamePaused)
        {
            Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();
            if (_moveLocked || moveInput.x == 0)
            {
                _direction = 0;
            }
            else if (moveInput.x < 0)
            {
                _direction = -1;
            }
            else if (moveInput.x > 0)
            {
                _direction = 1;
            }

            UpdateScale(_direction);
            SurfaceDetection();
            HandleRythmAction(_direction);
            HandleMovement(_direction);
            ResolveDash(_direction);
            ResolveTimeBuffers(_direction);
            HandleAnimations(_direction, _velocity.y);
        }
    }

    private void HandleAnimations(int input, float velocityY)
    {
        _artAnimator.SetFloat(SpeedFloat, Mathf.Abs(input));
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
        _wallRiding = _leftCast.Check(_leftCast.Left) || _rightCast.Check(_rightCast.Right);
        
        if (_downBuffer.Check(_downBuffer.TouchDown))
        {
            _groundBuffer = true;
            _flags.CanDash = true;
            _flags.CanJump = true;
            _needsReset = false;
        }
        else
        {
            _groundBuffer = false;
        }
        
        if (_rightBuffer.Check(_rightBuffer.TouchRight))
        {
            _wall = 1;
            _flags.CanDash = true;
            _needsReset = false;
        }
        else if (_leftBuffer.Check(_leftBuffer.TouchLeft))
        {
            _wall = -1;
            _flags.CanDash = true;
            _needsReset = false;
        }
        else
        {
            _wall = 0;
        }
    }

    private void HandleRythmAction(int moveInput)
    {
        if (!_flags.ActionAvailable)
        {
            if (_input.Player.Jump.triggered || _input.Player.Dash.triggered)
            {
                var action = _input.Player.Jump.triggered ? PlayerActions.Jump : PlayerActions.Dash;
                OnActionPerformed(this,
                    new OnActionEventArgs() {Move = action, Score = SongSynchronizer.EventScore.Failed});
                _additionalSpeed = 0;
            }

            return;
        }

        if (_input.Player.Jump.triggered)
        {
            _flags.ActionAvailable = false;
            if (_groundBuffer || _wall != 0 || _flags.CanJump && !_needsReset)
            {
                if (!_groundBuffer && _wall == 0 && _flags.CanJump)
                {
                    _needsReset = true;
                    _flags.CanJump = false;
                }

                Jump();
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
            }
        }
        else if (_input.Player.Dash.triggered && _flags.CanDash && moveInput != _wall && !_needsReset)
        {
            _flags.ActionAvailable = false;
            if (Mathf.Abs(moveInput) > 0)
            {
                _needsReset = true;
                Dash(moveInput);
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
            }
            else
            {
                _dashBuffer = dashBuffer;
            }
        }

        _superSpeedValue = _additionalSpeed == numberOfSteps ? superSpeed : 0;
    }

    private void ResolveDash(float moveInput)
    {
        if (_dashing)
        {
            if (_dashTime <= 0)
            {
                _dashTime = dashDuration;
                _velocity.y = 0;
                _velocity.x = (speed + _superSpeedValue) * moveInput;
                _dashing = false;
            }
            else
            {
                _dashTime -= Time.deltaTime;
            }
        }
    }

    private void ResolveTimeBuffers(int moveInput)
    {
        if (_dashBuffer > 0)
        {
            if (Mathf.Abs(moveInput) > 0)
            {
                _dashBuffer = 0;
                Dash(moveInput);
                if (_additionalSpeed < numberOfSteps) _additionalSpeed++;
                return;
            }

            _dashBuffer -= Time.deltaTime;
        }
    }

    private void HandleMovement(int moveInput)
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
                (speed + _superSpeedValue) * moveInput,
                acceleration * Time.deltaTime);
        }
        else
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
        }

        // Apply gravity
        if (_dashing || _grounded)
        {
            _velocity.y = 0;
        } else if (_roofed)
        {
            _velocity.y = Physics2D.gravity.y * 2f * Time.deltaTime;
        } else
        {
            _velocity.y += Physics2D.gravity.y * 2f * Time.deltaTime;
        }
        
        // Prevent speed gain against wall
        if (_wallRiding && (_wall < 0 && _velocity.x < 0 || _wall > 0 && _velocity.x > 0))
        {
            _velocity.x = 0;
        }
        
        // Define ending point
        var pos = _velocity * Time.deltaTime;
        
        // Check if collision is expected
        var distances = new float[2];
        
        _collisionX.Distance = pos.magnitude;
        _collisionX.Direction = _velocity.normalized;
        distances[0] = _collisionX.MinDistance(pos.x > 0 ? Vector2.right : Vector2.left);

        _collisionY.Distance = pos.magnitude;
        _collisionY.Direction = _velocity.normalized;
        distances[1] = _collisionY.MinDistance(pos.y > 0 ? Vector2.up : Vector2.down);

        pos = _velocity.normalized * distances.Min();
        
        _artAnimator.SetBool(WallridingAnimatorBool, _wallRiding);
        
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
                _scoreState.Score = SongSynchronizer.EventScore.Nice;
                break;
            case SongSynchronizer.EventState.Mid:
                _scoreState.Score = SongSynchronizer.EventScore.Perfect;
                break;
            case SongSynchronizer.EventState.End:
                _flags.ActionAvailable = false;
                _scoreState.Score = SongSynchronizer.EventScore.Nice;
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
            _velocity.x = -_wall * wallJumpSpeed * 2;
            LockMove(wallJumpLock);
        }

        _grounded = false;
        OnJump?.Invoke();
    }

    private void Dash(int moveInput)
    {
        _artAnimator.SetTrigger(DashAnimatorTrigger);
        OnActionPerformed(this, new OnActionEventArgs() {Move = PlayerActions.Dash, Score = _scoreState.Score});
        OnDash?.Invoke();
        _dashing = true;
        _velocity.y = 0;
        _velocity.x = Mathf.MoveTowards(_velocity.x, dashSpeed * moveInput, dashAcceleration);
        _flags.CanDash = false;
        
        _rippleController.startRipple();
        /*_trailPS.Play();*/
    }

    public void LockMove(float duration)
    {
        _moveLocked = true;
        var coroutine = ResolveMoveLock(duration);
        StartCoroutine(coroutine);
    }
    IEnumerator ResolveMoveLock(float duration)
    {
        yield return new WaitForSeconds(duration);
        _moveLocked = false;
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