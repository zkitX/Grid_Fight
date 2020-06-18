using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{
    protected bool MoveCoOn = true;
    private IEnumerator MoveActionCo;
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

    public void CharacterInputHandler(InputActionType action)
    {
        if (CharacterInputQueuer != null) StopCoroutine(CharacterInputQueuer);
        CharacterInputQueuer = CharacterInputQueue(action);
        StartCoroutine(CharacterInputQueuer);
    }

    IEnumerator CharacterInputQueuer = null;
    IEnumerator CharacterInputQueue(InputActionType action)
    {
        isSpecialStop = false;

        while (isMoving)
        {
            yield return null;
        }

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
                break;
            case InputActionType.Defend:
                StartDefending();
                break;
            case InputActionType.Defend_Stop:
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
        UMS.SelectionIndicator.eulerAngles = new Vector3(0, 0, CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
    }


    public override void SetCharDead(bool hasToDisappear = true)
    {
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        IsOnField = false;
        battleTime.isStopped = true;
        base.SetCharDead(false);
        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            //BattleManagerScript.Instance.UpdateCurrentSelectedCharacters(this, null, UMS.Side);
            NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        }
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
        EventManager.Instance?.AddCharacterArrival((BaseCharacter)this);
       
    }

    public override void SetUpLeavingBattle()
    {
        battleTime.isStopped = true;
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        isDefending = false;

        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.ExitBattleJump, AudioBus.HighPrio);
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
                if (!CharActionlist.Contains(CharacterActionType.Skill1))
                {
                    return;
                }
                StartCoroutine(StartSkillAttack(AttackInputType.Skill1));

                break;
            case AttackInputType.Skill2:
                if (!CharActionlist.Contains(CharacterActionType.Skill2))
                {
                    return;
                }
                StartCoroutine(StartSkillAttack(AttackInputType.Skill2));
                break;
            case AttackInputType.Skill3:
                if (!CharActionlist.Contains(CharacterActionType.Skill3))
                {
                    return;
                }
                StartCoroutine(StartSkillAttack(AttackInputType.Skill3));
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

        yield return BattleManagerScript.Instance.WaitUpdate(() => currentAttackPhase != AttackPhasesType.End);
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
            while (isSpecialLoading && !VFXTestMode)
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
                if (IsOnField || VFXTestMode)
                {
                    while (isMoving)
                    {
                        yield return new WaitForEndOfFrame();

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
        ScriptableObjectAttackBase nxtAtk = CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == inputSkill).First();
        SkillCoolDownClass  scdc = skillCoolDown.Where(r => r.Skill == inputSkill).First();
        if (!GetCanUseStamina(nxtAtk.StaminaCost) || scdc.IsCoGoing)
        {
            yield break;
        }
        nextAttack = nxtAtk;
        scdc.IsCoGoing = true;
        //GameObject go = ParticleManagerScript.Instance.GetParticle(ParticlesType.Skill_Darkening);
        //go.transform.position = SpineAnim.FiringPints[(int)inputSkill].transform.position;
        //go.SetActive(true);
        yield return BattleManagerScript.Instance.WaitUpdate(() => currentAttackPhase != AttackPhasesType.End);
        BattleManagerScript.Instance.BattleSpeed = 0.2f;
        SpineAnim.SetSkeletonOrderInLayer(300);
        SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
        currentAttackPhase = AttackPhasesType.Start;
        yield return BattleManagerScript.Instance.WaitFor(1.8f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        SetAnimation(nxtAtk.PrefixAnim + "_AtkToIdle", false, 0);
        yield return BattleManagerScript.Instance.WaitUpdate(() => currentAttackPhase != AttackPhasesType.End);
        BattleManagerScript.Instance.BattleSpeed = 1;
        SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
        yield return BattleManagerScript.Instance.WaitFor(nxtAtk.CoolDown, ()=> BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        scdc.IsCoGoing = false;
    }


    public override void SetFinalDamage(BaseCharacter attacker, float damage)
    {
        Sic.DamageReceived += damage;
        base.SetFinalDamage(attacker, damage);
    }

    //Set ste special attack
    public void SpecialAttack(ScriptableObjectAttackBase atkType)
    {
        nextAttack = atkType;

        if (chargingAudio != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingRelease, AudioBus.LowPrio, transform);
        }

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



    //Create and set up the basic info for the bullet
    public override void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
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
        bs.VFXTestMode = VFXTestMode;
        bs.CharOwner = this;
        bs.attackAudioType = GetAttackAudio();
        bs.BulletEffects = bulletBehaviourInfo.Effects;

        if (!GridManagerScript.Instance.isPosOnFieldByHeight(UMS.CurrentTilePos + bulletBehaviourInfo.BulletDistanceInTile))
        {
            bs.gameObject.SetActive(false);
            return;
        }

        if (UMS.Facing == FacingType.Right)
        {
            bs.DestinationTile = new Vector2Int(UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, UMS.CurrentTilePos.y + bulletBehaviourInfo.BulletDistanceInTile.y > 11 ? 11 : UMS.CurrentTilePos.y + bulletBehaviourInfo.BulletDistanceInTile.y);
        }
        else
        {
            bs.DestinationTile = new Vector2Int(UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, UMS.CurrentTilePos.y - bulletBehaviourInfo.BulletDistanceInTile.y < 0 ? 0 : UMS.CurrentTilePos.y - bulletBehaviourInfo.BulletDistanceInTile.y);
        }
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Bullet : nextAttack.Particles.Right.Bullet, CharInfo.CharacterID, AttackParticlePhaseTypes.Bullet, bullet.transform, UMS.Side,
            nextAttack.AttackInput, CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script ? true : false);


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
    }

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {

        if (PlayQueuedAnim()) return;

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
            transform.position = new Vector3(100, 100, 100);
            SetAttackReady(false);
        }

        if (completedAnim == CharacterAnimationStateType.JumpTransition_OUT.ToString())
        {
            transform.position = new Vector3(100, 100, 100);
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

        if (!animState.ToString().Contains("Atk"))
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        if((SpineAnim.CurrentAnim.Contains("S_Buff") && !animState.Contains("S_Buff")) || (SpineAnim.CurrentAnim.Contains("S_DeBuff") && !animState.Contains("S_DeBuff")))
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
        return base.SetDamage(attacker ,damage, elemental, isCritical);
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