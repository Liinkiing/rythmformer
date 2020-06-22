using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerDelay : MonoBehaviour
{
    [SerializeField] private float delay;
    private VideoPlayer _player;

    private void Awake()
    {
        _player = GetComponent<VideoPlayer>();
        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        yield return new WaitForSeconds(delay);
        _player.Play();
    }
    
}