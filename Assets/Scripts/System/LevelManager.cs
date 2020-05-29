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

    public LevelConfig Config = new LevelConfig() {World = World.Castle, Level = Level.Tutorial};
    public bool isGamePaused;
    [HideInInspector] public float TimeElapsed;
    private SongSynchronizer _songSynchronizer;
    private bool _hasFinishedLevel;
    private CharacterController2D _player;
    private PlayerInput _input;
    private LevelUI _levelUI;

    [Space(), Header("Events")] public UnityEvent OnLevelReset;
    public UnityEvent OnLevelPause;

    private void Awake()
    {
        isGamePaused = false;
        TimeElapsed = 0f;

        _input = new PlayerInput();
        _input.Global.Reset.performed += OnResetPerformedHandler;
        _input.Player.Pause.performed += PauseOnPerformed;

        _levelUI = Utils.FindObjectOfTypeOrThrow<LevelUI>();
        GameManager.instance.state = GameManager.GameState.InGame;
        _songSynchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
    }

    private void Update()
    {
        if (isGamePaused) return;
        TimeElapsed += Time.deltaTime;
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
        if (isGamePaused || _hasFinishedLevel) return;
        TimeElapsed = 0;
        OnLevelReset?.Invoke();
    }

    private void PauseOnPerformed(InputAction.CallbackContext obj)
    {
        if (_hasFinishedLevel) return;
        OnLevelPause?.Invoke();
    }

    public void TogglePause()
    {
        if (_hasFinishedLevel) return;
        isGamePaused = !isGamePaused;

        _levelUI.TogglePauseCanvas();
        _songSynchronizer.ToggleLowPassFilter(isGamePaused);
    }

    public void FinishLevel()
    {
        _hasFinishedLevel = true;
        isGamePaused = true;
        _songSynchronizer.ToggleLowPassFilter(true);
    }
}