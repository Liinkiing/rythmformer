using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    public float pauseTransitionDuration = 0.8f;
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private Button _continuePauseButton;
    private PlayerInput _input;
    private CanvasGroup _pauseCanvasGroup;
    private bool _isPauseCanvasDisplayed;
    private LevelManager _levelManager;

    void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _pauseCanvasGroup = _pauseCanvas.GetComponent<CanvasGroup>();
        _input = new PlayerInput();
        _pauseCanvasGroup.alpha = 0;
        _pauseCanvasGroup.blocksRaycasts = false;
        _isPauseCanvasDisplayed = false;
        
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
        _pauseCanvasGroup.blocksRaycasts = !_pauseCanvasGroup.blocksRaycasts;

        DOTween
            .To(() => _pauseCanvasGroup.alpha, x => _pauseCanvasGroup.alpha = x, _isPauseCanvasDisplayed ? 0 : 1, pauseTransitionDuration)
            .SetEase(Ease.InOutQuint);
        
        _isPauseCanvasDisplayed = !_isPauseCanvasDisplayed;
    }

    public void BackToChapter()
    {
        StartCoroutine(SceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    } 
}
