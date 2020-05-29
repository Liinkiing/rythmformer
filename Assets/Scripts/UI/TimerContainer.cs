using System;
using DG.Tweening;
using UnityEngine;

public class TimerContainer : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        UpdateValues(GameManager.instance.Difficulty);
    }

    private void OnEnable()
    {
        GameManager.instance.DifficultyChanged += OnDifficultyChanged;
    }

    private void OnDisable()
    {
        GameManager.instance.DifficultyChanged -= OnDifficultyChanged;
    }
    private void OnDifficultyChanged(GameManager sender, Difficulty difficulty)
    {
        UpdateValues(difficulty);
    }
    
    private void UpdateValues(Difficulty difficulty)
    {
        var isProMode = difficulty == Difficulty.ProGamer;
        _canvasGroup.DOFade(isProMode ? 1 : 0, 0.3f);
    }
}