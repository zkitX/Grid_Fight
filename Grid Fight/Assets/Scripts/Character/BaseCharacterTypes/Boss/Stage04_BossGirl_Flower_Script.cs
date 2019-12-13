using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossGirl_Flower_Script : BaseCharacter
{
    private enum MonsterFlowerType
    {
        Head1,
        Head2,
        Head3,
        Head4
    }

    [SerializeField]
    private MonsterFlowerType MonsterFlower;



    protected override void SetCharDead()
    {
        base.SetCharDead();

    }

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Growing);
    }
}


