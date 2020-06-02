﻿using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage09_Boss_Geisha_Script : MinionType_Script
{

    public override bool Attacking
    {
        get
        {
            if (oniFormeActive) return oniForme.Attacking;
            else return _Attacking;
        }
        set
        {
            if (oniFormeActive) oniForme.Attacking = value;
            else _Attacking = value;
        }
    }
    public override CharacterInfoScript CharInfo
    {
        get
        {
            if (_CharInfo == null)
            {
                _CharInfo = GetComponentInChildren<CharacterInfoScript>(true);
                _CharInfo.BaseSpeedChangedEvent += _CharInfo_BaseSpeedChangedEvent;
                _CharInfo.DeathEvent += _CharInfo_DeathEvent;
            }
            if (BossPhase == bossPhasesType.Phase1_) return _CharInfo;
            else return oniForme.CharInfo;
        }
    }
    Stage09_BossInfo_Script bossInfo = null;
    bool IsCharArrived = false;
    bool oniFormeActive = false;
    Stage09_Boss_NoFace_Script oniForme;
    public bossPhasesType BossPhase = bossPhasesType.Phase1_;
    bool shielded = false;
    GameObject shieldParticles = null;
    public bool isImmune = false;
    public override ScriptableObjectAttackBase nextAttack
    {
        get
        {
            return BossPhase == bossPhasesType.Phase1_ ? _nextAttack : oniForme.nextAttack;
        }
        set
        {
            if (BossPhase == bossPhasesType.Phase1_) _nextAttack = value;
            else oniForme.nextAttack = value;
        }
    }

    #region Initial Setup

    public override void Start()
    {
        if (bossInfo == null) SetupFromBossInfo();

        if (oniForme == null) GenerateBoss();

        StartCoroutine(DelayedSetupSequence());
    }

    void SetupFromBossInfo()
    {
        bossInfo = GetComponentInChildren<Stage09_BossInfo_Script>();
        bossInfo.InitialiseBossInfo();
        if (oniForme != null && oniForme.bossInfo == null) oniForme.bossInfo = bossInfo;
    }

    void GenerateBoss()
    {
        oniForme = (Stage09_Boss_NoFace_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(CharacterNameType.Stage09_Boss_NoFace.ToString(), CharacterSelectionType.Up,
        new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage09_Boss_NoFace, WalkingSideType.RightSide, AttackType.Tile, BaseCharType.None, new List<CharacterActionType>(), LevelType.Novice), transform);
        oniForme.bossInfo = bossInfo;
        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.EnableBattleBars(false);
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;
        oniForme.baseForme = this;
        SetOniForme(false);
    }

    IEnumerator DelayedSetupSequence()
    {
        while (UIBattleManager.Instance == null || !IsOnField)
        {
            yield return null;
        }
        SetupBossHealthBar();
    }

    protected void SetupBossHealthBar()
    {
        if (!UIBattleManager.Instance.UIBoss.gameObject.activeInHierarchy)
        {
            UIBattleManager.Instance.UIBoss.gameObject.SetActive(true);
        }
        UIBattleManager.Instance.UIBoss.UpdateHp((100f * _CharInfo.HealthStats.Health) / _CharInfo.HealthStats.Base);
    }

    #endregion

    #region Enquiries


    #endregion

    #region Entering and Exiting

    public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        if (oniForme == null) GenerateBoss();
        if (bossInfo == null) SetupFromBossInfo();
        SetAnimation("Idle", true);

        UMS.EnableBattleBars(false);
        CharArrivedOnBattleField();

        WaveManagerScript.Instance.BossArrived(this);
        CanAttack = true;
        IsOnField = true;
        oniForme.IsOnField = true;

        SetFormeAttackReady(this, true);
        SetFormeAttackReady(oniForme, false);
        oniForme.CharInfo.HealthStats.Regeneration = 0f;
        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;

        float timer = 0;
        while (timer <= 3)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
    }

    public override void SetUpLeavingBattle()
    {
        return;
    }

    public override void SetCharDead(bool hasToDisappear = true)
    {
        return;
    }

    public void GeishaFinalDeath()
    {
        if (!oniForme.isDead)
        {
            return;
        }
        CanAttack = false;
        IsOnField = false;
        oniForme.IsOnField = false;
        SetFormeAttackReady(this, false);
        SetFormeAttackReady(oniForme, false);
        EventManager.Instance.AddCharacterDeath(this);
        EventManager.Instance.AddCharacterDeath(oniForme);
        StopCoroutine(oniForme.ActiveAI);
        StopCoroutine(ActiveAI);

        BossPhase = bossPhasesType.Phase1_;
        SetAnimation("Idle", true);
        SpineAnim.SetAnimationSpeed(0.6f);
    }

    #endregion

    #region AI

    public override IEnumerator AI()
    {
        yield return GeishaAI();
    }
    public IEnumerator ActiveAI = null;
    IEnumerator GeishaAI()
    {
        while (BattleManagerScript.Instance.PlayerControlledCharacters.Length == 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        bool val = true;
        while (val)
        {
            yield return null;
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return null;
            }

            if (IsOnField && CanAttack && BossPhase == bossPhasesType.Phase1_ && !isImmune)
            {
                int randomiser = Random.Range(0, 100);
                if (randomiser < bossInfo.moonlightBlessOdds && !shielded)
                {
                    Debug.Log("GEISHA Defence");
                    yield return StartShieldSequence();
                    yield return new WaitForSeconds(Random.Range(bossInfo.moonlightBlessCastCoolDown.x, bossInfo.moonlightBlessCastCoolDown.y));
                }
                else
                {
                    List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                    if (enemys.Count > 0)
                    {
                        BaseCharacter targetChar = enemys.Count != 0 ? enemys[Random.Range(0, enemys.Count)] : null;
                        if (targetChar != null)
                        {
                            nextAttackPos = targetChar.UMS.CurrentTilePos;
                            Debug.Log("GEISHA ATTACK");
                            yield return AttackSequence();
                            yield return new WaitForSeconds(Random.Range(bossInfo.maidenFormeAttackCoolDown.x, bossInfo.maidenFormeAttackCoolDown.y));
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Combat
    bool creatingShield = false;
    public override void CreateTileAttack()
    {
        base.CreateTileAttack();
    }


    public override void CastAttackParticles()
    {
        //Debug.Log("Cast");
        if (creatingShield)
        {
            creatingShield = false;
            return;
        }


        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Cast : nextAttack.Particles.Right.Cast, 
            BossPhase == bossPhasesType.Phase1_? CharInfo.CharacterID : oniForme.CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
            BossPhase == bossPhasesType.Phase1_ ? SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position : 
            oniForme.SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);
        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);

        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                //_CharInfo.Stamina -= _CharInfo.RapidAttack.Stamina_Cost_Atk;
                EventManager.Instance?.UpdateStamina(this);
            }
            else if (SpineAnim.CurrentAnim.Contains("Atk2"))
            {
                //_CharInfo.Stamina -= _CharInfo.PowerfulAttac.Stamina_Cost_Atk;
                EventManager.Instance?.UpdateStamina(this);
            }
        }
    }

    public override IEnumerator MoveCharOnDir_Co(InputDirection nextDir)
    {
        yield break; //char doesnt move
        yield return base.MoveCharOnDir_Co(nextDir);
        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;
    }

    public override IEnumerator AttackSequence(ScriptableObjectAttackBase atk = null)
    {
        Attacking = true;
        if (atk != null)
        {
            nextAttack = atk;
        }
        else
        {
            GetAttack();
        }
        string animToFire = "bippidi boppidi";
        switch (nextAttack.AttackAnim)
        {
            case AttackAnimType.Weak_Atk:
                animToFire = "Atk1_IdleToAtk";
                break;
            case AttackAnimType.Strong_Atk:
                animToFire = "Atk2_IdleToAtk";
                break;
            default:
                Debug.LogError("This attack animation type does not exist in the geisha, only use ATK or RAPIDATK");
                break;
        }

        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(animToFire, false, 0f);
        // CreateTileAttack();

        while (shotsLeftInAttack != 0 && Attacking)
        {
            yield return null;
        }
        yield break;
    }

    IEnumerator StartShieldSequence()
    {
        creatingShield = true;
        Attacking = true;
        nextAttack = _CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == AttackAnimType.Buff).First();
        SetAnimation("S_Buff_IdleToAtk", false, 0.3f);
        while (Attacking)
        {
            yield return null;
        }
    }

    IEnumerator ShieldedSequencer = null;
    IEnumerator ShieldedSequence()
    {
        shielded = true;
        _CharInfo.HealthStats.Regeneration = _CharInfo.HealthStats.BaseHealthRegeneration * bossInfo.moonlightBlessRegenMultiplier;

        if (shieldParticles == null)
        {
            shieldParticles = Instantiate(nextAttack.Particles.CastLoopPS, transform);
        }
        else
        {
            shieldParticles.SetActive(true);
        }

        yield return new WaitForSeconds(Random.Range(bossInfo.moonlightBlessDuration.x, bossInfo.moonlightBlessDuration.y));

        _CharInfo.HealthStats.Regeneration = _CharInfo.HealthStats.BaseHealthRegeneration;
        shielded = false;
        shieldParticles.SetActive(false);

        Debug.Log("GEISHA Defence ends");
    }

    void InteruptShield()
    {
        if (ShieldedSequencer != null) StopCoroutine(ShieldedSequencer);
        _CharInfo.HealthStats.Regeneration = _CharInfo.HealthStats.BaseHealthRegeneration;
        shielded = false;
        shieldParticles?.SetActive(false);
        Debug.Log("GEISHA Defence Interrupted");
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (BossPhase == bossPhasesType.Phase1_ && !isImmune)
        {
            float prevHealthPerc = _CharInfo.HealthPerc;
            bool boolToReturn = base.SetDamage(attacker, shielded ? damage * bossInfo.moonlightBlessAttackDampener : damage, elemental, isCritical, isAttackBlocking);
            CheckIfCanTransform(prevHealthPerc);
            return boolToReturn;
        }
        else
        {
            oniForme.SetDamage(attacker, damage, elemental, isCritical, isAttackBlocking);
        }
        return false;
    }

    void CheckIfCanTransform(float prevHealthPerc)
    {
        if (oniForme.intensityLevel >= bossInfo.divineEvocationLevels.Count - 1)
        {
            return;
        }

        if (_CharInfo.HealthPerc <= bossInfo.divineEvocationLevels[oniForme.intensityLevel + 1])
        {
            InteruptAttack();
            isImmune = true;
            SetOniForme(true);
            return;
        }
    }

    #endregion

    #region Functionality

    public void SetOniForme(bool state)
    {
        if (state == oniFormeActive)
        {
            return;
        }

        oniFormeActive = state;

        if (state)
        {
            oniForme.intensityLevel++;
            InteruptShield();
            _CharInfo.HealthStats.Regeneration = 0f;
            oniForme.TransformToNoFace();
        }
        else
        {
            oniForme.CharInfo.HealthStats.Regeneration = 0f;
            TransformFromNoFace();
        }

        if (_CharInfo.Health > 0f || state)
        {
            SetFormeAttackReady(this, !state);
            SetFormeAttackReady(oniForme, state);
        }
    }

    void TransformFromNoFace()
    {
        StartCoroutine(GeishaTransformation());
    }

    IEnumerator GeishaTransformation()
    {
        _Attacking = false;
        Attacking = false;
        isImmune = true;
        oniForme.isImmune = true;
        oniForme.CanAttack = false;

        CanAttack = false;
        SetAnimation("Monster_Death", false, 0.5f);
        while (!SpineAnim.CurrentAnim.Contains("Idle"))
        {
            yield return null;
        }

        if (_CharInfo.Health <= 0f)
        {
            oniForme.isDead = true;
            GeishaFinalDeath();
        }
        else
        {
            _CharInfo.HealthStats.Regeneration = _CharInfo.HealthStats.BaseHealthRegeneration;
            isImmune = false;
            CanAttack = true;
        }
        BossPhase = bossPhasesType.Phase1_;
    }

    protected void SetFormeAttackReady(MinionType_Script forme, bool state)
    {
        if (forme._CharInfo.CharacterID == CharacterNameType.Stage09_Boss_NoFace)
        {
            if (oniForme.ActiveAI == null)
            {
                oniForme.ActiveAI = oniForme.AI();
                StartCoroutine(oniForme.ActiveAI);
            }
            if (!state) oniForme.InteruptAttack();
        }
        else
        {
            if (ActiveAI == null)
            {
                ActiveAI = AI();
                StartCoroutine(ActiveAI);
            }
            if (!state) InteruptAttack();
        }
        forme.currentAttackPhase = AttackPhasesType.End;
        forme.CharOredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
        if (forme._CharInfo.UseLayeringSystem)
        {
            forme.SpineAnim.SetSkeletonOrderInLayer(forme.CharOredrInLayer);
        }
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (animState.Contains("GettingHit") && (Attacking || oniForme.Attacking))
        {
            return;
        }
        if (animState != "Idle" && animState != "Monster_Idle" && (SpineAnim.CurrentAnim.Contains("Death") || SpineAnim.CurrentAnim.Contains("Transformation")))
        {
            return;
        }

        transition = 0.2f;
        if (animState == "Idle")
        {
            transition = 1f;
        }


        if (animState.Contains("IdleToAtk") && !Attacking)
        {
        }

        if (!animState.Contains("Monster_") && !animState.Contains("Phase1_"))
        {
            animState = animState != "Transformation" ? BossPhase.ToString() + animState.ToString() : animState;
        }

        if (animState == bossPhasesType.Phase1_.ToString() + "Atk2_Charging")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }



        if (animState == bossPhasesType.Phase1_.ToString() + "Atk2_Loop")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }

        if (animState == bossPhasesType.Phase1_.ToString() + "S_Buff_IdleToAtk")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_IdleToAtk";
            loop = false;
        }
        if (animState == bossPhasesType.Phase1_.ToString() + "S_Buff_Charging")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_Charging";
            loop = true;
        }
        if (animState == bossPhasesType.Phase1_.ToString() + "S_Buff_Loop")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }

        if (animState == bossPhasesType.Monster_.ToString() + "Atk2_Loop")
        {
            animState = bossPhasesType.Monster_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }
        if (animState == bossPhasesType.Monster_.ToString() + "Atk3_Loop")
        {
            animState = bossPhasesType.Monster_.ToString() + "Atk3_AtkToIdle";
            loop = false;
        }
        Debug.Log("new    " + animState);

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    void InteruptAttack()
    {
        CanAttack = false;
        Attacking = false;
        shotsLeftInAttack = 0;
        currentAttackPhase = AttackPhasesType.End;
    }

    #endregion



    public bool changeAnim = false;
    public CharacterAnimationStateType NextAnimToFire;
    public bool Loop = true;

    protected override void Update()
    {
        base.Update();
        if (changeAnim)
        {
            changeAnim = false;
            SetAnimation(NextAnimToFire, Loop, 0);
        }
        UIBattleManager.Instance.UIBoss.UpdateHp(BossPhase == bossPhasesType.Phase1_ ?
            ((100f * _CharInfo.HealthStats.Health) / _CharInfo.HealthStats.Base) :
            ((100f * oniForme.CharInfo.HealthStats.Health) / oniForme.CharInfo.HealthStats.Base));
    }

    public enum bossPhasesType
    {
        Monster_,
        Phase1_
    }

    public override void SetAttackReady(bool value)
    {
        CharBoxCollider.enabled = value;
        return;
    }

    public override CastLoopImpactAudioClipInfoClass GetAttackAudio()
    {
        if (nextAttack == null) return null;

        if (nextAttack.AttackAnim == AttackAnimType.Boss_Atk3)
        {
            return ((Boss_Stage09AudioProfileSO)_CharInfo.AudioProfile).BossAttack3;
        }
        return base.GetAttackAudio();
    }


    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
         || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        Debug.Log("Complete_    " + completedAnim);

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {

            SetAnimation((nextAttack == null ? AttackAnimPrefixType.Atk2 : nextAttack.PrefixAnim) + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {

            //If they can still attack, keep them in the charging loop
            if (shotsLeftInAttack > 0)
            {
                SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            }
            //otherwise revert them to the idle postion
            else
            {
                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                currentAttackPhase = AttackPhasesType.End;
            }
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;

            if (BossPhase == bossPhasesType.Phase1_ ? shotsLeftInAttack == 0 : oniForme.shotsLeftInAttack == 0)
            {
                if (BossPhase == bossPhasesType.Phase1_) Attacking = false;
                else if (BossPhase == bossPhasesType.Monster_) oniForme.Attacking = false;
            }
        }

        if (completedAnim == "Transformation")
        {
            SetAnimation("Monster_Idle", true);
            return;
        }

        string[] res = completedAnim.Split('_');
        if (res.Last() != CharacterAnimationStateType.Idle.ToString() && !SpineAnim.Loop && !Attacking)
        {
            SpineAnim.SetAnimationSpeed(_CharInfo.BaseSpeed);
            Debug.Log("IDLE     " + completedAnim.ToString());
            SpineAnim.SpineAnimationState.SetAnimation(0, BossPhase.ToString() + CharacterAnimationStateType.Idle.ToString(), true);
            //SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
        }

    }

    public override void SpecialAttackImpactEffects(Vector3 tilePos)
    {
        if(nextAttack.AttackAnim == AttackAnimType.Buff)
        {
            if (ShieldedSequencer != null) StopCoroutine(ShieldedSequencer);
            ShieldedSequencer = ShieldedSequence();
            StartCoroutine(ShieldedSequencer);
            return;
        }
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Hit, CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, tilePos, UMS.Side, nextAttack.AttackInput);
    }
}