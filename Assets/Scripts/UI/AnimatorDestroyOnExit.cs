using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDestroyOnExit : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We keep SpriteScore to instantiate SpriteScoreClone at the right place
        if (animator.gameObject.name != "SpriteScore" )
        {
            Destroy(animator.gameObject);  
        }
    }
}
