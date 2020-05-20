using System;
using HttpModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelEnd : MonoBehaviour
{
    private LevelManager _levelManager;
    [SerializeField] private TextMeshProUGUI _bestWorldTimeText;

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
                    _bestWorldTimeText.text = _bestWorldTimeText.text.Replace("{TIME}", entry.timer.ToString("F"));
                })
            .Send();
    }
}