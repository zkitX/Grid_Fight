using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class Stage09_Boss_Geisha : MinionType_Script
{
    Stage09_BossInfo bossInfo = null;
    bool IsCharArrived = false;
    bool oniFormeActive = false;
    Stage09_Boss_NoFace oniForme;
    public bossPhasesType BossPhase = bossPhasesType.Phase1_;
    bool shielded = false;
    GameObject shieldParticles = null;
    public bool isImmune = false;

    #region Initial Setup

    public override void Start()
    {
        if(bossInfo == null)SetupFromBossInfo();

        if (oniForme == null) GenerateBoss();

        StartCoroutine(DelayedSetupSequence());
    }

    void SetupFromBossInfo()
    {
        bossInfo = GetComponentInChildren<Stage09_BossInfo>();
        bossInfo.InitialiseBossInfo();
        if (oniForme != null && oniForme.bossInfo == null) oniForme.bossInfo = bossInfo;
    }

    void GenerateBoss()
    {
        oniForme = (Stage09_Boss_NoFace)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(CharacterNameType.Stage09_Boss_NoFace.ToString(), CharacterSelectionType.Up,
        CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage09_Boss_NoFace, WalkingSideType.RightSide, AttackType.Tile, BaseCharType.None), transform);
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
        UIBattleManager.Instance.UIBoss.UpdateHp((100f * CharInfo.HealthStats.Health) / CharInfo.HealthStats.Base);
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

        bool val = true;
        while (val)
        {
            yield return null;
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
                    BaseCharacter targetChar = enemys[Random.Range(0, enemys.Count)];
                    if (targetChar != null)
                    {
                        Debug.Log("GEISHA ATTACK");
                        yield return AttackSequence();
                        yield return new WaitForSeconds(Random.Range(bossInfo.maidenFormeAttackCoolDown.x, bossInfo.maidenFormeAttackCoolDown.y));
                    }
                }
            }
        }
    }

    #endregion

    #region Combat

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
            case AttackAnimType.Rapid_Atk:
                animToFire = "Atk1_IdleToAtk";
                break;
            case AttackAnimType.Powerful_Atk:
                animToFire = "Atk2_IdleToAtk";
                break;
            default:
                Debug.LogError("This attack animation type does not exist in the geisha, only use ATK or RAPIDATK");
                break;
        }

        currentAttackPhase = AttackPhasesType.Start;
        sequencedAttacker = true;
        SetAnimation(animToFire, false, 0f);
        CreateTileAttack();

        while (shotsLeftInAttack != 0)
        {
            yield return null;
        }

        currentAttackPhase = AttackPhasesType.End;
        Attacking = false;
        yield break;
    }

    IEnumerator StartShieldSequence()
    {
        shielded = true;
        Attacking = true;
        SetAnimation("Atk2_IdleToAtk", false, 0.3f);
        while (!SpineAnim.CurrentAnim.Contains("Idle"))
        {
            yield return null;
        }
        if (ShieldedSequencer != null) StopCoroutine(ShieldedSequencer);
        ShieldedSequencer = ShieldedSequence();
        StartCoroutine(ShieldedSequencer);
        Attacking = false;
    }

    IEnumerator ShieldedSequencer = null;
    IEnumerator ShieldedSequence()
    {
        CharInfo.HealthStats.Regeneration = CharInfo.HealthStats.BaseHealthRegeneration * bossInfo.moonlightBlessRegenMultiplier;

        if (shieldParticles == null)
        {
            shieldParticles = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
            shieldParticles.transform.localScale *= 3f; //remove when actual particles added
        }
        else
        {
            shieldParticles.SetActive(true);
        }
        
        yield return new WaitForSeconds(Random.Range(bossInfo.moonlightBlessDuration.x, bossInfo.moonlightBlessDuration.y));

        CharInfo.HealthStats.Regeneration = CharInfo.HealthStats.BaseHealthRegeneration;
        shielded = false;
        shieldParticles.SetActive(false);

        Debug.Log("GEISHA Defence ends");
    }

    void InteruptShield()
    {
        if (ShieldedSequencer != null) StopCoroutine(ShieldedSequencer);
        CharInfo.HealthStats.Regeneration = CharInfo.HealthStats.BaseHealthRegeneration;
        shielded = false;
        shieldParticles?.SetActive(false);
        Debug.Log("GEISHA Defence Interrupted");
    }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if(BossPhase == bossPhasesType.Phase1_ && !isImmune)
        {
            float prevHealthPerc = CharInfo.HealthPerc;
            bool boolToReturn = base.SetDamage(shielded ? damage * bossInfo.moonlightBlessAttackDampener : damage, elemental, isCritical, isAttackBlocking);
            CheckIfCanTransform(prevHealthPerc);
            return boolToReturn;
        }
        else
        {
            oniForme.SetDamage(damage, elemental, isCritical, isAttackBlocking);
        }
        return false;
    }

    void CheckIfCanTransform(float prevHealthPerc)
    {
        if(oniForme.intensityLevel >= bossInfo.divineEvocationLevels.Count - 1)
        {
            return;
        }

        if(CharInfo.HealthPerc <= bossInfo.divineEvocationLevels[oniForme.intensityLevel + 1])
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
            CharInfo.HealthStats.Regeneration = 0f;
            InteruptShield();
            BossPhase = bossPhasesType.Monster_;
            oniForme.TransformToNoFace();
        }
        else
        {
            oniForme.CharInfo.HealthStats.Regeneration = 0f;
            SetAnimation(CharacterAnimationStateType.Death, false);
            TransformFromNoFace();
            BossPhase = bossPhasesType.Phase1_;
        }

        if(CharInfo.Health > 0f || state)
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
        Attacking = false;
        isImmune = true;
        oniForme.isImmune = true;
        oniForme.CanAttack = false;

        CanAttack = false;
        SetAnimation("Death", false, 0.5f);
        while (!SpineAnim.CurrentAnim.Contains("Idle"))
        {
            yield return null;
        }

        if (CharInfo.Health <= 0f)
        {
            oniForme.isDead = true;
            SetCharDead();
        }
        else
        {
            CharInfo.HealthStats.Regeneration = CharInfo.HealthStats.BaseHealthRegeneration;
            isImmune = false;
            CanAttack = true;
        }
    }

    protected void SetFormeAttackReady(MinionType_Script forme, bool state)
    {
        if (forme.CharInfo.CharacterID == CharacterNameType.Stage09_Boss_NoFace)
        {
            if (oniForme.ActiveAI == null)
            {
                oniForme.ActiveAI = oniForme.AI();
                StartCoroutine(oniForme.ActiveAI);
            }
            if(!state)oniForme.InteruptAttack();
        }
        else
        {
            if (ActiveAI == null)
            {
                ActiveAI = AI();
                StartCoroutine(ActiveAI);
            }
            if(!state)InteruptAttack();
        }
        forme.currentAttackPhase = AttackPhasesType.End;
        forme.CharOredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
        if (forme.CharInfo.UseLayeringSystem)
        {
            forme.SpineAnim.SetSkeletonOrderInLayer(forme.CharOredrInLayer);
        }
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0)
    {
        if(animState == "GettingHit" && (Attacking || oniForme.Attacking))
        {
            return;
        }
        if (animState != "Idle" && (SpineAnim.CurrentAnim.Contains("Death") || SpineAnim.CurrentAnim.Contains("Transformation")))
        {
            return;
        }
        if(animState == "Idle")
        {
            transition = 1f;
        }


        if(!animState.Contains("Monster_") && !animState.Contains("Phase1_"))
        {
            animState = animState != "Transformation" ? BossPhase.ToString() + animState.ToString() : animState;
        }

        if (animState == bossPhasesType.Phase1_.ToString() + "Atk2_Charging")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }
        if(animState == bossPhasesType.Phase1_.ToString() + "Atk2_Loop")
        {
            animState = bossPhasesType.Phase1_.ToString() + "Atk2_AtkToIdle";
            loop = false;
        }
        if(animState == bossPhasesType.Monster_.ToString() + "Atk1_Loop")
        {
            animState = bossPhasesType.Monster_.ToString() + "Atk1_AtkToIdle";
            loop = false;
        }
        if (animState == bossPhasesType.Monster_.ToString() + "Atk3_Loop")
        {
            animState = bossPhasesType.Monster_.ToString() + "Atk3_AtkToIdle";
            loop = false;
        }

        base.SetAnimation(animState, loop, transition);
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
            ((100f * CharInfo.HealthStats.Health) / CharInfo.HealthStats.Base) :
            ((100f * oniForme.CharInfo.HealthStats.Health) / oniForme.CharInfo.HealthStats.Base));
    }

    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
         || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
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
            shotsLeftInAttack = 0;
        }

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() || completedAnim.Contains("Growing"))
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            CharArrivedOnBattleField();
        }

        if (completedAnim.Contains("Charging"))
        {

        }


        if (completedAnim.Contains(CharacterAnimationStateType.Idle.ToString()) && !SpineAnim.Loop)
        {
            SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
            //Debug.Log("IDLE     " + completedAnim.ToString());
            SpineAnim.SpineAnimationState.SetAnimation(0, BossPhase.ToString() + CharacterAnimationStateType.Idle.ToString(), true);
            //SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
        }
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
}