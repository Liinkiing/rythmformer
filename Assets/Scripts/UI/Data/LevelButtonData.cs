using System;
using UnityEngine;

public class LevelButtonData : DataHolder<LevelButtonData>
{
    public World World;
    public Level Level;

    public void FillFromLevelData(GameManager.LevelData data)
    {
        World = data.World;
        Level = data.Level;
    }
}