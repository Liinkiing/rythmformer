using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using HttpModel;
using TMPro;

public class LevelButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public World world;
    public Level level;

    [SerializeField] private CanvasGroup canvasContainerTimer;
    [SerializeField] private TextMeshProUGUI localTimerText;
    [SerializeField] private TextMeshProUGUI worldTimerText;
    
    private LevelSelector _levelSelectorController;
    private float _bestLocalScore;
    private float _bestWorldScore;

    private void Awake()
    {
        _levelSelectorController = GameObject.Find("LevelSelector UI").GetComponent<LevelSelector>();
    }

    private void Start()
    {
        _bestLocalScore = GameManager.instance.GetLocalScore(world, level).Timer;
        
        LeaderboardManager.instance.FetchBestTimerForLevel(world, level)
            .OnError(response => Debug.LogError(response.Text))
            .OnSuccess(
                response =>
                {
                    var entry = JsonUtility.FromJson<ScoreEntry>(response.Text);
                    _bestWorldScore = entry.timer;
                    
                    localTimerText.SetText($"Your best time : {UIManager.instance.FormatTimer(_bestLocalScore)}");
                    worldTimerText.SetText($"World best time : {UIManager.instance.FormatTimer(_bestWorldScore)}");
                })
            .Send();
    }

    public void OnSelect(BaseEventData data)
    {
        int indexSelectedButton = _levelSelectorController._levelButtons.FindIndex(0, o => o == data.selectedObject);
        _levelSelectorController.AnimateSun(indexSelectedButton);
        
        canvasContainerTimer
            .DOFade(1,0.5f)
            .SetEase(Ease.InOutQuint);
    }
    
    public void OnDeselect(BaseEventData data)
    {
        _levelSelectorController.lastSelectedLevelButton = gameObject;
        canvasContainerTimer
            .DOFade(0,0.5f)
            .SetEase(Ease.InOutQuint);
    }
}
