using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ChapterButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Direction _direction;
    [SerializeField] GameObject _stripBackground;
    [SerializeField] private int _padding = 30;

    private Vector3 _initialLocalPosition;
    [SerializeField] private HorizontalLayoutGroup _layoutGroup;
    
    private enum Direction
    {
        Left,
        Right
    }

    private void Awake()
    {
        _initialLocalPosition = transform.localPosition;
        

        SetStyleWithDirection();
    }

    public void OnSelect(BaseEventData data)
    {
        if (_direction == Direction.Left)
        {
            var targetPosition = new Vector3(_initialLocalPosition.x - 50, _initialLocalPosition.y, _initialLocalPosition.z);
            transform
                .DOLocalMove(targetPosition, 0.5f)
                .SetEase(Ease.InOutQuint);
        }
        if (_direction == Direction.Right)
        {
            var targetPosition = new Vector3(_initialLocalPosition.x + 50, _initialLocalPosition.y, _initialLocalPosition.z);
            transform
                .DOLocalMove(targetPosition, 0.5f)
                .SetEase(Ease.InOutQuint);
        }
    }
    
    public void OnDeselect(BaseEventData data)
    {
        transform
            .DOLocalMove(_initialLocalPosition, 0.5f)
            .SetEase(Ease.InOutQuint);
    }

    private void SetStyleWithDirection()
    {
        var localScale = _stripBackground.transform.localScale;
        var padding = _layoutGroup.padding;
        
        if (_direction == Direction.Right)
        {
            localScale = new Vector3(1, localScale.y, localScale.z);
            _stripBackground.transform.localScale = localScale;

            padding.right = _padding;
            padding.left = 0;
        }
        else
        {
            localScale = new Vector3(-1, localScale.y, localScale.z);
            _stripBackground.transform.localScale = localScale;

            padding.right = 0;
            padding.left = _padding;
        }
    }
    
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            SetStyleWithDirection(); 
        }
    }
}
