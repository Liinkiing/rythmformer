using System;
using System.Collections;
using System.Collections.Generic;
using UIEventDelegate;
using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    public List<EventDelegate> OnCollide;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("COlide");
        if (OnCollide.Count > 0) EventDelegate.Execute(OnCollide);
    }
}
