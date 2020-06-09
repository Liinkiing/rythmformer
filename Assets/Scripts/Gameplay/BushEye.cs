using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BushEye : MonoBehaviour
{
    private enum Position
    {
        Left,
        Middle,
        Right
    }
    
    public Transform leftMarker;
    public Transform rightMarker;
    public Transform pupil;
    
    private SongSynchronizer _synchronizer;
    private Position _position = Position.Middle;
    private const float TweenDuration = 0.3f;
    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
    }
    private void OnEnable()
    {
        _synchronizer.Step += OnStep;
    }

    private void OnDisable()
    {
        _synchronizer.Step -= OnStep;
    }

    private void OnStep(SongSynchronizer sender, EventArgs evt)
    {
        switch (_position)
        {
            case Position.Left:
                pupil.DOMove(rightMarker.position, TweenDuration);
                _position = Position.Right;
                break;
            case Position.Middle:
            case Position.Right:
                pupil.DOMove(leftMarker.position, TweenDuration);
                _position = Position.Left;
                break;
        }
    }
}
