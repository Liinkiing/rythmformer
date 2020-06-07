using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bumper : Trap
{
    [SerializeField, Range(-180f, 180f), Tooltip("Bump angle")]
    private float angle;
    [SerializeField, Tooltip("Bump power")]
    private float power;
    [SerializeField, Tooltip("timing after bump during which controls are locked")]
    private float lockTime;
    [SerializeField, Tooltip("Active on Kick")]
    private bool kick;

    private float _prevAngle;

    public UnityEvent onBump;
    public AudioClip BumpSound;

    private Vector2 _direction;
    private void Start()
    {
        UpdateAngle();
        OnKick = kick;
    }

    protected override void UpdateValues()
    {
        if (angle != _prevAngle)
        {
            UpdateAngle();
        }
        Debug.DrawRay(transform.position, _direction, Color.green);
    }

    protected override void TrapAction()
    {
        base.TrapAction();
        Player.Velocity = _direction * power;
        Player.Bumping = true;
        onBump?.Invoke();
        MusicManager.instance.PlaySFX(BumpSound);
        if (lockTime > 0)
        {
            Player.LockMove(lockTime);
        }
    }

    private void UpdateAngle()
    {
        _prevAngle = angle;
        _direction.x = Mathf.Cos((angle * -1 + 90) / 180 * Mathf.PI);
        _direction.y = Mathf.Sin((angle * -1 + 90) / 180 * Mathf.PI);
    }
}
