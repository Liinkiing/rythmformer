using System;
using System.Collections.Generic;

[Serializable]
public enum LevelName
{
    Level1,
    Level2,
    Level3,
    Level4
}

[Serializable]
public class SaveData
{
    public Dictionary<LevelName, bool> LevelProgression = new Dictionary<LevelName, bool>()
    {
        [LevelName.Level1] = true,
        [LevelName.Level2] = true,
        [LevelName.Level3] = false,
        [LevelName.Level4] = true,
    };
}