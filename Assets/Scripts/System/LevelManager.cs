using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    public struct LevelConfig
    {
        public World World;
        public Level Level;
    }

    public LevelConfig Config = new LevelConfig() { World = World.Castle, Level =  Level.Tutorial };
    public bool isGamePaused;
    private SongSynchronizer _songSynchronizer;
    private CharacterController2D _player;
    private PlayerInput _input;
    private UIManager _UIManager;

    [Space(), Header("Events")]
    public UnityEvent OnLevelReset;
    public UnityEvent OnLevelPause;

    private void Awake()
    {
        isGamePaused = false;
        
        _input = new PlayerInput();
        _input.Global.Reset.performed += OnResetPerformedHandler;
        _input.Player.Pause.performed += PauseOnPerformed;
        
        _UIManager = Utils.FindObjectOfTypeOrThrow<UIManager>();
        _songSynchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
    }

    private void OnEnable()
    {
        _input?.Enable();
        OnLevelPause.AddListener(TogglePause);
    }

    private void OnDisable()
    {
        _input?.Disable();
        OnLevelPause.RemoveListener(TogglePause);
    }
    
    private void OnDestroy()
    {
        _input.Player.Pause.performed -= PauseOnPerformed;
    }

    private void OnResetPerformedHandler(InputAction.CallbackContext context)
    {
        OnLevelReset?.Invoke();
    }
    
    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        OnLevelPause?.Invoke();
    }

    public void TogglePause()
    {
        _UIManager = Utils.FindObjectOfTypeOrThrow<UIManager>();
        _songSynchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();

        isGamePaused = !isGamePaused;
        _UIManager.TogglePauseCanvas();
        _songSynchronizer.ToggleLowPassFilter(isGamePaused);
    }
}