#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.Events;

public class SongSynchronizer : MonoBehaviour
{
    public enum EventState
    {
        Start,
        End
    }

    private struct ThresholdState
    {
        public bool Start;
        public bool End;
    }

    public float Threshold = 0.05f;
    public event Action<SongSynchronizer, EventArgs> Step;
    public event Action<SongSynchronizer, EventState> StepThresholded;
    public event Action<SongSynchronizer, EventArgs> EveryTwoStep;
    public event Action<SongSynchronizer, EventState> EveryTwoStepThresholded;
    public event Action<SongSynchronizer, EventArgs> FirstAndThirdStep;
    public event Action<SongSynchronizer, EventState> FirstAndThirdStepThresholded;
    public event Action<SongSynchronizer, EventArgs> HalfBeat;
    public event Action<SongSynchronizer, EventState> HalfBeatThresholded;
    public event Action<SongSynchronizer, EventArgs> Beat;
    public event Action<SongSynchronizer, EventState> BeatThresholded;

    private ThresholdState _thresholdState = new ThresholdState() {Start = false, End = false};

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

    [Space, Header("Options")] [SerializeField]
    private float _offset = 0.05f;

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
        // Debug.Log(_nextTick);
        // Debug.Log(AudioSettings.dspTime);
        if (!_ticked && _nextTick >= AudioSettings.dspTime)
        {
            _ticked = true;
            DoOnStep();
            if (!_thresholdState.Start)
            {
                _thresholdState.Start = true;
                DoOnStepThresholded(EventState.Start);
            }
        }

        // if (!_thresholdState.Start && (_nextTick + 0 >= AudioSettings.dspTime) )
        // {
        //     _thresholdState.Start = true;
        //     DoOnStepThresholded(EventState.Start);
        // }

        // if (_nextTick - Threshold <= AudioSettings.dspTime - Threshold && !_thresholdState.End)
        // {
        //     _thresholdState.End = true;
        //     DoOnStepThresholded(EventState.End);
        // }
        // else if (_nextTick + Threshold >= AudioSettings.dspTime + Threshold && !_thresholdState.Start)
        // {
        //     _thresholdState.Start = true;
        //     DoOnStepThresholded(EventState.Start);
        // }
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
            _thresholdState.Start = false;
            _thresholdState.End = false;
        }
    }

    private void DoOnStepThresholded(EventState state)
    {
        // Debug.Log(_measure);
        OnStepThresholded(this, state);

        if (_measure == 1)
        {
            Debug.Log($"OUI BONSOIR TU PEUX {(state == EventState.End ? "PLUS " : "")}SAUTER EN BEAT");
            OnBeatThresholded(this, state);
        }

        if (_measure % 2 == 0)
        {
            OnEveryTwoStepThresholded(this, state);
        }

        if (_measure == 1 || _measure == 3)
        {
            OnFirstAndThirdStepThresholded(this, state);
        }

        if (_measure == (songInfo.signature.numerator / 2) + 1)
        {
            OnHalfBeatThresholded(this, state);
        }

        if (_measure == (songInfo.signature.numerator / 2) + 1)
        {
            OnFirstAndThirdStepThresholded(this, state);
        }
    }

    void DoOnStep()
    {
        Debug.Log($"{_measure}/{songInfo.signature.numerator} ({Sources.Melody.time}s)");

        OnStep(this);

        if (playMetronome)
        {
            MusicManager.instance.Sources.SFX.pitch = _measure == 1 ? 2 : 1;
            MusicManager.instance.Sources.SFX.PlayOneShot(metronome);
        }

        if (_measure == 1)
        {
            OnBeat(this);
        }

        if (_measure % 2 == 0)
        {
            OnEveryTwoStep(this);
        }

        if (_measure == 1 || _measure == 3)
        {
            OnFirstAndThirdStep(this);
        }

        if (_measure == (songInfo.signature.numerator / 2) + 1)
        {
            OnHalfBeat(this);
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

    protected virtual void OnStepThresholded(SongSynchronizer sender, EventState state)
    {
        StepThresholded?.Invoke(sender, state);
    }

    protected virtual void OnHalfBeatThresholded(SongSynchronizer sender, EventState state)
    {
        HalfBeatThresholded?.Invoke(sender, state);
    }

    protected virtual void OnBeatThresholded(SongSynchronizer sender, EventState state)
    {
        BeatThresholded?.Invoke(sender, state);
    }

    protected virtual void OnEveryTwoStepThresholded(SongSynchronizer sender, EventState state)
    {
        EveryTwoStepThresholded?.Invoke(sender, state);
    }

    protected virtual void OnFirstAndThirdStepThresholded(SongSynchronizer sender, EventState state)
    {
        FirstAndThirdStepThresholded?.Invoke(sender, state);
    }
}