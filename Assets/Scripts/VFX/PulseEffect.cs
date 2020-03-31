using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    private SongSynchronizer _synchronizer;

    private float _pulseDuration;
    [SerializeField] private Material _pulseEffectMaterial;
    private static readonly int TintIntensity = Shader.PropertyToID("Tint_intensity");

    void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _pulseDuration = 60f / _synchronizer.song.Informations.bpm;
    }

    private void OnEnable()
    {
        _synchronizer.StepThresholded += OnThresholdedAction;
    }

    private void OnDisable()
    {
        _synchronizer.StepThresholded -= OnThresholdedAction;
    }

    private void OnThresholdedAction(SongSynchronizer sender, SongSynchronizer.EventState state)
    {
        if (state == SongSynchronizer.EventState.Start)
        {
            Sequence pulseSequence = DOTween.Sequence();
            pulseSequence.Append(_pulseEffectMaterial.DOFloat(0, TintIntensity, _pulseDuration));
            pulseSequence.AppendCallback(ResetMaterial);
            pulseSequence.Play();
        }
    }
    
    private void ResetMaterial()
    {
        _pulseEffectMaterial.SetFloat(TintIntensity, 1f);
    }
}
