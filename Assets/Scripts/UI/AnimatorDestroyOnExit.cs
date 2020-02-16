using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDestroyOnExit : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We keep TextReference to instantiate TextScoreClone at the right place
        if (animator.gameObject.name != "TextScore")
        {
            Destroy(animator.gameObject);  
        }
    }
}
