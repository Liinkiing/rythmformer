using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using Rythmformer;
using UnityEditor;

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
    [SerializeField] private GameObject _dummySunSprite;
    [SerializeField] private Material _gradientMaterial;
    
    private GameObject _buttonWrapper;
    private Button _lastChapterButton;
    private TextMeshProUGUI _lastChapterText;
    private Button _nextChapterButton;
    private TextMeshProUGUI _nextChapterText;
    private Tween _pathTween;
    public List<GameObject> _levelButtons;
    public GameObject lastSelectedLevelButton;

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
        InitAnimateSun();
    }

    private void InitAnimateSun()
    {
        List<Vector3> listWayPoints = new List<Vector3>();
        Rect rect = _buttonWrapper.GetComponent<RectTransform>().rect;
        
        var endPositionTarget = _levelButtons[3].transform.localPosition;
        var endPositionPoint = new Vector3(endPositionTarget.x, endPositionTarget.y + 45);
        var inControlPoint = new Vector3(rect.center.x, rect.y + rect.height + 80);
        var outControlPoint = inControlPoint;

        listWayPoints.Add(endPositionPoint);
        listWayPoints.Add(inControlPoint);
        listWayPoints.Add(outControlPoint);

        _dummySunSprite.transform.localPosition = _sunSprite.transform.localPosition;

        /*
         * We init a path on a fake gameObject to later use it with DoVirtual.
         * We never create a new tween, we always use this value of _pathTween
         */
        _pathTween = _dummySunSprite.transform
            .DOLocalPath(listWayPoints.ToArray(), 1f, PathType.CubicBezier);
        _pathTween.Pause();
        _pathTween.ForceInit();
    }

    public void AnimateSun(int buttonIndex)
    {
        if (_pathTween != null)
        {
            Rect rect = _buttonWrapper.GetComponent<RectTransform>().rect;

            var pathLength = _pathTween.PathLength();
            var totalAngle = 3 * (Mathf.PI / 5);
            var circleLength = (rect.width / 2) * totalAngle;
            var distanceBetween2Points = (rect.width / 2) * (Mathf.PI / 5);
            var ratio = pathLength / circleLength;
            int indexButtonLastSelected = _levelButtons.FindIndex(0, o => o == lastSelectedLevelButton);

            float GetRatioDistanceOrigin()
            {
                if (indexButtonLastSelected < buttonIndex)
                {
                    return (distanceBetween2Points * (buttonIndex - 1)) / circleLength;
                }

                return (distanceBetween2Points * (indexButtonLastSelected)) / circleLength;

            }

            float GetRatioDistanceTarget()
            {
                if (indexButtonLastSelected < buttonIndex)
                {
                    return (distanceBetween2Points * (buttonIndex)) / circleLength;
                }

                return (distanceBetween2Points * (indexButtonLastSelected - 1)) / circleLength;
            }

            var ratioDistanceOrigin = GetRatioDistanceOrigin();
            var ratioDistanceTarget = GetRatioDistanceTarget();

            float start = Convert.ToSingle(Math.Round(ratioDistanceOrigin / ratio, 2));
            float end = Convert.ToSingle(Math.Round(ratioDistanceTarget / ratio, 2));

            DOVirtual.Float(start, end, 1f, f =>
            {
                Vector3 point = _pathTween.PathGetPoint(f);
                _sunSprite.transform.localPosition = point;
            });
        }

        if (buttonIndex < 1)
        {
            Color32 startColorGreen = new Color32(211, 244, 222, 255);
            Color32 endColorGreen = new Color32(197, 242, 249, 255);
            
            _gradientMaterial.DOColor(startColorGreen, "color_bottom", 1f);
            _gradientMaterial.DOColor(endColorGreen, "color_top", 1f);
        }
        else if (buttonIndex == 1)
        {
            Color32 startColorGreen = new Color32(254, 180, 157, 255);
            Color32 endColorGreen = new Color32(254, 131, 156, 255);
            
            _gradientMaterial.DOColor(startColorGreen, "color_bottom", 1f);
            _gradientMaterial.DOColor(endColorGreen, "color_top", 1f);
        } else
        {
            Color32 startColorPurple = new Color32(39, 51, 38, 255);
            Color32 endColorPurple = new Color32(94, 57, 131, 255);

            _gradientMaterial.DOColor(startColorPurple, "color_bottom", 1f);
            _gradientMaterial.DOColor(endColorPurple, "color_top", 1f);
        }
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
        
        float baseAngle = Mathf.PI / (4 + 1f);
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
    
    private void ResetMaterial()
    {
        Color32 startColorGreen = new Color32(254, 180, 157, 255);
        Color32 endColorGreen = new Color32(254, 131, 156, 255);
            
        _gradientMaterial.DOColor(startColorGreen, "color_bottom", 1f);
        _gradientMaterial.DOColor(endColorGreen, "color_top", 1f);
    }

    private void OnApplicationQuit()
    {
        ResetMaterial();
    }
}