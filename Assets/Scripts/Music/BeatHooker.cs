using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatHooker : MonoBehaviour
{
    [Serializable]
    private struct SongPart
    {
        [Header("Time")] public bool AllSong;
        [ConditionalField("AllSong", true)] public float From;
        [ConditionalField("AllSong", true)] public float To;

        [Space()] [Header("Events")]
        public UnityEvent OnStep;
        public UnityEvent OnEveryTwoStep;
        public UnityEvent OnFirstAndThirdStep;
        public UnityEvent OnHalfBeat;
        public UnityEvent OnBeat;
    }

    [SerializeField] private List<SongPart> SongParts;

    private enum BeatEvents
    {
        Step,
        EveryTwoStep,
        FirstAndThirdStep,
        HalfBeat,
        Beat
    }

    private readonly Dictionary<SongPart, Dictionary<BeatEvents, Action<SongSynchronizer, EventArgs>>> _handlers =
        new Dictionary<SongPart, Dictionary<BeatEvents, Action<SongSynchronizer, EventArgs>>>();

    private void Awake()
    {
        foreach (var part in SongParts)
        {
            _handlers.Add(part, new Dictionary<BeatEvents, Action<SongSynchronizer, EventArgs>>()
            {
                [BeatEvents.Step] = (manager, args) =>
                {
                    if (part.AllSong ||
                        (manager.Sources.Melody.time >= part.From && manager.Sources.Melody.time <= part.To))
                    {
                        part.OnStep.Invoke();
                    }
                },
                [BeatEvents.EveryTwoStep] = (manager, args) =>
                {
                    if (part.AllSong ||
                        (manager.Sources.Melody.time >= part.From && manager.Sources.Melody.time <= part.To))
                    {
                        part.OnEveryTwoStep.Invoke();
                    }
                },
                [BeatEvents.FirstAndThirdStep] = (manager, args) =>
                {
                    if (part.AllSong ||
                        (manager.Sources.Melody.time >= part.From && manager.Sources.Melody.time <= part.To))
                    {
                        part.OnFirstAndThirdStep.Invoke();
                    }
                },
                [BeatEvents.HalfBeat] = (manager, args) =>
                {
                    if (part.AllSong ||
                        (manager.Sources.Melody.time >= part.From && manager.Sources.Melody.time <= part.To))
                    {
                        part.OnHalfBeat.Invoke();
                    }
                },
                [BeatEvents.Beat] = (manager, args) =>
                {
                    if (part.AllSong ||
                        (manager.Sources.Melody.time >= part.From && manager.Sources.Melody.time <= part.To))
                    {
                        part.OnBeat.Invoke();
                    }
                },
            });
        }
    }

    private void Start()
    {
        foreach (var handler in _handlers)
        {
            FindObjectOfType<SongSynchronizer>().Step += handler.Value[BeatEvents.Step];
            FindObjectOfType<SongSynchronizer>().EveryTwoStep += handler.Value[BeatEvents.EveryTwoStep];
            FindObjectOfType<SongSynchronizer>().FirstAndThirdStep += handler.Value[BeatEvents.FirstAndThirdStep];
            FindObjectOfType<SongSynchronizer>().HalfBeat += handler.Value[BeatEvents.HalfBeat];
            FindObjectOfType<SongSynchronizer>().Beat += handler.Value[BeatEvents.Beat];
        }
    }

    private void OnDestroy()
    {
        foreach (var handler in _handlers)
        {
            FindObjectOfType<SongSynchronizer>().Step -= handler.Value[BeatEvents.Step];
            FindObjectOfType<SongSynchronizer>().EveryTwoStep -= handler.Value[BeatEvents.EveryTwoStep];
            FindObjectOfType<SongSynchronizer>().FirstAndThirdStep -= handler.Value[BeatEvents.FirstAndThirdStep];
            FindObjectOfType<SongSynchronizer>().HalfBeat -= handler.Value[BeatEvents.HalfBeat];
            FindObjectOfType<SongSynchronizer>().Beat -= handler.Value[BeatEvents.Beat];
        }
    }
}