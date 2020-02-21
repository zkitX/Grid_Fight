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

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        EventManager.Instance.AddCharacterSwitched((BaseCharacter)this);
    }

    public override void SetAttackReady(bool value)
    {
        StartMoveCo();
        CharInfo.DefenceStats.BaseDefence = Random.Range(0.7f, 1);
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

        

        base.SetAnimation(animState, loop, transition);
    }

    //Basic attack sequence
    public override IEnumerator AttackSequence()
    {
        /*shotsLeftInAttack = 0;
        if (currentAttackPhase != AttackPhasesType.End) yield break;

        yield return null;*/
        shotsLeftInAttack = GetHowManyAttackAreOnBattleField(((ScriptableObjectAttackTypeOnBattlefield)nextAttack).BulletTrajectories);
        Attacking = true;
        if (nextAttack.Anim == CharacterAnimationStateType.Atk)
        {
            //Temporary until anims are added
            
            sequencedAttacker = false;
            chargeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
            SetAnimation(CharacterAnimationStateType.Idle, true);
            currentAttackPhase = AttackPhasesType.Cast_Powerful;
            CreateTileAttack();
        }
        //If it does have the correct animation setup, play that charged animation
        else
        {
            currentAttackPhase = AttackPhasesType.Start;
            sequencedAttacker = true; //Temporary until anims are added
            SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
        }

        while (shotsLeftInAttack != 0)
        {
            yield return null;
        }

        currentAttackPhase = AttackPhasesType.End;
        //attacking = false; //Temporary until anims are added
        yield break;
    }


    public override void fireAttackAnimation()
    {
        //Debug.Log("<b>Shots left in this charge of attacks: </b>" + shotsLeftInAttack);
        if (sequencedAttacker) SetAnimation(CharacterAnimationStateType.Atk1_Loop);
        else SetAnimation(CharacterAnimationStateType.Atk); //Temporary until anims are added
        if (chargeParticles != null && shotsLeftInAttack == 0)
        {
            chargeParticles.SetActive(false);

            chargeParticles = null;
        }
    }


    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        damage = damage * CharInfo.DefenceStats.BaseDefence;
        return base.SetDamage(damage, elemental, isCritical);
    }
}
