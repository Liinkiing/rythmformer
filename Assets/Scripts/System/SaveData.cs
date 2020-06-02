using System;
using System.Collections.Generic;

[Serializable]
public enum Level
{
    Tutorial,
    Level1,
    Level2,
    Level3,
    Level4
}

public enum World
{
    Castle,
    Forest
}

public enum Difficulty
{
    Chill,
    ProGamer
}

public struct LevelScoreData
{
    public float Timer;
    public int Score;
}

[Serializable]
public class SaveData
{
    public Dictionary<World, Dictionary<Level, bool>> LevelProgression;
    public Dictionary<World, Dictionary<Level, LevelScoreData>> LevelScores;
    public Difficulty Difficulty;
}