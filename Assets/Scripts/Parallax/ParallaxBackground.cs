﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    private List<ParallaxLayer> _parallaxLayers = new List<ParallaxLayer>();

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
        _parallaxLayers.Clear();
        GameObject[] ParallaxLayers = GameObject.FindGameObjectsWithTag("parallaxLayer");

        for (int i = 0; i < ParallaxLayers.Length; i++)
        {
            ParallaxLayer layer = ParallaxLayers[i].GetComponent<ParallaxLayer>();
  
            if (layer != null)
            {
                _parallaxLayers.Add(layer);
            }
        }
    }
    void Move(Vector3 delta)
    {
        foreach (ParallaxLayer layer in _parallaxLayers)
        {
            layer.Move(delta);
        }
    }
}
