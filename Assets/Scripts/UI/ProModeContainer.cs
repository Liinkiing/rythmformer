using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProModeContainer : MonoBehaviour
{
    private CanvasGroup _canvas;
    private string _initialText;
    [SerializeField] private TextMeshProUGUI _proText;
    private const string PlaceholderText = "{VALUE}";

    private void Awake()
    {
        _canvas = GetComponent<CanvasGroup>();
        _initialText = _proText.text;
        UpdateValues(GameManager.instance.Difficulty);
    }

    private void UpdateValues(Difficulty difficulty)
    {
        var isProMode = difficulty == Difficulty.ProGamer;
        _canvas.DOFade(isProMode ? 1 : 0.3f, 0.3f);
        _proText.text = _initialText.Replace(PlaceholderText, isProMode ? "ON" : "OFF");
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
}