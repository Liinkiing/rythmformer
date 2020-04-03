using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour 
{
    public delegate void ParallaxCameraDelegate(Vector3 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
    private Vector3 _oldPosition;
    void Awake()
    {
        _oldPosition = transform.position;
    }
    void Update()
    {
        if (transform.position != _oldPosition)
        {
            if (onCameraTranslate != null)
            {
                Vector3 delta = _oldPosition - transform.position;
                onCameraTranslate(delta);
            }
            _oldPosition = transform.position;
        }
    }
}

