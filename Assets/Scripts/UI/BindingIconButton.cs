using System;
using UnityEngine;
using UnityEngine.UI;

public class BindingIconButton : MonoBehaviour
{
    [SerializeField] private GameAction action;
    private Image _image;
    private SpriteRenderer _spriteRenderer;
    private BindingIcons _bindingIcons;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bindingIcons = UIManager.instance.GetComponent<BindingIcons>();
        ChangeSprite(GameManager.instance.CurrentBindingScheme);
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
        ChangeSprite(scheme);
    }

    private void ChangeSprite(BindingScheme scheme)
    {
        if (_image != null)
        {
            _image.sprite = _bindingIcons.GetSprite(scheme, action);
        }

        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = _bindingIcons.GetSprite(scheme, action);
        }
    }
}