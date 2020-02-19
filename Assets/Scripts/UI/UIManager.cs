using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private GameObject SceneTransition;
    private PlayerInput _input;

    void Awake()
    {
        _input = new PlayerInput();
        _input.Player.Menu.performed += MenuOnperformed;
    }
    private void OnEnable()
    {
        _input?.Enable();
    }
    
    private void OnDisable()
    {
        _input?.Disable();
    }
    
    private void OnDestroy()
    {
        _input.Player.Menu.performed -= MenuOnperformed;
    }

    private void MenuOnperformed(InputAction.CallbackContext obj)
    {
        StartCoroutine(SceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    }
}
