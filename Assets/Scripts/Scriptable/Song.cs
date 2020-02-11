using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Song", order = 1)]
[Serializable]
public class Song : ScriptableObject
{
    [Serializable]
    public struct SongStems
    {
        public AudioClip Bass;
        public AudioClip Drums;
        public AudioClip Melody;
        public AudioClip All;
    }

    [Serializable]
    public struct TimeSignature
    {
        public int numerator;
        public int denominator;
    }

    [Serializable]
    public struct SongInformations
    {
        public TimeSignature signature;
        public float bpm;
    }

    public SongInformations Informations = new SongInformations()
    {
        bpm = 130,
        signature = new TimeSignature() {denominator = 4, numerator = 4}
    };

    public SongStems Stems;
}