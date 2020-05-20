using HttpModel;
using TMPro;
using UnityEngine;


public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    private LevelManager _levelManager;
        
    private void Awake()
    {
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
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
    }
}
