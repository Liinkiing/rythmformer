using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private GameObject _continueLastSaveButton;
    [SerializeField] private GameObject _startNewGameButton;
    [SerializeField] private GameObject _settingsButton;
    [SerializeField] private CanvasGroup _settingsUICanvasGroup;
    [SerializeField] private CanvasGroup _mainMenuUICanvasGroup;
    [SerializeField] private GameObject _settingsChillButton;
    [SerializeField] private GameObject _settingsProGamerlButton;
    

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
        _mainMenuUICanvasGroup.blocksRaycasts = false;
        _mainMenuUICanvasGroup.interactable = false;
        
        _settingsUICanvasGroup.blocksRaycasts = true;
        _settingsUICanvasGroup.interactable = true;
        
        EventSystem.current.SetSelectedGameObject(SaveManager.instance.Data.Difficulty == Difficulty.Chill ? _settingsChillButton : _settingsProGamerlButton);
        
        DOTween
            .To(() => _settingsUICanvasGroup.alpha, x => _mainMenuUICanvasGroup.alpha = x, 0,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);
        DOTween
            .To(() => _mainMenuUICanvasGroup.alpha, x => _settingsUICanvasGroup.alpha = x, 1,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);
    }
    
    public void CloseSettings()
    {
        _settingsUICanvasGroup.blocksRaycasts = false;
        _settingsUICanvasGroup.interactable = false;
        
        _mainMenuUICanvasGroup.blocksRaycasts = true;
        _mainMenuUICanvasGroup.interactable = true;
        
        EventSystem.current.SetSelectedGameObject(_settingsButton);
        
        DOTween
            .To(() => _settingsUICanvasGroup.alpha, x => _settingsUICanvasGroup.alpha = x, 0,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);
        DOTween
            .To(() => _mainMenuUICanvasGroup.alpha, x => _mainMenuUICanvasGroup.alpha = x, 1,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);
    }
}
