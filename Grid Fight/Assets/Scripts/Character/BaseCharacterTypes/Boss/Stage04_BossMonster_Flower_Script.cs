using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Flower_Script : BaseCharacter
{
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Growing);
    }
}
