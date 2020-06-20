using System;
using DG.Tweening;
using UnityEngine;

public class RythmAnimator : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private Animator _animator;
    [SerializeField] private SongSynchronizer.PossibleMeasure measure = SongSynchronizer.PossibleMeasure.Step;
    [SerializeField] private string AnimationToTrigger;
    
    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _animator = GetComponent<Animator>();
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
        _animator.SetTrigger(AnimationToTrigger);
    }
}