using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class AnimatorEventsScript_BringToIdle : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if(animatorStateInfo.normalizedTime >= 1)
        {
            animator.SetInteger("CharacterState", (int)CharacterAnimationStateType.Idle);
        }
    }
}

