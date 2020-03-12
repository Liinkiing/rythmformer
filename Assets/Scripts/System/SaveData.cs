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

[Serializable]
public class SaveData
{
    public Dictionary<World, Dictionary<Level, bool>> LevelProgression = new Dictionary<World, Dictionary<Level, bool>>()
    {
        [World.Castle] = new Dictionary<Level, bool>()
        {
            [Level.Tutorial] = false,
        },
        [World.Forest] = new Dictionary<Level, bool>()
        {
            [Level.Level1] = false,
            [Level.Level2] = false,
            [Level.Level3] = false,
            [Level.Level4] = false,
        }
    };
}