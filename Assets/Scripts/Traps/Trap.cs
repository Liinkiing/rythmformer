using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class Trap : MonoBehaviour {
    
    [SerializeField]
    private SongSynchronizer.ThresholdBeatEvents restrictActionOn;
    
    private SongSynchronizer _synchronizer;
    private CharacterController2D _player;
    private Collider2D _hitbox;
    private bool _onTouch = true;
    private bool _onKick = true;
    private bool _trapRecovery;
    private bool _recovering;

    public Collider2D Hitbox
    {
        get => _hitbox;
        set => _hitbox = value;
    }
    public CharacterController2D Player
    {
        get => _player;
        set => _player = value;
    }
    public bool OnTouch
    {
        get => _onTouch;
        set => _onTouch = value;
    }
    public bool OnKick
    {
        get => _onKick;
        set => _onKick = value;
    }
    public bool TrapRecovery
    {
        get => _trapRecovery;
        set => _trapRecovery = value;
    }

    void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _player = Utils.FindObjectOfTypeOrThrow<CharacterController2D>();
        _hitbox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        UpdateValues();
        if (_onTouch && !_onKick && TrapTouched() && !_recovering) TrapAction();
        if (_recovering && !TrapTouched()) _recovering = false;
    }

    protected virtual void TrapAction()
    {
        if (_onTouch && !_onKick && _trapRecovery) _recovering = true;
    }
    protected virtual void UpdateValues()
    {}
    protected virtual void StartHook()
    {}
    protected virtual void MidHook()
    {}
    protected virtual void EndHook()
    {}
    
    private void AddThresholdedBeatEvents()
    {
        if (_synchronizer == null) return;
        switch (restrictActionOn) {
            case SongSynchronizer.ThresholdBeatEvents.Step:
                _synchronizer.StepThresholded += OnTresholdedAction;
                break;
            case SongSynchronizer.ThresholdBeatEvents.HalfBeat:
                _synchronizer.HalfBeatThresholded += OnTresholdedAction;
                break;
            case SongSynchronizer.ThresholdBeatEvents.Beat:
                _synchronizer.BeatThresholded += OnTresholdedAction;
                break;
        }
    }

    private void RemoveThresholdedBeatEvents()
    {
        if (_synchronizer == null) return;
        _synchronizer.StepThresholded -= OnTresholdedAction;
        _synchronizer.HalfBeatThresholded -= OnTresholdedAction;
        _synchronizer.BeatThresholded -= OnTresholdedAction;
    }
    private void OnEnable()
    {
        AddThresholdedBeatEvents();
    }

    private void OnDisable()
    {
        RemoveThresholdedBeatEvents();
    }

    private void OnTresholdedAction(SongSynchronizer sender, SongSynchronizer.EventState state)
    {
        switch (state)
        {
            case SongSynchronizer.EventState.Start:
                StartHook();
                break;
            case SongSynchronizer.EventState.Mid:
                MidHook();
                if (!_onTouch && _onKick || _onTouch && _onKick && TrapTouched()) TrapAction();
                break;
            case SongSynchronizer.EventState.End:
                EndHook();
                break;
        }
    }

    private bool TrapTouched()
    {
        ColliderDistance2D colliderDistance = _hitbox.Distance(_player.BoxCollider);
        return colliderDistance.isOverlapped;
    }
}
