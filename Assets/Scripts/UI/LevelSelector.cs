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
    private GameObject _ButtonWrapper;
     
    #endregion

    private void Awake()
    {
        _ButtonWrapper = GameObject.Find("Content");

        foreach (var LEVEL in _Levels)
        {
            GameObject button = Instantiate(_ButtonPrefab, _ButtonWrapper.transform.position, _ButtonWrapper.transform.rotation);
            button.transform.SetParent(_ButtonWrapper.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = LEVEL.SceneName;
            button.GetComponent<Button>().onClick.AddListener(delegate { Select(LEVEL.ScenePath); });
        }
    }

    public void Select(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
