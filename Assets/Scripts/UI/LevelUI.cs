﻿using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;
public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseContainer;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _scoreboardButton;
    [SerializeField] private TextMeshProUGUI _UITimer;
    private CanvasGroup _pauseCanvas;
    private float _timeElapsed;
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _pauseCanvas = _pauseContainer.GetComponent<CanvasGroup>();
        
        _pauseCanvas.alpha = 0;
        _pauseCanvas.blocksRaycasts = false;
        _pauseCanvas.interactable = false;

        _continueButton.onClick.AddListener(() =>
        {
            _levelManager.OnLevelPause?.Invoke();
        });
        
        _scoreboardButton.onClick.AddListener(() =>
        {
            UIManager.instance.ToggleLevelUI();
            UIManager.instance.ToggleLevelEndUI();
        });
        
        _timeElapsed = 0;
    }
    
    private void Update()
    {
        if (!_levelManager.isGamePaused)
        {
            _timeElapsed += Time.deltaTime;
            _UITimer.SetText($"Time : {_timeElapsed:0.000}");
        }
    }
    
    private void OnEnable()
    {
        _levelManager?.OnLevelReset.AddListener(OnLevelReset);
    }

    private void OnDisable()
    {
        _levelManager?.OnLevelReset.AddListener(OnLevelReset);
    }

    private void OnLevelReset()
    {
        _timeElapsed = 0;
    }
    
    public void TogglePauseCanvas()
    {
        _pauseCanvas.blocksRaycasts = _levelManager.isGamePaused;
        _pauseCanvas.interactable = _levelManager.isGamePaused;
        
        DOTween
            .To(() => _pauseCanvas.alpha, x => _pauseCanvas.alpha = x, _levelManager.isGamePaused ? 1 : 0, UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);

        if (_levelManager.isGamePaused)
        {
            EventSystem.current.SetSelectedGameObject(_continueButton.gameObject);
        }
    }
}
