﻿using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.Events;

public class SongSynchronizer : MonoBehaviour
{
    public event Action<SongSynchronizer, EventArgs> Step;
    public event Action<SongSynchronizer, EventArgs> EveryTwoStep;
    public event Action<SongSynchronizer, EventArgs> FirstAndThirdStep;
    public event Action<SongSynchronizer, EventArgs> HalfBeat;
    public event Action<SongSynchronizer, EventArgs> Beat;

    [Serializable]
    private struct TimeSignature
    {
        public int numerator;
        public int denominator;
    }

    [Serializable]
    private struct SongInformations
    {
        public TimeSignature signature;
        public float bpm;
    }

    [Serializable]
    public struct AudioSources
    {
        public AudioSource Bass;
        public AudioSource Drums;
        public AudioSource Melody;
    }
    
    [Serializable]
    private struct SongStems
    {
        public AudioClip Bass;
        public AudioClip Drums;
        public AudioClip Melody;
    }

    [SerializeField] private SongInformations songInfo = new SongInformations()
    {
        bpm = 130,
        signature = new TimeSignature() {denominator = 4, numerator = 4}
    };

    [Space, Header("Audio sources")] [SerializeField]
    public AudioSources Sources;

    [SerializeField] private bool playMetronome;

    [Space, Header("Audio clips")] [SerializeField]
    private SongStems stems;

    [SerializeField] private AudioClip metronome;

    private double _nextTick = 0.0F; // The next tick in dspTime
    private bool _ticked = false;
    private int _beats = 1;
    private int _measure = 1;
    
    [Space, Header("Options")] [SerializeField] private float _offset = 0.05f;
    [SerializeField] private bool _runInBackground = true;

    void Start()
    {
        var startTick = AudioSettings.dspTime;

        _nextTick = startTick + (60.0 / songInfo.bpm);
        // Sources = GetComponent<AudioSource>();
        Sources.Bass.clip = stems.Bass;
        Sources.Melody.clip = stems.Melody;
        Sources.Drums.clip = stems.Drums;
        
        Sources.Bass.PlayScheduled(startTick);
        Sources.Melody.PlayScheduled(startTick);
        Sources.Drums.PlayScheduled(startTick);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (_runInBackground) return;
        AudioListener.pause = pauseStatus;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (_runInBackground) return;
        AudioListener.pause = !hasFocus;
    }

    void LateUpdate()
    {
        if (!_ticked && _nextTick >= AudioSettings.dspTime)
        {
            _ticked = true;
            this.DoOnStep();
        }
    }

    void FixedUpdate()
    {
        double timePerTick = 60.0f / songInfo.bpm;
        double dspTime = AudioSettings.dspTime;
        // Debug.Log($"dspTime: {dspTime}, nextTick: {_nextTick}");
        while ((dspTime + _offset) >= _nextTick)
        {
            _ticked = false;
            _nextTick += timePerTick;
        }
    }

    void DoOnStep()
    {
        Debug.Log($"{_measure}/{songInfo.signature.numerator} ({Sources.Melody.time}s)");

        this.OnStep(this);

        if (playMetronome)
        {
            MusicManager.instance.Sources.SFX.pitch = _measure == 1 ? 2 : 1;
            MusicManager.instance.Sources.SFX.PlayOneShot(metronome);
        }

        if (_measure == 1)
        {
            this.OnBeat(this);
        }
        if (_measure % 2 == 0)
        {
            this.OnEveryTwoStep(this);
        }

        if (_measure == 1 || _measure == 3)
        {
            this.OnFirstAndThirdStep(this);
        }
        if (_measure == (songInfo.signature.numerator / 2) + 1)
        {
            this.OnHalfBeat(this);
        }

        if (_measure % songInfo.signature.numerator == 0)
        {
            _measure = 1;
        }
        else
        {
            _measure += 1;
        }
    }

    protected virtual void OnStep(SongSynchronizer sender)
    {
        Step?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnHalfBeat(SongSynchronizer sender)
    {
        HalfBeat?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnBeat(SongSynchronizer sender)
    {
        Beat?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnEveryTwoStep(SongSynchronizer sender)
    {
        EveryTwoStep?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnFirstAndThirdStep(SongSynchronizer sender)
    {
        FirstAndThirdStep?.Invoke(sender, EventArgs.Empty);
    }
}