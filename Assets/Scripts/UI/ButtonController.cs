using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode]
public class ButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
 [SerializeField] private string _textButton;
 [SerializeField] private TextMeshProUGUI _firstLetterTM;
 [SerializeField] private TextMeshProUGUI _textTM;
 private Shadow _buttonShadow;
 private int _shadowWidth = 8;
 private float _hoverAnimationDuration = 0.3f;

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
 }
 public void OnDeselect(BaseEventData data)
 {
  transform
   .DOBlendableMoveBy(new Vector3(_shadowWidth/2, -_shadowWidth/2, 0), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
  
  DOTween
   .To(() => _buttonShadow.effectDistance, x => _buttonShadow.effectDistance = x, Vector2.zero, _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
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
}
