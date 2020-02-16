using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    
    #region Fields
    private PlayerInput _input;
    #endregion

    private void Awake()
    {
        _input = new PlayerInput();
    }

    public void Select(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
