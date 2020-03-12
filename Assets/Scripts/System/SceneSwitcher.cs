using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField, Scene] private string loadScene;

    private void Start()
    {
        SceneManager.LoadScene(loadScene);
    }
}