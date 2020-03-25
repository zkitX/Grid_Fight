using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{

    private float atkHoldingTimer = 0;
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;
    public bool Atk1Queueing = false;
    [SerializeField] protected bool CharacterJumping = false;



    #region Unity Life Cycles
    public override void Start()
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


    public override void SetCharDead(bool hasToDisappear = true)
    {
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        IsOnField = false;
        BattleManagerScript.Instance.PlayablesCharOnScene.Where(r => r.CName == CharInfo.CharacterID).First().isUsed = false;
        base.SetCharDead(false);
        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            //BattleManagerScript.Instance.UpdateCurrentSelectedCharacters(this, null, UMS.Side);
            NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        }

        StartCoroutine(ReviveSequencer());
    }

    IEnumerator ReviveSequencer()
    {
        float timeElapsed = 0f;
        float timeToWait = CharInfo.CharacterRespawnLength;
        while (timeElapsed != timeToWait)
        {
            if(BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle 
                || BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)
            {
                timeElapsed = Mathf.Clamp(timeElapsed + Time.deltaTime, 0f, timeToWait);
            }
            yield return null;
        }
        CharBackFromDeath();
    }

    public void CharBackFromDeath()
    {
        gameObject.SetActive(true);
        CharInfo.HealthStats.Health = CharInfo.HealthStats.Base;
        CharInfo.ShieldStats.Shield = CharInfo.ShieldStats.Base;
        CharInfo.StaminaStats.Stamina = CharInfo.StaminaStats.Base;
        //SET UI OF THE CHARACTER TO ALIVE HERE
        NewIManager.Instance.ToggleUICharacterDead(this, false);
        //NewIManager.Instance.SetUICharacterToButton(this, CharInfo.CharacterSelection);
    }

    
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
        AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
        EventManager.Instance?.AddCharacterArrival((BaseCharacter)this);
    }

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        isDefending = false;
        EventManager.Instance?.AddCharacterSwitched((BaseCharacter)this);
    }

    #endregion

    #region Attack

    public void SetParticlesLayer(GameObject ps)
    {
        foreach (ParticleSystemRenderer item in ps.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            item.sortingOrder = CharOredrInLayer;
        }
    }


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
                else
                {
                    SetParticlesLayer(ps);
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
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Powerfulattack);
        SetAnimation(CharacterAnimationStateType.Atk2_AtkToIdle);
        ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side);
    }

    public void QuickAttack()
    {
        Atk1Queueing = false;
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

    #region Changing
    /*IEnumerator GridJumpSequencer = null;
    public void StartGridJump(float duration)
    {
        if (GridJumpSequencer != null) StopCoroutine(GridJumpSequencer);
        GridJumpSequencer = GridJumpSequence(duration);
        StartCoroutine(GridJumpSequencer);
    }

    IEnumerator GridJumpSequence(float duration) //WHEN REFACTORING: MAKE THIS CURVE BASED AND NOT 2 LERPS WITH A WEIRD WAIT IN BETWEEN
    {
        CharacterAnimationStateType jumpAnim = CharacterAnimationStateType.DashUp;
        float jumpAnimLength = SpineAnim.GetAnimLenght(jumpAnim);
        float jumpHeight = 2f;
        //float jumpSlowAmount = 1.2f;

        SetAnimation(jumpAnim);

        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + new Vector3(0, jumpHeight, 0);

        float timeCounter = 0f;
        float animProg = 0f;
        float jumpProg = 0f;
        Vector3 newPos = Vector3.zero;
        while(timeCounter != duration)
        {
            timeCounter = Mathf.Clamp(timeCounter + Time.deltaTime, 0f, duration);

            jumpProg = EnvironmentManager.Instance.characterJumpCurve.Evaluate(timeCounter / duration);
            animProg = EnvironmentManager.Instance.jumpAnimationCurve.Evaluate(timeCounter / duration);

            SpineAnim.SetAnimationSpeed(animProg);
            newPos = Vector3.Lerp(startPos, endPos, jumpProg);
            transform.position = new Vector3(transform.position.x, newPos.y, transform.position.z);
            yield return null;
        }
    }*/


    #endregion

    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, ControllerType player)
    {
        NewIManager.Instance.SetSelected(isSelected, player, CharInfo.CharacterID, UMS.Side);   
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

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);

        return base.SetDamage(damage, elemental, isCritical);
    }

}

