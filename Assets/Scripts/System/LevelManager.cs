using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private CharacterController2D _player;
    private PlayerInput _input;

    [Space(), Header("Events")] public UnityEvent OnLevelReset;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Global.Reset.performed += OnResetPerformedHandler;
    }

    private void OnEnable()
    {
        _input?.Enable();
    }

    private void OnDisable()
    {
        _input?.Disable();
    }

    private void OnResetPerformedHandler(InputAction.CallbackContext context)
    {
        OnLevelReset?.Invoke();
    }
}