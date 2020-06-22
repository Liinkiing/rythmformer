using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public const string FirstCutsceneSceneName = "Scenes/Main/cutscene_1";
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private GameObject _continueLastSaveButton;
    [SerializeField] private GameObject _startNewGameButton;
    [SerializeField] private GameObject _settingsButton;
    [SerializeField] private CanvasGroup _settingsUICanvasGroup;
    [SerializeField] private CanvasGroup _mainMenuUICanvasGroup;
    [SerializeField] private GameObject _settingsChillButton;
    [SerializeField] private GameObject _settingsProGamerlButton;
    [SerializeField] private Button _backToHomeButton;

    private void Awake()
    {
        GameManager.instance.state = GameManager.GameState.MainMenu;
    }

    private void Start()
    {
        UIManager.instance.SetEventSystemsTarget(_continueLastSaveButton);
        
        _settingsButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.instance.SetUIContainerStateWithInternalNavigation(
                _mainMenuUI,
                _mainMenuUICanvasGroup,
                _settingsUI, 
                _settingsUICanvasGroup,
                SaveManager.instance.Data.Difficulty == Difficulty.Chill ? _settingsChillButton : _settingsProGamerlButton
                );
        });
        
        _backToHomeButton.onClick.AddListener(() =>
        {
            UIManager.instance.SetUIContainerStateWithInternalNavigation(
                _settingsUI, 
                _settingsUICanvasGroup,
                _mainMenuUI,
                _mainMenuUICanvasGroup,
                _settingsButton
            );
        });
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

        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(FirstCutsceneSceneName));
    }

}
