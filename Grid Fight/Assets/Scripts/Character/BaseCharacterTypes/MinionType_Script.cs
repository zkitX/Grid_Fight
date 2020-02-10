using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{

    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
        }
    }

    public int _shotsLeftInAttack = 0;
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;

    // Temp variables to allow the minions without proper animations setup to charge attacks
    public bool sequencedAttacker = false;
    bool attacking = false;
    GameObject chargeParticles = null;


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
            if(MoveCoOn && currentAttackPhase == AttackPhasesType.End && attacking == false)
            {
                float timer = 0;
                float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
                while (timer < MoveTime)
                {
                    yield return new WaitForFixedUpdate();
                    while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                    {
                        yield return new WaitForFixedUpdate();
                    }

                    timer += Time.fixedDeltaTime;
                }
                if (CharInfo.Health > 0)
                {
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


    private int GetHowManyAttackAreOnBattleField(List<BulletBehaviourInfoClassOnBattleField> bulTraj)
    {
        int res = 0;
        Vector2Int basePos = new Vector2Int(0, GridManagerScript.Instance.YGridSeparator);
        foreach (BulletBehaviourInfoClassOnBattleField item in bulTraj)
        {
            foreach (Vector2Int target in item.BulletEffectTiles)
            {

                Vector2Int posToCheck =  basePos - target;
                if (GridManagerScript.Instance.isPosOnField(posToCheck))
                {
                    res++;
                }
            }
        }

        return res;
    }


    //Basic attack sequence
    public override IEnumerator AttackSequence()
    {
        /*shotsLeftInAttack = 0;
        if (currentAttackPhase != AttackPhasesType.End) yield break;

        yield return null;*/
        shotsLeftInAttack = GetHowManyAttackAreOnBattleField(((ScriptableObjectAttackTypeOnBattlefield)nextAttack).BulletTrajectories);

        if (nextAttack.Anim == CharacterAnimationStateType.Atk)
        {
            //Temporary until anims are added
            attacking = true; 
            sequencedAttacker = false;
            chargeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
            SetAnimation(CharacterAnimationStateType.Idle, true);
            currentAttackPhase = AttackPhasesType.Cast_Powerful;
            CreateAttack();
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
        attacking = false; //Temporary until anims are added
        yield break;
    }

    public void fireAttackAnimation()
    {
        shotsLeftInAttack--;
        //Debug.Log("<b>Shots left in this charge of attacks: </b>" + shotsLeftInAttack);
        if(sequencedAttacker)SetAnimation(CharacterAnimationStateType.Atk1_Loop);
        else SetAnimation(CharacterAnimationStateType.Atk); //Temporary until anims are added
        if (chargeParticles != null && shotsLeftInAttack == 0)
        {
            chargeParticles.SetActive(false);

            chargeParticles = null;
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
        if (animState == CharacterAnimationStateType.GettingHit && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk)
        {
            return;
        }
        base.SetAnimation(animState, loop, transition);
    }
}
