using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class HomeButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    
    private Shadow _buttonShadow;
    private readonly int _shadowWidth = 8;
    private readonly float _hoverAnimationDuration = 0.3f;

    private void Awake()
    {
        _buttonShadow = GetComponent<Shadow>();
    }
    
    public void OnSelect(BaseEventData data)
    {
        AnimateShadow();
    }
    public void OnDeselect(BaseEventData data)
    {
        ResetShadowAnimation();
    }
    
    private void AnimateShadow()
    {
        transform
            .DOBlendableMoveBy(new Vector3(-_shadowWidth/2, _shadowWidth/2, 0), _hoverAnimationDuration)
            .SetEase(Ease.InOutQuint);
  
        DOTween
            .To(()=> _buttonShadow.effectDistance, x=> _buttonShadow.effectDistance = x, new Vector2(_shadowWidth, -_shadowWidth), _hoverAnimationDuration)
            .SetEase(Ease.InOutQuint);
    }
    
    private void ResetShadowAnimation()
    {
        transform
            .DOBlendableMoveBy(new Vector3(_shadowWidth/2, -_shadowWidth/2, 0), _hoverAnimationDuration)
            .SetEase(Ease.InOutQuint);
  
        DOTween
            .To(() => _buttonShadow.effectDistance, x => _buttonShadow.effectDistance = x, Vector2.zero, _hoverAnimationDuration)
            .SetEase(Ease.InOutQuint);
    }
}
