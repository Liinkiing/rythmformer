using System;
using Rythmformer;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoSingleton<UIManager>
{
    public float transitionUIDuration = 0.8f;
    [SerializeField] private GameObject _sceneTransition;
    private PlayerInput _input;
    private int _transitionSequenceID;
    private GameObject _eventSystemTarget;

    public enum UIContainerAction
    {
        Enable,
        Disable,
        Toggle
    }

    void Awake()
    {
        _input = new PlayerInput();
    }
    
    private void OnEnable()
    {
        _input?.Enable();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SetEventSystemsTarget(EventSystem.current.currentSelectedGameObject);
        }
        else
        {
            SetEventSystemsTarget(_eventSystemTarget);
        }
    }

    private void OnDisable()
    {
        _input?.Disable();
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
        GameObject UIContainerTarget, 
        CanvasGroup CanvasGroupTarget, 
        GameObject nextEventSytemTarget = null)
    {
     
        CanvasGroupTarget.blocksRaycasts = true;
        CanvasGroupTarget.interactable = true;
        
        UIContainerTarget.SetActive(true);

        if (nextEventSytemTarget)
        {
            SetEventSystemsTarget(nextEventSytemTarget);    
        }
        
        if (DOTween.IsTweening(_transitionSequenceID))
        {
            DOTween.Kill(_transitionSequenceID);
        }
        
        Sequence transitionSequence =  DOTween.Sequence();
        transitionSequence.Append(
            CanvasGroupCurrent
                .DOFade(0, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );
        transitionSequence.Append(
            CanvasGroupTarget
                .DOFade(1, transitionUIDuration)
                .SetEase(Ease.InOutQuint)
        );

        transitionSequence.AppendCallback(() =>
        {
            Debug.Log("Should disable current container");
            CanvasGroupCurrent.blocksRaycasts = false;
            CanvasGroupCurrent.interactable = false;
            UIContainerCurrent.SetActive(false);
        });
            
        transitionSequence.Play();
        _transitionSequenceID = transitionSequence.intId;
    }

    public void SetEventSystemsTarget(GameObject obj)
    {
        _eventSystemTarget = obj;
        EventSystem.current.SetSelectedGameObject(_eventSystemTarget);
    }

    public void NavigateToScene(string sceneName)
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel(sceneName));
    }
}
