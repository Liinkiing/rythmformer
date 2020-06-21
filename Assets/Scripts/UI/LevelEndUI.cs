using System;
using HttpModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _localTimeText;
    [SerializeField] private TextMeshProUGUI _worldTimeText;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _scoreBox;
    [SerializeField] private GameObject _highscoreContainer;
    [SerializeField] private VerticalLayoutGroup _verticalLayout;
    [SerializeField] private Image stamp;
    private LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        var isPro = GameManager.instance.Difficulty == Difficulty.ProGamer;
        _scoreBox.SetActive(isPro);
        _verticalLayout.padding.top = isPro ? 0 : 120;
    }

    private void Start()
    {
        _levelManager.FinishLevel();
        GameManager.instance.UnlockLevel(_levelManager.Config.World, _levelManager.Config.Level);
        var localScore = GameManager.instance.GetLocalScore(_levelManager.Config.World, _levelManager.Config.Level);
        var hasBestTimer = (Math.Abs(localScore.Timer) < 0.1 || _levelManager.TimeElapsed < localScore.Timer) &&
                           GameManager.instance.Difficulty == Difficulty.ProGamer;
        if (hasBestTimer)
        {
            _highscoreContainer.SetActive(true);
            GameManager.instance.WriteLocalScore(_levelManager.Config.World, _levelManager.Config.Level,
                new LevelScoreData() {Score = 0, Timer = _levelManager.TimeElapsed});
        }

        _localTimeText.text = _localTimeText.text.Replace("{TIME}", UIManager.instance.FormatTimer(_levelManager.TimeElapsed));
        LeaderboardManager.instance.FetchBestTimerForLevel(_levelManager.Config.World, _levelManager.Config.Level)
            .OnError(response => Debug.LogError(response.Text))
            .OnSuccess(
                response =>
                {
                    var entry = JsonUtility.FromJson<ScoreEntry>(response.Text);
                    _worldTimeText.text = _worldTimeText.text.Replace("{TIME}", UIManager.instance.FormatTimer(entry.timer));
                })
            .Send();

        UIManager.instance.SetEventSystemsTarget(_continueButton);
        
        var index = Array.IndexOf(Enum.GetValues(typeof(Level)), _levelManager.Config.Level);
        stamp.sprite = UIManager.instance.stampList[index];
        stamp.gameObject.SetActive(true);
    }
}