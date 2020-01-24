using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{
    public float shotsLeftInAttack = 0f;
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
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
        base.SetCharDead();
    }

   

    public virtual IEnumerator Move()
    {
        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForFixedUpdate();
        }
        while (MoveCoOn && currentAttackPhase == AttackPhasesType.End)
        {
            float timer = 0;
            float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
            while (timer < 1)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForFixedUpdate();
                }

                timer += Time.fixedDeltaTime / MoveTime;
            }
            if (CharInfo.Health > 0)
            {
                MoveCharOnDirection((InputDirection)Random.Range(0, 4));
            }
        }
    }

    

    //Basic attack sequence
    public override IEnumerator AttackSequence()
    {
        shotsLeftInAttack = ((ScriptableObjectAttackTypeOnBattlefield)nextAttack).BulletTrajectories.Count;

        if(nextAttack.Anim == CharacterAnimationStateType.Atk) base.AttackSequence();
        //If it does have the correct animation setup, play that charged animation
        else
        {
            SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
        }

        while(shotsLeftInAttack != 0)
        {
            yield return null;
        }

        currentAttackPhase = AttackPhasesType.End;
        yield return null;
    }

    public void fireAttackAnimation()
    {
        shotsLeftInAttack--;
        SetAnimation(CharacterAnimationStateType.Atk1_Loop);
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
        if (animState == CharacterAnimationStateType.GettingHit && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk)
        {
            return;
        }
        base.SetAnimation(animState, loop, transition);
    }
}
