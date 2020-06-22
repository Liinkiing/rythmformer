using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Rythmformer;
using UnityEngine;
using UnityEngine.InputSystem;

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

    #region Events

    public event Action<GameManager, Difficulty> DifficultyChanged;

    #endregion

    #region Public Fields

    [HideInInspector] public GameState state = GameState.InGame;
    [HideInInspector] public bool GamePaused => (state == GameState.Pause || state == GameState.LevelEnd);

    #endregion

    #region Public Structures

    public enum GameState
    {
        MainMenu,
        InGame,
        Pause,
        LevelEnd,
    }

    [Serializable]
    public struct LevelData
    {
        public World World;
        public Level Level;
        [Scene] public string Scene;
    }

    #endregion

    #region Public Fields

    [Space, Header("General")] public List<LevelData> Levels;

    private LevelData _lastUnlockedLevel;
    private PlayerInput _playerInput;

    [HideInInspector] public BindingScheme CurrentBindingScheme = BindingScheme.Keyboard;

    public Difficulty Difficulty => SaveManager.instance.Data.Difficulty;

    #endregion

    public override void Init()
    {
        Debug.Log("[INIT] GameManager");
        LeaderboardManager.instance.WakeServer();
        Debug.Log($"Difficulty: {SaveManager.instance.Data.Difficulty.ToString()}");
        _playerInput = new PlayerInput();
        _playerInput.Global.Switchmode.performed += OnSwitchModeButtonPerformed;
        UpdateLastUnlockedLevel();
    }

    private void OnSwitchModeButtonPerformed(InputAction.CallbackContext obj)
    {
        if (state == GameState.InGame || state == GameState.LevelEnd) return;
        ToggleDifficulty();
        OnDifficultyChanged(this, Difficulty);
    }

    #region Unity Hooks

    private void OnEnable()
    {
        _playerInput?.Enable();
    }

    private void OnDisable()
    {
        _playerInput?.Disable();
    }

    #endregion

    #region Public Methods

    public LevelScoreData GetLocalScore(World world, Level level)
    {
        return SaveManager.instance.Data.LevelScores[world][level];
    }

    public void WriteLocalScore(World world, Level level, LevelScoreData score)
    {
        SaveManager.instance.Data.LevelScores[world][level] = score;
        SaveManager.instance.Save();
    }

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
        OnDifficultyChanged(this, Difficulty);
        Debug.Log($"Changed difficulty to : {difficulty}");
    }

    public void UpdateLastUnlockedLevel()
    {
        if (Levels == null || Levels.Count == 0) return;
        LevelData localLastUnlockedLevel = Levels[0];

        foreach (var levelData in Levels)
        {
            if (HasUnlockedLevel(levelData.World, levelData.Level))
            {
                localLastUnlockedLevel = levelData;
            }
        }

        LastUnlockedLevel = localLastUnlockedLevel;
    }

    public LevelData LastUnlockedLevel
    {
        get => _lastUnlockedLevel;
        set => _lastUnlockedLevel = value;
    }

    #endregion

    protected virtual void OnDifficultyChanged(GameManager sender, Difficulty newDifficulty)
    {
        DifficultyChanged?.Invoke(this, newDifficulty);
    }
}