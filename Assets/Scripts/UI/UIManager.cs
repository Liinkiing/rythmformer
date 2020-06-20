using System;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum BindingScheme
{
    Gamepad,
    Keyboard
}

public enum GameAction
{
    Jump,
    Dash,
    Retry,
    SwitchMode,
}

[RequireComponent(typeof(BindingIcons))]
public class UIManager : MonoSingleton<UIManager>
{
    public float transitionUIDuration = 0.8f;
    [SerializeField] private GameObject _sceneTransition;
    private PlayerInput _input;
    private int _transitionSequenceID;
    private GameObject _eventSystemTarget;
    private InputAction _gamepadAction = new InputAction(binding: "<Gamepad>/*", interactions: "press");
    private InputAction _keyboardAction = new InputAction(binding: "<Keyboard>/*");

    public GameObject levelUI;
    public CanvasGroup levelUICanvasGroup;
    public GameObject levelEndUI;
    public CanvasGroup levelEndUICanvasGroup;

    private List<InputAction> _mousesAction = new List<InputAction>()
    {
        new InputAction(binding: "<Mouse>/leftButton"),
        new InputAction(binding: "<Mouse>/rightButton"),
        new InputAction(binding: "<Mouse>/rightButton"),
        new InputAction(binding: "<Mouse>/middleButton"),
        new InputAction(binding: "<Mouse>/forwardButton"),
        new InputAction(binding: "<Mouse>/backButton"),
        new InputAction(binding: "<Mouse>/press"),
    };

    public event Action<UIManager, BindingScheme, string> SchemeChanged;

    public enum UIContainerAction
    {
        Enable,
        Disable,
        Toggle
    }

    void Awake()
    {
        _input = new PlayerInput();
        _gamepadAction.performed += context =>
        {
            if (GameManager.instance.CurrentBindingScheme == BindingScheme.Gamepad) return;
            OnSchemeChanged(this, BindingScheme.Gamepad, context.control.path);
            GameManager.instance.CurrentBindingScheme = BindingScheme.Gamepad;
        };
        _keyboardAction.performed += context =>
        {
            if (GameManager.instance.CurrentBindingScheme == BindingScheme.Keyboard) return;
            OnSchemeChanged(this, BindingScheme.Keyboard, context.control.path);
            GameManager.instance.CurrentBindingScheme = BindingScheme.Keyboard;
        };

        foreach (var mouseAction in _mousesAction)
        {
            mouseAction.performed += context =>
            {
                if (GameManager.instance.CurrentBindingScheme == BindingScheme.Keyboard) return;
                OnSchemeChanged(this, BindingScheme.Keyboard, context.control.path);
                GameManager.instance.CurrentBindingScheme = BindingScheme.Keyboard;
            };
        }
    }

    private void OnEnable()
    {
        _input?.Enable();
        _gamepadAction.Enable();
        _keyboardAction.Enable();
        _mousesAction.ForEach(m => m.Enable());
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SetEventSystemsTarget(EventSystem.current.currentSelectedGameObject);
        }
        else
        {
            SetEventSystemsTarget(_eventSystemTarget);
        }
    }

    private void OnDisable()
    {
        _input?.Disable();
        _gamepadAction.Disable();
        _keyboardAction.Disable();
        _mousesAction.ForEach(m => m.Disable());
    }

    public void ShowLevelEndScreen()
    {
        SetUIContainerStateWithInternalNavigation(levelUI, levelUICanvasGroup, levelEndUI,
            levelEndUICanvasGroup, GameObject.Find("Continue button"));
    }

    public void SetUIContainerState(GameObject UIContainer, UIContainerAction action)
    {
        switch (action)
        {
            case UIContainerAction.Disable:
                UIContainer.SetActive(false);
                break;
            case UIContainerAction.Enable:
                UIContainer.SetActive(true);
                break;
            case UIContainerAction.Toggle:
                UIContainer.SetActive(!UIContainer.activeSelf);
                break;
        }
    }

    public void SetUIContainerStateWithInternalNavigation(
        GameObject UIContainerCurrent,
        CanvasGroup CanvasGroupCurrent,
        GameObject UIContainerTarget,
        CanvasGroup CanvasGroupTarget,
        GameObject nextEventSytemTarget = null)
    {
        CanvasGroupTarget.blocksRaycasts = true;
        CanvasGroupTarget.interactable = true;

        UIContainerTarget.SetActive(true);

        if (nextEventSytemTarget)
        {
            SetEventSystemsTarget(nextEventSytemTarget);
        }

        if (DOTween.IsTweening(_transitionSequenceID))
        {
            DOTween.Kill(_transitionSequenceID);
        }

        Sequence transitionSequence = DOTween.Sequence();
        transitionSequence.Append(
            CanvasGroupCurrent
                .DOFade(0, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );
        transitionSequence.Append(
            CanvasGroupTarget
                .DOFade(1, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );

        transitionSequence.AppendCallback(() =>
        {
            CanvasGroupCurrent.blocksRaycasts = false;
            CanvasGroupCurrent.interactable = false;
            UIContainerCurrent.SetActive(false);
        });

        transitionSequence.Play();
        _transitionSequenceID = transitionSequence.intId;
    }

    public void SetEventSystemsTarget(GameObject obj)
    {
        _eventSystemTarget = obj;
        EventSystem.current.SetSelectedGameObject(_eventSystemTarget);
    }

    public void NavigateToScene(string sceneName)
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(sceneName));
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public string FormatTimer(float timer)
    {
        return timer.ToString("0.000");
    }

    protected virtual void OnSchemeChanged(UIManager sender, BindingScheme scheme, string path)
    {
        SchemeChanged?.Invoke(sender, scheme, path);
    }
}