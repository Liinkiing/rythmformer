using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseContainer;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _scoreboardButton;
    [SerializeField] private TextMeshProUGUI _UITimer;
    [SerializeField] private GameObject _levelUI;
    [SerializeField] private CanvasGroup _levelUICanvasGroup;
    [SerializeField] private GameObject _levelEndUI;
    [SerializeField] private CanvasGroup _levelEndUICanvasGroup;
    private CanvasGroup _pauseCanvas;
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _pauseCanvas = _pauseContainer.GetComponent<CanvasGroup>();

        _pauseCanvas.alpha = 0;
        _pauseCanvas.blocksRaycasts = false;
        _pauseCanvas.interactable = false;

        _continueButton.onClick.AddListener(() => { _levelManager.OnLevelPause?.Invoke(); });

        _scoreboardButton.onClick.AddListener(() =>
        {
            UIManager.instance.SetUIContainerStateWithInternalNavigation(
                _levelUI,
                _levelUICanvasGroup,
                _levelEndUI,
                _levelEndUICanvasGroup);
        });
    }

    private void Update()
    {
        if (_levelManager.isGamePaused) return;
        _UITimer.SetText($"{_levelManager.TimeElapsed:0.000}");
    }

    public void TogglePauseCanvas()
    {
        _pauseCanvas.blocksRaycasts = _levelManager.isGamePaused;
        _pauseCanvas.interactable = _levelManager.isGamePaused;

        DOTween
            .To(() => _pauseCanvas.alpha, x => _pauseCanvas.alpha = x, _levelManager.isGamePaused ? 1 : 0,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);

        if (_levelManager.isGamePaused)
        {
            GameManager.instance.state = GameManager.GameState.Pause;
            UIManager.instance.SetEventSystemsTarget(_continueButton.gameObject);
        }
        else
        {
            GameManager.instance.state = GameManager.GameState.InGame;
        }
    }
}