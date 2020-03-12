using System;
using System.Collections;
using System.Collections.Generic;
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

    void FixedUpdate()
    {
        float pulseIntensity = Mathf.Lerp(1, 0, Time.fixedTime % _pulseDuration);
        _pulseEffectMaterial.SetFloat(TintIntensity, pulseIntensity);
    }
}
