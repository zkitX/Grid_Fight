using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class Stage02_Boss_Script : MinionType_Script
{
    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if(animState == "S_Buff_IdleToAtk")
        {
            animState = "Paste";
        }

        if (animState.Contains("Atk1"))
        {
            animState = animState.Replace("Atk1", "Undo");
        }
        if (animState.Contains("Atk2"))
        {
            animState = animState.Replace("Atk2", "Copy");
        }
        if (animState.Contains("Atk3"))
        {
            animState = animState.Replace("Atk3", "Redo");
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }
}
