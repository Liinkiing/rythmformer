using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private GameObject _pauseCanvas;
    private PlayerInput _input;
    private CanvasGroup _pauseCanvasGroup;
    private bool _isPauseCanvasDisplayed;
    public float pauseTransitionDuration = 0.8f;

    void Awake()
    {
        _pauseCanvasGroup = _pauseCanvas.GetComponent<CanvasGroup>();
        _input = new PlayerInput();
        _pauseCanvasGroup.alpha = 0;
        _pauseCanvasGroup.blocksRaycasts = false;
        _isPauseCanvasDisplayed = false;
    }
    private void OnEnable()
    {
        _input?.Enable();
    }
    
    private void OnDisable()
    {
        _input?.Disable();
    }

    public void TogglePauseCanvas()
    {
        _pauseCanvasGroup.blocksRaycasts = !_pauseCanvasGroup.blocksRaycasts;

        DOTween
            .To(() => _pauseCanvasGroup.alpha, x => _pauseCanvasGroup.alpha = x, _isPauseCanvasDisplayed ? 0 : 1, pauseTransitionDuration)
            .SetEase(Ease.InOutQuint);
        
        _isPauseCanvasDisplayed = !_isPauseCanvasDisplayed;
    }

    public void BackToChapter()
    {
        StartCoroutine(SceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    } 
}
