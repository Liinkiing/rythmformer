using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Scene]
    public string sceneToLoad;
#if UNITY_EDITOR
    private void Awake()
    {
        if (LoadingSceneIntegration.otherScene > 0)
        {
            SceneManager.LoadScene(LoadingSceneIntegration.otherScene);
        }
    }
#endif

    private void Start()
    {
        if (LoadingSceneIntegration.otherScene < 0)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}