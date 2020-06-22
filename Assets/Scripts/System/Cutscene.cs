using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class Cutscene : MonoBehaviour
{
    [SerializeField] private UnityEvent OnCutsceneEnd;
    private VideoPlayer _player;

    private void Awake()
    {
        _player = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        _player.loopPointReached += OnLoopPointReached;
    }

    private void OnLoopPointReached(VideoPlayer source)
    {
        OnCutsceneEnd?.Invoke();
    }

    private void OnDisable()
    {
        _player.loopPointReached -= OnLoopPointReached;
    }
}