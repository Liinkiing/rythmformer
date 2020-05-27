using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : Trap
{
    [SerializeField, Range(-180f, 180f), Tooltip("Bump angle")]
    private float angle;
    [SerializeField, Tooltip("Bump power")]
    private float power;
    [SerializeField, Tooltip("Active on Kick")]
    private bool kick;

    private float _prevAngle;

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
    }

    private void UpdateAngle()
    {
        _prevAngle = angle;
        _direction.x = Mathf.Cos((angle * -1 + 90) / 180 * Mathf.PI);
        _direction.y = Mathf.Sin((angle * -1 + 90) / 180 * Mathf.PI);
    }
}
