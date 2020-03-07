using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    #region Fields

    private LevelManager _levelManager;
    private TextMeshProUGUI _UITimer;
    private float _timeElapsed;

    #endregion

    void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _timeElapsed = 0;
        _UITimer = GetComponent<TextMeshProUGUI>();
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

    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        _UITimer.SetText("Time : " + _timeElapsed.ToString("0.000"));
    }
}