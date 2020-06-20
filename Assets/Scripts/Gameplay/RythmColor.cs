using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class RythmColor : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private SpriteRenderer _renderer;
    private Color initialColor;
    [SerializeField] private Color desiredColor = Color.black;
    [SerializeField] private SongSynchronizer.PossibleMeasure measure = SongSynchronizer.PossibleMeasure.Step;
    [SerializeField] private float TweenDuration = 0.2f;

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
        if (_renderer.color == initialColor)
        {
            _renderer.DOColor(desiredColor, TweenDuration);
        }
        else
        {
            _renderer.DOColor(initialColor, TweenDuration);
        }
    }
}