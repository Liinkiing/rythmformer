﻿using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : MonoSingleton<MusicManager>
{

    [Serializable]
    public struct AudioSources
    {
        public AudioSource SFX;
    }
    
    [Space, Header("Audio sources")] [SerializeField]
    public AudioSources Sources;

    public void PlaySFX(AudioClip clip, float volume = 1.2f)
    {
        Sources.SFX.PlayOneShot(clip, volume);
    }
}