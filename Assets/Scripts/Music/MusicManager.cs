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

    public void PlaySFX(AudioClip clip)
    {
        Sources.SFX.pitch = 1f;
        Sources.SFX.PlayOneShot(clip, 1.2f);
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1.2f)
    {
        Sources.SFX.pitch = 1f;
        Sources.SFX.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1.2f, float pitch = 1f)
    {
        Sources.SFX.pitch = pitch;
        Sources.SFX.PlayOneShot(clip, volume);
    }

    public void PlaySFX(AudioClip clip, float volume = 1.2f, float pitch = 1f, float delay = 0f)
    {
        StartCoroutine(DoPlay(clip, volume, pitch, delay));
    }

    private IEnumerator DoPlay(AudioClip clip, float volume = 1.2f, float pitch = 1f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        Sources.SFX.pitch = pitch;
        Sources.SFX.PlayOneShot(clip, volume);
    }
}