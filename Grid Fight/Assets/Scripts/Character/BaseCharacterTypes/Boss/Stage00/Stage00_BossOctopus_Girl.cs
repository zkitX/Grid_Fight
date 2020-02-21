using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus_Girl : MinionType_Script
{
   
    public bool CanGetDamage = false;
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();
    public Stage00_BossOctopus BaseBoss;
    public Transform CenteringPoint;


    public override void SetUpEnteringOnBattle()
    {

    }

    public override IEnumerator Move()
    {
        yield return null;
    }

    public override IEnumerator AttackSequence()
    {
        yield return null;
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        yield return new WaitForFixedUpdate();
    }

    public override void CharArrivedOnBattleField()
    {
        BaseBoss.IsCharArrived = true;
        base.CharArrivedOnBattleField();
    }

    public override void SetCharDead()
    {
        if(SpineAnim.CurrentAnim != CharacterAnimationStateType.Death)
        {
            CameraManagerScript.Instance.CameraShake();
            BattleManagerScript.Instance.CurrentBattleState = BattleState.Event;
            ParticleManagerScript.Instance.AttackParticlesFired.ForEach(r => r.PS.SetActive(false));
            ParticleManagerScript.Instance.ParticlesFired.ForEach(r => r.PS.SetActive(false));
            StartCoroutine(DeathStasy());
        }
    }

    private IEnumerator DeathStasy()
    {
        yield return null;
    }

     public override IEnumerator AttackAction(bool yieldBefore)
     {
         yield break;
     }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        if (CanGetDamage)
        {
            return base.SetDamage(damage, elemental, isCritical);
        }
        return false;

    }

    public IEnumerator CenterCharacterToTile(float duration)
    {
        float durationLeft = duration;
        while (durationLeft != 0f)
        {
            durationLeft = Mathf.Clamp(durationLeft - Time.deltaTime, 0f, 100f);
            transform.position = Vector3.Lerp(transform.position, CenteringPoint.position, 1f - (durationLeft / duration));
            yield return null;
        }
    }
}
