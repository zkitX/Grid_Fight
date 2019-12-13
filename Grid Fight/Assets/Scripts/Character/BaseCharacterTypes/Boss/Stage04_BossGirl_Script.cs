using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossGirl_Script : BaseCharacter
{

    public GameObject Flower1;
    public GameObject Flower2;
    public GameObject Flower3;
    public GameObject Flower4;

 //   private List

 /*   public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }*/

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        SetAnimation(CharacterAnimationStateType.Idle);

        yield return new WaitForSecondsRealtime(3);




        yield return null;
    }
}
