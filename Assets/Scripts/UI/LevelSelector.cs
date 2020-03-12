using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class LevelSelector : MonoBehaviour
{
    #region Fields

    [SerializeField] private SceneField[] _Levels;
    [SerializeField] private GameObject _ButtonPrefab;
    [SerializeField] private GameObject SceneTransition;
    private GameObject _ButtonWrapper;

    #endregion

    private void Awake()
    {
        _ButtonWrapper = GameObject.Find("Content");

        // foreach (var LEVEL in _Levels)
        // {

        // }
        foreach (var world in SaveManager.instance.Data.LevelProgression.Keys)
        {
            Debug.Log(world.ToString());
            foreach (var entry in SaveManager.instance.Data.LevelProgression[world])
            {
                var button = CreateButton($"{world} - {entry.Key.ToString()}");
                button.GetComponent<CanvasGroup>().alpha = entry.Value ? 1f : 0.4f;
            }
        }
    }

    private Button CreateButton(string content)
    {
        var button = Instantiate(_ButtonPrefab, _ButtonWrapper.transform.position,
            _ButtonWrapper.transform.rotation);
        button.transform.SetParent(_ButtonWrapper.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = content;
        return button.GetComponent<Button>();
    }
}