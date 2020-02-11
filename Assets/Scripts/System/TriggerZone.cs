using System;
using System.Collections.Generic;
using UIEventDelegate;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerZone : MonoBehaviour
{
    [Serializable]
    private struct EnterTriggers
    {
        public List<EventDelegate> onEnterLeft;
        public List<EventDelegate> onEnterRight;
        public List<EventDelegate> onEnterTop;
        public List<EventDelegate> onEnterBottom;
    }

    [Serializable]
    private struct ExitTriggers
    {
        public List<EventDelegate> onExitLeft;
        public List<EventDelegate> onExitRight;
        public List<EventDelegate> onExitTop;
        public List<EventDelegate> onExitBottom;
    }

    [SerializeField, TagSelector] private string collidesWith;
    [SerializeField] private List<EventDelegate> onEnter;
    [SerializeField] private List<EventDelegate> onExit;
    [SerializeField] private List<EventDelegate> onStay;
    [Space, SerializeField] private EnterTriggers enterSpecificTriggers;
    [Space, SerializeField] private ExitTriggers exitSpecificTriggers;

    private Collider2D _collider;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var distance = other.Distance(_collider).normal;
        if (collidesWith == "" || other.CompareTag(collidesWith))
        {
            if (onEnter.Count > 0) EventDelegate.Execute(onEnter);
            switch (distance.x)
            {
                case 1.0f:
                    if (enterSpecificTriggers.onEnterLeft.Count > 0)
                        EventDelegate.Execute(enterSpecificTriggers.onEnterLeft);
                    break;
                case -1.0f:
                    if (enterSpecificTriggers.onEnterRight.Count > 0)
                        EventDelegate.Execute(enterSpecificTriggers.onEnterRight);
                    break;
            }

            switch (distance.y)
            {
                case -1.0f:
                    if (enterSpecificTriggers.onEnterTop.Count > 0)
                        EventDelegate.Execute(enterSpecificTriggers.onEnterTop);
                    break;
                case 1.0f:
                    if (enterSpecificTriggers.onEnterBottom.Count > 0)
                        EventDelegate.Execute(enterSpecificTriggers.onEnterBottom);
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var distance = other.Distance(_collider).normal;
        if (collidesWith == "" || other.CompareTag(collidesWith))
        {
            if (onExit.Count > 0) EventDelegate.Execute(onExit);
            switch (distance.x)
            {
                case 1.0f:
                    if (exitSpecificTriggers.onExitRight.Count > 0)
                        EventDelegate.Execute(exitSpecificTriggers.onExitRight);
                    break;
                case -1.0f:
                    if (exitSpecificTriggers.onExitLeft.Count > 0)
                        EventDelegate.Execute(exitSpecificTriggers.onExitLeft);
                    break;
            }

            switch (distance.y)
            {
                case -1.0f:
                    if (exitSpecificTriggers.onExitBottom.Count > 0)
                        EventDelegate.Execute(exitSpecificTriggers.onExitBottom);
                    break;
                case 1.0f:
                    if (exitSpecificTriggers.onExitTop.Count > 0)
                        EventDelegate.Execute(exitSpecificTriggers.onExitTop);
                    break;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (collidesWith == "" || other.CompareTag(collidesWith))
            if (onStay.Count > 0)
                EventDelegate.Execute(onStay);
    }
}