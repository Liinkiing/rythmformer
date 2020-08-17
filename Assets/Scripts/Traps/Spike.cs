using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spike : Trap {

    [SerializeField, Tooltip("Number of kicks between each toggle")]
    private int toggleOnKick;
    [SerializeField, Tooltip("Spike state on initialization")]
    private bool activeOnStart = true;
    
    private HealthPool _health;
    private bool _active = true;
    private int _kicksDone;

    private void Start()
    {
        //
        OnKick = false;
        OnTouch = true;
        TrapRecovery = false;
        _active = activeOnStart;
        _health = Player.gameObject.GetComponent<HealthPool>();
    }

    protected override void MidHook()
    {
        if (toggleOnKick > 0)
        {
            _kicksDone++;
            if (toggleOnKick == _kicksDone)
            {
                _kicksDone = 0;
                _active = !_active;
            }
        }
    }
    protected override void TrapAction()
    {
        if (_active)
        {
            base.TrapAction();
            _health.Damage();
        }
    }
}
