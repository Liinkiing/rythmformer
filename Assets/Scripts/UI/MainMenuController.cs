using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private GameObject _continueLastSaveButton;
    [SerializeField] private GameObject _startNewGameButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_continueLastSaveButton);
    }

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

    public void OpenSettings()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("Settings"));
    }
}
