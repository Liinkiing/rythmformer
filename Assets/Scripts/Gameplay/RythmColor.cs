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

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        initialColor = _renderer.color;
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
        if (_renderer.color == initialColor)
        {
            _renderer.DOColor(desiredColor, 0.2f);
        }
        else
        {
            _renderer.DOColor(initialColor, 0.2f);
        }
    }
}