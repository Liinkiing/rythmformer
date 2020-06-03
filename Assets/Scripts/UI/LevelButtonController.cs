using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class LevelButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image _banner;
    [SerializeField] private GameObject _sun;
    private void Awake()
    {
        _sun = GameObject.Find("Sun");
    }

    public void OnSelect(BaseEventData data)
    {
        _sun.transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 45);
    }
    
    public void OnDeselect(BaseEventData data)
    {
    }
}
