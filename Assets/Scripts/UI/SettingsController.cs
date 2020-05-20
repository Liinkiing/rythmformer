using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject _sceneTransition;
    
    public void BackToMainMenu()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("MainMenu"));
    }
}
