using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : Trap {
    private bool _locked = true;
    public bool Locked
    {
        get => _locked;
    }

    private void Start()
    {
        OnKick = false;
        OnTouch = true;
    }
    protected override void TrapAction()
    {
        base.TrapAction();
        _locked = false;
    }
}
