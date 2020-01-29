using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{

    private float atkHoldingTimer = 0;

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
        base.SetCharDead();
    }

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
        SFXmanager.Instance.PlayOnce(SFXmanager.Instance.ArrivingSpawn);
        
    }

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
    }

    #endregion

    #region Attack

    public bool Atk1Queueing = false;
    public bool AtkCharging = false;
    //Load the special attack and fire it if the load is complete
    public IEnumerator LoadSpecialAttack()
    {
        //Debug.Log(SpineAnim.CurrentAnim.ToString() + isSpecialLoading);


        if (CharInfo.StaminaStats.Stamina - CharInfo.StaminaStats.Stamina_Cost_S_Atk01 >= 0 
            && !isSpecialLoading && CanAttack)
        {

            GameObject ps = null;
            bool isChargingParticlesOn = false;
            isSpecialLoading = true;
            float timer = 0;
            AtkCharging = true;

            if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_Loop && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
                SpineAnim.SetAnimationSpeed(2);
            }
            else if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                Atk1Queueing = true;
            }

            while (isSpecialLoading && !VFXTestMode && !isSpecialQueueing)
            {
                yield return BattleManagerScript.Instance.PauseUntil();
                timer += Time.fixedDeltaTime;
                if (timer > 3 && !isChargingParticlesOn)
                {
                    isChargingParticlesOn = true;
                    ps = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
                }
            }
            AtkCharging = false;
            if (timer > 3)
            {
                currentAttackPhase = AttackPhasesType.Loading;
                if (IsOnField || VFXTestMode)
                {
                    while (isMoving)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    ps.SetActive(false);
                    //ps = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side);
                    SpecialAttack(CharInfo.CharacterLevel);
                }
            }
            else if (timer > 0.5f)
            {
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
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
    }

    public void SpecialAttackLoop()
    {
        Atk1Queueing = false;
        GetAttack(CharacterAnimationStateType.Atk);
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_Loop);
    }

    public void SecondSpecialAttackChargingLoop()
    {
        currentAttackPhase = AttackPhasesType.Loading;
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
    public void SetCharSelected(bool isSelected,Sprite big, Sprite small, Color selectionIndicatorColorSelected)
    {
        if(isSelected)
        {
            UMS.IndicatorAnim.SetBool("indicatorOn", true);
            UMS.SelectionIndicatorSprite.color = selectionIndicatorColorSelected;
            UMS.SelectionIndicatorPlayerSmall.color = selectionIndicatorColorSelected;
            UMS.SelectionIndicatorPlayerNumberSmall.color = selectionIndicatorColorSelected;
            UMS.SelectionIndicatorPlayerNumberBig.sprite = big;
            UMS.SelectionIndicatorPlayerNumberSmall.sprite = small;
        }
        else
        {
            UMS.IndicatorAnim.SetBool("indicatorOn", false);
            UMS.SelectionIndicatorSprite.color = UMS.SelectionIndicatorColorUnselected;
        }

        
    }

}

