using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    private void Awake()
    {
        if (parallaxCamera == null)
            parallaxCamera = Camera.main.GetComponent<ParallaxCamera>();   
    }

    void Start()
    {
        SetLayers();
    }

    private void OnEnable()
    {
        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate += Move;
    }

    private void OnDisable()
    {
        if (parallaxCamera != null)
            parallaxCamera.onCameraTranslate -= Move;
    }

    void SetLayers()
    {
        parallaxLayers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();
  
            if (layer != null)
            {
                parallaxLayers.Add(layer);
            }
        }
    }
    void Move(Vector3 delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}
