using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CutsteneSkipper : MonoBehaviour
{
    private PlayerInput _input;
    [SerializeField] private UnityEvent OnCutsceneSkip;

    private void Awake()
    {
        _input = new PlayerInput();
        _input.Global.SkipCutscene.performed += OnSkipCutscenePerformed;
    }

    private void OnSkipCutscenePerformed(InputAction.CallbackContext obj)
    {
        OnCutsceneSkip?.Invoke();
    }

    private void OnEnable()
    {
        _input?.Enable();
    }

    private void OnDisable()
    {
        _input?.Disable();
    }
}