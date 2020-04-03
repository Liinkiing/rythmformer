using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float _parallaxFactor;
    [SerializeField] private bool _enableYParallax;
    public void Move(Vector3 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * _parallaxFactor;
        if (_enableYParallax)
        {
            newPos.y -= delta.y * _parallaxFactor;
        }
        transform.localPosition = newPos;
    }
}
