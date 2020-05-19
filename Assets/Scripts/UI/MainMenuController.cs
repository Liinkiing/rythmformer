using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private GameObject _continueLastSaveButton;
    [SerializeField] private GameObject _startNewGameButton;

    public void ContinueLastSave()
    {
        
        _continueLastSaveButton.GetComponent<LevelButtonData>().FillFromLevelData(GameManager.instance.LastUnlockedLevel);
        
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(GameManager.instance.LastUnlockedLevel.Scene));
    }

    public void StartNewGame()
    {
        GameManager.instance.LockAllLevels();
        GameManager.instance.UnlockLevel(World.Castle, Level.Tutorial);

        GameManager.LevelData firstLevel = GameManager.instance.Levels[0];
        
        _startNewGameButton.GetComponent<LevelButtonData>().FillFromLevelData(firstLevel);
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(firstLevel.Scene));
    }
}
