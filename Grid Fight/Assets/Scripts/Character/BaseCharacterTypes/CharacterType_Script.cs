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
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalSpawn, AudioBus.MediumPriority);
        //AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
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


    IEnumerator charging_Co = null;
    public void StartChargingAtk(AttackAnimType atkType)
    {
        /*  if (charging_Co != null)
          {
              StopCoroutine(charging_Co);
          }

          charging_Co = StartChargingAttack(atkType);
          StartCoroutine(charging_Co);*/


        StartCoroutine(StartChargingAttack(atkType));
    }

    //Load the special attack and fire it if the load is complete
    public float chargingAttackTimer = 0;
    bool isChargingParticlesOn = false;

    public IEnumerator StartChargingAttack(AttackAnimType nextAtkType)
    {
        if (CharInfo.StaminaStats.Stamina - CharInfo.PowerfulAttac.Stamina_Cost_Atk >= 0
           && CanAttack && !isSpecialLoading)
        {
            AttackAnimPrefixType atkType = AttackAnimPrefixType.Atk2;
            switch (nextAtkType)
            {

                case AttackAnimType.Powerful_Atk:
                    atkType = AttackAnimPrefixType.Atk2;
                    break;
                case AttackAnimType.Skill1:
                    atkType = AttackAnimPrefixType.S_Buff;
                    break;
                case AttackAnimType.Skill2:
                    atkType = AttackAnimPrefixType.S_DeBuff;
                    break;
            }
            GameObject ps = null;
            isSpecialLoading = true;
            chargingAttackTimer = 0;

            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(atkType + "_IdleToAtk", false, 0);

            while (isSpecialLoading && !VFXTestMode)
            {
                yield return BattleManagerScript.Instance.PauseUntil();
                chargingAttackTimer += Time.fixedDeltaTime;

                if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString())
                {
                    SetAnimation(atkType + "_IdleToAtk");
                }
                if (!isChargingParticlesOn || ps == null)
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
                    if (ps != null)
                    {
                        ps.transform.parent = null;
                        ps.SetActive(false);
                    }

                    yield break;
                }
            }
            if (chargingAttackTimer > 1)
            {
                currentAttackPhase = AttackPhasesType.Loading;
                StopPowerfulAtk = SpecialAttackStatus.Start;
                if (IsOnField || VFXTestMode)
                {
                    while (isMoving)
                    {
                        yield return new WaitForEndOfFrame();

                        if (StopPowerfulAtk == SpecialAttackStatus.Stop)
                        {
                            StopPowerfulAtk = SpecialAttackStatus.None;
                            ps.transform.parent = null;
                            ps.SetActive(false);
                            yield break;
                        }
                    }
                    SpecialAttack(CharInfo.CharacterLevel, nextAtkType);
                }
            }
            else
            {
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
            ps.transform.parent = null;
            ps.SetActive(false);
        }
    }



    public void StartQuickAttack(bool attackRegardless)
    {
        if ((CharInfo.StaminaStats.Stamina - CharInfo.RapidAttack.Stamina_Cost_Atk >= 0
           && CanAttack && !isMoving) || attackRegardless)
        {

            if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_Loop.ToString() && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
            {
                SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
                SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            }
            else if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
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
    public void SpecialAttack(CharacterLevelType attackLevel, AttackAnimType atkType)
    {
        NextAttackLevel = attackLevel;
        nextAttack = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == atkType).First();
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Powerfulattack);

        if (chargingAudio != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingRelease, AudioBus.LowPriority, transform);
            chargingAudio.ResetSource();
            chargingAudio = null;
        }

        SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");

        ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side);
    }

    public void QuickAttack()
    {
        Atk1Queueing = false;
        nextAttack = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == AttackAnimType.Rapid_Atk).First();
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_Loop);
    }

    public ManagedAudioSource chargingAudio = null;
    public void ChargingLoop(string atk)
    {
        if(chargingAudio == null) chargingAudio = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoop, AudioBus.MediumPriority, transform, true);
        SetAnimation(atk + "_Charging", true);
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

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        //Debug.Log(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "   " + CurrentAnim.ToString());
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
           || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;


        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString())
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            for (int i = 0; i < UMS.Pos.Count; i++)
            {
                GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
                UMS.Pos[i] = Vector2Int.zero;
            }
            SetAttackReady(false);
            transform.position = new Vector3(100, 100, 100);
            return;
        }

      
        if (completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            SetAttackReady(false);
        }


        if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString() && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
        {
            QuickAttack();
            return;
        }
        if (completedAnim == CharacterAnimationStateType.Atk1_Loop.ToString() &&
            SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
        {
            if (Atk1Queueing)
            {
                QuickAttack();
            }
            else
            {
                SetAnimation(CharacterAnimationStateType.Atk1_AtkToIdle);
                SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk1_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            }
            return;
        }
        if (completedAnim.Contains("Atk2") || completedAnim.Contains("S_Buff") || completedAnim.Contains("S_Debuff") || completedAnim.Contains("Atk3"))
        {
            if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.ToString().Contains("IdleToAtk"))
            {
                string[] res = completedAnim.Split('_');
                ChargingLoop(res.Length == 2 ? res.First() : res[0] + "_" + res[1]);
                return;
            }
        }
       
        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        base.SpineAnimationState_Complete(trackEntry);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        if (!animState.ToString().Contains("Atk"))
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        if (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Atk2_AtkToIdle.ToString()) && !animState.Contains(CharacterAnimationStateType.Defeat_ReverseArrive.ToString()))
        {
            return;
        }


        base.SetAnimation(animState, loop, transition);
    }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);

        return base.SetDamage(damage, elemental, isCritical);
    }

}

