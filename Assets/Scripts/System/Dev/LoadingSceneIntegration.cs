using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneIntegration
{
    public static int otherScene = -2;
#if UNITY_EDITOR

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitLoadingScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 0) return;

        otherScene = sceneIndex;
        SceneManager.LoadScene(0);
    }
#endif
}