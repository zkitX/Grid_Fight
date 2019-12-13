using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Flower_Script : BaseCharacter
{
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation((CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), CharacterAnimationStateType.Growing.ToString() + Random.Range(1, 3).ToString()));
    }
}
