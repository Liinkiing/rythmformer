using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Fields
    [SerializeField] private Animator _SceneTransition;
    [SerializeField] private float transitionTime = 1f;
    #endregion

    public IEnumerator LoadLevel(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        _SceneTransition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        asyncLoad.allowSceneActivation = true;
    }
}
