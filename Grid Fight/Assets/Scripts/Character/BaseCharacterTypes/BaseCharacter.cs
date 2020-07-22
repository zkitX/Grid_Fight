using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, IDisposable
{

    public delegate void CurrentCharIsDead(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsDead CurrentCharIsDeadEvent;

    public delegate void TileMovementComplete(BaseCharacter movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;

    public delegate void HealthStatsChanged(float value, HealthChangedType changeType, Transform charOwner);
    public event HealthStatsChanged HealthStatsChangedEvent;

    public delegate void CurrentCharIsRebirth(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsRebirth CurrentCharIsRebirthEvent;

  


    public virtual CharacterInfoScript CharInfo
    {
        get
        {
            if (_CharInfo == null)
            {
                _CharInfo = GetComponentInChildren<CharacterInfoScript>(true);
                _CharInfo.BaseSpeedChangedEvent += _CharInfo_BaseSpeedChangedEvent;
                _CharInfo.DeathEvent += _CharInfo_DeathEvent;
            }
            return _CharInfo;
        }
    }


    public bool IsOnField
    {
        get
        {
            return _IsOnField;
        }
        set
        {
            _IsOnField = value;
        }
    }


    public CharacterInfoScript _CharInfo;
    public bool isMoving = false;
    protected IEnumerator MoveCo;
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    public SpineAnimationManager SpineAnim;
    public bool _IsOnField = false;
    public bool CanAttack = false;
    public bool isSpecialLoading
    {
        get
        {
            return isSpecialStop ? false : _isSpecialLoading;
        }
        set
        {
            _isSpecialLoading = value;
        }
    }

    public bool _isSpecialLoading = false;
    public bool isSpecialStop = false;

    public bool isDefending
    {
        get
        {
            return isDefendingStop ? false : _isDefending;
        }
        set
        {
            _isDefending = value;
        }
    }


    public bool _isDefending = false;
    public bool isDefendingStop = false;
    [HideInInspector]
    public bool bulletFired = false;

    public List<BuffDebuffClass> BuffsDebuffsList = new List<BuffDebuffClass>();
    public List<CharacterActionType> CharActionlist = new List<CharacterActionType>();
    public UnitManagementScript UMS;
    public BoxCollider CharBoxCollider;
    public ScriptableObjectAttackBase _nextAttack = null;
    public virtual ScriptableObjectAttackBase nextAttack
    {
        get
        {
            return _nextAttack;
        }
        set
        {
            _nextAttack = value;
        }
    }
    public AttackPhasesType currentAttackPhase = AttackPhasesType.End;
    public DeathProcessStage currentDeathProcessPhase = DeathProcessStage.None;
    public SpecialAttackStatus StopPowerfulAtk;
    protected float DefendingHoldingTimer = 0;
    public bool IsSwapping = false;
    public bool SwapWhenPossible = false;
    public GameObject chargeParticles = null;
    protected bool canDefend = true;
    public StatisticInfoClass Sic;
    public Vector3 LocalSpinePosoffset;
    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
            _shotsLeftInAttack =_shotsLeftInAttack < 0 ? 0 : _shotsLeftInAttack;
        }
    }

    public int _shotsLeftInAttack = 0;
    public ControllerType CurrentPlayerController;
    [HideInInspector]
    public bool _Attacking = false;
    public virtual bool Attacking
    {
        get
        {
            return shotsLeftInAttack > 0 ? true : _Attacking;
        }
        set
        {
            _Attacking = value;
        }
    }
    [HideInInspector] public int CharOredrInLayer = 0;
    protected List<BattleTileScript> currentBattleTilesToCheck = new List<BattleTileScript>();
    public IEnumerator AICo = null;
    

    public virtual void Start()
    {
        Sic = new StatisticInfoClass(CharInfo.CharacterID, UMS.PlayerController);
    }

    protected virtual void Update()
    {

        UMS.HPBar.localScale = new Vector3((1f / 100f) * CharInfo.HealthPerc, 1, 1);

        UMS.StaminaBar.localScale = new Vector3((1f / 100f) * CharInfo.StaminaPerc, 1, 1);
    }

    #region Setup Character
    public virtual void SetupCharacterSide()
    {

        if (!UMS.PlayerController.Contains(ControllerType.Enemy))
        {
            UMS.SelectionIndicator.gameObject.SetActive(true);
        }

        SpineAnimatorsetup();
        LocalSpinePosoffset = SpineAnim.transform.localPosition;
        int layer = UMS.Side == SideType.LeftSide ? 9 : 10;
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.gameObject.layer = layer;
        }


    }

    public void _CharInfo_BaseSpeedChangedEvent(float baseSpeed)
    {
        SpineAnim.SetAnimationSpeed(baseSpeed);
    }

    public void _CharInfo_DeathEvent()
    {
        SetCharDead();
    }

    public virtual void SetAttackReady(bool value)
    {
        //Debug.Log(CharInfo.CharacterID + "  " + value);
        if (CharBoxCollider != null)
        {
            CharBoxCollider.enabled = value;
        }
        CanAttack = value;
        IsOnField = value;
        currentAttackPhase = AttackPhasesType.End;
        CharOredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
        }
    }

    public virtual void SetCharDead()
    {
        foreach(ManagedAudioSource audioSource in GetComponentsInChildren<ManagedAudioSource>())
        {
            audioSource.gameObject.transform.parent = AudioManagerMk2.Instance.transform;
        }
        isMoving = false;
        SetAttackReady(false);
        LocalSpinePosoffset = SpineAnim.transform.localPosition;
        Call_CurrentCharIsDeadEvent();
        shotsLeftInAttack = 0;
    }

    protected virtual void Call_CurrentCharIsDeadEvent()
    {
        CurrentCharIsDeadEvent?.Invoke(CharInfo.CharacterID, UMS.PlayerController, UMS.Side);
    }

    protected virtual void Call_CurrentCharIsRebirthEvent()
    {
        CurrentCharIsRebirthEvent(CharInfo.CharacterID, UMS.PlayerController, UMS.Side);
    }

    public virtual void SetUpEnteringOnBattle()
    {

    }

    public virtual void SetUpLeavingBattle()
    {

    }

    public virtual void CharArrivedOnBattleField()
    {
        SetAttackReady(true);
    }

    #endregion
    #region Attack

    public virtual void SpecialAttackImpactEffects(Vector3 tilePos)
    {
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Hit, CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, tilePos, UMS.Side, nextAttack.AttackInput);
    }

    public virtual IEnumerator AttackSequence()
    {
        yield return null;
    }

    public virtual void fireAttackAnimation(Vector3 pos)
    {

    }

    public virtual void FireAttackAnimAndBullet(Vector3 pos)
    {

    }

    public int GetHowManyAttackAreOnBattleField(List<BulletBehaviourInfoClassOnBattleFieldClass> bulTraj)
    {
        int res = 0;
        foreach (BulletBehaviourInfoClassOnBattleFieldClass item in bulTraj)
        {
            foreach (BattleFieldAttackTileClass target in item.BulletEffectTiles)
            {
                if (GridManagerScript.Instance.isPosOnField(target.Pos))
                {
                    res++;
                }
            }
        }

        return res;
    }

    /*public virtual IEnumerator AttackSequence()
    {
        SetAnimation(nextAttack.Anim);

        while (currentAttackPhase == AttackPhasesType.Start)
        {
            while (SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk)
            {
                yield return null;
                if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle && !isMoving)
                {
                    SetAnimation(nextAttack.Anim);
                }
            }
            yield return null;
        }
    }*/


    [HideInInspector] public List<ScriptableObjectAttackBase> availableAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public List<ScriptableObjectAttackBase> currentTileAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public ScriptableObjectAttackBase atkToCheck;
    public virtual void GetAttack()
    {
        currentTileAtks = CharInfo.CurrentAttackTypeInfo.Where(r => r != null && r.CurrentAttackType == AttackType.Tile).ToList();
        availableAtks.Clear();
        for (int i = 0; i < currentTileAtks.Count; i++)
        {
            atkToCheck = currentTileAtks[i];
            switch (atkToCheck.TilesAtk.StatToCheck)
            {
                case StatsCheckType.Health:
                    switch (atkToCheck.TilesAtk.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (CharInfo.HealthPerc < atkToCheck.TilesAtk.PercToCheck)
                            {

                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (CharInfo.HealthPerc == atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (CharInfo.HealthPerc > atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                    }
                    break;
                case StatsCheckType.Stamina:
                    switch (atkToCheck.TilesAtk.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (CharInfo.StaminaPerc < atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (CharInfo.StaminaPerc == atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (CharInfo.StaminaPerc > atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                    }
                    break;
                case StatsCheckType.None:
                    nextAttack = atkToCheck;
                    availableAtks.Add(atkToCheck);
                    break;
            }
        }

        int totalchances = 0;
        availableAtks.ForEach(r =>
        {
            totalchances += r.TilesAtk.Chances;
        });
        int chances = UnityEngine.Random.Range(0, totalchances);
        int sumc = 0;
        for (int i = 0; i < availableAtks.Count; i++)
        {
            sumc += availableAtks[i].TilesAtk.Chances;

            if (chances < sumc)
            {
                nextAttack = availableAtks[i];
                return;
            }
        }
    }


    public void FireCastParticles()
    {
        CastAttackParticles();
    }

    //start the casting particlaes foe the attack
    public virtual void CastAttackParticles()
    {
        if(nextAttack != null)
        {
            GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Cast : nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
           SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);
            cast.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharInfo.BaseSpeed);

            if (nextAttack.CurrentAttackType == AttackType.Particles)
            {
                CharInfo.Stamina -= nextAttack.StaminaCost;
                EventManager.Instance?.UpdateStamina(this);
                if (nextAttack.AttackInput > AttackInputType.Weak)
                {
                    CameraManagerScript.Instance.CameraShake(CameraShakeType.Powerfulattack);
                }
            }
        }
    }

    public void CreateParticleAttack()
    {
        if(nextAttack != null)
        {
            if (nextAttack.CurrentAttackType == AttackType.Particles)
            {
                foreach (BulletBehaviourInfoClass item in nextAttack.ParticlesAtk.BulletTrajectories)
                {
                    CreateBullet(item);
                }
            }
            else if(nextAttack.CurrentAttackType == AttackType.Tile)
            {
                CreateTileAttack();
            }
            else if(nextAttack.CurrentAttackType == AttackType.Totem)
            {
                CreateTotemAttack();
            }
        }
    }

    public virtual void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
    }

    public virtual void CreateBullet(BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo)
    {
    }

    public Vector2Int nextAttackPos;
    public virtual void CreateTileAttack()
    {

        if (nextAttack != null && nextAttack.CurrentAttackType == AttackType.Tile && CharInfo.Health > 0 && IsOnField)
        {
            CharInfo.RapidAttack.DamageMultiplier = CharInfo.RapidAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;
            CharInfo.PowerfulAttac.DamageMultiplier = CharInfo.PowerfulAttac.B_DamageMultiplier * nextAttack.DamageMultiplier;

            if(nextAttack.AttackInput > AttackInputType.Strong && CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
            {
                StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == CharInfo.CharacterID).First();
                sic.DamageExp += nextAttack.ExperiencePoints;
            }


            for (int i = 0; i < nextAttack.TilesAtk.BulletTrajectories.Count; i++)
            {
                foreach (BattleFieldAttackTileClass target in nextAttack.TilesAtk.BulletTrajectories[i].BulletEffectTiles)
                {
                    int rand = UnityEngine.Random.Range(0, 100);
                    if (rand <= nextAttack.TilesAtk.BulletTrajectories[i].ExplosionChances)
                    {
                        Vector2Int res = nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget ? target.Pos + nextAttackPos :
                        nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf ? target.Pos + UMS.CurrentTilePos : target.Pos;
                        if (GridManagerScript.Instance.isPosOnField(res))
                        {
                            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(res);
                            if (bts._BattleTileState != BattleTileStateType.NonUsable)
                            {
                                if (nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf && bts.WalkingSide == UMS.WalkingSide)
                                {
                                    shotsLeftInAttack++;

                                    bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, res,
                                    0, CharInfo.Elemental, this,
                                    target, target.EffectChances);
                                }
                                else if (nextAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf && bts.WalkingSide != UMS.WalkingSide)
                                {
                                    //new way

                                    string animName = GetAttackAnimName();
                                    Spine.Animation anim = SpineAnim.skeleton.Data.FindAnimation(animName);

                                    List<Spine.Timeline> evs = anim?.Timelines?.Items?.Where(r => r is Spine.EventTimeline).ToList();
                                    Spine.EventTimeline ev = evs.Where(r => ((Spine.EventTimeline)r).Events.Where(p => p.Data.Name == "FireBulletParticle").ToList().Count > 0).First() as Spine.EventTimeline;

                                    float animDelay = ev.Events.Where(r => r.Data.Name == "FireBulletParticle").First().Time;

                                    shotsLeftInAttack++;
                                    AttackedTiles(bts);
                                    if (nextAttack.AttackInput > AttackInputType.Weak && i == 0)
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, res,
                                    (CharInfo.DamageStats.BaseDamage * nextAttack.DamageMultiplier) * GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).TileADStats.x, CharInfo.Elemental, this,
                                    target, target.EffectChances, (nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y))) + animDelay);//(nextAttack.TilesAtk.BulletTrajectories[i].Delay * 0.1f)
                                    }
                                    else if(nextAttack.AttackInput == AttackInputType.Weak)
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, res,
                                    (CharInfo.DamageStats.BaseDamage * nextAttack.DamageMultiplier) * GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).TileADStats.x, CharInfo.Elemental, this,
                                    target, target.EffectChances, (nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y))) + animDelay); // 
                                    }
                                    else
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, res,
                                   (CharInfo.DamageStats.BaseDamage * nextAttack.DamageMultiplier) * GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).TileADStats.x, CharInfo.Elemental, this,
                                   target, target.EffectChances);
                                    }



                                    /*
                                  OLD Way
                                  
                                bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, res,
                                    CharInfo.DamageStats.BaseDamage, CharInfo.Elemental, this,
                                    target, target.EffectChances);
                                 */

                                }
                            }
                        }
                    }
                }
            }
            if(shotsLeftInAttack == 0)
            {
                Attacking = false;
                currentAttackPhase = AttackPhasesType.End;
            }
        }
    }

    public virtual void CreateTotemAttack()
    {
        if (nextAttack != null && nextAttack.CurrentAttackType == AttackType.Totem && CharInfo.Health > 0 && IsOnField)
        {
            CharInfo.RapidAttack.DamageMultiplier = CharInfo.RapidAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;
            CharInfo.PowerfulAttac.DamageMultiplier = CharInfo.PowerfulAttac.B_DamageMultiplier * nextAttack.DamageMultiplier;
            StartCoroutine(Totem());
        }
    }


    IEnumerator Totem()
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => (currentAttackPhase != AttackPhasesType.End || CharInfo.HealthPerc <= 0));
        BattleTileScript res;
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        SideType side = nextAttack.TotemAtk.IsPlayerSide ? UMS.Side : UMS.Side == SideType.LeftSide ? SideType.RightSide : SideType.LeftSide;
        res = GridManagerScript.Instance.GetFreeBattleTile(side == SideType.LeftSide ? WalkingSideType.LeftSide : WalkingSideType.RightSide);
        res.SetupEffect(nextAttack.TotemAtk.Effects, nextAttack.TotemAtk.DurationOnField, nextAttack.TotemAtk.TotemIn);
        List<TotemTentacleClass> tentacles = new List<TotemTentacleClass>();
        TotemTentacleClass checker;
        GameObject ps = null;
        if (nextAttack.TotemAtk.TentaclePrefab != ParticlesType.None)
        {
            float timer = 0;
            while (timer < nextAttack.TotemAtk.DurationOnField)
            {
                foreach (TotemTentacleClass item in tentacles)
                {
                    item.isActive = false;
                }
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
                timer += BattleManagerScript.Instance.DeltaTime;
                List<BaseCharacter> enemy = (matchType == MatchType.PPPPvE || matchType == MatchType.PPvE || matchType == MatchType.PvE) ?
                WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList() :
                BattleManagerScript.Instance.GetAllPlayerActiveChars().Where(r => r.UMS.Side == side).ToList();

                foreach (BaseCharacter item in enemy)
                {
                    checker = tentacles.Where(r => r.CharAffected == item).FirstOrDefault();
                    if (checker != null)
                    {
                        checker.isActive = true;
                    }
                    else
                    {
                        if (nextAttack.TotemAtk.TentaclePrefab != ParticlesType.None)
                        {
                            ps = ParticleManagerScript.Instance.GetParticle(nextAttack.TotemAtk.TentaclePrefab);
                            ps.transform.position = res.transform.position;
                            ps.SetActive(true);
                            foreach (VFXOffsetToTargetVOL pstimeG in ps.GetComponentsInChildren<VFXOffsetToTargetVOL>())
                            {
                                pstimeG.Target = item.CharInfo.Head;
                            }
                        }

                        foreach (ScriptableObjectAttackEffect effect in nextAttack.TotemAtk.Effects.Where(r => r.StatsToAffect != BuffDebuffStatsType.BlockTile).ToList())
                        {
                            item.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Dark, this, effect));
                        }


                        foreach (ScriptableObjectAttackEffect effect in nextAttack.TotemAtk.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.BlockTile).ToList())
                        {
                            res.BlockTileForTime(effect.Duration, ParticleManagerScript.Instance.GetParticle(effect.Particles));
                        }


                        tentacles.Add(new TotemTentacleClass(item, ps, true));
                    }
                }

                foreach (TotemTentacleClass item in tentacles.Where(r => !r.isActive).ToList())
                {
                    item.PS.gameObject.SetActive(false);
                    tentacles.Remove(item);
                }
            }
        }
    }


    public virtual string GetAttackAnimName()
    {
        return nextAttack.PrefixAnim + (nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? "_Loop" : "_AtkToIdle");
    }

    public virtual void AttackedTiles(BattleTileScript bts)
    {

    }

    #endregion
    #region Defence

    protected float defenceAnimSpeedMultiplier = 5f;
    protected bool IsDefStartCo = false;

    public void StartDefending()
    {
        if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CharActionlist.Contains( CharacterActionType.Defence) || !canDefend)
        {
            return;
        }
        SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
        SpineAnim.SetAnimationSpeed(defenceAnimSpeedMultiplier);
        if (canDefend && CharInfo.Shield >= UniversalGameBalancer.Instance.defenceCost)
        {
            CharInfo.Shield -= UniversalGameBalancer.Instance.defenceCost;
            isDefendingStop = false;
            isDefending = true;
            IsDefStartCo = false;
            DefendingHoldingTimer = 0;
            StartCoroutine(Defending_Co());
        }
        else
        {
            StartCoroutine(RejectDefending_Co());
        }
    }

    private IEnumerator RejectDefending_Co()
    {
        float timer = (SpineAnim.GetAnimLenght(CharacterAnimationStateType.Defending) / defenceAnimSpeedMultiplier) * 0.25f;
        while (timer != 0f)
        {
            timer = Mathf.Clamp(timer - BattleManagerScript.Instance.DeltaTime, 0f, (SpineAnim.GetAnimLenght(CharacterAnimationStateType.Defending) / defenceAnimSpeedMultiplier) * 0.25f);
            yield return null;
        }
        SetAnimation(CharacterAnimationStateType.Idle, true);
        yield return null;
    }

    private IEnumerator ReloadDefending_Co()
    {
        NewIManager.Instance.PlayLowShieldIndicatorForCharacter(CharInfo.CharacterID, UMS.Side);

        StopDefending();
        canDefend = false;
        while (CharInfo.ShieldPerc != 100f)
        {
            yield return null;
        }

        NewIManager.Instance.StopLowShieldIndicatorForCharacter(CharInfo.CharacterID, UMS.Side);
        canDefend = true;
    }

    private IEnumerator Defending_Co()
    {
        while (isDefending && CharInfo.Shield > 0f && canDefend)
        {
            if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString())
            {
                SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                SpineAnim.SetAnimationSpeed(5);
            }
            yield return null;
            DefendingHoldingTimer += BattleManagerScript.Instance.DeltaTime;
            if (CharInfo.ShieldPerc == 0)
            {
                StartCoroutine(ReloadDefending_Co());
            }
        }
        DefendingHoldingTimer = 0;
    }

    public void StopDefending()
    {
        if (isDefending)
        {
            isDefendingStop = true;
            if(SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defending.ToString()))
            {
                Debug.Log("FINISHED STOP <color=blue>DEFENDING</color>");
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }

    }

    #endregion
    #region Move

    public virtual IEnumerator MoveCharOnDir_Co(InputDirection nextDir)
    {
        if ((CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString() && CharActionlist.Contains(CharacterActionType.Move)) || BattleManagerScript.Instance.VFXScene)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            CharacterAnimationStateType AnimState;
            Vector2Int dir;
            AnimationCurve curve;
            GetDirectionVectorAndAnimationCurve(nextDir, out AnimState, out dir, out curve);

            currentBattleTilesToCheck = CheckTileAvailabilityUsingDir(dir);

            if (currentBattleTilesToCheck.Count > 0 &&
                currentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                currentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count && GridManagerScript.Instance.isPosOnField(UMS.CurrentTilePos + dir))
            {
                SetAnimation(AnimState);
                isMoving = true;
                if (prevBattleTile.Count > 1)
                {

                }
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                    item.isTaken = false;
                }
                UMS.CurrentTilePos += dir;
                CharOredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
                if (CharInfo.UseLayeringSystem)
                {
                    SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
                }

                CurrentBattleTiles = currentBattleTilesToCheck;
                UMS.Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in currentBattleTilesToCheck)
                {
                    //Debug.LogError(item.Pos + "               " + BattleTileStateType.Occupied);
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    UMS.Pos.Add(item.Pos);
                }

                BattleTileScript resbts = CurrentBattleTiles.Where(r => r.Pos == UMS.CurrentTilePos).First();

                if (resbts != null)
                {
                    foreach (BattleTileScript item in prevBattleTile)
                    {
                        BattleManagerScript.Instance.OccupiedBattleTiles.Remove(item);
                    }
                    BattleManagerScript.Instance.OccupiedBattleTiles.AddRange(CurrentBattleTiles);
                    stopCo = true;

                    MoveCo = MoveByTileSpace(resbts.transform.position, curve, SpineAnim.GetAnimLenght(AnimState));
                    yield return MoveCo;
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                if (TileMovementCompleteEvent != null && TileMovementCompleteEvent.Target != null)
                {
                    TileMovementCompleteEvent(this);
                }
            }
        }
    }




    public virtual void MoveCharOnDirection(InputDirection nextDir)
    {
        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString() || SpineAnim.CurrentAnim == CharacterAnimationStateType.Arriving.ToString() ||
            SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle.ToString() || SwapWhenPossible || CharInfo.SpeedStats.MovementSpeed * CharInfo.SpeedStats.BaseSpeed <= 0)
        {
            return;
        }

        if (currentAttackPhase == AttackPhasesType.Loading || currentAttackPhase == AttackPhasesType.Cast_Strong || currentAttackPhase == AttackPhasesType.Bullet_Strong ||
             SpineAnim.CurrentAnim.Contains("S_Buff") || SpineAnim.CurrentAnim.Contains("S_DeBuff"))
        {
            return;
        }

        StartCoroutine(MoveCharOnDir_Co(nextDir));

    }

    public void GetDirectionVectorAndAnimationCurve(InputDirection nextDir, out CharacterAnimationStateType AnimState, out Vector2Int dir, out AnimationCurve curve)
    {
        AnimState = CharacterAnimationStateType.Idle;
        curve = new AnimationCurve();
        dir = Vector2Int.zero;
        switch (nextDir)
        {
            case InputDirection.Up:
                dir = new Vector2Int(-1, 0);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement;
                AnimState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirection.Down:
                dir = new Vector2Int(1, 0);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.DownMovement : SpineAnim.Speed_Time_Curves.DownMovement;
                AnimState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirection.Right:
                dir = new Vector2Int(0, 1);
                AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                if (AnimState == CharacterAnimationStateType.DashLeft)
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.BackwardMovement : SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.ForwardMovement : SpineAnim.Speed_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirection.Left:
                dir = new Vector2Int(0, -1);
                AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                if(AnimState == CharacterAnimationStateType.DashLeft)
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.BackwardMovement : SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.ForwardMovement : SpineAnim.Speed_Time_Curves.ForwardMovement;
                }

                break;
        }
    }


    protected List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        List<Vector2Int> nextPos = CalculateNextPosUsingDir(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(UMS.Pos, nextPos, UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    protected List<BattleTileScript> CheckTileAvailabilityUsingPos(Vector2Int dir)
    {
        List<Vector2Int> nextPos = CalculateNextPosUsinPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(UMS.Pos, nextPos, UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsingDir(Vector2Int direction)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        UMS.Pos.ForEach(r => res.Add(r + direction));
        return res;
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsinPos(Vector2Int direction)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        UMS.Pos.ForEach(r => res.Add((r - UMS.CurrentTilePos) + direction));
        return res;
    }


    bool stopCo = false;
    public virtual IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        //Debug.Log("StartMoveCo  " + Time.time);
        float timer = 0;
        stopCo = false;
        float spaceTimer = 0;
        bool isMovCheck = false;
        bool isDefe = false;
        Transform spineT = SpineAnim.transform;
        Vector3 offset = spineT.position;
        transform.position = nextPos;
        spineT.position = offset;
        Vector3 localoffset = spineT.localPosition;

        while (timer < 1 && !stopCo)
        {
            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            timer += (BattleManagerScript.Instance.FixedDeltaTime / (animLength / (CharInfo.SpeedStats.MovementSpeed * CharInfo.SpeedStats.BaseSpeed * BattleManagerScript.Instance.MovementMultiplier)));
            spaceTimer = curve.Evaluate(timer);
            spineT.localPosition = Vector3.Lerp(localoffset, LocalSpinePosoffset, spaceTimer);

            if (timer > 0.7f && !isMovCheck)
            {
                isMovCheck = true;
                isMoving = false;
                if (isDefending && !isDefe)
                {
                   /* isDefe = true;
                    SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                    SpineAnim.SetAnimationSpeed(5);*/
                }
                TileMovementCompleteEvent?.Invoke(this);
            }

            if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
            {
                isMoving = false;
                TileMovementCompleteEvent?.Invoke(this);
                MoveCo = null;
                spineT.localPosition = LocalSpinePosoffset;
                yield break;
            }
        }

        spineT.localPosition = LocalSpinePosoffset;
        //Debug.Log("EndMoveCo");
    }

    #endregion
    #region Buff/Debuff


    public void Buff_DebuffCo(Buff_DebuffClass bdClass)
    {
        if(!SpineAnim.CurrentAnim.Contains("Reverse") && CharInfo.HealthPerc > 0)
        {
            BuffDebuffClass item = BuffsDebuffsList.Where(r => r.Stat == bdClass.Effect.StatsToAffect).FirstOrDefault();
            string[] newBuffDebuff = bdClass.Effect.Name.Split('_');
            if (item == null)
            {
                //Debug.Log(bdClass.Name + "   " + newBuffDebuff.Last());
                item = new BuffDebuffClass(bdClass.Effect.Name, bdClass.Effect.StatsToAffect, Convert.ToInt32(newBuffDebuff.Last()), bdClass, bdClass.Effect.Duration, bdClass.EffectMaker);
                item.BuffDebuffCo = Buff_DebuffCoroutine(item);
                BuffsDebuffsList.Insert(0, item);
                UMS.buffIconHandler.RefreshIcons(BuffsDebuffsList);
                StartCoroutine(item.BuffDebuffCo);
            }
            else
            {
                if (item.Level <= Convert.ToInt32(newBuffDebuff.Last()))
                {
                    string[] currentBuffDebuff = item.Name.ToString().Split('_');
                    item.CurrentBuffDebuff.Stop_Co = true;
                    int index = BuffsDebuffsList.IndexOf(item);
                    BuffsDebuffsList.Remove(item);
                    item = new BuffDebuffClass(bdClass.Effect.Name, bdClass.Effect.StatsToAffect, Convert.ToInt32(newBuffDebuff.Last()), bdClass, bdClass.Effect.Duration, bdClass.EffectMaker);
                    item.BuffDebuffCo = Buff_DebuffCoroutine(item);
                    BuffsDebuffsList.Insert(index, item);
                    StartCoroutine(item.BuffDebuffCo);
                }
            }
        }
    }

    //Used to Buff/Debuff the character
    public IEnumerator Buff_DebuffCoroutine(BuffDebuffClass bdClass)
    {
        GameObject ps = null;
        if (bdClass.CurrentBuffDebuff.Effect.Particles != ParticlesType.None)
        {
            ps = ParticleManagerScript.Instance.GetParticle(bdClass.CurrentBuffDebuff.Effect.Particles);
            ps.transform.parent = SpineAnim.transform;
            ps.transform.localPosition = Vector3.zero;
            ps.SetActive(true);
            ps.GetComponent<ParticleHelperScript>().UpdatePSTime(bdClass.Duration);
        }

        float val = 0;

        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.DamageStats_BaseDamage:
                CharInfo.DamageStats.BaseDamage += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.DamageStats.B_BaseDamage / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                break;
            case BuffDebuffStatsType.Health:
                val = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.HealthStats.B_Base / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                CharInfo.Health += val;
                HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Heal, bdClass.EffectMaker.SpineAnim.transform);
                EventManager.Instance?.UpdateHealth(this);
                HealthStatsChangedEvent?.Invoke(bdClass.CurrentBuffDebuff.Value, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, SpineAnim.transform);
                break;
            case BuffDebuffStatsType.SpeedStats_BaseSpeed:
                if (bdClass.CurrentBuffDebuff.Value > 0)
                {
                    CharInfo.SpeedStats.BaseSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.SpeedStats.B_BaseSpeed / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                }
                else
                {
                    while (isMoving)
                    {
                        yield return null;
                    }
                    CharInfo.SpeedStats.BaseSpeed = 0;
                }
                SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.BaseSpeed);
                if (bdClass.CurrentBuffDebuff.Value == 0)
                {
                    if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
                    {
                        yield return BattleManagerScript.Instance.WaitFor(0.5f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
                        CharActionlist.Remove(CharacterActionType.SwitchCharacter);
                        BattleManagerScript.Instance.DeselectCharacter(CharInfo.CharacterID, UMS.Side, CurrentPlayerController);
                        BattleManagerScript.Instance.CurrentSelectedCharacters[CurrentPlayerController].Character = null;
                        UMS.IndicatorAnim.SetBool("indicatorOn", false);
                        if (BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, CurrentPlayerController) != null)
                        {
                            CharacterType_Script cb = (CharacterType_Script)BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, CurrentPlayerController);
                            BattleManagerScript.Instance.SetCharOnBoardOnFixedPos(CurrentPlayerController, cb.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeBattleTile(UMS.WalkingSide).Pos);
                            cb.SetCharSelected(true, CurrentPlayerController);
                            BattleManagerScript.Instance.SelectCharacter(CurrentPlayerController, cb);
                        }
                    }
                }
                break;
            case BuffDebuffStatsType.SpeedStats_MovementSpeed:
                CharInfo.SpeedStats.MovementSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.SpeedStats.B_MovementSpeed / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                break;
            case BuffDebuffStatsType.Drain:
                val = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Value ? bdClass.CurrentBuffDebuff.Value : (CharInfo.HealthStats.B_Base / 100) * bdClass.CurrentBuffDebuff.Value;
                bdClass.EffectMaker.CharInfo.Health += val;
                EventManager.Instance?.UpdateHealth(this);
                EventManager.Instance?.UpdateHealth(bdClass.EffectMaker);
                HealthStatsChangedEvent?.Invoke(val, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, SpineAnim.transform);
                HealthStatsChangedEvent?.Invoke(val, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, bdClass.EffectMaker.SpineAnim.transform);
                break;
            case BuffDebuffStatsType.Zombification:
                if (CharInfo.Health > 0)
                {
                    BattleManagerScript.Instance.Zombification(this, bdClass.Duration, bdClass.CurrentBuffDebuff.Effect.AIs);
                }
                break;
            case BuffDebuffStatsType.ShieldStats_BaseShieldRegeneration:
                CharInfo.ShieldStats.BaseShieldRegeneration += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.ShieldStats.B_BaseShieldRegeneration / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                break;
            case BuffDebuffStatsType.AttackChange:
                CharInfo.CurrentAttackTypeInfo.Add(bdClass.CurrentBuffDebuff.Effect.Atk);
                break;
            case BuffDebuffStatsType.StaminaStats_Stamina:
                CharInfo.StaminaStats.Stamina += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.StaminaStats.B_Base / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                break;
        }

        if (bdClass.Duration > 0)
        {
            int iterator = 0;
            while (bdClass.CurrentBuffDebuff.Timer <= bdClass.Duration && !bdClass.CurrentBuffDebuff.Stop_Co)
            {
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

                bdClass.CurrentBuffDebuff.Timer += BattleManagerScript.Instance.DeltaTime;

                if (((int)bdClass.CurrentBuffDebuff.Timer) > iterator && bdClass.Stat.ToString().Contains("Overtime"))
                {
                    iterator++;
                    if (bdClass.Stat == BuffDebuffStatsType.Health_Overtime)
                    {
                        CharInfo.Health += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Value ? bdClass.CurrentBuffDebuff.Value : (CharInfo.HealthStats.Base / 100) * bdClass.CurrentBuffDebuff.Value;
                        HealthStatsChangedEvent?.Invoke(bdClass.CurrentBuffDebuff.Value, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, SpineAnim.transform);
                        EventManager.Instance?.UpdateHealth(this);

                        //Apply Bleed
                        if (bdClass.CurrentBuffDebuff.Value < 0)
                        {
                            ParticleManagerScript.Instance.FireParticlesInPosition(ParticleManagerScript.Instance.GetParticlePrefabByName(ParticlesType.Status_Debuff_Bleed), CharacterNameType.None, AttackParticlePhaseTypes.Cast, SpineAnim.transform.position, SideType.LeftSide, AttackInputType.Weak);
                        }
                    }
                    if (bdClass.Stat == BuffDebuffStatsType.Drain_Overtime)
                    {
                        val = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Value ? bdClass.CurrentBuffDebuff.Value : (CharInfo.HealthStats.Base / 100) * bdClass.CurrentBuffDebuff.Value;
                        HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Heal, bdClass.EffectMaker.SpineAnim.transform);
                        HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Damage, SpineAnim.transform);
                        bdClass.EffectMaker.CharInfo.Health += val;
                        CharInfo.Health -= val;
                        EventManager.Instance?.UpdateHealth(bdClass.EffectMaker);
                        EventManager.Instance?.UpdateHealth(this);
                    }
                }
            }

            ps?.SetActive(false);
            if(!bdClass.CurrentBuffDebuff.Stop_Co)
            {
                switch (bdClass.Stat)
                {
                    case BuffDebuffStatsType.DamageStats_BaseDamage:
                        CharInfo.DamageStats.BaseDamage = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.DamageStats.B_BaseDamage / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                        break;
                    case BuffDebuffStatsType.SpeedStats_BaseSpeed:
                        if (bdClass.CurrentBuffDebuff.Value > 0)
                        {
                            CharInfo.SpeedStats.BaseSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.SpeedStats.B_BaseSpeed / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                        }
                        else
                        {
                            CharInfo.SpeedStats.BaseSpeed = CharInfo.SpeedStats.B_BaseSpeed;
                        }
                        SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.BaseSpeed);
                        if (bdClass.CurrentBuffDebuff.Value == 0 && CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
                        {
                            CharActionlist.Add(CharacterActionType.SwitchCharacter);
                            if (BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r=> r.Value.Character == null).ToList().Count > 0)
                            {
                                CurrentPlayerController = BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == null).OrderBy(a=> a.Value.NotPlayingTimer).First().Key;
                                ((CharacterType_Script)this).SetCharSelected(true, CurrentPlayerController);
                                BattleManagerScript.Instance.SelectCharacter(CurrentPlayerController, (CharacterType_Script)this);
                            }
                            else
                            {
                                yield return BattleManagerScript.Instance.RemoveCharacterFromBaord(this, true);
                            }
                        }
                        break;
                    case BuffDebuffStatsType.SpeedStats_MovementSpeed:
                        CharInfo.SpeedStats.MovementSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.SpeedStats.B_MovementSpeed / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                        break;
                    case BuffDebuffStatsType.ShieldStats_BaseShieldRegeneration:
                        CharInfo.ShieldStats.BaseShieldRegeneration -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.ShieldStats.B_BaseShieldRegeneration / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                        break;
                    case BuffDebuffStatsType.AttackChange:
                        CharInfo.CurrentAttackTypeInfo.Remove(bdClass.CurrentBuffDebuff.Effect.Atk);
                        break;
                    case BuffDebuffStatsType.StaminaStats_Stamina:
                        CharInfo.StaminaStats.Stamina -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? (CharInfo.StaminaStats.B_Base / 100f) * bdClass.CurrentBuffDebuff.Value : bdClass.CurrentBuffDebuff.Value;
                        break;
                }
            }
        }
        BuffsDebuffsList.Remove(bdClass); 
        UMS.buffIconHandler.RefreshIcons(BuffsDebuffsList);
        if (ps != null && ps.activeInHierarchy)
        {
            yield return BattleManagerScript.Instance.WaitFor(1, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            ps?.SetActive(false);
        }
    }


    private void ElementalResistance(Buff_DebuffClass bdClass)
    {
        /*CurrentBuffsDebuffsClass currentBuffDebuff = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
        ElementalWeaknessType BaseWeakness = GetElementalMultiplier(CharInfo.DamageStats.ElementalsResistence, bdClass.ElementalResistence.Elemental);
        CurrentBuffsDebuffsClass newBuffDebuff = new CurrentBuffsDebuffsClass();
        newBuffDebuff.ElementalResistence = bdClass.ElementalResistence;
        newBuffDebuff.Duration = bdClass.Duration;
        if (currentBuffDebuff != null)
        {
            StopCoroutine(currentBuffDebuff.BuffDebuffCo);
            BuffsDebuffs.Remove(currentBuffDebuff);
            newBuffDebuff.BuffDebuffCo = ElementalBuffDebuffCo(newBuffDebuff);

            ElementalWeaknessType newBuffDebuffValue = bdClass.ElementalResistence.ElementalWeakness + (int)currentBuffDebuff.ElementalResistence.ElementalWeakness > ElementalWeaknessType.ExtremelyResistent ?
                ElementalWeaknessType.ExtremelyResistent : bdClass.ElementalResistence.ElementalWeakness + (int)currentBuffDebuff.ElementalResistence.ElementalWeakness < ElementalWeaknessType.ExtremelyWeak ? ElementalWeaknessType.ExtremelyWeak :
                bdClass.ElementalResistence.ElementalWeakness + (int)currentBuffDebuff.ElementalResistence.ElementalWeakness;

            newBuffDebuff.ElementalResistence.ElementalWeakness = newBuffDebuffValue + (int)BaseWeakness > ElementalWeaknessType.ExtremelyResistent ?
                ElementalWeaknessType.ExtremelyResistent - (int)BaseWeakness : newBuffDebuffValue + (int)BaseWeakness < ElementalWeaknessType.ExtremelyWeak ? ElementalWeaknessType.ExtremelyWeak + (int)BaseWeakness :
                newBuffDebuffValue + (int)BaseWeakness;

            BuffsDebuffs.Add(newBuffDebuff);
            StartCoroutine(newBuffDebuff.BuffDebuffCo);
        }
        else
        {
            newBuffDebuff.BuffDebuffCo = ElementalBuffDebuffCo(newBuffDebuff);
            newBuffDebuff.ElementalResistence.ElementalWeakness = bdClass.ElementalResistence.ElementalWeakness + (int)BaseWeakness > ElementalWeaknessType.ExtremelyResistent ?
                ElementalWeaknessType.ExtremelyResistent - (int)BaseWeakness : bdClass.ElementalResistence.ElementalWeakness + (int)BaseWeakness < ElementalWeaknessType.ExtremelyWeak ? ElementalWeaknessType.ExtremelyWeak + (int)BaseWeakness :
                bdClass.ElementalResistence.ElementalWeakness + (int)BaseWeakness;
            BuffsDebuffs.Add(newBuffDebuff);
            StartCoroutine(newBuffDebuff.BuffDebuffCo);
        }*/
    }



    private IEnumerator ElementalBuffDebuffCo(CurrentBuffsDebuffsClass newBuffDebuff)
    {
        /*float timer = 0;
         float newDuration = newBuffDebuff.Duration - Mathf.Abs((int)newBuffDebuff.ElementalResistence.ElementalWeakness);
         while (timer <= newDuration)
         {
             yield return BattleManagerScript.Instance.PauseUntil();

             timer += Time.fixedDeltaTime;
         }

         for (int i = 0; i < Mathf.Abs((int)newBuffDebuff.ElementalResistence.ElementalWeakness); i++)
         {
             timer = 0;
             while (timer <= 1)
             {
                 yield return BattleManagerScript.Instance.PauseUntil();

                 timer += Time.fixedDeltaTime;
             }
         }

         BuffsDebuffs.Remove(newBuffDebuff);*/

        yield return null;
    }

    #endregion
    #region Animation

    public void ArrivingEvent()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
        if (CharInfo.AudioProfile.ArrivingCry != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.ArrivingCry, AudioBus.MidPrio, transform);
        }

        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalImpact, AudioBus.MidPrio, transform);
        UMS.ArrivingParticle.transform.position = transform.position;
        UMS.ArrivingParticle.SetActive(true);
    }


    string queuedAnim_Name = "";
    bool queuedAnim_Loop = false;
    float queuedAnim_Transition = 0f;
    float queuedAnim_Speed = 1f;
    public void QueueAnimation(string animState, bool loop = false, float transition = 0, float speed = 1f)
    {
        queuedAnim_Name = animState;
        queuedAnim_Loop = loop;
        queuedAnim_Transition = transition;
        queuedAnim_Speed = speed;
    }
    public bool PlayQueuedAnim()
    {
        if (queuedAnim_Name != "")
        {
            SetAnimation(queuedAnim_Name, queuedAnim_Loop, queuedAnim_Transition);
            SpineAnim.SetAnimationSpeed(queuedAnim_Speed);
            QueueAnimation("", false, 0f, 1f);
            return true;
        }
        return false;
    }

    protected bool isPuppeting = false;
    protected int puppetAnimCompleteTick = 0;
    public IEnumerator PuppetAnimation(string animState, int loops, bool _pauseOnEndFrame = false, float animSpeed = 1f, bool loop = false)
    {
        isPuppeting = true;
        puppetAnimCompleteTick = 0;
        int currentAnimPlay = 0;
        while(currentAnimPlay < loops)
        {
            SetAnimation(animState, loop, _pauseOnLastFrame: (currentAnimPlay + 1 == loops && _pauseOnEndFrame));
            SpineAnim.SetAnimationSpeed(animSpeed);
            while(currentAnimPlay == puppetAnimCompleteTick)
            {
                yield return null;
            }
            currentAnimPlay++;
        }
        isPuppeting = false;
    }

    public virtual void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    public IEnumerator SlowDownAnimation(float perc, Func<bool> condition)
    {
        yield return BattleManagerScript.Instance.WaitUpdate(()=>
        {
            SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed * perc);
        }, condition);

        SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
    }

    protected bool pauseOnLastFrame = false;
    public virtual void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        
        if (CharInfo.SpeedStats.BaseSpeed <= 0)
        {
            return;
        }
        //Debug.Log(animState.ToString() + SpineAnim.CurrentAnim.ToString() + CharInfo.CharacterID.ToString());
        if (animState == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
        }

        if (animState.Contains(CharacterAnimationStateType.GettingHit.ToString()) && currentAttackPhase != AttackPhasesType.End)
        {
            return;
        }

        if (animState.Contains(CharacterAnimationStateType.GettingHit.ToString()) && Attacking)
        {
            return;
        }


        if (isMoving && (animState.ToString() != CharacterAnimationStateType.Reverse_Arriving.ToString() && animState.ToString() != CharacterAnimationStateType.Defeat_ReverseArrive.ToString()))
        {
            return;
        }


        if (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Arriving.ToString()) || SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Reverse_Arriving.ToString()))
        {
            return;
        }

        float AnimSpeed = 1;

        if(CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && animState.Contains("IdleToAtk"))
        {
            //AnimSpeed = CharInfo.SpeedStats.AttackSpeed * CharInfo.BaseSpeed;
            AnimSpeed = SpineAnim.GetAnimLenght(animState) / 0.01f;
        }
        else if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && animState.Contains("Loop"))
        {
            //AnimSpeed = CharInfo.SpeedStats.AttackSpeed * CharInfo.BaseSpeed;
            AnimSpeed = SpineAnim.GetAnimLenght(animState) / 0.2f;
        }
        else if (animState.Contains("Dash"))
        {
            AnimSpeed = CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed;
        }
        else if (animState.Contains("JumpTransition") || animState.Contains("Arriv"))
        {
            AnimSpeed = CharInfo.SpeedStats.LeaveSpeed;
        }
        else
        {
            AnimSpeed = CharInfo.BaseSpeed;
        }

        pauseOnLastFrame = _pauseOnLastFrame;
        SpineAnim.SetAnim(animState, loop, transition);
        SpineAnim.SetAnimationSpeed(AnimSpeed);
    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);
        SpineAnim.SetupSpineAnim();

        if (SpineAnim.SpineAnimationState != null)
        {
            SpineAnim.CharOwner = this;
            SpineAnim.SpineAnimationState.Complete += SpineAnimationState_Complete;
            SpineAnim.SpineAnimationState.Event += SpineAnimationState_Event;

            if (SpineAnim.CurveType == MovementCurveType.Space_Time)
            {
                UMS.HpBarContainer.parent = SpineAnim.transform;
                UMS.StaminaBarContainer.parent = SpineAnim.transform;
                UMS.IndicatorContainer.parent = SpineAnim.transform;
                UMS.buffIconHandler.transform.parent = SpineAnim.transform;
            }
        }
    }

    public virtual void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {

    }

    public virtual void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        if (isPuppeting) puppetAnimCompleteTick++;
        if (pauseOnLastFrame) return;
        if (PlayQueuedAnim()) return;

        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() || completedAnim == CharacterAnimationStateType.JumpTransition_IN.ToString() || completedAnim.Contains("Growing") )
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            CharArrivedOnBattleField();
        }

        if (completedAnim != CharacterAnimationStateType.Idle.ToString() && !SpineAnim.Loop)
        {
            SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
            //Debug.Log("IDLE     " + completedAnim.ToString());
            SpineAnim.SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            //SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
        }
    }

    #endregion

    public virtual bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        return SetDamage(attacker, damage, elemental, isCritical);
    }

    public virtual bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        if (!IsOnField)
        {
            return false;
        }
        HealthChangedType healthCT = HealthChangedType.Damage;
        bool res;
        if (isDefending)
        {
            GameObject go;
            if (DefendingHoldingTimer < CharInfo.DefenceStats.Invulnerability)
            {
                Sic.ReflexExp += damage;
                damage = 0;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Full, AudioBus.MidPrio);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.fullDefenceCost;
                CharInfo.Stamina += UniversalGameBalancer.Instance.staminaRegenOnPerfectBlock;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.full);
                Sic.CompleteDefences++;
                ComboManager.Instance.TriggerComboForCharacter(CharInfo.CharacterID, ComboType.Defence, true, transform.position);
            }
            else
            {
                Sic.ReflexExp += damage  * 0.5f;
                damage = damage - CharInfo.DefenceStats.BaseDefence;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldNormal);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Partial, AudioBus.HighPrio);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.partialDefenceCost;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.partial);
                Sic.Defences++;
                ComboManager.Instance.TriggerComboForCharacter(CharInfo.CharacterID, ComboType.Defence, false);
                damage = damage < 0 ? 1 : damage;
            }
            healthCT = HealthChangedType.Defend;
            res = false;
            if (UMS.Facing == FacingType.Left)
            {
                go.transform.localScale = Vector3.one;
            }
            else
            {
                go.transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            //Play getting hit sound only if the character is a playable one
            if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
            {
                // AudioManager.Instance.PlayGeneric("Get_Hit_20200217");
            }
            SetAnimation(CharacterAnimationStateType.GettingHit, false, 0.1f);
            healthCT = isCritical ? HealthChangedType.CriticalHit : HealthChangedType.Damage;
            healthCT = damage < 0 ? HealthChangedType.Heal : healthCT;
            res = true;
        }

        /*  ElementalWeaknessType ElaboratedWeakness;
          CurrentBuffsDebuffsClass buffDebuffWeakness = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == elemental).FirstOrDefault();

          ElementalWeaknessType BaseWeakness = GetElementalMultiplier(CharInfo.DamageStats.ElementalsResistence, elemental);
          if (buffDebuffWeakness == null)
          {
              ElaboratedWeakness = BaseWeakness;
          }
          else
          {
              ElaboratedWeakness = BaseWeakness + (int)buffDebuffWeakness.ElementalResistence.ElementalWeakness;
          }

          switch (ElaboratedWeakness)
          {
              case ElementalWeaknessType.ExtremelyWeak:
                  damage = damage + (damage * 0.7f);
                  break;
              case ElementalWeaknessType.VeryWeak:
                  damage = damage + (damage * 0.5f);
                  break;
              case ElementalWeaknessType.Weak:
                  damage = damage + (damage * 0.3f);
                  break;
              case ElementalWeaknessType.Neutral:
                  break;
              case ElementalWeaknessType.Resistent:
                  damage = damage - (damage * 0.3f);
                  break;
              case ElementalWeaknessType.VeryResistent:
                  damage = damage - (damage * 0.5f);
                  break;
              case ElementalWeaknessType.ExtremelyResistent:
                  damage = damage - (damage * 0.7f);
                  break;
          }*/


       
        if (CharInfo.Health == 0)
        {
            EventManager.Instance?.AddCharacterDeath(this);
        }
        EventManager.Instance?.UpdateHealth(this);
        EventManager.Instance?.UpdateStamina(this);

        SetFinalDamage(attacker , damage / GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos).TileADStats.y);


        HealthStatsChangedEvent?.Invoke(Mathf.Abs(damage), healthCT, SpineAnim.transform);
        return res;
    }

    public virtual void SetFinalDamage(BaseCharacter attacker, float damage)
    {
        CharInfo.Health -= damage;
    }


    public ElementalWeaknessType GetElementalMultiplier(List<ElementalResistenceClass> armorElelmntals, ElementalType elementalToCheck)
    {
        int resVal = 0;

        foreach (ElementalResistenceClass elemental in armorElelmntals)
        {

            if (elemental.Elemental != elementalToCheck)
            {
                int res = (int)elemental.Elemental + (int)elementalToCheck;
                if (res > 0)
                {
                    res -= 8;
                }

                resVal += (int)(ElementalWeaknessType)System.Enum.Parse(typeof(ElementalWeaknessType), ((RelationshipBetweenElements)res).ToString().Split('_').First()); ;
            }
            else
            {
                resVal = (int)ElementalWeaknessType.Neutral;
            }
        }

        return (ElementalWeaknessType)(resVal);
    }

    public void SetValueFromVariableName(string vName, object value)
    {
        GetType().GetField(vName).SetValue(this, value);
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    public virtual CastLoopImpactAudioClipInfoClass GetAttackAudio()
    {
        if (nextAttack == null) return null;

        switch (nextAttack.AttackInput)
        {
            case (AttackInputType.Strong):
                return CharInfo.AudioProfile.PowerfulAttack;
            case (AttackInputType.Weak):
                return CharInfo.AudioProfile.RapidAttack;
            case (AttackInputType.Skill1):
                return CharInfo.AudioProfile.Skill1;
            case (AttackInputType.Skill2):
                return CharInfo.AudioProfile.Skill2;
            case (AttackInputType.Skill3):
                return CharInfo.AudioProfile.Skill3;
        }

        return null;
    }

    public bool AreTileNearEmpty()
    {
        List<BattleTileScript> res = CheckTileAvailabilityUsingDir(Vector2Int.up);
        res.AddRange(CheckTileAvailabilityUsingDir(Vector2Int.down));
        res.AddRange(CheckTileAvailabilityUsingDir(Vector2Int.left));
        res.AddRange(CheckTileAvailabilityUsingDir(Vector2Int.right));

        if (res.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected BaseCharacter GetTargetChar(List<BaseCharacter> enemys)
    {
        return enemys.OrderBy(r => (r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)).First();
    }
}



public class ArmorClass
{
    public float Armor;
    public float MovementSpeed;
    public float Health;
}


public class WeaponClass
{
    public float Damage;
    public float MovementSpeed;
    public float Health;
}

[System.Serializable]
public class Buff_DebuffClass
{
    public float Value
    {
        get
        {
            return UnityEngine.Random.Range(Effect.Value.x, Effect.Value.y);
        }
    }
    public ElementalResistenceClass ElementalResistence;
    public ElementalType ElementalPower;
    public float Timer;
    public bool Stop_Co = false;
    public Sprite icon;
    public BaseCharacter EffectMaker;
    public ScriptableObjectAttackEffect Effect;

    public Buff_DebuffClass(ElementalResistenceClass elementalResistence, ElementalType elementalPower
        , BaseCharacter effectMaker, ScriptableObjectAttackEffect effect)
    {
        ElementalResistence = elementalResistence;
        ElementalPower = elementalPower;
        EffectMaker = effectMaker;
        Effect = effect;
        icon = effect.icon;
    }

    public Buff_DebuffClass()
    {

    }
}


[System.Serializable]
public class CurrentBuffsDebuffsClass
{
    public ElementalResistenceClass ElementalResistence;
    public float Duration;
    public IEnumerator BuffDebuffCo;

    public CurrentBuffsDebuffsClass()
    {
    }

    public CurrentBuffsDebuffsClass(ElementalResistenceClass elementalResistence, IEnumerator buffDebuffCo, float duration)
    {
        ElementalResistence = elementalResistence;
        BuffDebuffCo = buffDebuffCo;
        Duration = duration;
    }
}


[System.Serializable]
public class BuffDebuffClass
{
    public string Name;
    public Buff_DebuffClass CurrentBuffDebuff;
    public IEnumerator BuffDebuffCo;
    public float Duration;
    public BuffDebuffStatsType Stat;
    public int Level;
    public BaseCharacter EffectMaker;

    public BuffDebuffClass()
    {

    }
    public BuffDebuffClass(string name, BuffDebuffStatsType stat, int level, Buff_DebuffClass currentCuffDebuff, float duration, BaseCharacter effectMaker)
    {
        Name = name;
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        Duration = duration;
        EffectMaker = effectMaker;
    }

    public BuffDebuffClass(string name, BuffDebuffStatsType stat, int level, Buff_DebuffClass currentCuffDebuff, IEnumerator buffDebuffCo, float duration)
    {
        Name = name;
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        BuffDebuffCo = buffDebuffCo;
        Duration = duration;
    }
}



[System.Serializable]
public class RelationshipClass
{
    public string name;
    [HideInInspector] public CharacterNameType CharOwnerId = CharacterNameType.None;
    public CharacterNameType CharacterId = CharacterNameType.None;

    [SerializeField] private int BasicValue;
    public int _CurrentValue;


    public int CurrentValue
    {
        get
        {
            return BasicValue + _CurrentValue;
        }
        set
        {
            _CurrentValue = value - BasicValue;
        }
    }

    public RelationshipClass()
    {

    }

    public RelationshipClass(CharacterNameType charOwnerId, CharacterNameType characterId, int basicValue)
    {
        CharOwnerId = charOwnerId;
        CharacterId = characterId;
        BasicValue = basicValue;
    }
}

[System.Serializable]
public class TotemTentacleClass
{
    public BaseCharacter CharAffected;
    public GameObject PS;
    public bool isActive;
    public TotemTentacleClass()
    {

    }

    public TotemTentacleClass(BaseCharacter charAffected, GameObject ps, bool isactive)
    {
        CharAffected = charAffected;
        PS = ps;
        isActive = isactive;
    }
}