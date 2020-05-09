using System.Collections;
 
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
public class RippleController : MonoBehaviour
{
    [Range(0, 1), Tooltip("Amount of waves"), SerializeField]
    private float friction = .92f;

    [Tooltip("Strength of effect")]
    public float strength = 4f;
 
    [Tooltip("Speed of ripple waves")]
    public float waveSpeed = 30f;
 
    [Range(0, 50), Tooltip("Amount of waves")]
    public float waveAmount = 10f;
 
    private Coroutine rippleRoutine;
    private Ripple ripple;
    private PostProcessVolume rippleVolume;
 
    private void Start()
    {
        ripple = ScriptableObject.CreateInstance<Ripple>();
        ripple.enabled.Override(false);
        ripple.Amount.Override(0f);
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
        ripple.WaveSpeed.Override(waveSpeed);
        ripple.WaveAmount.Override(waveAmount);
 
        rippleRoutine = StartCoroutine(DoRipple());
    }

    private IEnumerator DoRipple()
    {
        ripple.enabled.Override(true);
 
        float amount = strength;
        while(amount > .5f)
        {
            ripple.Amount.value = amount;
            amount *= friction;
            yield return null;
        }
 
        ripple.enabled.Override(false);
    }
}