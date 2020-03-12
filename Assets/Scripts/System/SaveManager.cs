using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NaughtyAttributes;
using Rythmformer;
using UnityEngine;

public class SaveManager : MonoSingleton<SaveManager>
{
    public event Action<SaveData> GameLoaded;
    public event Action<SaveData> GameSaved;

    [SerializeField] private string filename = "save.dat";
    private string _path;
    private SaveData _save;

    public SaveData Data
    {
        get
        {
            if (_save == null)
            {
                Load();
            }

            return _save;
        }
    }

    public override void Init()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        Load();
    }

    public void Save()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        if (!File.Exists(_path))
        {
            File.WriteAllText(_path, "");
        }

        var bf = new BinaryFormatter();
        var file = File.Create(_path);
        var json = JSONSerializer.Serialize(typeof(SaveData), _save);
        try
        {
            bf.Serialize(file, json);
            Debug.Log("Saved game data to : " + _path);
            GameSaved?.Invoke(_save);
        }
        catch (SerializationException e)
        {
            Debug.LogError($"Failed to serialize. Reason: {e.Message}");
            throw;
        }
        finally
        {
            file.Close();
        }
    }

    public void Load()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        if (!File.Exists(_path))
        {
            Debug.LogWarning("No savefile found. Creating a new one");
            File.WriteAllText(_path, "");
        }

        var bf = new BinaryFormatter();
        var file = File.Open(_path, FileMode.Open);
        try
        {
            if (file.Length == 0)
            {
                file.Close();
                _save = CreateSaveData();
                Save();
            }
            else
            {
                _save = JSONSerializer.Deserialize<SaveData>(typeof(SaveData), (string) bf.Deserialize(file));
            }

            Debug.Log($"Successfully loaded savefile ({_path})");
            GameLoaded?.Invoke(_save);
        }
        finally
        {
            file.Close();
        }
    }

    public void Clear()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
            Debug.Log("Successfully cleared savefile");
        }
        else
        {
            Debug.LogWarning("No savefile found.");
        }

        _save = new SaveData();
    }

    private SaveData CreateSaveData()
    {
        var save = new SaveData {LevelProgression = new Dictionary<World, Dictionary<Level, bool>>()};
        foreach (var entry in GameManager.instance.Levels.GroupBy(levelData => levelData.World)
            .ToDictionary(p => p.Key, p => p.ToDictionary(data => data.Level, data => false)))
        {
            save.LevelProgression.Add(entry.Key, entry.Value);
        }

        return save;
    }

#if UNITY_EDITOR
    [Button("Save")]
    private void DoSave()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        Save();
    }

    [Button("Load")]
    private void DoLoad()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        Load();
    }

    [Button("Clear")]
    private void DoClear()
    {
        _path = $"{Application.persistentDataPath}/{filename}";
        Clear();
    }
#endif
}