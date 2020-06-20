using System;
using DG.Tweening;
using UnityEngine;

public class RythmScaler : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private Vector3 initialScale;
    [SerializeField] private Vector3 desiredScale = new Vector3(0.5f, 0.5f);
    [SerializeField] private SongSynchronizer.PossibleMeasure measure = SongSynchronizer.PossibleMeasure.Step;
    [SerializeField] private float TweenDuration = 0.2f;

    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        initialScale = transform.localScale;
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
        if (transform.localScale == initialScale)
        {
            transform.DOScale(desiredScale, TweenDuration);
        }
        else
        {
            transform.DOScale(initialScale, TweenDuration);
        }
    }
}