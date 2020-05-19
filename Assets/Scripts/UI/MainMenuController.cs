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
        GameManager.LevelData lastUnlockLevel = GameManager.instance.Levels[0];
        foreach (var levelData in GameManager.instance.Levels)
        {
            if (GameManager.instance.HasUnlockedLevel(levelData.World, levelData.Level))
            {
                lastUnlockLevel = levelData;
            };
            
        }
        _continueLastSaveButton.GetComponent<LevelButtonData>().FillFromLevelData(lastUnlockLevel);
        
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(lastUnlockLevel.Scene));
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
