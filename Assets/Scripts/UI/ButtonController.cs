using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonController : MonoBehaviour
{
 [SerializeField] private string _textButton;
 [SerializeField] private TextMeshProUGUI _firstLetterTM;
 [SerializeField] private TextMeshProUGUI _textTM;

 private void Awake()
 {
  SetButtonText();
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
