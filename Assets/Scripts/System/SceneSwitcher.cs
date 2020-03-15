using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    
#if UNITY_EDITOR 
    private void Awake()
    {

        if (LoadingSceneIntegration.otherScene > 0)
        {
            SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
        }
    }
#endif
}