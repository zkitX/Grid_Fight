//using nn.ec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, IDisposable
{
    [HideInInspector] public ScriptableObjectAttackEffect testAtkEffect = null;





    //minion
    protected float LastAttackTime;
    public int AttackWillPerc = 13;
    public int MoveWillPerc = 100;
    public int UpDownMovementPerc = 13;
    public int TowardMovementPerc = 13;
    public int AwayMovementPerc = 13;
    public List<AggroInfoClass> AggroInfoList = new List<AggroInfoClass>();
    protected float totDamage = 0;
    protected bool strongAnimDone = false;
    public ScriptableObjectAI CurrentAIState;
    public GameObject psAI = null;
    public BattleTileScript possiblePos = null;
    public Vector2Int[] path;
    public bool found = false;
    public List<BattleTileScript> possiblePositions = new List<BattleTileScript>();
    protected float lastAttackTime = 0;


    //------------------------------------

    #region Events
    public delegate void CurrentCharIsDead(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsDead CurrentCharIsDeadEvent;

    public delegate void TileMovementComplete(BaseCharacter movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;

    public delegate void HealthStatsChanged(float value, HealthChangedType changeType, Transform charOwner);
    public event HealthStatsChanged HealthStatsChangedEvent;

    public delegate void CurrentCharIsRebirth(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsRebirth CurrentCharIsRebirthEvent;

    public delegate void CurrentCharStartingAction(ControllerType playerController, CharacterActionType action);
    public event CurrentCharStartingAction CurrentCharStartingActionEvent;
    #endregion

    #region BaseChar Variables

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
    public CharacterInfoScript _CharInfo;

    public bool died = false;


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
    public bool _IsOnField = false;


    public bool isMoving = false;

    public SpineAnimationManager SpineAnim;

    public DeathAnimType DeathAnim;

    public List<CharacterActionType> CharActionlist = new List<CharacterActionType>();
    public UnitManagementScript UMS;
    public BoxCollider CharBoxCollider;
    public StatisticInfoClass Sic;

    public Vector3 LocalSpinePosoffset
    {
        get
        {
            return _LocalSpinePosoffset;
        }
        set
        {
            if (_LocalSpinePosoffset == new Vector3(-100, -100, -100))
            {
                _LocalSpinePosoffset = value;
            }
        }
    }
    public Vector3 _LocalSpinePosoffset = new Vector3(-100, -100, -100);
    public ControllerType CurrentPlayerController;

    protected List<HitInfoClass> HittedByList = new List<HitInfoClass>();
    protected HitInfoClass LastHitter
    {
        get
        {
            HitInfoClass lastHitter = null;
            foreach (HitInfoClass hitter in HittedByList)
            {
                if (lastHitter == null || lastHitter.TimeLastHit < hitter.TimeLastHit) lastHitter = hitter;
            }
            return lastHitter;
        }
    }

    [HideInInspector] public int CharOredrInLayer = 0;
    protected List<BattleTileScript> currentBattleTilesToCheck = new List<BattleTileScript>();
    public IEnumerator AICo = null;

    [HideInInspector] public Vector2Int nextAttackPos;
    public virtual bool EndAxisMovement
    {
        get
        {
            return _EndAxisMovement;
        }
        set
        {
            //_EndAxisMovement = value;
        }
    }
    private bool _EndAxisMovement = true;
    protected bool pauseOnLastFrame = false;
    public bool lastAttack = false;
    float AnimSpeed = 1;
    #endregion

    #region Defence Variables
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

    protected float DefendingHoldingTimer = 0;

    protected bool canDefend = true;

    protected float defenceAnimSpeedMultiplier = 5f;
    protected bool IsDefStartCo = false;
    #endregion

    #region Attack Variables

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

    [HideInInspector]
    public bool bulletFired = false;
    [HideInInspector] public List<ScriptableObjectAttackBase> availableAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public List<ScriptableObjectAttackBase> currentTileAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public ScriptableObjectAttackBase atkToCheck;


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
    public ScriptableObjectAttackBase _nextAttack = null;

    public float NextAttackDamage
    {
        get
        {
            return CharInfo.DamageStats.BaseDamage * GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).TileADStats.x
                    * (nextAttack.AttackInput == AttackInputType.Weak ? UnityEngine.Random.Range(CharInfo.WeakAttack.DamageMultiplier.x, CharInfo.WeakAttack.DamageMultiplier.y) : 
                    UnityEngine.Random.Range(CharInfo.StrongAttack.DamageMultiplier.x, CharInfo.StrongAttack.DamageMultiplier.y)) * nextAttack.DamageMultiplier;
        }
    }
    public float NextAttackTileDamage
    {
        get
        {
            return (CharInfo.DamageStats.BaseDamage * (nextAttack.AttackInput == AttackInputType.Weak ? UnityEngine.Random.Range(CharInfo.WeakAttack.DamageMultiplier.x, CharInfo.WeakAttack.DamageMultiplier.y) :
                UnityEngine.Random.Range(CharInfo.StrongAttack.DamageMultiplier.x, CharInfo.StrongAttack.DamageMultiplier.y)) * nextAttack.DamageMultiplier) * GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).TileADStats.x;
        }
    }

    public List<ScriptableObjectAttackBase> nextSequencedAttacks = new List<ScriptableObjectAttackBase>();

    public AttackPhasesType currentAttackPhase = AttackPhasesType.End;

    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
            _shotsLeftInAttack = _shotsLeftInAttack < 0 ? 0 : _shotsLeftInAttack;
            if(_shotsLeftInAttack == 0)
            {
                Attacking = false;
            }
        }
    }
    public int _shotsLeftInAttack = 0;

   
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
    public bool _Attacking = false;


    #endregion

    #region Variables that we have to decide if still useful
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    public bool CanAttack = false;

    public DeathProcessStage currentDeathProcessPhase = DeathProcessStage.None;

    #endregion

    #region Buff_Debuff Variables

    public List<BuffDebuffClass> BuffsDebuffsList = new List<BuffDebuffClass>();


    #endregion

    #region SwapChar Variables
    public bool IsSwapping = false;
    public bool SwapWhenPossible = false;
    #endregion

    #region SupportVariables
    protected GameObject chargeParticles = null;
    protected GameObject tempGameObject = null;
    protected float tempFloat_1;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    Spine.Animation tempAnimation;
    List<Spine.Timeline> tempTimeLine;
    Spine.EventTimeline tempEventTimeLine;
    BattleTileScript tempBattleTileScript;
    List<BattleTileScript> prevBattleTile = new List<BattleTileScript>();
    CharacterAnimationStateType animState;
    Vector2Int dir;
    AnimationCurve curve;
    protected BaseCharacter target = null;
    public float AICoolDownOffset = 0;
    ScriptableObjectAI prev;
    List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();
    Transform spineT;

    #endregion


    public virtual void Start()
    {
        Sic = new StatisticInfoClass(CharInfo.CharacterID, UMS.PlayerController);
    }

    protected virtual void Update()
    {

        UMS.HPBar.localScale = new Vector3((1f / 100f) * CharInfo.HealthPerc, 1, 1);

        UMS.StaminaBar.localScale = new Vector3((1f / 100f) * CharInfo.EtherPerc, 1, 1);
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
        if (HasBuffDebuff(BuffDebuffStatsType.Rebirth))
        {
            RebirthEffect();
            return;
        }
        if (CharInfo.Health > 0f) return; //An additional check since for some reason the rebirth isn't applied before a second attack triggers death for some reason
        SetCharDead();
    }

    public virtual void RebirthEffect()
    {
        tempFloat_1 = CharInfo.HealthStats.Base;
        CharInfo.Health += tempFloat_1;
        HealthStatsChangedEvent?.Invoke(tempFloat_1, HealthChangedType.Rebirth, SpineAnim.transform);
        GetBuffDebuff(BuffDebuffStatsType.Rebirth).CurrentBuffDebuff.Stop_Co = true;
    }

    public virtual void SetAttackReady(bool value)
    {
        died = false;

        if (value)
        {
            HittedByList.Clear();
        }
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
        if (died) return;
        died = true;

        EventManager.Instance?.AddCharacterDeath(this);

        foreach (ManagedAudioSource audioSource in GetComponentsInChildren<ManagedAudioSource>())
        {
            audioSource.gameObject.transform.parent = AudioManagerMk2.Instance.transform;
        }

        //PREVIOUS LAST HITTER CODE, CAN BE REUSED
        //if(LastHitter != null && LastHitter.hitter != null && LastHitter.hitter.IsOnField && LastHitter.hitter.HasBuffDebuff(BuffDebuffStatsType.Rebirth))
        //{
        //    BuffDebuffClass rebirth = LastHitter.hitter.GetBuffDebuff(BuffDebuffStatsType.Rebirth);
        //    float HealAmount = rebirth.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Perc ? LastHitter.hitter.CharInfo.HealthStats.Base * rebirth.CurrentBuffDebuff.Value : rebirth.CurrentBuffDebuff.Value;
        //    LastHitter.hitter.CharInfo.Health += HealAmount;
        //    LastHitter.hitter.HealthStatsChangedEvent?.Invoke(HealAmount, HealthChangedType.Heal, LastHitter.hitter.SpineAnim.transform);
        //}
        isMoving = false;
        isDefendingStop = true;
        isDefending = false;
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

    public virtual void OnDestroy()
    {
        CurrentCharIsDeadEvent = null;
        CurrentCharIsRebirthEvent = null;
        CurrentCharStartingActionEvent = null;
        TileMovementCompleteEvent = null;
        HealthStatsChangedEvent = null;

    }

    #endregion
    #region Attack

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
        tempInt_1 = 0;
        foreach (BulletBehaviourInfoClassOnBattleFieldClass item in bulTraj)
        {
            foreach (BattleFieldAttackTileClass target in item.BulletEffectTiles)
            {
                if (GridManagerScript.Instance.isPosOnField(target.Pos))
                {
                    tempInt_1++;
                }
            }
        }

        return tempInt_1;
    }

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
                        case ValueCheckerType.Between:
                            if (CharInfo.HealthPerc <= atkToCheck.TilesAtk.InBetween.x && CharInfo.HealthPerc >= atkToCheck.TilesAtk.InBetween.y)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                    }
                    break;
                case StatsCheckType.Ether:
                    switch (atkToCheck.TilesAtk.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (CharInfo.EtherPerc < atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (CharInfo.EtherPerc == atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (CharInfo.EtherPerc > atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.Between:
                            if (CharInfo.EtherPerc <= atkToCheck.TilesAtk.InBetween.x && CharInfo.EtherPerc >= atkToCheck.TilesAtk.InBetween.y)
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

        tempInt_1 = 0;
        availableAtks.ForEach(r =>
        {
            tempInt_1 += r.TilesAtk.Chances;
        });
        tempInt_1 = UnityEngine.Random.Range(0, tempInt_1);
        tempInt_2 = 0;
        for (int i = 0; i < availableAtks.Count; i++)
        {
            tempInt_2 += availableAtks[i].TilesAtk.Chances;

            if (tempInt_1 < tempInt_2)
            {
                nextAttack = availableAtks[i];
                return;
            }
        }
    }

    public virtual void BackfireEffect(float damage)
    {
        //BACKFIRE APPLY DAMAGE BASED ON HOW MUCH DAMAGE WAS DEALT
        SetDamage(this, GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ?
                GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.Value * damage : GetBuffDebuff(BuffDebuffStatsType.Backfire).CurrentBuffDebuff.Value, ElementalType.Dark, false);

        ParticleManagerScript.Instance.FireParticlesInPosition(
            UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Hit : nextAttack.Particles.Right.Hit,
            CharInfo.CharacterID,
            AttackParticlePhaseTypes.Hit, transform.position, UMS.Side, nextAttack.AttackInput
            );

        shotsLeftInAttack = 0;
        Attacking = false;
        currentAttackPhase = AttackPhasesType.End;
        SetAnimation("Idle", true);
    }

    public void FireCastParticles()
    {
        CastAttackParticles();
    }

    //start the casting particlaes foe the attack
    public virtual void CastAttackParticles()
    {
        if (nextAttack != null)
        {
            tempGameObject = ParticleManagerScript.Instance.FireParticlesInPosition(UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Cast : nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
           SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);
            tempGameObject.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharInfo.BaseSpeed);

            if (nextAttack.CurrentAttackType == AttackType.Particles)
            {
                CharInfo.Ether -= nextAttack.StaminaCost;
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
        if (nextAttack != null)
        {
            if (nextAttack.CurrentAttackType == AttackType.Particles)
            {
                foreach (BulletBehaviourInfoClass item in nextAttack.ParticlesAtk.BulletTrajectories)
                {
                    CreateBullet(item);
                }
            }
            else if (nextAttack.CurrentAttackType == AttackType.Tile)
            {
                CreateTileAttack();
            }
            else if (nextAttack.CurrentAttackType == AttackType.Totem)
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

    public virtual void CreateTileAttack()
    {
        if (HasBuffDebuff(BuffDebuffStatsType.Backfire) && NextAttackTileDamage > 0f)
        {
            BackfireEffect(NextAttackTileDamage);
            return;
        }

        if (nextAttack != null && nextAttack.CurrentAttackType == AttackType.Tile && CharInfo.Health > 0 && IsOnField)
        {
            CharInfo.WeakAttack.DamageMultiplier = CharInfo.WeakAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;
            CharInfo.StrongAttack.DamageMultiplier = CharInfo.StrongAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;

            if (nextAttack.AttackInput > AttackInputType.Strong && CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
            {
                StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == CharInfo.CharacterID).First().DamageExp += nextAttack.ExperiencePoints;
            }

            for (int i = 0; i < nextAttack.TilesAtk.BulletTrajectories.Count; i++)
            {
                foreach (BattleFieldAttackTileClass target in nextAttack.TilesAtk.BulletTrajectories[i].BulletEffectTiles)
                {
                    tempInt_1 = UnityEngine.Random.Range(0, 100);
                    if (tempInt_1 <= nextAttack.TilesAtk.BulletTrajectories[i].ExplosionChances)
                    {
                        tempVector2Int = nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget ? target.Pos + nextAttackPos :
                        nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf ? target.Pos + UMS.CurrentTilePos : target.Pos;
                        if (GridManagerScript.Instance.isPosOnField(tempVector2Int))
                        {
                            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(tempVector2Int);
                            if (bts._BattleTileState != BattleTileStateType.NonUsable)
                            {
                                if (nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf && bts.WalkingSide == UMS.WalkingSide)
                                {
                                    shotsLeftInAttack++;

                                    bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharInfo.Elemental, this,
                                    target, target.EffectChances);
                                }
                                else if (nextAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf && bts.WalkingSide != UMS.WalkingSide)
                                {
                                    //new way

                                    tempString = GetAttackAnimName();
                                    tempAnimation = SpineAnim.skeleton.Data.FindAnimation(tempString);

                                    tempTimeLine = tempAnimation?.Timelines?.Items?.Where(r => r is Spine.EventTimeline).ToList();
                                    tempEventTimeLine = tempTimeLine.Where(r => ((Spine.EventTimeline)r).Events.Where(p => p.Data.Name == "FireBulletParticle").ToList().Count > 0).First() as Spine.EventTimeline;

                                    tempFloat_1 = tempEventTimeLine.Events.Where(r => r.Data.Name == "FireBulletParticle").First().Time;

                                    shotsLeftInAttack++;
                                    AttackedTiles(bts);
                                    if (nextAttack.AttackInput > AttackInputType.Weak && i == 0)
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharInfo.Elemental, this,
                                    target, target.EffectChances, (nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y))) + tempFloat_1);//(nextAttack.TilesAtk.BulletTrajectories[i].Delay * 0.1f)
                                    }
                                    else if (nextAttack.AttackInput == AttackInputType.Weak)
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharInfo.Elemental, this,
                                    target, target.EffectChances, (nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y))) + tempFloat_1); // 
                                    }
                                    else
                                    {
                                        bts.BattleTargetScript.SetAttack(nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharInfo.Elemental, this,
                                    target, target.EffectChances, false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (shotsLeftInAttack == 0)
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
            CharInfo.WeakAttack.DamageMultiplier = CharInfo.WeakAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;
            CharInfo.StrongAttack.DamageMultiplier = CharInfo.StrongAttack.B_DamageMultiplier * nextAttack.DamageMultiplier;
            StartCoroutine(Totem());
        }
    }
    //TODO to rework
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

    public void StartDefending()
    {
        if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CharActionlist.Contains(CharacterActionType.Defence) || !canDefend)
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
        BattleManagerScript.Instance.WaitFor((SpineAnim.GetAnimLenght(CharacterAnimationStateType.Defending) / defenceAnimSpeedMultiplier) * 0.25f);
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
            if (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defending.ToString()))
            {
                Debug.Log("FINISHED STOP <color=blue>DEFENDING</color>");
                SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }

    }

    #endregion
    #region Move

    public virtual IEnumerator MoveCharOnDir_Co(InputDirectionType nextDir)
    {
        yield return MoveSystem(nextDir);
    }

    private IEnumerator MoveSystem(InputDirectionType nextDir)
    {
        if ((CharInfo.Health > 0 && !isMoving && IsOnField && EndAxisMovement && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString() && CharActionlist.Contains(CharacterActionType.Move)) || BattleManagerScript.Instance.VFXScene)
        {
            prevBattleTile = CurrentBattleTiles;
            GetDirectionVectorAndAnimationCurve(nextDir);
            currentBattleTilesToCheck = CheckTileAvailabilityUsingDir(dir);

            if (currentBattleTilesToCheck.Count > 0 &&
                currentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                currentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count && GridManagerScript.Instance.isPosOnField(UMS.CurrentTilePos + dir))
            {
                if (animState.ToString() == SpineAnim.CurrentAnim)
                { 
                    tempFloat_1 = ((SpineAnim.GetAnimLenght(animState) * CharInfo.SpeedStats.LoopPerc) / CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed;
                    SpineAnim.SetAnimationSpeed(tempFloat_1);
                    SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = SpineAnim.GetAnimLenght(animState) * CharInfo.SpeedStats.IntroPerc;
                }
                else
                {
                    SetAnimation(animState.ToString());
                    SpineAnim.SetAnimationSpeed((SpineAnim.GetAnimLenght(animState) * (CharInfo.SpeedStats.IntroPerc + CharInfo.SpeedStats.LoopPerc) / CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed);
                }
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
                UMS.Pos.Clear();
                foreach (BattleTileScript item in currentBattleTilesToCheck)
                {
                    //Debug.LogError(item.Pos + "               " + BattleTileStateType.Occupied);
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    UMS.Pos.Add(item.Pos);
                }

                tempBattleTileScript = CurrentBattleTiles.Where(r => r.Pos == UMS.CurrentTilePos).First();

                if (tempBattleTileScript != null)
                {
                    foreach (BattleTileScript item in prevBattleTile)
                    {
                        BattleManagerScript.Instance.OccupiedBattleTiles.Remove(item);
                    }
                    BattleManagerScript.Instance.OccupiedBattleTiles.AddRange(CurrentBattleTiles);
                    FireActionEvent(CharacterActionType.Move);
                    switch (nextDir)
                    {
                        case InputDirectionType.Up:
                            FireActionEvent(CharacterActionType.MoveUp);
                            break;
                        case InputDirectionType.Down:
                            FireActionEvent(CharacterActionType.MoveDown);
                            break;
                        case InputDirectionType.Left:
                            FireActionEvent(CharacterActionType.MoveLeft);
                            break;
                        case InputDirectionType.Right:
                            FireActionEvent(CharacterActionType.MoveRight);
                            break;
                    }
                    yield return MoveByTileSpace(tempBattleTileScript.transform.position, curve, CharInfo.SpeedStats.CuttingPerc);
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

    public virtual void MoveCharOnDirection(InputDirectionType nextDir)
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

    public void GetDirectionVectorAndAnimationCurve(InputDirectionType nextDir)
    {
        animState = CharacterAnimationStateType.Idle;
        curve = new AnimationCurve();
        dir = Vector2Int.zero;
        switch (nextDir)
        {
            case InputDirectionType.Up:
                dir = new Vector2Int(-1, 0);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.Down:
                dir = new Vector2Int(1, 0);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.DownMovement : SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirectionType.Right:
                dir = new Vector2Int(0, 1);
                animState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                if (animState == CharacterAnimationStateType.DashLeft)
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.BackwardMovement : SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.ForwardMovement : SpineAnim.Speed_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirectionType.Left:
                dir = new Vector2Int(0, -1);
                animState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                if (animState == CharacterAnimationStateType.DashLeft)
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.BackwardMovement : SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.ForwardMovement : SpineAnim.Speed_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirectionType.UpLeft:
                dir = new Vector2Int(-1, -1);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.UpRight:
                dir = new Vector2Int(-1, 1);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.DownLeft:
                dir = new Vector2Int(1, -1);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.DownMovement : SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirectionType.DownRight:
                dir = new Vector2Int(1, 1);
                curve = SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.DownMovement : SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
        }
    }

    protected void SetCurrentAIValues()
    {
        AttackWillPerc = CurrentAIState.AttackWill;
        MoveWillPerc = CurrentAIState.MoveWill;
        TowardMovementPerc = CurrentAIState.Chaseing_Flee;
    }

    public virtual IEnumerator AI()
    {
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
                prev = CurrentAIState;
                CurrentAIState = CharInfo.GetCurrentAI(AggroInfoList, UMS.CurrentTilePos, this, ref target);
                if (prev == null || prev.AI_Type != CurrentAIState.AI_Type)
                {
                    SetCurrentAIValues();
                    if (prev != null)
                    {
                        prev.ResetStats(CharInfo);

                    }
                    CurrentAIState.ModifyStats(CharInfo);
                    if (CurrentAIState.AIPs != null && CurrentAIState.AIPs.PSType != ParticlesType.None)
                    {
                        if (psAI != null)
                        {
                            psAI.SetActive(false);
                        }
                        psAI = ParticleManagerScript.Instance.GetParticle(CurrentAIState.AIPs.PSType);
                        psAI.transform.parent = SpineAnim.transform;
                        psAI.transform.localPosition = Vector3.zero;
                        psAI.SetActive(true);
                    }
                   
                    AICoolDownOffset = 0;
                }

                tempInt_1 = UnityEngine.Random.Range(0, 100);
                nextAttack = null;
                if (target != null)
                {
                    GetAttack();
                }

                if (target != null && tempInt_1 < AttackWillPerc && nextAttack != null && (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
                {
                    lastAttackTime = Time.time;
                    nextAttackPos = target.UMS.CurrentTilePos;
                    if (possiblePos != null)
                    {
                        possiblePos.isTaken = false;
                        possiblePos = null;
                    }

                    yield return AttackSequence();
                }
                else
                {
                    tempInt_2 = UnityEngine.Random.Range(0, 100);
                    if (AreTileNearEmpty() && tempInt_2 < MoveWillPerc)
                    {
                        if (possiblePos == null)
                        {
                            tempInt_1 = UnityEngine.Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
                            if (TowardMovementPerc > tempInt_1 && (Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
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
                                                    tempVector2Int = path[0] - UMS.CurrentTilePos;
                                                    possiblePos.isTaken = true;
                                                    yield return MoveCharOnDir_Co(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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
                                            if (CurrentAIState.IdleMovement > UnityEngine.Random.Range(0f, 1f))
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
                                    tempVector2Int = path[0] - UMS.CurrentTilePos;

                                    yield return MoveCharOnDir_Co(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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

    protected List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsingDir(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(UMS.Pos, tempList_Vector2int, UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    protected List<BattleTileScript> CheckTileAvailabilityUsingPos(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsinPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(UMS.Pos, tempList_Vector2int, UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsingDir(Vector2Int direction)
    {
        tempList_Vector2int.Clear();
        UMS.Pos.ForEach(r => tempList_Vector2int.Add(r + direction));
        return tempList_Vector2int;
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsinPos(Vector2Int direction)
    {
        tempList_Vector2int.Clear();
        UMS.Pos.ForEach(r => tempList_Vector2int.Add((r - UMS.CurrentTilePos) + direction));
        return tempList_Vector2int;
    }

    public virtual IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        //Debug.Log("StartMoveCo  " + Time.time);
        float timer = 0;
        float spaceTimer = 0;
        bool isMovCheck = false;
        EndAxisMovement = false;
        Vector3 offset = spineT.position;
        transform.position = nextPos;
        spineT.position = offset;
        Vector3 localoffset = spineT.localPosition;

        while (timer < 1)
        {
            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            timer += (BattleManagerScript.Instance.FixedDeltaTime / (CharInfo.SpeedStats.TileMovementTime / (CharInfo.SpeedStats.MovementSpeed * CharInfo.SpeedStats.BaseSpeed * BattleManagerScript.Instance.MovementMultiplier)));
            spaceTimer = curve.Evaluate(timer);
            spineT.localPosition = Vector3.Lerp(localoffset, LocalSpinePosoffset, spaceTimer);

            if (timer > animPerc && !isMovCheck)
            {
                isMovCheck = true;
                isMoving = false;
                if (isDefending)
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
                spineT.localPosition = LocalSpinePosoffset;
                yield break;
            }
        }
        spineT.localPosition = LocalSpinePosoffset;
        //Debug.Log("EndMoveCo");
    }

    #endregion
    #region Buff/Debuff

    public bool HasBuffDebuff(BuffDebuffStatsType type)
    {
        return BuffsDebuffsList.Where(r => r.CurrentBuffDebuff.Effect.StatsToAffect == type).ToArray().Length > 0;
    }
    public BuffDebuffClass GetBuffDebuff(BuffDebuffStatsType type)
    {
        return BuffsDebuffsList.Where(r => r.CurrentBuffDebuff.Effect.StatsToAffect == type).FirstOrDefault();
    }


    BuffDebuffClass buffDebuff;
    public void Buff_DebuffCo(Buff_DebuffClass bdClass)
    {
        if (!SpineAnim.CurrentAnim.Contains("Reverse") && CharInfo.HealthPerc > 0)
        {
            List<BuffDebuffClass> items = BuffsDebuffsList.Where(r => r.Stat == bdClass.Effect.StatsToAffect).ToList();
            if (items.Count == 0) //Create the new buffDebuff
            {
                //Debug.Log(bdClass.Name + "   " + newBuffDebuff.Last());
                buffDebuff = new BuffDebuffClass(bdClass.Effect.StatsToAffect, bdClass.Effect.level, bdClass, bdClass.Effect.Duration, bdClass.EffectMaker);
                buffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(buffDebuff);
                BuffsDebuffsList.Insert(0, buffDebuff);
                UMS.buffIconHandler.RefreshIcons(BuffsDebuffsList);
                buffDebuff.currentBuffValue = bdClass.Value;
                StartCoroutine(buffDebuff.BuffDebuffCo);
            }
            else //Refresh current BuffDebuff duration
            {
                if (items[0].CurrentBuffDebuff.Effect.level == bdClass.Effect.level)
                {
                    if (bdClass.Effect.StackType == BuffDebuffStackType.Stackable && items[0].CurrentStack < bdClass.Effect.maxStack)
                    {
                        buffDebuff = new BuffDebuffClass(bdClass.Effect.StatsToAffect, bdClass.Effect.level, bdClass, bdClass.Effect.Duration, bdClass.EffectMaker);
                        buffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(buffDebuff);
                        BuffsDebuffsList.Insert(0, buffDebuff);
                        UMS.buffIconHandler.RefreshIcons(BuffsDebuffsList);
                        buffDebuff.currentBuffValue = bdClass.Value;
                        buffDebuff.CurrentStack = items[0].CurrentStack + 1;
                        StartCoroutine(buffDebuff.BuffDebuffCo);
                        foreach (var item in items)
                        {
                            item.CurrentStack++;
                        }
                    }
                    else if (bdClass.Effect.StackType == BuffDebuffStackType.Refreshable)
                    {
                        items[0].Duration = bdClass.Effect.Duration;
                        items[0].CurrentBuffDebuff.Timer = 0;
                    }
                }
                else if (items[0].CurrentBuffDebuff.Effect.level > bdClass.Effect.level)
                {
                    
                }
                else if (items[0].CurrentBuffDebuff.Effect.level < bdClass.Effect.level)
                {
                    
                    buffDebuff = new BuffDebuffClass(bdClass.Effect.StatsToAffect, bdClass.Effect.level, bdClass, bdClass.Effect.Duration, bdClass.EffectMaker);
                    buffDebuff.BuffDebuffCo = Buff_DebuffCoroutine(buffDebuff);
                    BuffsDebuffsList.Insert(0, buffDebuff);
                    UMS.buffIconHandler.RefreshIcons(BuffsDebuffsList);
                    buffDebuff.currentBuffValue = bdClass.Value;
                    StartCoroutine(buffDebuff.BuffDebuffCo);
                    foreach (var item in items)
                    {
                        item.CurrentStack++;
                    }

                    if (bdClass.Effect.StackType == BuffDebuffStackType.Refreshable)
                    {
                        items[0].CurrentBuffDebuff.Stop_Co = true;
                    }
                }
            }
        }
    }


    public float StatsMultipler(float b_Value, float multiplier)
    {
        return (b_Value * multiplier) - b_Value;
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
            case BuffDebuffStatsType.Damage:
                CharInfo.DamageStats.BaseDamage += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.DamageStats.B_BaseDamage, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.Health:
                val = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.Health += val;
                HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Heal, bdClass.EffectMaker.SpineAnim.transform);
                EventManager.Instance?.UpdateHealth(this);
                HealthStatsChangedEvent?.Invoke(bdClass.currentBuffValue, bdClass.currentBuffValue > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, SpineAnim.transform);
                break;
            case BuffDebuffStatsType.HealthRegeneration:
                CharInfo.HealthStats.Regeneration += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Regeneration, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.Armour:
                CharInfo.HealthStats.Armour += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Armour, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.BaseSpeed:
                if (bdClass.currentBuffValue > 0)
                {
                    CharInfo.SpeedStats.BaseSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_BaseSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
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
                if (bdClass.currentBuffValue == 0)
                {
                    if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
                    {

                        yield return BattleManagerScript.Instance.WaitFor(0.5f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

                        CharActionlist.Remove(CharacterActionType.SwitchCharacter);

                        ControllerType c = ControllerType.None;
                        if (CurrentPlayerController == ControllerType.None && BattleManagerScript.Instance.CurrentSelectedCharacters.Values.Where(r => r.Character == this).ToList().Count > 0)
                        {
                            c = BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == this).First().Key;
                            BattleManagerScript.Instance.CurrentSelectedCharacters.Values.Where(r => r.Character == this).First().Character = null;
                        }
                        else if (CurrentPlayerController == ControllerType.None)
                        {
                            BattleManagerScript.Instance.DeselectCharacter((CharacterType_Script)this, c);
                            break;
                        }
                        else
                        {
                            c = CurrentPlayerController;
                            BattleManagerScript.Instance.CurrentSelectedCharacters[c].Character = null;
                        }
                        BattleManagerScript.Instance.DeselectCharacter((CharacterType_Script)this, c);
                        UMS.IndicatorAnim.SetBool("indicatorOn", false);
                        if (BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, c) != null && c <= ControllerType.Player4)
                        {
                            CharacterType_Script cb = (CharacterType_Script)BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, c);
                            BattleManagerScript.Instance.SetCharOnBoardOnFixedPos(c, cb.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeBattleTile(UMS.WalkingSide).Pos);
                            cb.SetCharSelected(true, c);
                            BattleManagerScript.Instance.SelectCharacter(c, cb);
                        }
                    }
                }
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharInfo.SpeedStats.MovementSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_MovementSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.Zombie:
                if (CharInfo.Health > 0)
                {
                    BattleManagerScript.Instance.Zombification(this, bdClass.Duration, bdClass.CurrentBuffDebuff.Effect.AIs);
                }
                break;
            case BuffDebuffStatsType.Legion:
                BattleManagerScript.Instance.CloneUnit(
                this, bdClass.CurrentBuffDebuff.Effect.CloneAsManyAsCurrentEnemies ?
                CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script ? WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList().Count :
                BattleManagerScript.Instance.AllCharactersOnField.Where(r => !r.IsOnField && r.CharInfo.HealthPerc > 0).ToList().Count : bdClass.CurrentBuffDebuff.Effect.CloneAmount,
                bdClass.CurrentBuffDebuff.Effect.ClonePowerScale, bdClass.CurrentBuffDebuff.Effect.ClonePrefab, bdClass.CurrentBuffDebuff.Effect.CloneStartingEffect
                );
                break;
            case BuffDebuffStatsType.Shield:
                CharInfo.ShieldStats.Shield += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_Shield, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.ShieldRegeneration:
                CharInfo.ShieldStats.BaseShieldRegeneration += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_BaseShieldRegeneration, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.ShieldAbsorbtion:
                CharInfo.ShieldStats.ShieldAbsorbtion += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_ShieldAbsorbtion, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.ShieldInvulnerabilityTime:
                CharInfo.ShieldStats.Invulnerability += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_Invulnerability, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.MinionShieldChances:
                CharInfo.ShieldStats.MinionShieldChances += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_MinionShieldChances, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.MinionPerfectShieldChances:
                CharInfo.ShieldStats.MinionPerfectShieldChances += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_MinionPerfectShieldChances, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.WeakBulletSpeed:
                CharInfo.SpeedStats.WeakBulletSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_WeakBulletSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.WeakAttackChances:
                CharInfo.WeakAttack.Chances.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_Chances.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.WeakAttack.Chances.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_Chances.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.WeakAttackCriticalChance:
                CharInfo.WeakAttack.CriticalChance.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_CriticalChance.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.WeakAttack.CriticalChance.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_CriticalChance.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.WeakAttackDamageMultiplier:
                CharInfo.WeakAttack.DamageMultiplier.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_DamageMultiplier.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.WeakAttack.DamageMultiplier.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_DamageMultiplier.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.StrongBulletSpeed:
                CharInfo.SpeedStats.StrongBulletSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_StrongBulletSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.StrongAttackChances:
                CharInfo.StrongAttack.Chances.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_Chances.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.StrongAttack.Chances.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_Chances.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.StrongAttackCriticalChance:
                CharInfo.StrongAttack.CriticalChance.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_CriticalChance.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.StrongAttack.CriticalChance.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_CriticalChance.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.StrongAttackDamageMultiplier:
                CharInfo.StrongAttack.DamageMultiplier.x += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_DamageMultiplier.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.StrongAttack.DamageMultiplier.y += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_DamageMultiplier.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.AttackChange:
                CharInfo.CurrentAttackTypeInfo.Add(bdClass.CurrentBuffDebuff.Effect.Atk);
                break;
            case BuffDebuffStatsType.Ether:
                CharInfo.EtherStats.Ether += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.EtherStats.B_Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                break;
            case BuffDebuffStatsType.Rage:
                CharInfo.SpeedStats.MovementSpeed += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_MovementSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.DamageStats.BaseDamage += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.DamageStats.B_BaseDamage, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.HealthStats.Armour -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Armour, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                CharInfo.AIs.Add(bdClass.CurrentBuffDebuff.Effect.RageAI);
                if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
                {

                    ControllerType c = ControllerType.None;
                    CharActionlist.Remove(CharacterActionType.SwitchCharacter);
                    if (CurrentPlayerController == ControllerType.None && BattleManagerScript.Instance.CurrentSelectedCharacters.Values.Where(r => r.Character == this).ToList().Count > 0)
                    {
                        c = BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == this).First().Key;
                        BattleManagerScript.Instance.CurrentSelectedCharacters.Values.Where(r => r.Character == this).First().Character = null;
                    }
                    else if (CurrentPlayerController == ControllerType.None)
                    {
                        BattleManagerScript.Instance.DeselectCharacter((CharacterType_Script)this, c);
                        break;
                    }
                    else
                    {
                        c = CurrentPlayerController;
                        BattleManagerScript.Instance.CurrentSelectedCharacters[c].Character = null;
                    }
                    BattleManagerScript.Instance.DeselectCharacter((CharacterType_Script)this, c);
                    UMS.IndicatorAnim.SetBool("indicatorOn", false);
                    if (BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, c) != null && c <= ControllerType.Player4)
                    {
                        CharacterType_Script cb = (CharacterType_Script)BattleManagerScript.Instance.GetFreeRandomChar(UMS.Side, c);
                        BattleManagerScript.Instance.SetCharOnBoardOnFixedPos(c, cb.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeBattleTile(UMS.WalkingSide).Pos);
                        cb.SetCharSelected(true, c);
                        BattleManagerScript.Instance.SelectCharacter(c, cb);
                    }
                    StartAI();
                }
                break;
            
        }

        if (bdClass.Duration > 0)
        {
            int iterator = 0;
            while (bdClass.CurrentBuffDebuff.Timer <= bdClass.Duration && !bdClass.CurrentBuffDebuff.Stop_Co && CharInfo.HealthPerc > 0)
            {
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

                bdClass.CurrentBuffDebuff.Timer += BattleManagerScript.Instance.DeltaTime;

                if (((int)bdClass.CurrentBuffDebuff.Timer) > iterator)
                {
                    iterator++;
                    if (bdClass.Stat == BuffDebuffStatsType.Regen)
                    {
                        CharInfo.Health += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                        HealthStatsChangedEvent?.Invoke(bdClass.currentBuffValue, HealthChangedType.Heal, SpineAnim.transform);
                        EventManager.Instance?.UpdateHealth(this);
                        //Apply Bleed
                        if (bdClass.currentBuffValue < 0)
                        {
                            ParticleManagerScript.Instance.FireParticlesInPosition(ParticleManagerScript.Instance.GetParticlePrefabByName(ParticlesType.Status_Debuff_Bleed), CharacterNameType.None, AttackParticlePhaseTypes.Cast, SpineAnim.transform.position, SideType.LeftSide, AttackInputType.Weak).transform.SetParent(SpineAnim.transform);
                        }
                    }
                    else if (bdClass.Stat == BuffDebuffStatsType.Bleed)
                    {
                        CharInfo.Health -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                        HealthStatsChangedEvent?.Invoke(bdClass.currentBuffValue, HealthChangedType.Damage, SpineAnim.transform);
                        EventManager.Instance?.UpdateHealth(this);
                        //Apply Bleed
                        ParticleManagerScript.Instance.FireParticlesInPosition(ParticleManagerScript.Instance.GetParticlePrefabByName(ParticlesType.Status_Debuff_Bleed), CharacterNameType.None, AttackParticlePhaseTypes.Cast, SpineAnim.transform.position, SideType.LeftSide, AttackInputType.Weak).transform.SetParent(SpineAnim.transform);
                        
                    }
                    else if (bdClass.Stat == BuffDebuffStatsType.Drain)
                    {
                        val = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                        HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Heal, bdClass.EffectMaker.SpineAnim.transform);
                        HealthStatsChangedEvent?.Invoke(val, HealthChangedType.Damage, SpineAnim.transform);
                        bdClass.EffectMaker.CharInfo.Health += val;
                        CharInfo.Health -= val;
                        EventManager.Instance?.UpdateHealth(bdClass.EffectMaker);
                        EventManager.Instance?.UpdateHealth(this);
                    }
                    else if (bdClass.Stat == BuffDebuffStatsType.Bliss)
                    {
                        CharInfo.Ether += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.EtherStats.Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                        //Apply Bleed
                        if (bdClass.currentBuffValue < 0)
                        {
                            ParticleManagerScript.Instance.FireParticlesInPosition(ParticleManagerScript.Instance.GetParticlePrefabByName(ParticlesType.Status_Debuff_Bleed), CharacterNameType.None, AttackParticlePhaseTypes.Cast, SpineAnim.transform.position, SideType.LeftSide, AttackInputType.Weak).transform.SetParent(SpineAnim.transform);
                        }
                    }
                    else if (bdClass.Stat == BuffDebuffStatsType.SoulCrash)
                    {
                        CharInfo.Ether -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.EtherStats.Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                        //Apply Bleed
                        if (bdClass.currentBuffValue < 0)
                        {
                            ParticleManagerScript.Instance.FireParticlesInPosition(ParticleManagerScript.Instance.GetParticlePrefabByName(ParticlesType.Status_Debuff_Bleed), CharacterNameType.None, AttackParticlePhaseTypes.Cast, SpineAnim.transform.position, SideType.LeftSide, AttackInputType.Weak).transform.SetParent(SpineAnim.transform);
                        }
                    }
                }
            }

            ps?.SetActive(false);
            switch (bdClass.Stat)
            {
                case BuffDebuffStatsType.Damage:
                    CharInfo.DamageStats.BaseDamage = bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.DamageStats.B_BaseDamage, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.BaseSpeed:
                    if (bdClass.currentBuffValue > 0)
                    {
                        CharInfo.SpeedStats.BaseSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_BaseSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    }
                    else
                    {
                        CharInfo.SpeedStats.BaseSpeed = CharInfo.SpeedStats.B_BaseSpeed;
                    }
                    SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.BaseSpeed);
                    if (bdClass.currentBuffValue == 0 && CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && !bdClass.CurrentBuffDebuff.Stop_Co)
                    {
                        CharActionlist.Add(CharacterActionType.SwitchCharacter);
                        if (BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == null).ToList().Count > 0)
                        {
                            CurrentPlayerController = BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == null).OrderBy(a => a.Value.NotPlayingTimer).First().Key;
                            ((CharacterType_Script)this).SetCharSelected(true, CurrentPlayerController);
                            BattleManagerScript.Instance.SelectCharacter(CurrentPlayerController, (CharacterType_Script)this);
                        }
                        else
                        {
                            yield return BattleManagerScript.Instance.RemoveCharacterFromBaord(this, true);
                        }
                    }
                    break;
                case BuffDebuffStatsType.HealthRegeneration:
                    CharInfo.HealthStats.Regeneration -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Regeneration, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.Armour:
                    CharInfo.HealthStats.Armour -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Armour, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.MovementSpeed:
                    CharInfo.SpeedStats.MovementSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_MovementSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.Shield:
                    CharInfo.ShieldStats.Shield -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_Shield, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.ShieldRegeneration:
                    CharInfo.ShieldStats.BaseShieldRegeneration -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_BaseShieldRegeneration, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.ShieldAbsorbtion:
                    CharInfo.ShieldStats.ShieldAbsorbtion -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_ShieldAbsorbtion, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.ShieldInvulnerabilityTime:
                    CharInfo.ShieldStats.Invulnerability -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_Invulnerability, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.MinionShieldChances:
                    CharInfo.ShieldStats.MinionShieldChances -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_MinionShieldChances, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.MinionPerfectShieldChances:
                    CharInfo.ShieldStats.MinionPerfectShieldChances -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.ShieldStats.B_MinionPerfectShieldChances, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.WeakBulletSpeed:
                    CharInfo.SpeedStats.WeakBulletSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_WeakBulletSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.WeakAttackChances:
                    CharInfo.WeakAttack.Chances.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_Chances.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.WeakAttack.Chances.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_Chances.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.WeakAttackCriticalChance:
                    CharInfo.WeakAttack.CriticalChance.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_CriticalChance.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.WeakAttack.CriticalChance.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_CriticalChance.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.WeakAttackDamageMultiplier:
                    CharInfo.WeakAttack.DamageMultiplier.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_DamageMultiplier.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.WeakAttack.DamageMultiplier.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.WeakAttack.B_DamageMultiplier.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.StrongBulletSpeed:
                    CharInfo.SpeedStats.StrongBulletSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_StrongBulletSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.StrongAttackChances:
                    CharInfo.StrongAttack.Chances.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_Chances.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.StrongAttack.Chances.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_Chances.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.StrongAttackCriticalChance:
                    CharInfo.StrongAttack.CriticalChance.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_CriticalChance.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.StrongAttack.CriticalChance.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_CriticalChance.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.StrongAttackDamageMultiplier:
                    CharInfo.StrongAttack.DamageMultiplier.x -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_DamageMultiplier.x, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.StrongAttack.DamageMultiplier.y -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.StrongAttack.B_DamageMultiplier.y, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.AttackChange:
                    CharInfo.CurrentAttackTypeInfo.Remove(bdClass.CurrentBuffDebuff.Effect.Atk);
                    break;
                case BuffDebuffStatsType.Ether:
                    CharInfo.EtherStats.Ether -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.EtherStats.B_Base, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    break;
                case BuffDebuffStatsType.Rage:
                    CharInfo.SpeedStats.MovementSpeed -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.SpeedStats.B_MovementSpeed, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.DamageStats.BaseDamage -= bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.DamageStats.B_BaseDamage, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.HealthStats.Armour += bdClass.CurrentBuffDebuff.Effect.StatsChecker == StatsCheckerType.Multiplier ? StatsMultipler(CharInfo.HealthStats.B_Armour, bdClass.currentBuffValue) : bdClass.currentBuffValue;
                    CharInfo.AIs.Remove(bdClass.CurrentBuffDebuff.Effect.RageAI);
                    if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && !bdClass.CurrentBuffDebuff.Stop_Co)
                    {
                        StopCoroutine(AICo);
                        CharActionlist.Add(CharacterActionType.SwitchCharacter);
                        if (BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == null).ToList().Count > 0)
                        {
                            CurrentPlayerController = BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character == null).OrderBy(a => a.Value.NotPlayingTimer).First().Key;
                            ((CharacterType_Script)this).SetCharSelected(true, CurrentPlayerController);
                            BattleManagerScript.Instance.SelectCharacter(CurrentPlayerController, (CharacterType_Script)this);
                        }
                        else
                        {
                            yield return BattleManagerScript.Instance.RemoveCharacterFromBaord(this, true);
                        }
                    }
                    break;
            }
        }

        if (bdClass.CurrentBuffDebuff.Effect.StackType == BuffDebuffStackType.Stackable || bdClass.CurrentStack > 0)
        {
            foreach (BuffDebuffClass item in BuffsDebuffsList.Where(r=> r.CurrentBuffDebuff.Effect.StatsToAffect == bdClass.CurrentBuffDebuff.Effect.StatsToAffect).ToList())
            {
                item.CurrentStack--;
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
        while (currentAnimPlay < loops)
        {
            SetAnimation(animState, loop, _pauseOnLastFrame: (currentAnimPlay + 1 == loops && _pauseOnEndFrame));
            SpineAnim.SetAnimationSpeed(animSpeed);
            while (currentAnimPlay == puppetAnimCompleteTick)
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
        yield return BattleManagerScript.Instance.WaitUpdate(() =>
        {
            SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed * perc);
        }, condition);

        SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
    }

  

    public virtual void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {

        if (CharInfo.SpeedStats.BaseSpeed <= 0)
        {
            return;
        }
        Debug.Log(animState + SpineAnim.CurrentAnim + CharInfo.CharacterID.ToString());
        if (animState == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
        }

        if (string.Equals(animState, CharacterAnimationStateType.GettingHit.ToString()) ||
            string.Equals(animState, CharacterAnimationStateType.Buff.ToString()) ||
            string.Equals(animState, CharacterAnimationStateType.Debuff.ToString()) && (currentAttackPhase != AttackPhasesType.End || Attacking))
        {
            return;
        }

        if (isMoving && (animState.ToString() != CharacterAnimationStateType.Reverse_Arriving.ToString() && animState.ToString() != CharacterAnimationStateType.Defeat_ReverseArrive.ToString()) && (!animState.ToString().Contains("Dash")))
        {
            return;
        }


        if (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Arriving.ToString()) || SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Reverse_Arriving.ToString()))
        {
            return;
        }


        if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && animState.Contains("IdleToAtk"))
        {
            AnimSpeed = SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.IdleToAttackDuration;
        }
        else if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && animState.Contains("Loop"))
        {
            AnimSpeed = SpineAnim.GetAnimLenght(animState) / CharInfo.SpeedStats.AttackLoopDuration;//lastAttack ? SpineAnim.GetAnimLenght(animState) / (CharInfo.SpeedStats.AttackLoopDuration / 5) : 
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
        spineT = SpineAnim.transform;
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

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() || completedAnim == CharacterAnimationStateType.JumpTransition_IN.ToString() || completedAnim.Contains("Growing"))
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

        if (attacker == this && HasBuffDebuff(BuffDebuffStatsType.Backfire) && damage > 0f)
        {
            healthCT = HealthChangedType.Backfire;
            res = true;
        }
        else if (HasBuffDebuff(BuffDebuffStatsType.Invulnerable))
        {
            damage = 0;
            healthCT = HealthChangedType.Invulnerable;
            GameObject go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
            go.transform.position = transform.position;
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Partial, AudioBus.MidPrio);
            res = false;
        }
        else if (isDefending)
        {
            GameObject go;
            if (DefendingHoldingTimer < CharInfo.ShieldStats.Invulnerability)
            {
                Sic.ReflexExp += damage;
                damage = 0;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Full, AudioBus.MidPrio);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.fullDefenceCost;
                CharInfo.Ether += UniversalGameBalancer.Instance.staminaRegenOnPerfectBlock;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.full);
                Sic.CompleteDefences++;
                ComboManager.Instance.TriggerComboForCharacter(CharInfo.CharacterID, ComboType.Defence, true, transform.position);
            }
            else
            {
                Sic.ReflexExp += damage * 0.5f;
  
                damage = damage - CharInfo.ShieldStats.ShieldAbsorbtion;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldNormal);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Partial, AudioBus.HighPrio);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.partialDefenceCost;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.partial);
                Sic.Defences++;
                ComboManager.Instance.TriggerComboForCharacter(CharInfo.CharacterID, ComboType.Defence, false);
                damage = damage < 0 ? 1 : damage;
            }

            FireActionEvent(CharacterActionType.Defence);
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

        EventManager.Instance?.UpdateHealth(this);
        EventManager.Instance?.UpdateStamina(this);
        SetFinalDamage(attacker, (healthCT != HealthChangedType.Heal ? damage - CharInfo.HealthStats.Armour > 0 ? damage - CharInfo.HealthStats.Armour : 0 : damage) / GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos).TileADStats.y);
        HealthStatsChangedEvent?.Invoke(Mathf.Abs(damage), healthCT, SpineAnim.transform);
        return res;
    }

    public virtual void SetFinalDamage(BaseCharacter attacker, float damage, HitInfoClass hic = null)
    {
        if (CharInfo.HealthPerc > 0)
        {
            if (hic == null) hic = HittedByList.Where(r => r.CharacterId == attacker.CharInfo.CharacterID).FirstOrDefault();
            if (hic == null) HittedByList.Add(new HitInfoClass(attacker, damage));
            if (hic != null)
            {
                hic.UpdateLastHitTime();
            }
            attacker?.MadeDamage(this, damage);
            CharInfo.Health -= damage;
        }
    }

    public virtual void MadeDamage(BaseCharacter target, float damage)
    {

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


    public void FireActionEvent(CharacterActionType action)
    {
        CurrentCharStartingActionEvent?.Invoke(CurrentPlayerController, action);
    }


    public void StartAI()
    {
        AICo = AI();
        StartCoroutine(AICo);
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
    public ElementalResistenceClass ElementalResistence;
    public ElementalType ElementalPower;
    public float Timer;
    public Vector2 values = new Vector2();
    public float Value
    {
        get
        {
            return UnityEngine.Random.Range(values.x, values.y);
        }
    }
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
        values = effect.Value;
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
    public Buff_DebuffClass CurrentBuffDebuff;
    public IEnumerator BuffDebuffCo;
    public float Duration;
    public BuffDebuffStatsType Stat;
    public int Level;
    public BaseCharacter EffectMaker;
    public float currentBuffValue = 0f;
    public int CurrentStack = 1;

    public void UpdateBuffValue()
    {
        currentBuffValue = CurrentBuffDebuff.Value;
    }

    public BuffDebuffClass()
    {

    }
    public BuffDebuffClass(BuffDebuffStatsType stat, int level, Buff_DebuffClass currentCuffDebuff, float duration, BaseCharacter effectMaker)
    {
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        Duration = duration;
        EffectMaker = effectMaker;
        CurrentStack = 1;
    }

    public BuffDebuffClass(BuffDebuffStatsType stat, int level, Buff_DebuffClass currentCuffDebuff, IEnumerator buffDebuffCo, float duration)
    {
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        BuffDebuffCo = buffDebuffCo;
        Duration = duration;
        CurrentStack = 1;
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