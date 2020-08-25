using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus_Girl_Script 
{
  /*  public Stage00_BossOctopus_Script bossParent;
    public bool CanGetDamage = false;
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();
    public Stage00_BossOctopus_Script BaseBoss;
    public Transform CenteringPoint;


    public override void SetUpEnteringOnBattle()
    {

    }

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        StartCoroutine(CheckHasLeftBattle());
        EventManager.Instance.AddCharacterSwitched((BaseCharacter)this);
    }

    IEnumerator CheckHasLeftBattle()
    {
        while(SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            yield return null;
        }
        gameObject.SetActive(false);
        bossParent.OctopusGirlLeaves();
    }

    //public override IEnumerator AttackSequence()
    //{
    //    yield return null;
    //}

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        yield return null;
    }

    public override void CharArrivedOnBattleField()
    {
        BaseBoss.IsCharArrived = true;
        base.CharArrivedOnBattleField();
    }

    public override void SetCharDead()
    {

    }


    private IEnumerator DeathStasy()
    {
        yield return null;
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (CanGetDamage)
        {
            return base.SetDamage(attacker, damage, elemental, isCritical, isAttackBlocking);
        }
        return false;

    }

    public IEnumerator CenterCharacterToTile(float duration)
    {
        float durationLeft = duration;
        Vector3 StartPos = transform.position;
        while (durationLeft != 0f)
        {
            durationLeft = Mathf.Clamp(durationLeft - BattleManagerScript.Instance.DeltaTime, 0f, 100f);
            transform.position = Vector3.Lerp(StartPos, CenteringPoint.position, 1f - (durationLeft / duration));
            yield return null;
        }
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        switch (animState)
        {
            case (CharacterAnimationStateType.Idle):
                transition = 0.3f;
                break;
            default:
                break;
        }
        base.SetAnimation(animState, loop, transition);
    }*/

}
