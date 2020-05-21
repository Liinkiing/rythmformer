using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoSingleton<UIManager>
{
    public float transitionUIDuration = 0.8f;
    
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private GameObject _selectDifficultyUI;
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _settingsUI;
    [SerializeField] private CanvasGroup _settingsUICanvasGroup;
    [SerializeField] private GameObject _levelUI;
    [SerializeField] private GameObject _levelEndUI;
    [SerializeField] private CanvasGroup _levelEndCanvasGroup;
    [SerializeField] private GameObject _levelSelectorUI;

    private PlayerInput _input;
    
    void Awake()
    {
        _input = new PlayerInput();
        
        _levelEndCanvasGroup.alpha = 0;
        _levelEndCanvasGroup.blocksRaycasts = false;
        _levelEndCanvasGroup.interactable = false;
        
        _settingsUICanvasGroup.alpha = 0;
        _settingsUICanvasGroup.blocksRaycasts = false;
        _settingsUICanvasGroup.interactable = false;
    }
    private void OnEnable()
    {
        _input?.Enable();
    }
    
    private void OnDisable()
    {
        _input?.Disable();
    }

    public void BackToChapter()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    }
    
    public void BackToMainMenu()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("MainMenu"));
    }

    public void ToggleLevelUI()
    {
        _levelUI.SetActive(_levelUI.activeSelf ? false : true);
    }
    
    public void ToggleLevelEndUI()
    {
        _levelEndCanvasGroup.blocksRaycasts = _levelEndUI.activeSelf ? false : true;
        _levelEndCanvasGroup.interactable = _levelEndUI.activeSelf ? false : true;
        
        if (_levelEndUI.activeSelf)
        {
            Sequence transitionSequence = DOTween.Sequence();
            transitionSequence.AppendCallback(() => _levelEndUI.SetActive(false));
            transitionSequence.Append(
                    DOTween
                        .To(() => _levelEndCanvasGroup.alpha, x => _levelEndCanvasGroup.alpha = x, 0, transitionUIDuration)
                        .SetEase(Ease.InOutQuint)
                    );
            
            transitionSequence.Play();
        }
        else
        {
            _levelEndUI.SetActive(true);
            DOTween
                .To(() => _levelEndCanvasGroup.alpha, x => _levelEndCanvasGroup.alpha = x, 1, transitionUIDuration)
                .SetEase(Ease.InOutQuint);
        }
    }
    public void ToggleSelectDifficultyUI()
    {
        _selectDifficultyUI.SetActive(_selectDifficultyUI.activeSelf ? false : true);
    }
    
    public void ToggleLevelSelectorUI()
    {
        _levelSelectorUI.SetActive(_levelSelectorUI.activeSelf ? false : true);
    }
    
    public void ToggleMainMenuUI()
    {
        _mainMenuUI.SetActive(_mainMenuUI.activeSelf ? false : true);
    }
    
    public void ToggleSettingsUI()
    {
        _settingsUICanvasGroup.alpha = _settingsUI.activeSelf ? 0 : 1;
        _settingsUICanvasGroup.blocksRaycasts = _settingsUI.activeSelf ? false : true;
        _settingsUICanvasGroup.interactable = _settingsUI.activeSelf ? false : true;
        
        _settingsUI.SetActive(_settingsUI.activeSelf ? false : true);
    }

    public void SetEventSystemsTarget(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(obj);
    }
}
