using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RythmScaler : MonoBehaviour
{
    private SongSynchronizer _synchronizer;
    private Vector3 initialScale;
    [SerializeField] private Vector3 desiredScale = new Vector3(0.5f, 0.5f);

    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        initialScale = transform.localScale;
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
        if (transform.localScale == initialScale)
        {
            transform.DOScale(desiredScale, 0.2f);
        }
        else
        {
            transform.DOScale(initialScale, 0.2f);
        }
    }
}