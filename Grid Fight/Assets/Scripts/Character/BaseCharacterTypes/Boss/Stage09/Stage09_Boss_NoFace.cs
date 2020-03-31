using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage09_Boss_NoFace : MinionType_Script
{
    public Stage09_Boss_Geisha baseForme;

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        baseForme.SetAnimation(animState, loop, transition);
    }

    public override void StartMoveCo()
    {
        return;
    }

    public override void SetCharDead(bool hasToDisappear = true)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        InteruptAttack();
        baseForme.SetOniForme(false);
    }

    void InteruptAttack()
    {
        Attacking = false;
        shotsLeftInAttack = 0;
        currentAttackPhase = AttackPhasesType.End;
    }

    public override IEnumerator AttackSequence()
    {
        return base.AttackSequence();
    }

    public override IEnumerator AttackAction(bool yieldBefore)
    {
        return base.AttackAction(yieldBefore);
    }

    public override void StopMoveCo()
    {
        return;
    }

    public override IEnumerator Move()
    {
        yield break;
    }
}
