using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

[ExecuteInEditMode]
public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
 [SerializeField] private string _textButton;
 [SerializeField] private TextMeshProUGUI _firstLetterTM;
 [SerializeField] private TextMeshProUGUI _textTM;
 private Shadow _buttonShadow;
 private int _shadowWidth = 8;
 private float _hoverAnimationDuration = 0.3f;
 private VertexGradient _firstLetterGradient;
 private Color32 green = new Color32(118, 227, 178, 255);
 private Color32 lightPurple = new Color32(192, 115, 255, 255);
 private DG.Tweening.Core.TweenerCore<Color, Color, ColorOptions> _tween;

 private void Awake()
 {
  _buttonShadow = GetComponent<Shadow>();
  SetButtonText();
 }
 
 public void OnSelect(BaseEventData data)
 {
  transform
   .DOBlendableMoveBy(new Vector3(-_shadowWidth/2, _shadowWidth/2, 0), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
  
  DOTween
   .To(()=> _buttonShadow.effectDistance, x=> _buttonShadow.effectDistance = x, new Vector2(_shadowWidth, -_shadowWidth), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
  
  AnimateFirstLetterGradient();
 }
 public void OnDeselect(BaseEventData data)
 {
  transform
   .DOBlendableMoveBy(new Vector3(_shadowWidth/2, -_shadowWidth/2, 0), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
  
  DOTween
   .To(() => _buttonShadow.effectDistance, x => _buttonShadow.effectDistance = x, Vector2.zero, _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);

  ResetFirstLetterGradient();
 }

 private void OnValidate()
 {
  if (!Application.isPlaying)
  {
   SetButtonText(); 
  }
 }

 private void SetButtonText()
 {
  string firstLetter = _textButton.Substring(0, 1);
  string text = _textButton.Substring(1, _textButton.Length - 1);
  
  _firstLetterTM.SetText(firstLetter);
  _textTM.SetText(text);
 }

 private void AnimateFirstLetterGradient()
 {

  var stepAnimation = 1;
  var targetColor = green;
  var color = lightPurple;

  _tween = DOTween
    .To(
     () => color, 
     x => color = x,
     targetColor,
     1f
     )
    .SetEase(Ease.Linear)
    .SetLoops(-1);

  _tween.OnUpdate(() => callback(color, stepAnimation));
  
  _tween.OnStepComplete(() =>
  {

   stepAnimation += 1;

   if (stepAnimation > 4) stepAnimation = 1;

   switch (stepAnimation)
   {
    case 1:
     targetColor = green;
     color = lightPurple;
     break;
    
    case 2:
     targetColor = green;
     color = lightPurple;
     break;
    
    case 3:
     targetColor = lightPurple;
     color = green;
     break;
    
    case 4:
     targetColor = lightPurple;
     color = green;
     break;
   }

   _tween.ChangeStartValue(color);
   _tween.ChangeEndValue(targetColor);
  });
 }

 private void callback(Color32 targetColor, int stepAnimation)
 {
  switch (stepAnimation)
  {
   case 1:
    _firstLetterGradient.topLeft = lightPurple;
    _firstLetterGradient.topRight = lightPurple;
    _firstLetterGradient.bottomLeft = targetColor;
    _firstLetterGradient.bottomRight = targetColor;
    break;
    
   case 2:
    _firstLetterGradient.topLeft = targetColor;
    _firstLetterGradient.topRight = targetColor;
    _firstLetterGradient.bottomLeft = green;
    _firstLetterGradient.bottomRight = green;
    break;
    
   case 3:
    _firstLetterGradient.topLeft = green;
    _firstLetterGradient.topRight = green;
    _firstLetterGradient.bottomLeft = targetColor;
    _firstLetterGradient.bottomRight = targetColor;
    break;
    
   case 4:
    _firstLetterGradient.topLeft = targetColor;
    _firstLetterGradient.topRight = targetColor;
    _firstLetterGradient.bottomLeft = lightPurple;
    _firstLetterGradient.bottomRight = lightPurple;
    break;
  }

  _firstLetterTM.colorGradient = _firstLetterGradient;
 }

 private void ResetFirstLetterGradient()
 {
  _tween.Kill();
  
  _firstLetterGradient.bottomLeft = lightPurple;
  _firstLetterGradient.bottomRight = lightPurple;
  _firstLetterGradient.topLeft = lightPurple;
  _firstLetterGradient.topRight = lightPurple;
  
  _firstLetterTM.colorGradient = _firstLetterGradient;
 }
}
