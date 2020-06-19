using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class DifficultyButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Sprite _activatedSprite;
    [SerializeField] private Sprite _disabledSprite;
    [SerializeField] private Image _imageDifficulty;
    [SerializeField] private Material _gradientMaterial;
    private Shadow _buttonShadow;
    private readonly int _shadowWidth = 8;
    private readonly float _hoverAnimationDuration = 0.3f;
    private Material _cloneMaterial;
    private Color green = new Color32(114, 240,193, 255);
    private Color blue = new Color32(144, 152,232,255);
    private Sequence gradientSequence;

    private void Awake()
    {
        _buttonShadow = GetComponent<Shadow>();
        
        _cloneMaterial = new Material(_gradientMaterial);
        GetComponent<Image>().material = _cloneMaterial;
        
        _cloneMaterial.SetColor("color_top", blue);
        _cloneMaterial.SetColor("color_bottom", green);
    }

    public void OnSelect(BaseEventData data)
    {
        ResetShadowAnimation();
        _imageDifficulty.sprite = _activatedSprite;
        
        transform
            .DOScale(new Vector3(1.1f,1.1f,1), 0.5f)
            .SetEase(Ease.InOutQuint);
        
        gradientSequence = DOTween.Sequence();
        gradientSequence.Append(_cloneMaterial.DOFloat(0.5f, "gradient_position", 1f));
        gradientSequence.Append(_cloneMaterial.DOFloat(1.5f, "gradient_position", 1f));
        gradientSequence.SetLoops(-1);
    }
    
    public void OnDeselect(BaseEventData data)
    {
        AnimateShadow();
        _imageDifficulty.sprite = _disabledSprite;
        
        transform
            .DOScale(new Vector3(1,1,1), 0.5f)
            .SetEase(Ease.InOutQuint);
        
        gradientSequence.Pause();
        gradientSequence.Kill();
        _cloneMaterial.DOFloat(1.5f, "gradient_position", 1f);
    }
    
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            _imageDifficulty.sprite = _activatedSprite;
        }
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
