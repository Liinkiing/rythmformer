#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;

public class SongSynchronizer : MonoBehaviour
{
    public enum EventState
    {
        Start,
        Mid,
        End
    }

    public enum ThresholdBeatEvents
    {
        Step,
        HalfBeat,
        Beat
    }
    
    public enum EventScore
    {
        Ok,
        Perfect
    }

    public event Action<SongSynchronizer, EventArgs> FourthStep;
    public event Action<SongSynchronizer, EventArgs> HalfStep;
    public event Action<SongSynchronizer, EventArgs> Step;
    public event Action<SongSynchronizer, EventArgs> EveryTwoStep;
    public event Action<SongSynchronizer, EventArgs> FirstAndThirdStep;
    public event Action<SongSynchronizer, EventArgs> HalfBeat;
    public event Action<SongSynchronizer, EventArgs> Beat;
    public event Action<SongSynchronizer, EventState> StepThresholded;
    public event Action<SongSynchronizer, EventState> HalfBeatThresholded;
    public event Action<SongSynchronizer, EventState> BeatThresholded;

    [Serializable]
    public struct AudioSources
    {
        public AudioSource Bass;
        public AudioSource Drums;
        public AudioSource Melody;
    }
    
    [Space, SerializeField] private Song _song;

    [Space, Header("Audio sources")] [SerializeField]
    public AudioSources Sources;

    [SerializeField] private bool playMetronome;

    [SerializeField] private AudioClip metronome;

    private double _nextTick = 0.0F; // The next tick in dspTime
    private bool _ticked = false;
    private int _beats = 1;
    private int _measure = 1;
    private int _quarters = 0;

    [Space, Header("Options")] [SerializeField]
    private float _offset = 0.05f;

    [SerializeField] private bool _runInBackground = true;
    
    void Start()
    {
        var startTick = AudioSettings.dspTime + 1f;

        _nextTick = startTick + ((60.0 / _song.Informations.bpm) / 4);

        if (_song.Stems.All != null)
        {
            Sources.Melody.clip = _song.Stems.All;
            
            Sources.Melody.PlayScheduled(startTick);
        }
        else
        {
            Sources.Bass.clip = _song.Stems.Bass;
            Sources.Melody.clip = _song.Stems.Melody;
            Sources.Drums.clip = _song.Stems.Drums;
            
            Sources.Bass.PlayScheduled(startTick);
            Sources.Melody.PlayScheduled(startTick);
            Sources.Drums.PlayScheduled(startTick);
        }
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
            DoOnFourthStep();
        }
    }

    void FixedUpdate()
    {
        double timePerTick = (60.0f / _song.Informations.bpm) / 4;
        double dspTime = AudioSettings.dspTime;
        while (dspTime >= _nextTick)
        {
            _ticked = false;
            _nextTick += timePerTick;
        }
    }

    void DoOnFourthStep()
    {
        var totalQuarters = _song.Informations.signature.numerator * 4;
        if (_quarters % totalQuarters == 0)
        {
            _quarters = 0;
        }

        if (_quarters % 2 == 0)
        {
            OnHalfStep(this);
        }

        if (_quarters % 4 == 0)
        {
            StartCoroutine(DoOnStep(_offset));
        }

        if (_quarters == 0)
        {
            OnBeatThresholded(this, EventState.Mid);
            OnHalfBeatThresholded(this, EventState.Mid);
        }
        else if (_quarters == 1)
        {
            OnBeatThresholded(this, EventState.End);
            OnHalfBeatThresholded(this, EventState.End);
        }

        /* TODO: Simplify this, pls some math expert I'm sure we can found something
        that works in fewer lines */
        switch (_quarters)
        {
            case 3:
            case 7:
            case 11:
            case 15:
                OnStepThresholded(this, EventState.Start);
                break;
            case 0:
            case 4:
            case 8:
            case 12:
            case 16:
                OnStepThresholded(this, EventState.Mid);
                break;
            case 1:
            case 5:
            case 9:
            case 13:
                OnStepThresholded(this, EventState.End);
                break;
        }

        if (_quarters == totalQuarters / 2 - 1)
        {
            OnHalfBeatThresholded(this, EventState.Start);
        }
        else if (_quarters == totalQuarters / 2)
        {
            OnHalfBeatThresholded(this, EventState.Mid);
        }
        else if (_quarters == totalQuarters / 2 + 1)
        {
            OnHalfBeatThresholded(this, EventState.End);
        }

        if (_quarters == totalQuarters - 1)
        {
            OnBeatThresholded(this, EventState.Start);
            OnHalfBeatThresholded(this, EventState.Start);
        }
        else if (_quarters == totalQuarters)
        {
            OnBeatThresholded(this, EventState.Mid);
            OnHalfBeatThresholded(this, EventState.Mid);
        }

        OnFourthStep(this);
        _quarters += 1;
    }

    private IEnumerator DoOnStep(float delay)
    {
        yield return new WaitForSeconds(Mathf.Abs(delay));
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

        if (_measure == (_song.Informations.signature.numerator / 2) + 1)
        {
            OnHalfBeat(this);
        }

        if (_measure % _song.Informations.signature.numerator == 0)
        {
            _measure = 1;
        }
        else
        {
            _measure += 1;
        }
    }

    protected virtual void OnFourthStep(SongSynchronizer sender)
    {
        FourthStep?.Invoke(sender, EventArgs.Empty);
    }

    protected virtual void OnHalfStep(SongSynchronizer sender)
    {
        HalfStep?.Invoke(sender, EventArgs.Empty);
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

    protected virtual void OnBeatThresholded(SongSynchronizer sender, EventState state)
    {
        BeatThresholded?.Invoke(sender, state);
    }

    protected virtual void OnHalfBeatThresholded(SongSynchronizer sender, EventState state)
    {
        HalfBeatThresholded?.Invoke(sender, state);
    }
}