using System;
using HttpModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Duck.Http.Service;

public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _localTimeText;
    [SerializeField] private TextMeshProUGUI _worldTimeText;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _scoreBox;
    [SerializeField] private GameObject _highscoreContainer;
    [SerializeField] private VerticalLayoutGroup _verticalLayout;
    [SerializeField] private Image stamp;
    [SerializeField] private GameObject rays;
    [SerializeField] private AudioClip endSfx;
    private LevelManager _levelManager;
    private SongSynchronizer _synchronizer;
    private Image _raysImage;

    private float _stepRaysAnimationDuration;
    private Vector3 _inScale = new Vector3(1.5f, 1.5f, 1);
    private Vector3 _outScale = new Vector3(1.7f, 1.7f, 1);
    private float _inAlpha = 0.6f;
    private float _outAlpha = 1f;
    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _raysImage = rays.GetComponent<Image>();
        var isPro = GameManager.instance.Difficulty == Difficulty.ProGamer;
        _scoreBox.SetActive(isPro);
        _verticalLayout.padding.top = isPro ? 0 : 120;
        
        _stepRaysAnimationDuration = (_synchronizer.song.Informations.bpm / 60f) / (_synchronizer.song.Informations.signature.denominator*2);
    }

    private void Start()
    {
        _levelManager.FinishLevel();
        MusicManager.instance.PlaySFX(endSfx, volume: 1.2f, delay: 0.45f);
        GameManager.instance.UnlockLevel(_levelManager.Config.NextWorld, _levelManager.Config.NextLevel);
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
        LeaderboardManager.instance.PostTimerForLevel(_levelManager.Config.World, _levelManager.Config.Level, _levelManager.TimeElapsed)
            .OnError(response =>
            {
                Debug.LogError(response.Text);
                FetchBestTimer(response);
            })
            .OnSuccess(
                response =>
                {
                    Debug.Log("Successfully persisted your score... Fetching latest data");
                    FetchBestTimer(response);
                })
            .Send();

        UIManager.instance.SetEventSystemsTarget(_continueButton);
        
        var index = Array.IndexOf(Enum.GetValues(typeof(Level)), _levelManager.Config.Level);
        Debug.Log($"Choosing stamp {index}");
        stamp.sprite = UIManager.instance.stampList[index];
        Debug.Log(UIManager.instance.stampList[index]);
        stamp.gameObject.SetActive(true);
    }

    private void FetchBestTimer(HttpResponse response)
    {
        LeaderboardManager.instance.FetchBestTimerForLevel(_levelManager.Config.World, _levelManager.Config.Level)
            .OnError(r => Debug.LogError(r.Text))
            .OnSuccess(
                r =>
                {
                    var entry = JsonUtility.FromJson<ScoreEntry>(r.Text);
                    _worldTimeText.text = _worldTimeText.text = $"{UIManager.instance.FormatTimer(entry.timer)}sc";
                })
            .Send();
    }


    private void OnEnable()
    {
        _synchronizer.Step += OnEveryTwoStep;
    }

    private void OnDisable()
    {
        _synchronizer.Step -= OnEveryTwoStep;
    }

    private void OnEveryTwoStep(SongSynchronizer sender, EventArgs evt)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rays.transform.DOScale(_inScale, _stepRaysAnimationDuration));
        sequence.Join(_raysImage.DOFade(_inAlpha, _stepRaysAnimationDuration));
        sequence.Append(rays.transform.DOScale(_outScale, _stepRaysAnimationDuration));
        sequence.Join(_raysImage.DOFade(_outAlpha, _stepRaysAnimationDuration));
    }
}