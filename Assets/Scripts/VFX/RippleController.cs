using System.Collections;
 
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
public class RippleController : MonoBehaviour
{
    [SerializeField] private float maxAmount = 25f;
    [SerializeField] private float friction = .95f;
 
    private Coroutine rippleRoutine;
    private Ripple ripple;
    private PostProcessVolume rippleVolume;
 
    private void Start()
    {
        ripple = ScriptableObject.CreateInstance<Ripple>();
        ripple.enabled.Override(false);
        ripple.Amount.Override(0f);
        ripple.WaveAmount.Override(10f);
        ripple.WaveSpeed.Override(15f);
        rippleVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, ripple);
    }
 
    private void OnDestroy()
    {
        StopAllCoroutines();
        if (rippleVolume)
        {
            RuntimeUtilities.DestroyVolume(rippleVolume, true, true);   
        }
    }

    public void startRipple()
    {
        if(rippleRoutine != null)
            StopCoroutine(rippleRoutine);
 
        ripple.CenterX.Override(Screen.width / 2);
        ripple.CenterY.Override(Screen.height / 2);
 
        rippleRoutine = StartCoroutine(DoRipple());
    }

    private IEnumerator DoRipple()
    {
        ripple.enabled.Override(true);
 
        float amount = maxAmount;
        while(amount > .5f)
        {
            ripple.Amount.value = amount;
            amount *= friction;
            yield return null;
        }
 
        ripple.enabled.Override(false);
    }
}