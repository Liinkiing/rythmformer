using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Fields
    [SerializeField] private Animator _SceneTransition;
    [SerializeField] private float transitionTime;
    #endregion

    public IEnumerator LoadLevel(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        _SceneTransition.SetTrigger(Constants.ANIM_SCENE_TRANSITION_START);
        yield return new WaitForSeconds(transitionTime);
        asyncLoad.allowSceneActivation = true;
    }
}
