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
 [SerializeField] private Animator _animatorClipFirstLetter;
 private Shadow _buttonShadow;
 private int _shadowWidth = 8;
 private float _hoverAnimationDuration = 0.3f;
 private VertexGradient _firstLetterGradient;
 private static readonly int StartAnimation = Animator.StringToHash("StartAnimation");
 private static readonly int ResetAnimation = Animator.StringToHash("ResetAnimation");

 private void Awake()
 {
  _buttonShadow = GetComponent<Shadow>();
  _animatorClipFirstLetter = _firstLetterTM.gameObject.GetComponent<Animator>();
  SetButtonText();
 }
 
 public void OnSelect(BaseEventData data)
 {
  AnimateShadow();
  AnimateFirstLetterGradient();
 }
 public void OnDeselect(BaseEventData data)
 {
ResetShadowAnimation();
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

 private void AnimateShadow()
 {
  transform
   .DOBlendableMoveBy(new Vector3(-_shadowWidth/2, _shadowWidth/2, 0), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
  
  DOTween
   .To(()=> _buttonShadow.effectDistance, x=> _buttonShadow.effectDistance = x, new Vector2(_shadowWidth, -_shadowWidth), _hoverAnimationDuration)
   .SetEase(Ease.InOutQuint);
 }

 private void AnimateFirstLetterGradient()
 {
  _animatorClipFirstLetter.SetBool(StartAnimation, true);
 }

 private void ResetFirstLetterGradient()
 {
  _animatorClipFirstLetter.SetBool(StartAnimation, false);
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
