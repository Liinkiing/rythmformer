using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    public bool enableYParallax;
    public void Move(Vector3 delta)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= delta.x * parallaxFactor;
        if (enableYParallax)
        {
            newPos.y -= delta.y * parallaxFactor;
        }
        transform.localPosition = newPos;
    }
}
