using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseContainer;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _UITimer;
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
    }

    private void Update()
    {
        if (GameManager.instance.GamePaused) return;
        _UITimer.SetText(_levelManager.TimeElapsed < 0f ? "0.000" : $"{_levelManager.TimeElapsed:0.000}");
    }

    public void TogglePauseCanvas()
    {
        var gamePaused = GameManager.instance.GamePaused;
        _pauseCanvas.blocksRaycasts = gamePaused;
        _pauseCanvas.interactable = gamePaused;

        DOTween
            .To(() => _pauseCanvas.alpha, x => _pauseCanvas.alpha = x, gamePaused ? 1 : 0,
                UIManager.instance.transitionUIDuration)
            .SetEase(Ease.InOutQuint);

        if (gamePaused)
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