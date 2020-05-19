using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoSingleton<UIManager>
{
    public float pauseTransitionDuration = 0.8f;
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private Button _continuePauseButton;
    private PlayerInput _input;
    private CanvasGroup _pauseCanvasGroup;
    private LevelManager _levelManager;

    void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _pauseCanvasGroup = _pauseCanvas.GetComponent<CanvasGroup>();
        _input = new PlayerInput();
        _pauseCanvasGroup.alpha = 0;
        _pauseCanvasGroup.blocksRaycasts = false;
        _pauseCanvasGroup.interactable = false;

        _continuePauseButton.onClick.AddListener(() =>
        {
            _levelManager.OnLevelPause?.Invoke();
        });
    }
    private void OnEnable()
    {
        _input?.Enable();
    }
    
    private void OnDisable()
    {
        _input?.Disable();
    }

    public void TogglePauseCanvas()
    {
        _pauseCanvasGroup.blocksRaycasts = _levelManager.isGamePaused;
        _pauseCanvasGroup.interactable = _levelManager.isGamePaused;
        
        DOTween
            .To(() => _pauseCanvasGroup.alpha, x => _pauseCanvasGroup.alpha = x, _levelManager.isGamePaused ? 1 : 0, pauseTransitionDuration)
            .SetEase(Ease.InOutQuint);

        if (_levelManager.isGamePaused)
        {
            EventSystem.current.SetSelectedGameObject(_continuePauseButton.gameObject);
        }
    }

    public void BackToChapter()
    {
        StartCoroutine(SceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    } 
}
