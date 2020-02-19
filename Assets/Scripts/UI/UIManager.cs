using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoSingleton<UIManager>
{
    private PlayerInput _input;
    private AsyncOperation _menuScene;

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
        _menuScene = SceneManager.LoadSceneAsync("LevelSelector", LoadSceneMode.Single);
    }
}
