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

    private void Update()
    {
        if (_input.Player.Menu.triggered)
        {
            print("test");
            LoadMenu();
        }
    }

    public void Select(string levelName)
    {
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public void LoadMenu()
    {
        print("Yolo");
        SceneManager.LoadScene("LevelSelector", LoadSceneMode.Additive);
    }
}
