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
    [SerializeField] private SongSynchronizer.PossibleMeasure measure = SongSynchronizer.PossibleMeasure.Step;
    [SerializeField] private float TweenDuration = 0.2f;

    private float initialIntensity = 2f;

    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _light = GetComponent<Light2D>();
        initialIntensity = _light.intensity;
    }

    private void OnEnable()
    {
        switch (measure)
        {
            case SongSynchronizer.PossibleMeasure.FourthStep:
                _synchronizer.FourthStep += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.HalfStep:
                _synchronizer.HalfStep += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.Step:
                _synchronizer.Step += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.EveryTwoStep:
                _synchronizer.EveryTwoStep += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.FirstAndThirdStep:
                _synchronizer.FirstAndThirdStep += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.HalfBeat:
                _synchronizer.HalfBeat += OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.Beat:
                _synchronizer.Beat += OnMeasure;
                break;
        }
    }

    private void OnDisable()
    {
        switch (measure)
        {
            case SongSynchronizer.PossibleMeasure.FourthStep:
                _synchronizer.FourthStep -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.HalfStep:
                _synchronizer.HalfStep -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.Step:
                _synchronizer.Step -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.EveryTwoStep:
                _synchronizer.EveryTwoStep -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.FirstAndThirdStep:
                _synchronizer.FirstAndThirdStep -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.HalfBeat:
                _synchronizer.HalfBeat -= OnMeasure;
                break;
            case SongSynchronizer.PossibleMeasure.Beat:
                _synchronizer.Beat -= OnMeasure;
                break;
        }
    }

    private void OnMeasure(SongSynchronizer sender, EventArgs evt)
    {
        if (_light.intensity == initialIntensity)
        {
            DOTween.To(()=> _light.intensity, x => _light.intensity = x, 0, TweenDuration);
        }
        else
        {
            DOTween.To(()=> _light.intensity, x => _light.intensity = x, initialIntensity, TweenDuration);
        }
    }
}