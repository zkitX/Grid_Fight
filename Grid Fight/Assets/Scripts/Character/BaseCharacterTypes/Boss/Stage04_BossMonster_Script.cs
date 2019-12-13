using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Script : BaseCharacter
{
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
    }
}
