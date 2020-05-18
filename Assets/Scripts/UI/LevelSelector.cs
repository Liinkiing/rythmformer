using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LevelSelector : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject _ButtonPrefab;
    [SerializeField] private GameObject SceneTransition;
    [SerializeField] private TextMeshProUGUI ChapterTitle;
    private GameObject _ButtonWrapper;
    private List<GameObject> _levelButtons;

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
        _ButtonWrapper = GameObject.Find("Levels");
        _levelButtons = new List<GameObject>();
    }

    private void Start()
    {
        var lastUnlockLevel = GameManager.instance.Levels[0];
        foreach (var levelData in GameManager.instance.Levels)
        {
            if (GameManager.instance.HasUnlockedLevel(levelData.World, levelData.Level))
            {
                lastUnlockLevel = levelData;
            };
        }

        var lastUnlockChapter = GameManager.instance.Levels.FindAll(data => data.World == lastUnlockLevel.World);
        int indexLastChapterUnlocked = Array.IndexOf(Enum.GetValues(typeof(World)), lastUnlockLevel.World);

        ChapterTitle.SetText($"{(indexLastChapterUnlocked > 0 ? "Chapter " + indexLastChapterUnlocked : "Prologue")}\n{lastUnlockLevel.World}");
        
        foreach (var levelData in lastUnlockChapter)
        {
            var button = CreateButton($"{levelData.World} - {levelData.Level.ToString()}");
            button.GetComponent<LevelButtonData>().FillFromLevelData(levelData);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(SceneTransition.GetComponent<SceneLoader>().LoadLevel(levelData.Scene));
            });
            _levelButtons.Add(button);
            RefreshButtons();
        }
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
        var button = Instantiate(_ButtonPrefab, _ButtonWrapper.transform.position,
            _ButtonWrapper.transform.rotation);
        button.transform.SetParent(_ButtonWrapper.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = content;
        return button;
    }
}