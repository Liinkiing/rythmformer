using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Rythmformer;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    #region Debug


    [Button("Lock all levels")]
    public void LockAllLevels()
    {
        SaveManager.instance.Data.LevelProgression[World.Castle] = SaveManager.instance.Data
            .LevelProgression[World.Castle].ToDictionary(pair => pair.Key, pair => false);
        SaveManager.instance.Data.LevelProgression[World.Forest] = SaveManager.instance.Data
            .LevelProgression[World.Forest].ToDictionary(pair => pair.Key, pair => false);
        SaveManager.instance.Save();
        UpdateLastUnlockedLevel();
        Debug.Log("Locked all levels");
    }

    [Button("Unlock all levels")]
    public void UnlockAllLevels()
    {
        SaveManager.instance.Data.LevelProgression[World.Castle] = SaveManager.instance.Data
            .LevelProgression[World.Castle].ToDictionary(pair => pair.Key, pair => true);
        SaveManager.instance.Data.LevelProgression[World.Forest] = SaveManager.instance.Data
            .LevelProgression[World.Forest].ToDictionary(pair => pair.Key, pair => true);
        SaveManager.instance.Save();
        UpdateLastUnlockedLevel();
        Debug.Log("Unlocked all levels");
    }

    [Button("Lock / Unlock Castle.Tutorial")]
    public void ToggleLockCastleTutorial()
    {
        if (HasUnlockedLevel(World.Castle, Level.Tutorial))
        {
            LockLevel(World.Castle, Level.Tutorial);
        }
        else
        {
            UnlockLevel(World.Castle, Level.Tutorial);
        }
    }

    [Button("Lock / Unlock Forest.Level1")]
    public void ToggleLockForestLevel1()
    {
        if (HasUnlockedLevel(World.Forest, Level.Level1))
        {
            LockLevel(World.Forest, Level.Level1);
        }
        else
        {
            UnlockLevel(World.Forest, Level.Level1);
        }
    }

    [Button("Lock / Unlock Forest.Level2")]
    public void ToggleLockForestLevel2()
    {
        if (HasUnlockedLevel(World.Forest, Level.Level2))
        {
            LockLevel(World.Forest, Level.Level2);
        }
        else
        {
            UnlockLevel(World.Forest, Level.Level2);
        }
    }

    [Button("Lock / Unlock Forest.Level3")]
    public void ToggleLockForestLevel3()
    {
        if (HasUnlockedLevel(World.Forest, Level.Level3))
        {
            LockLevel(World.Forest, Level.Level3);
        }
        else
        {
            UnlockLevel(World.Forest, Level.Level3);
        }
    }

    [Button("Lock / Unlock Forest.Level4")]
    public void ToggleLockForestLevel4()
    {
        if (HasUnlockedLevel(World.Forest, Level.Level4))
        {
            LockLevel(World.Forest, Level.Level4);
        }
        else
        {
            UnlockLevel(World.Forest, Level.Level4);
        }
    }

    [Button("Toggle Difficulty")]
    public void ToggleDifficulty()
    {
        if (SaveManager.instance.Data.Difficulty == Difficulty.Chill)
        {
            ChangeDifficulty(Difficulty.ProGamer);
        }
        else
        {
            ChangeDifficulty(Difficulty.Chill);
        }
    }


    #endregion

    #region Public Structures

    [Serializable]
    public struct LevelData
    {
        public World World;
        public Level Level;
        [Scene] public string Scene;
    }

    #endregion
    
    #region Public Fields

    [Space, Header("General")]
    public List<LevelData> Levels;

    private LevelData _lastUnlockedLevel;
    
    public BindingScheme CurrentBindingScheme = BindingScheme.Keyboard;

    #endregion
    public override void Init()
    {
        Debug.Log("[INIT] GameManager");
        LeaderboardManager.instance.WakeServer();
        Debug.Log($"Difficulty: {SaveManager.instance.Data.Difficulty.ToString()}");
        UpdateLastUnlockedLevel();
    }

    #region Public Methods

    public void UnlockLevel(World world, Level level)
    {
        SaveManager.instance.Data.LevelProgression[world][level] = true;
        SaveManager.instance.Save();
        UpdateLastUnlockedLevel();
        Debug.Log($"Unlocked {world.ToString()}.{level.ToString()}");
    }

    public void LockLevel(World world, Level level)
    {
        SaveManager.instance.Data.LevelProgression[world][level] = false;
        SaveManager.instance.Save();
        UpdateLastUnlockedLevel();
        Debug.Log($"Locked {world.ToString()}.{level.ToString()}");
    }

    public bool HasUnlockedLevel(World world, Level level)
    {
        return SaveManager.instance.Data.LevelProgression[world][level];
    }

    public void ChangeDifficulty(Difficulty difficulty)
    {
        SaveManager.instance.Data.Difficulty = difficulty;
        SaveManager.instance.Save();
        Debug.Log($"Changed difficulty to : {difficulty}");
    }

    public void UpdateLastUnlockedLevel()
    {
        LevelData localLastUnlockedLevel = Levels[0];
        
        foreach (var levelData in Levels)
        {
            if (HasUnlockedLevel(levelData.World, levelData.Level))
            {
                localLastUnlockedLevel = levelData;
            };
        }

        LastUnlockedLevel = localLastUnlockedLevel;
    }

    public LevelData LastUnlockedLevel
    {
        get => _lastUnlockedLevel;
        set => _lastUnlockedLevel = value;
    }

    #endregion
}