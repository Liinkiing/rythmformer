using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LevelSelector : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private TextMeshProUGUI _chapterTitle;
    [SerializeField] private GameObject _nextChapter;
    [SerializeField] private GameObject _lastChapter;
    [SerializeField] private TextMeshProUGUI _ToggleDifficultyButton;
    private GameObject _buttonWrapper;
    private List<GameObject> _levelButtons;
    private Button _lastChapterButton;
    private TextMeshProUGUI _lastChapterText;
    private Button _nextChapterButton;
    private TextMeshProUGUI _nextChapterText;

    #endregion

    private void OnEnable()
    {
        SaveManager.instance.GameSaved += OnGameSaved;
    }

    private void OnDisable()
    {
        SaveManager.instance.GameSaved -= OnGameSaved;
    }

    private void OnGameSaved(SaveData save)
    {
        RefreshButtons();
    }

    private void Awake()
    {
        _buttonWrapper = GameObject.Find("Levels");
        _levelButtons = new List<GameObject>();
        _lastChapterButton = _lastChapter.GetComponent<Button>();
        _lastChapterText = _lastChapter.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _nextChapterButton = _nextChapter.GetComponent<Button>();
        _nextChapterText = _nextChapter.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GenerateUI(GameManager.instance.LastUnlockedLevel.World);
        RefreshDifficultyButton();
    }

    public void GenerateUI(World chapter)
    {
        RemoveButtons();
        _levelButtons.Clear();
        
        var levelsInChapter = GameManager.instance.Levels.FindAll(data => data.World == chapter);
        int indexChapter = Array.IndexOf(Enum.GetValues(typeof(World)), chapter);
        string lastChapterName = Enum.GetName(typeof(World), indexChapter-1);

        #region Generate last and next chapter buttons
        if (lastChapterName != null)
        {
            _lastChapter.SetActive(true);
            var lastChapter = (World)Enum.Parse(typeof(World), lastChapterName);
            _lastChapterText.SetText($"Chapter {indexChapter - 1}");
            
            _lastChapterButton.onClick.AddListener(() =>
            {
                GenerateUI(lastChapter);
            });
        }
        else
        {
            _lastChapter.SetActive(false);
        }

        string nextChapterName = Enum.GetName(typeof(World), indexChapter+1);
        
        if (nextChapterName != null)
        {
            _nextChapter.SetActive(true);
            var nextChapter = (World)Enum.Parse(typeof(World), nextChapterName);
            _nextChapterText.SetText($"Chapter {indexChapter + 1}");
            
            _nextChapterButton.onClick.AddListener(() =>
            {
                GenerateUI(nextChapter);
            });
        }
        else
        {
            _nextChapter.SetActive(false);
        }

        #endregion
        
        _chapterTitle.SetText($"{(indexChapter > 0 ? "Chapter " + indexChapter : "Prologue")}\n{chapter}");

        foreach (var levelData in levelsInChapter)
        {
            var button = CreateButton($"{levelData.World} - {levelData.Level.ToString()}");
            button.GetComponent<LevelButtonData>().FillFromLevelData(levelData);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(levelData.Scene));
            });
            _levelButtons.Add(button);
            RefreshButtons();
        }
        
        UIManager.instance.SetEventSystemsTarget(_levelButtons[0]);
    }

    private void RefreshDifficultyButton()
    {
        _ToggleDifficultyButton.SetText(SaveManager.instance.Data.Difficulty == Difficulty.Chill ? "Chill Gamer" : "Pro Gamer");
    }
    
    public void ToggleDifficulty()
    {
        GameManager.instance.ToggleDifficulty();
        RefreshDifficultyButton();
    }

    private void OnDestroy()
    {
        _levelButtons.Clear();
    }

    private void RefreshButtons()
    {
        foreach (var go in _levelButtons)
        {
            var button = go.GetComponent<Button>();
            var data = button.GetComponent<LevelButtonData>();
            var hasUnlockedLevel = GameManager.instance.HasUnlockedLevel(data.World, data.Level);
            button.interactable = hasUnlockedLevel;
            button.GetComponent<CanvasGroup>().alpha =
                hasUnlockedLevel ? 1f : 0.4f;
        }
    }

    private GameObject CreateButton(string content)
    {
        var button = Instantiate(_buttonPrefab, _buttonWrapper.transform.position,
            _buttonWrapper.transform.rotation);
        button.transform.SetParent(_buttonWrapper.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = content;
        return button;
    }

    private void RemoveButtons()
    {
        _levelButtons.ForEach(Destroy);
    }
}