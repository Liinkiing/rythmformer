using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoSingleton<UIManager>
{
    public float transitionUIDuration = 0.8f;
    public GameObject _sceneTransition;
    private PlayerInput _input;
    private Sequence transitionSequence;

    public enum UIContainerAction
    {
        Enable,
        Disable,
        Toggle
    }

    void Awake()
    {
        _input = new PlayerInput();
        
        transitionSequence = DOTween.Sequence();
    }
    private void OnEnable()
    {
        _input?.Enable();
    }
    
    private void OnDisable()
    {
        _input?.Disable();
    }

    public void BackToChapter()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    }
    
    public void BackToMainMenu()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("MainMenu"));
    }
    
    public void SetUIContainerState(GameObject UIContainer, UIContainerAction action)
    {
        switch (action)
        {
            case UIContainerAction.Disable:
                UIContainer.SetActive(false);
                break;
            case UIContainerAction.Enable:
                UIContainer.SetActive(true);
                break;
            case UIContainerAction.Toggle:
                UIContainer.SetActive(!UIContainer.activeSelf);
                break;
        }
    }

    public void SetUIContainerStateWithInternalNavigation(
        GameObject UIContainerCurrent, 
        CanvasGroup CanvasGroupCurrent, 
        GameObject UIContainerTarget, CanvasGroup CanvasGroupTarget, 
        GameObject nextEventSytemTarget = null)
    {
        CanvasGroupTarget.blocksRaycasts = true;
        CanvasGroupTarget.interactable = true;
        

        UIContainerTarget.SetActive(true);
        if (nextEventSytemTarget)
        {
            SetEventSystemsTarget(nextEventSytemTarget);    
        }
        
        transitionSequence.Append(
            DOTween
                .To(() => CanvasGroupCurrent.alpha, x => CanvasGroupCurrent.alpha = x, 0, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );
        transitionSequence.Append(
            DOTween
                .To(() => CanvasGroupTarget.alpha, x => CanvasGroupTarget.alpha = x, 1, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );
        transitionSequence.AppendCallback(() =>
        {
            CanvasGroupCurrent.blocksRaycasts = false;
            CanvasGroupCurrent.interactable = false;
            UIContainerCurrent.SetActive(false);
        });
            
        transitionSequence.Play();
    }

    public void SetEventSystemsTarget(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(obj);
    }
}
