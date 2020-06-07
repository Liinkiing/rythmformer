using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class RythmFlowerFlicker : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private Light2D _light;

    private float initialIntensity = 2f;

    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _light = GetComponent<Light2D>();
        initialIntensity = _light.intensity;
    }

    private void OnEnable()
    {
        _synchronizer.Step += OnHalfBeat;
    }

    private void OnDisable()
    {
        _synchronizer.Step -= OnHalfBeat;
    }

    private void OnHalfBeat(SongSynchronizer sender, EventArgs evt)
    {
        if (_light.intensity == initialIntensity)
        {
            DOTween.To(()=> _light.intensity, x => _light.intensity = x, 0, 0.2f);
        }
        else
        {
            DOTween.To(()=> _light.intensity, x => _light.intensity = x, initialIntensity, 0.2f);
        }
    }
}