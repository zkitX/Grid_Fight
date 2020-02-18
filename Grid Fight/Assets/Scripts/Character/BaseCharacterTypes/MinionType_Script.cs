using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{

    
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;

   

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
    }

    public override void SetAttackReady(bool value)
    {
        StartMoveCo();
        base.SetAttackReady(value);
    }


    public override void StartMoveCo()
    {
        MoveCoOn = true;
        MoveActionCo = Move();
        StartCoroutine(MoveActionCo);
    }

    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake();
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        StopAllCoroutines();
        base.SetCharDead();
    }

   

    public virtual IEnumerator Move()
    {
        while (true)
        {
            if(MoveCoOn && currentAttackPhase == AttackPhasesType.End && !Attacking)
            {
                float timer = 0;
                float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
                while (timer < MoveTime)
                {
                    yield return new WaitForFixedUpdate();
                    while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || Attacking)
                    {
                        yield return new WaitForFixedUpdate();
                    }

                    timer += Time.fixedDeltaTime;
                }
                if (CharInfo.Health > 0)
                {
                    while (currentAttackPhase != AttackPhasesType.End)
                    {
                        yield return null;
                    }

                    MoveCharOnDirection((InputDirection)Random.Range(0, 4));
                }
                else
                {
                    timer = 0;
                }
            }
            yield return null;
        }
    }


    public override void StopMoveCo()
    {
        MoveCoOn = false;
        if (MoveActionCo != null)
        {
            StopCoroutine(MoveActionCo);
        }
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        if (animState == CharacterAnimationStateType.GettingHit && Attacking)
        {
            return;
        }

        base.SetAnimation(animState, loop, transition);
    }
}
