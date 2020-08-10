using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{


    public delegate void CurrentCharSkillCompleted(AttackInputType inputSkill, float duration);
    public event CurrentCharSkillCompleted CurrentCharSkillCompletedEvent;

    protected bool MoveCoOn = true;
    public bool Atk1Queueing = false;
    [SerializeField] protected bool CharacterJumping = false;
    ManagedAudioSource chargingAudio = null;
    ManagedAudioSource chargingAudioStrong = null;
    public float chargingAttackTimer = 0;
    public GameTime battleTime;

    private List<SkillCoolDownClass> skillCoolDown = new List<SkillCoolDownClass>()
    {
        new SkillCoolDownClass(AttackInputType.Skill1, false),
        new SkillCoolDownClass(AttackInputType.Skill2, false),
        new SkillCoolDownClass(AttackInputType.Skill3, false)
    };
    public IEnumerator SkillActivation = null;
    public float LastAxisValue = 0;

    


    #region Unity Life Cycles
    public override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        base.Update();
    }
    #endregion

    #region Setup Character

    /// <summary>
    /// Must be called to set up the battleTime Variable
    /// </summary>
    public void SetBattleTime()
    {
        battleTime = new GameTime();
        battleTime.SetupBasics();
        battleTime.isStopped = true;
        StartCoroutine(battleTime.standardTicker);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        CurrentCharSkillCompletedEvent = null;
    }

    public void CharacterInputHandler(InputActionType action)
    {
        if (!HasBuffDebuff(BuffDebuffStatsType.Rage))
        {
            StartCoroutine(CharacterInputQueue(action));
        }
    }
    IEnumerator CharacterInputQueue(InputActionType action)
    {
        isSpecialStop = false;
        if(action == InputActionType.Defend)
        {
            IsDefStartCo = true;
        }
        while (isMoving)
        {
            yield return null;
        }
        Debug.Log(action);
        switch (action)
        {
            case InputActionType.Weak:
                StartWeakAttack(false);
                break;
            case InputActionType.Strong:
                StartChargingAtk(AttackInputType.Strong);
                break;
            case InputActionType.Skill1:
                StartChargingAtk(AttackInputType.Skill1);
                break;
            case InputActionType.Skill2:
                StartChargingAtk(AttackInputType.Skill2);
                break;
            case InputActionType.Skill3:
                StartChargingAtk(AttackInputType.Skill3);
                break;
            case InputActionType.Defend:
                StartDefending();
                break;
            case InputActionType.Defend_Stop:
                while (IsDefStartCo)
                {
                    yield return null;
                }
                StopDefending();
                break;
            case InputActionType.Move_Up:
                break;
            case InputActionType.Move_Down:
                break;
            case InputActionType.Move_Left:
                break;
            case InputActionType.Move_Right:
                break;
            default:
                break;
        }
    }

    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
            MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
            switch (matchType)
            {
                case MatchType.PvE:
                    UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    break;
                case MatchType.PvP:
                    if (UMS.PlayerController.Contains(ControllerType.Player2))
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    else
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    break;
                case MatchType.PPvE:
                    UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    break;
                case MatchType.PPvPP:
                    if (UMS.PlayerController.Contains(ControllerType.Player3) && UMS.PlayerController.Contains(ControllerType.Player4))
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    else
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    break;
            case MatchType.PPPPvE:
                UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                break;
        }
      //  UMS.SelectionIndicator.eulerAngles = new Vector3(0, 0, CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
       //     CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
      //      CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
        CharInfo.SetupChar();

    }


    public override void SetCharDead()
    {
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        SetAttackReady(false);
        if(battleTime != null)battleTime.isStopped = true;
        base.SetCharDead();
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        ResetAudioManager();
        StartCoroutine(ReviveSequencer());
    }


    public void ResetAudioManager()
    {
        if (chargingAudio != null)
        {
            chargingAudio.ResetSource();
            chargingAudio = null;
        }
        if (chargingAudioStrong != null)
        {
            chargingAudioStrong.ResetSource();
            chargingAudioStrong = null;
        }
    }

    IEnumerator ReviveSequencer()
    {
        float timeElapsed = 0f;
        float timeToWait = CharInfo.CharacterRespawnLength;
        while (timeElapsed != timeToWait)
        {
            if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle
                || BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)
            {
                timeElapsed = Mathf.Clamp(timeElapsed + BattleManagerScript.Instance.DeltaTime, 0f, timeToWait);
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
        if(battleTime == null)
        {
            SetBattleTime();
        }
        battleTime.isStopped = false;
        SetAnimation(CharacterAnimationStateType.Arriving);
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalSpawn, AudioBus.MidPrio);
        //AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
        EventManager.Instance?.AddCharacterArrival(this);
       
    }

    public override void SetUpLeavingBattle()
    {
        if(battleTime != null)
        {
            battleTime.isStopped = true;
        }

        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        isDefendingStop = true;
        isDefending = false;
        CurrentPlayerController = ControllerType.None;
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.ExitBattleJump, AudioBus.HighPrio);
        EventManager.Instance?.AddCharacterSwitched(this);
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

    public void StartChargingAtk(AttackInputType atkType)
    {
        switch (atkType)
        {
            case AttackInputType.Strong:
                if (!CharActionlist.Contains(CharacterActionType.StrongAttack))
                {
                    return;
                }
                StartCoroutine(StartChargingAttack(atkType));
                break;
            case AttackInputType.Skill1:
                if (!CharActionlist.Contains(CharacterActionType.Skill1) || CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill1);
                StartCoroutine(SkillActivation);

                break;
            case AttackInputType.Skill2:
                if (!CharActionlist.Contains(CharacterActionType.Skill2) || CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill2);
                StartCoroutine(SkillActivation);
                break;
            case AttackInputType.Skill3:
                if (!CharActionlist.Contains(CharacterActionType.Skill3) || CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill3);
                StartCoroutine(SkillActivation);
                break;
                
        }
    }

    IEnumerator StartChargingAttack(AttackInputType nextAtkType)
    {
        yield return StartChargingAttack_Co(nextAtkType);
        if (chargingPs != null)
        {
            chargingPs.transform.parent = null;
            chargingPs.SetActive(false);
            chargingPs = null;
        }
        isSpecialStop = false;
        isSpecialLoading = false;
        isChargingParticlesOn = false;
        ResetAudioManager();
    }

    public bool GetCanUseStamina(float valueRequired)
    {
        if (CharInfo.StaminaStats.Stamina - valueRequired >= 0) return true;

        UMS.StaminaBarContainer.GetComponentInChildren<Animation>().Play();
        return false;
    }

    //Load the special attack and fire it if the load is complete
    bool isChargingParticlesOn = false;
    GameObject chargingPs = null;
    public IEnumerator StartChargingAttack_Co(AttackInputType nextAtkType)
    {
        if (CanAttack && !isSpecialLoading)
        {
            ScriptableObjectAttackBase nxtAtk = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == nextAtkType).First();
            if(!GetCanUseStamina(nxtAtk.StaminaCost))
            {
                yield break;
            }
           
            isSpecialLoading = true;
            chargingAttackTimer = 0;
            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
            if (chargingAudio != null)
            {
                chargingAudio.ResetSource();
            }
            if (chargingAudioStrong != null)
            {
                chargingAudioStrong.ResetSource();
            }
            chargingAudio = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoop, AudioBus.MidPrio, transform, true, 1f);
            while (isSpecialLoading)
            {
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
                chargingAttackTimer += BattleManagerScript.Instance.DeltaTime;
                if(chargingAudioStrong == null && chargingAttackTimer >= 1.5f)
                {
                    chargingAudioStrong = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoopStrong, AudioBus.MidPrio, transform, true, 1f);
                }

                if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString())
                {
                    SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk");
                }
                if (!isChargingParticlesOn || chargingPs == null)
                {
                    isChargingParticlesOn = true;
                    //Check
                    chargingPs = ParticleManagerScript.Instance.FireParticlesInPosition(nxtAtk.Particles.CastLoopPS, CharInfo.CharacterID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side, nxtAtk.AttackInput);
                    chargingPs.transform.parent = SpineAnim.transform;
                    chargingPs.transform.localPosition = Vector3.zero;
                }
                else
                {
                    SetParticlesLayer(chargingPs);
                }

                if (!IsOnField)
                {
                    yield break;
                }
            }
            if (chargingAttackTimer > 1f && CharInfo.Health > 0f)
            {
                currentAttackPhase = AttackPhasesType.Loading;
                StopPowerfulAtk = SpecialAttackStatus.Start;
                if (IsOnField)
                {
                    while (isMoving)
                    {
                        yield return null;
                        Debug.Log("Moving");
                        if (StopPowerfulAtk == SpecialAttackStatus.Stop)
                        {
                            StopPowerfulAtk = SpecialAttackStatus.None;
                            yield break;
                        }
                    }
                    SpecialAttack(nxtAtk);
                }
            }
            else
            {
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }
    }

    public void StartWeakAttack(bool attackRegardless)
    {
        if (!CharActionlist.Contains(CharacterActionType.WeakAttack))
        {
            return;
        }
        if (CanAttack || attackRegardless)
        {
            ScriptableObjectAttackBase nxtAtk = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Weak).First();
            if (!GetCanUseStamina(nxtAtk.StaminaCost))
            {
                return;
            }
            Attacking = true;
            lastAttack = false;
            FireActionEvent(CharacterActionType.WeakAttack);
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

    public IEnumerator StartSkillAttack(AttackInputType inputSkill)
    {
        ScriptableObjectAttackBase nxtAtk = null;

        switch (inputSkill)
        {
            case AttackInputType.Skill1:
                nxtAtk = CharInfo.Mask.Skill1;
                break;
            case AttackInputType.Skill2:
                nxtAtk = CharInfo.Mask.Skill2;
                break;
            case AttackInputType.Skill3:
                nxtAtk = CharInfo.Mask.Skill3;
                break;
        }

        SkillCoolDownClass  scdc = skillCoolDown.Where(r => r.Skill == inputSkill).First();
        if (!GetCanUseStamina(nxtAtk.StaminaCost) || scdc.IsCoGoing)
        {
            SkillActivation = null;
            yield break;
        }
        switch (inputSkill)
        {
            case AttackInputType.Skill1:
                FireActionEvent(CharacterActionType.Skill1);

                break;
            case AttackInputType.Skill2:
                FireActionEvent(CharacterActionType.Skill2);

                break;
            case AttackInputType.Skill3:
                FireActionEvent(CharacterActionType.Skill3);

                break;
        }


        nextAttack = nxtAtk;
        scdc.IsCoGoing = true;
        yield return BattleManagerScript.Instance.WaitUpdate(() => currentAttackPhase != AttackPhasesType.End);
        CharInfo.BaseSpeed *= 100;
        BattleManagerScript.Instance.BattleSpeed = 0.01f;
        SpineAnim.SetSkeletonOrderInLayer(300);
        SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
        currentAttackPhase = AttackPhasesType.Start;
        yield return BattleManagerScript.Instance.WaitFor(0.018f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle, ()=> CharInfo.HealthPerc <= 0);
        if(CharInfo.HealthPerc > 0)
        {
            SetAnimation(nxtAtk.PrefixAnim + "_AtkToIdle", false, 0);
        }
        yield return BattleManagerScript.Instance.WaitUpdate(() => (currentAttackPhase != AttackPhasesType.End || CharInfo.HealthPerc <= 0));
        BattleManagerScript.Instance.BattleSpeed = 1;
        SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
        CharInfo.BaseSpeed /= 100;
        float coolDown = nxtAtk.CoolDown;
        CurrentCharSkillCompletedEvent?.Invoke(inputSkill, coolDown);
        yield return BattleManagerScript.Instance.WaitFor(0.5f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        SkillActivation = null;
        yield return BattleManagerScript.Instance.WaitFor(coolDown - 0.5f, ()=> BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        scdc.IsCoGoing = false;
    }


    public override void SetFinalDamage(BaseCharacter attacker, float damage, HitInfoClass hic = null)
    {
        Sic.DamageReceived += damage;
        base.SetFinalDamage(attacker, damage, hic);
    }

    //Set ste special attack
    public void SpecialAttack(ScriptableObjectAttackBase atkType)
    {
        nextAttack = atkType;

        if (chargingAudio != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingRelease, AudioBus.LowPrio, transform);
        }
        FireActionEvent(CharacterActionType.StrongAttack);
        SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");

        ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.CastActivationPS, CharInfo.CharacterID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side, nextAttack.AttackInput);
    }

    public void QuickAttack()
    {
        Atk1Queueing = false;
        nextAttack = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == AttackAnimType.Weak_Atk).First();
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_Loop);
    }



    public void ChargingLoop(string atk)
    {
        SetAnimation(atk + "_Charging", true);
    }

    public void SecondSpecialAttackStarting()
    {
        Atk1Queueing = false;
        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(CharacterAnimationStateType.Atk1_AtkToIdle, false);
    }


    int iter = 0;
    //Create and set up the basic info for the bullet
    public override void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
        if (HasBuffDebuff(BuffDebuffStatsType.Backfire))
        {
            BackfireEffect(NextAttackDamage);
            return;
        }

        iter++;
        // Debug.Log(isSpecialLoading);
        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.SOAttack = nextAttack;
        bs.BulletBehaviourInfo = bulletBehaviourInfo;
        bs.Facing = UMS.Facing;
        bs.Elemental = CharInfo.DamageStats.CurrentElemental;
        bs.Side = UMS.Side;
        bs.isColliding = true;
        bs.CharOwner = this;
        bs.attackAudioType = GetAttackAudio();
        ScriptableObjectAttackEffect[] abAtkBase = new ScriptableObjectAttackEffect[bulletBehaviourInfo.Effects.Count];
        bulletBehaviourInfo.Effects.CopyTo(abAtkBase);
        bs.BulletEffects = abAtkBase.ToList();
        if (!GridManagerScript.Instance.isPosOnFieldByHeight(UMS.CurrentTilePos + bulletBehaviourInfo.BulletDistanceInTile))
        {
            bs.gameObject.SetActive(false);
            return;
        }


        bs.iter = iter;

        if (UMS.Facing == FacingType.Right)
        {
            bs.DestinationTile = new Vector2Int(UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, UMS.CurrentTilePos.y + bulletBehaviourInfo.BulletDistanceInTile.y > 11 ? 11 : UMS.CurrentTilePos.y + bulletBehaviourInfo.BulletDistanceInTile.y);
        }
        else
        {
            bs.DestinationTile = new Vector2Int(UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, UMS.CurrentTilePos.y - bulletBehaviourInfo.BulletDistanceInTile.y < 0 ? 0 : UMS.CurrentTilePos.y - bulletBehaviourInfo.BulletDistanceInTile.y);
        }
      


        if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
        {
            bs.gameObject.SetActive(true);
            bs.StartMoveToTile();
        }
        else
        {
            bs.gameObject.SetActive(false);
        }
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


    public override void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        CastLoopImpactAudioClipInfoClass attackTypeAudioInfo = GetAttackAudio();
        if (e.Data.Name.Contains("StopDefending"))
        {
            SpineAnim.SetAnimationSpeed(0);
        }
        else if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            ArrivingEvent();
        }
        else if (e.Data.Name.Contains("FireCastParticle"))
        {
            if (attackTypeAudioInfo != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackTypeAudioInfo.Cast, AudioBus.LowPrio, transform);
            }

            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
                CreateParticleAttack();
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;

            }
            FireCastParticles();
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;
                CreateParticleAttack();
            }
        }
        else if (e.Data.Name.Contains("FireTileAttack") && !trackEntry.Animation.Name.Contains("Loop"))
        {
            
        }
        else if(e.Data.Name.Contains("EndLoop"))
        {

        }
    }

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {

        if (PlayQueuedAnim()) return;
        //Debug.Log(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "   " + CurrentAnim.ToString());
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
           || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString() || (isMoving && (!trackEntry.Animation.Name.Contains("Dash") && !trackEntry.Animation.Name.Contains("_"))))
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;


        if(completedAnim.Contains("Dash") && NewMovementSystem)
        {
            if(completedAnim.Contains("Intro"))
            {
                string nextAnim = completedAnim.Split('_').First() + "_Loop";
                SpineAnim.SpineAnimationState.SetAnimation(0, nextAnim, false);
                SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.TileMovementTime);
                SpineAnim.CurrentAnim = nextAnim;
                return;
            }

            if (completedAnim.Contains("Loop"))
            {
                if(waitingForNextMove == waitingForNextMoveType.none)
                {
                    string nextAnim = completedAnim.Split('_').First() + "_End";
                    SpineAnim.SpineAnimationState.SetAnimation(0, nextAnim, false);
                    //SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.EndTileMovementSpeed);
                    SpineAnim.CurrentAnim = nextAnim;
                }
                else if (waitingForNextMove == waitingForNextMoveType.none)
                {
                    string nextAnim = completedAnim.Split('_').First() + "_Loop";
                    SpineAnim.SpineAnimationState.SetAnimation(0, nextAnim, false);
                    SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.TileMovementTime);
                    SpineAnim.CurrentAnim = nextAnim;
                }
                return;
            }
        }


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
            transform.position = new Vector3(100, 100, 100);
            SetAttackReady(false);
        }

        if (completedAnim == CharacterAnimationStateType.JumpTransition_OUT.ToString())
        {
            transform.position = new Vector3(100, 100, 100);
            SpineAnim.SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
            SetAttackReady(false);
            return;
        }


        if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString() && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
        {
            QuickAttack();
            return;
        }
        if (completedAnim == CharacterAnimationStateType.Atk1_Loop.ToString() &&
            SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
        {
            if (!lastAttack && Atk1Queueing)
            {
                QuickAttack();
            }
            else if (lastAttack && Atk1Queueing)
            {
                Atk1Queueing = false;
                QuickAttack();
            }
            else
            {
                SetAnimation(CharacterAnimationStateType.Atk1_AtkToIdle);
                SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk1_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            }
            return;
        }
        if (completedAnim.Contains("Atk2") || completedAnim.Contains("S_Buff") || completedAnim.Contains("S_DeBuff") || completedAnim.Contains("Atk3"))
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
            Attacking = false;
        }

        base.SpineAnimationState_Complete(trackEntry);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
        //Debug.Log(animState.ToString());

        if (!animState.ToString().Contains("Atk") && !animState.ToString().Contains("S_DeBuff") && !animState.ToString().Contains("S_Buff"))
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        if((SpineAnim.CurrentAnim.Contains("Atk2_AtkToIdle") || 
            (SpineAnim.CurrentAnim.Contains("S_Buff") && !animState.Contains("S_Buff")) ||
            (SpineAnim.CurrentAnim.Contains("S_DeBuff") && !animState.Contains("S_DeBuff"))) && !animState.Contains("Reverse"))
        {
            return;
        }

        if(animState.Contains("rriv") || animState.Contains("Transition"))
        {
            ResetAudioManager();
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        Sic.HitReceived++;

        damage *= UniversalGameBalancer.Instance.difficulty.enemyDamageScaler;

        NewIManager.Instance.TakeDamageSliceOnCharacter(CharInfo.CharacterID, UMS.Side);

        return base.SetDamage(attacker ,damage, elemental, isCritical);
    }



    public override IEnumerator AI()
    {
        nextAttack = CharInfo.CurrentAttackTypeInfo.Where(r=> r.PrefixAnim == AttackAnimPrefixType.Atk1).First();
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField && CharInfo.Health > 0)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }
                CurrentAIState = CharInfo.AIs[0];
                target = WaveManagerScript.Instance.WaveCharcters.Where(r => r.isActiveAndEnabled && r.IsOnField).ToList().OrderBy(a => (Mathf.Abs(a.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x))).FirstOrDefault();
                   
                SetCurrentAIValues();
                CurrentAIState.ModifyStats(CharInfo);
                AICoolDownOffset = 0;

                int atkChances = Random.Range(0, 100);
                if (target != null && atkChances < AttackWillPerc &&  (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
                {
                    lastAttackTime = Time.time;
                    nextAttackPos = target.UMS.CurrentTilePos;
                    if (possiblePos != null)
                    {
                        possiblePos.isTaken = false;
                        possiblePos = null;
                    }
                    if (s != null)
                    {
                        StopCoroutine(s);
                    }
                    s = AttackSequence();

                    yield return s;
                }
                else
                {
                    if (AreTileNearEmpty())
                    {
                        if (possiblePos == null)
                        {
                            int movementChances = UnityEngine.Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
                            if (TowardMovementPerc > movementChances && (Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderBy(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenBy(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            else if ((Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderByDescending(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenByDescending(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            if (possiblePositions.Count > 0)
                            {
                                found = false;
                                while (!found)
                                {
                                    if (possiblePositions.Count > 0)
                                    {
                                        possiblePos = possiblePositions.First();
                                        if (possiblePos.Pos != UMS.CurrentTilePos)
                                        {
                                            if (possiblePos.BattleTileState == BattleTileStateType.Empty)
                                            {
                                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(UMS.WalkingSide));
                                                if (path != null && path.Length > 0)
                                                {
                                                    found = true;
                                                    Vector2Int move = path[0] - UMS.CurrentTilePos;
                                                    possiblePos.isTaken = true;
                                                    yield return MoveCharOnDir_Co(move == new Vector2Int(1, 0) ? InputDirectionType.Down : move == new Vector2Int(-1, 0) ? InputDirectionType.Up : move == new Vector2Int(0, 1) ? InputDirectionType.Right : InputDirectionType.Left);
                                                }
                                                else
                                                {
                                                    possiblePositions.Remove(possiblePos);
                                                }
                                            }
                                            else
                                            {
                                                possiblePositions.Remove(possiblePos);
                                            }
                                        }
                                        else
                                        {
                                            if (CurrentAIState.IdleMovement)
                                            {
                                                possiblePos = null;
                                                found = true;
                                            }
                                            else
                                            {
                                                if (possiblePositions.Count <= 1)
                                                {
                                                    possiblePos = null;
                                                    found = true;
                                                }
                                                else
                                                {
                                                    possiblePositions.Insert(0, GridManagerScript.Instance.GetFreeBattleTile(possiblePos.WalkingSide));
                                                    yield return null;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        possiblePos = null;
                                        found = true;
                                    }

                                }
                                yield return null;
                            }
                            else
                            {
                                found = true;
                                possiblePos = null;
                            }
                        }
                        else
                        {
                            if (possiblePos.Pos != UMS.CurrentTilePos)
                            {
                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(UMS.WalkingSide));
                                if (path == null || (path != null && path.Length == 1) || possiblePos.Pos == UMS.CurrentTilePos)
                                {
                                    possiblePos.isTaken = false;
                                    possiblePos = null;
                                }
                                if (path.Length > 0)
                                {
                                    Vector2Int move = path[0] - UMS.CurrentTilePos;

                                    yield return MoveCharOnDir_Co(move == new Vector2Int(1, 0) ? InputDirectionType.Down : move == new Vector2Int(-1, 0) ? InputDirectionType.Up : move == new Vector2Int(0, 1) ? InputDirectionType.Right : InputDirectionType.Left);
                                }
                            }
                            else
                            {
                                possiblePos = null;
                            }
                        }

                    }
                    else
                    {
                        if (possiblePos != null)
                        {
                            possiblePos.isTaken = false;
                            possiblePos = null;
                        }
                    }
                }
                yield return null;
            }
            else
            {
                if (possiblePos != null)
                {
                    possiblePos.isTaken = false;
                    possiblePos = null;
                }

            }
        }
    }



    public override IEnumerator AttackSequence()
    {
        yield return CharacterInputQueue(InputActionType.Weak);
        while (Attacking)
        {
            yield return null;
        }
    }
}

public class SkillCoolDownClass
{
    public AttackInputType Skill;
    public bool IsCoGoing;

    public SkillCoolDownClass()
    {

    }

    public SkillCoolDownClass(AttackInputType skill, bool isCoGoing)
    {
        Skill = skill;
        IsCoGoing = isCoGoing;
    }
}