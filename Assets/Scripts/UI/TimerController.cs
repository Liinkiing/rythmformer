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
        _timeElapsed = Time.fixedTime;
        _UITimer.SetText("Time : " + _timeElapsed.ToString("0.000"));
    }
}
