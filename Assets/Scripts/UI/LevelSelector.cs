using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class LevelSelector : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private GameObject _sceneTransition;
    [SerializeField] private TextMeshProUGUI _chapterTitle;
    [SerializeField] private GameObject _nextChapter;
    [SerializeField] private GameObject _lastChapter;
    [SerializeField] private GameObject _sunSprite;
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
        _lastChapterText = _lastChapter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _nextChapterButton = _nextChapter.GetComponent<Button>();
        _nextChapterText = _nextChapter.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GenerateUI(GameManager.instance.LastUnlockedLevel.World);
        
        //TODO: WIP
        //AnimateSun();
    }

    private void AnimateSun()
    {
        int index = 0;
        
        List<Vector3> ListWayPoints = new List<Vector3>();

        foreach (var button in _levelButtons)
        {
            
            /*new Vector3(_levelButtons[1].transform.localPosition.x, _levelButtons[1].transform.localPosition.y + 45),
            new Vector3(_levelButtons[0].transform.localPosition.x, _levelButtons[0].transform.localPosition.y + 80),  
            new Vector3(_levelButtons[1].transform.localPosition.x - 80, _levelButtons[1].transform.localPosition.y + 45),*/

            /*if (index != 0 && index != _levelButtons.Count - 1)*/
            if (index == 1)
            {
                Vector3 targetPoint = new Vector3(_levelButtons[index].transform.localPosition.x,
                    _levelButtons[index].transform.localPosition.y + 45);
            
                Vector3 inControlPoint = new Vector3(_levelButtons[index-1].transform.localPosition.x,
                    _levelButtons[index-1].transform.localPosition.y + 80);
            
                Vector3 outControlPoint = new Vector3(_levelButtons[index].transform.localPosition.x - 80,
                    _levelButtons[index].transform.localPosition.y + 45);

                ListWayPoints.Add(targetPoint);
                ListWayPoints.Add(inControlPoint);
                ListWayPoints.Add(outControlPoint);
            }

            index++;
        }
        


        _sunSprite.transform.DOLocalPath(ListWayPoints.ToArray(), 2, PathType.Linear);
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
            _lastChapterText.SetText($"{indexChapter - 1}");
            
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
            _nextChapterText.SetText($"{indexChapter + 1}");
            
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

        int index = 0;
        foreach (var levelData in levelsInChapter)
        {
            GameObject button = CreateButton(levelData.Level.ToString(), index);
            button.GetComponent<LevelButtonData>().FillFromLevelData(levelData);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(levelData.Scene));
            });
            _levelButtons.Add(button);
            RefreshButtons();
            index++;
        }

        GameObject targetButton = _levelButtons[0];
        Vector3 targetButtonLocalPosition = targetButton.transform.localPosition; 
        Rect targetButtonRect = targetButton.GetComponent<RectTransform>().rect;
        
        _sunSprite.transform.localPosition = new Vector3(targetButtonLocalPosition.x, targetButtonLocalPosition.y + targetButtonRect.height/2 ,0);
        
        UIManager.instance.SetEventSystemsTarget(_levelButtons[0]);
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

    private GameObject CreateButton(string content, int index)
    {
        Rect rect = _buttonWrapper.GetComponent<RectTransform>().rect;
        
        float baseAngle = Mathf.PI * (0 + 1) / (4 + 1f);
        float heightFactor = (Mathf.Sin(baseAngle) * rect.height) / rect.height;

        float angle = Mathf.PI * (4 - index) / (4 + 1f);
        float x = Mathf.Cos(angle) * rect.width/2;
        float y = Mathf.Sin(angle) * rect.height/heightFactor - (Mathf.Sin(baseAngle) * rect.height/heightFactor);

        x = Mathf.Round(x);
        y = Mathf.Round(y);

        var button = Instantiate(_buttonPrefab, _buttonWrapper.transform.position,
            _buttonWrapper.transform.rotation);
        
        button.transform.SetParent(_buttonWrapper.transform);
        button.transform.localScale = Vector3.one;
        button.transform.localPosition = new Vector3(x, y, 0);

        button.GetComponentInChildren<TextMeshProUGUI>().text = content;
        return button;
    }

    private void RemoveButtons()
    {
        _levelButtons.ForEach(Destroy);
    }
}