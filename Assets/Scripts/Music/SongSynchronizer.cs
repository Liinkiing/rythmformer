#pragma warning disable 0649

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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
        Failed,
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
        public AudioSource SFX;
    }

    [Space, SerializeField] private Song _song;

    [Space, Header("Audio sources")] [SerializeField]
    public AudioSources Sources;

    [SerializeField] private bool playMetronome;

    [SerializeField] private AudioClip metronome;
    
    [Space, Header("Options"), Tooltip("Used to PlayScheduled the song to be on the exact dspTime and to avoid unpredictable behaviours")]
    [SerializeField] private float delay = 2f;

    private bool _ticked;
    private double _secondsElapsed;
    private double _startTick;
    private float _secondsPerTicks;
    private int _songPosInTicks;
    private int _tick;
    private int _measure;
    private int _quarters;
    private LevelManager _levelManager;
    
    [SerializeField] private bool runInBackground = true;

    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
    }

    private void OnEnable()
    {
        _levelManager?.OnLevelReset.AddListener(ResetSong);
    }

    private void OnDisable()
    {
        _levelManager?.OnLevelReset.RemoveListener(ResetSong);
    }

    void Start()
    {
        _startTick = AudioSettings.dspTime + delay;
        _secondsPerTicks = (60 / _song.Informations.bpm) / 4;

        if (_song.Stems.All != null)
        {
            Sources.Melody.clip = _song.Stems.All;

            Sources.Melody.PlayScheduled(_startTick);
        }
        else
        {
            Sources.Bass.clip = _song.Stems.Bass;
            Sources.Melody.clip = _song.Stems.Melody;
            Sources.Drums.clip = _song.Stems.Drums;

            Sources.Bass.PlayScheduled(_startTick);
            Sources.Melody.PlayScheduled(_startTick);
            Sources.Drums.PlayScheduled(_startTick);
        }
    }

    private void FixedUpdate()
    {
        _secondsElapsed = AudioSettings.dspTime - _startTick;
        _songPosInTicks = (int) (_secondsElapsed / _secondsPerTicks);
        if (_songPosInTicks <= _tick)
        {
            _ticked = false;
        }
        else if (_tick - _songPosInTicks <= 0)
        {
            _ticked = false;
        }
    }

    public void ResetSong()
    {
        _startTick = AudioSettings.dspTime;
        _tick = 0;
        _measure = 0;
        _quarters = 0;
        _ticked = false;
        if (_song.Stems.All != null)
        {
            Sources.Melody.timeSamples = 0;
        }
        else
        {
            Sources.Bass.timeSamples = 0;
            Sources.Melody.timeSamples = 0;
            Sources.Drums.timeSamples = 0;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (runInBackground) return;
        AudioListener.pause = pauseStatus;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (runInBackground) return;
        AudioListener.pause = !hasFocus;
    }

    void LateUpdate()
    {
        if (!_ticked && _songPosInTicks >= _tick)
        {
            _ticked = true;
            _tick++;
            DoOnFourthStep();
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
            DoOnStep();
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

    private void DoOnStep()
    {
        OnStep(this);

        if (playMetronome)
        {
            Sources.SFX.pitch = ((_tick - 1) / 4) % _song.Informations.signature.numerator == 0 ? 2 : 1;
            Sources.SFX.PlayOneShot(metronome);
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