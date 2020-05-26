using System;
using UnityEngine;
using UnityEngine.UI;

public class BindingIconButton : MonoBehaviour
{
    [SerializeField] private GameAction action;
    private Image _image;
    private BindingIcons _bindingIcons;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _bindingIcons = UIManager.instance.GetComponent<BindingIcons>();
        _image.sprite = _bindingIcons.GetSprite(GameManager.instance.CurrentBindingScheme, action);
    }

    private void OnEnable()
    {
        UIManager.instance.SchemeChanged += OnSchemeChanged;
    }

    private void OnDisable()
    {
        UIManager.instance.SchemeChanged -= OnSchemeChanged;
    }

    private void OnSchemeChanged(UIManager sender, BindingScheme scheme, string path)
    {
        _image.sprite = _bindingIcons.GetSprite(scheme, action);
    }
}