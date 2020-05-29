using HttpModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _scoreBox;
    [SerializeField] private VerticalLayoutGroup _verticalLayout;
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
        LeaderboardManager.instance.FetchBestTimerForLevel(_levelManager.Config.World, _levelManager.Config.Level)
            .OnError(response => Debug.LogError(response.Text))
            .OnSuccess(
                response =>
                {
                    var entry = JsonUtility.FromJson<ScoreEntry>(response.Text);
                    _scoreText.text = _scoreText.text.Replace("{TIME}", entry.timer.ToString("F"));
                })
            .Send();
        
        UIManager.instance.SetEventSystemsTarget(_continueButton);
    }
}
