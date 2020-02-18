using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{

    private float atkHoldingTimer = 0;
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;
    #region Unity Life Cycles
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Setup Character


    public override void StartMoveCo()
    {
        MoveCoOn = true;
        MoveActionCo = Move();
        StartCoroutine(MoveActionCo);
    }

    public virtual IEnumerator Move()
    {
        while (true)
        {
            if (MoveCoOn && currentAttackPhase == AttackPhasesType.End && !Attacking)
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



    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
        UMS.SelectionIndicator.eulerAngles = new Vector3(0, 0, CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
    }


    public override void SetCharDead()
    {
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        if(UMS.CurrentAttackType == AttackType.Particles)
        {
            BattleManagerScript.Instance.UpdateCurrentSelectedCharacters(this, null);
            NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo);
        }
        base.SetCharDead();
    }

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
        AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
        EventManager.Instance.AddCharacterArrival((BaseCharacter)this);
    }

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        EventManager.Instance.AddCharacterSwitched((BaseCharacter)this);
    }

    #endregion

    #region Attack

    public bool Atk1Queueing = false;
    //Load the special attack and fire it if the load is complete
    public IEnumerator StartChargingAttack()
    {
        if (CharInfo.StaminaStats.Stamina - CharInfo.PowerfulAttac.Stamina_Cost_Atk >= 0 
           && CanAttack)
        {
            GameObject ps = null;
            bool isChargingParticlesOn = false;
            isSpecialLoading = true;
            float timer = 0;
            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(CharacterAnimationStateType.Atk2_IdleToAtk);
            SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            while (isSpecialLoading && !VFXTestMode)
            {
                yield return BattleManagerScript.Instance.PauseUntil();
                timer += Time.fixedDeltaTime;

                if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle)
                {
                    SetAnimation(CharacterAnimationStateType.Atk2_IdleToAtk);
                    SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
                }
                if (!isChargingParticlesOn)
                {
                    isChargingParticlesOn = true;
                    ps = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
                    ps.transform.parent = transform;
                   
                }

                if (!IsOnField)
                {
                    if(ps != null)
                    {
                        ps.transform.parent = null;
                        ps.SetActive(false);
                    }
                   
                    yield break;
                }
            }
            if (timer > 1)
            {
                currentAttackPhase = AttackPhasesType.Loading;
                StopPowerfulAtk = SpecialAttackStatus.Start;
                if (IsOnField || VFXTestMode)
                {
                    while (isMoving)
                    {
                        yield return new WaitForEndOfFrame();

                        if(StopPowerfulAtk == SpecialAttackStatus.Stop)
                        {
                            StopPowerfulAtk = SpecialAttackStatus.None;
                            ps.transform.parent = null;
                            ps.SetActive(false);
                            yield break;
                        }
                    }


                    CharInfo.DamageStats.CurrentDamage = CharInfo.PowerfulAttac.BaseDamage;
                    ps.transform.parent = null;
                    ps.SetActive(false);
                    SpecialAttack(CharInfo.CharacterLevel);
                }
            }
            else
            {
                ps.transform.parent = null;
                ps.SetActive(false);
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }
    }



    public void StartQuickAttack(bool attackRegardless)
    {
        if ((CharInfo.StaminaStats.Stamina - CharInfo.RapidAttack.Stamina_Cost_Atk >= 0
           && CanAttack && !isMoving) || attackRegardless)
        {

            if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_Loop && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                CharInfo.DamageStats.CurrentDamage = CharInfo.RapidAttack.BaseDamage;
                SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
                SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            }
            else if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                Atk1Queueing = true;
            }
        }
    }
    



    private IEnumerator AtkHoldingCo()
    {
        while (true)
        {
            yield return BattleManagerScript.Instance.PauseUntil();
            atkHoldingTimer += Time.fixedDeltaTime;
        }
    }


    //Set ste special attack
    public void SpecialAttack(CharacterLevelType attackLevel)
    {
        NextAttackLevel = attackLevel;
        GetAttack(CharacterAnimationStateType.Atk1);
        SetAnimation(CharacterAnimationStateType.Atk2_AtkToIdle);
        ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side);
    }

    public void QuickAttack()
    {
        Atk1Queueing = false;
        GetAttack(CharacterAnimationStateType.Atk);
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_Loop);
    }

    public void ChargingLoop()
    {
        SetAnimation(CharacterAnimationStateType.Atk2_Charging, true);
    }

    public void SecondSpecialAttackStarting()
    {
        Atk1Queueing = false;
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_AtkToIdle, false);
    }


    #endregion

    #region Move

    #endregion

    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, ControllerType player)
    {
        NewIManager.Instance.SetSelected(isSelected, player, CharInfo.CharacterID);   
    }


    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
        

        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle)
        {
            return;
        }

        if (!animState.ToString().Contains("Atk"))
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        base.SetAnimation(animState, loop, transition);
    }

}

