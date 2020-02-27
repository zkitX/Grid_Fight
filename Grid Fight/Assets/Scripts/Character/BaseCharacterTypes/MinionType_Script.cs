using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{


    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;
    private float LastAttackTime;

    public AIType CurrentAI;

    private bool UnderAttack
    {
        get
        {
            if (Time.time > LastAttackTime + 10f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    private bool AIMove = false;

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
        StartAttakCo();
        StartMoveCo();
        CharInfo.DefenceStats.BaseDefence = Random.Range(0.7f, 1);
        base.SetAttackReady(value);
    }

    public override void StartMoveCo()
    {
        MoveCoOn = true;
        if(MoveActionCo != null)
        {
            StopCoroutine(MoveActionCo);

        }
        MoveActionCo = Move();
        StartCoroutine(MoveActionCo);
    }

    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        StopAllCoroutines();
        base.SetCharDead();
    }



    public virtual IEnumerator Move()
    {
        while (true)
        {
            if (MoveCoOn && currentAttackPhase == AttackPhasesType.End && !Attacking)
            {
                float timer = 0;
                float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
                while (timer < MoveTime && !AIMove)
                {
                    yield return null;
                    while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || Attacking)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                    Debug.Log(timer + "    " + MoveTime);
                    timer += Time.deltaTime;
                }
                AIMove = false;
                if (CharInfo.Health > 0)
                {
                    while (currentAttackPhase != AttackPhasesType.End)
                    {
                        yield return null;
                    }
                    InputDirection dir = InputDirection.Up;

                    foreach (var item in BattleManagerScript.Instance.AllCharactersOnField.Where(a=> a.IsOnField).OrderBy(r=> Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)))
                    {
                        dir = item.UMS.CurrentTilePos.x > UMS.CurrentTilePos.x ? InputDirection.Down :
                            item.UMS.CurrentTilePos.x < UMS.CurrentTilePos.x ? InputDirection.Up :
                            item.UMS.CurrentTilePos.y < UMS.CurrentTilePos.y ? InputDirection.Left : InputDirection.Right;
                        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + GridManagerScript.Instance.GetVectorFromDirection(dir));
                        if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                        {
                            break;
                        }
                    }

                    MoveCharOnDirection(dir);
                }
                else
                {
                    timer = 0;
                }
            }
            yield return null;
        }
    }

    protected override void Update()
    {
        base.Update();
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
            bool res = false;

            switch (CurrentAI)
            {
                case AIType.GeneralAI:
                    res = GeneralTestAI();
                    break;
                case AIType.AggressiveAI:
                    res = AggressiveTestAI();
                    break;
            }
            if (res)
            {
                currentAttackPhase = AttackPhasesType.Start;
                sequencedAttacker = true; //Temporary until anims are added
                SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
            }
            else
            {
                Attacking = false;
                shotsLeftInAttack = 0;
            }

        }

        while (shotsLeftInAttack != 0)
        {
            yield return null;
        }

        currentAttackPhase = AttackPhasesType.End;
        //attacking = false; //Temporary until anims are added
        yield break;
    }


    public bool GeneralTestAI()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x && r.IsOnField).FirstOrDefault();
        if (cb != null)
        {
            if (UnderAttack)
            {
                if (CharInfo.HealthPerc > 40)
                {
                    return true;
                }
                else
                {
                    AIMove = false;
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            AIMove = true;
            return false;
        }

    }

    public bool AggressiveTestAI()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
        if (cb != null)
        {
            if (CharInfo.HealthPerc > 20)
            {
                return true;
            }
            else
            {
                AIMove = false;
                return false;
            }
        }
        else
        {
            AIMove = true;
            return false;
        }

    }


    public override void fireAttackAnimation(Vector3 pos)
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


    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {

        if (isAttackBlocking)
        {
            int rand = UnityEngine.Random.Range(0, 100);

            if (rand <= 200)
            {
                Attacking = false;
                shotsLeftInAttack = 0;
            }
        }

        LastAttackTime = Time.time;
        return base.SetDamage(damage, elemental, isCritical);
    }


    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        damage = damage * CharInfo.DefenceStats.BaseDefence;
        return base.SetDamage(damage, elemental, isCritical);
    }
}
