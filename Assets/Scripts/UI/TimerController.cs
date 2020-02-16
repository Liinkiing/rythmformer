using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    #region Fields
    private TextMeshProUGUI _UITimer;
    private float _timeElapsed;
    #endregion
    void Awake()
    {
        _timeElapsed = 0;
        _UITimer = GetComponent<TextMeshProUGUI>();
    }

    private void FixedUpdate()
    {
        _UITimer.SetText("Time : " + Time.timeSinceLevelLoad.ToString("0.000"));
    }
}
