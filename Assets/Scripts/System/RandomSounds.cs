using System;
using System.Collections;
using System.Collections.Generic;
using Rythmformer;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> sounds;
    [SerializeField] private float intervalToPlay = 5f;
    [SerializeField] private float percentageToPlay = 0.3f;


    private void OnEnable()
    {
        InvokeRepeating(nameof(DoPlay), intervalToPlay, intervalToPlay);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(DoPlay));
    }

    private void DoPlay()
    {
        float random = Random.value;
        Debug.Log($"random: {random}");
        if (random < percentageToPlay)
        {
            MusicManager.instance.PlaySFX(sounds.Random(), 0.5f);
        }
    }
}