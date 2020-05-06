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
    public bool IsUsingAPortal = false;
    protected IEnumerator MoveCo;
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    public SpineAnimationManager SpineAnim;
    public bool _IsOnField = false;
    public bool CanAttack = false;
    public bool isSpecialLoading = false;
    public bool isSpecialQueueing = false;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();

    public List<BuffDebuffClass> BuffsDebuffsList = new List<BuffDebuffClass>();
    public List<CharacterActionType> CharActionlist = new List<CharacterActionType>();
    public bool VFXTestMode = false;
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
    protected IEnumerator attackCoroutine = null;
    public SpecialAttackStatus StopPowerfulAtk;
    private float DefendingHoldingTimer = 0;
    public bool IsSwapping = false;
    public bool SwapWhenPossible = false;
    public GameObject chargeParticles = null;
    protected bool canDefend = true;
    public bool isDefending = false;
    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
        }
    }

    public int _shotsLeftInAttack = 0;

    [HideInInspector]
    public bool _Attacking = false;
    public virtual bool Attacking
    {
        get
        {
            return _Attacking;
        }
        set
        {
            _Attacking = value;
        }
    }
    [HideInInspector] public int CharOredrInLayer = 0;

    public virtual void Start()
    {
        if (VFXTestMode)
        {
            StartAttakCo();
            StartMoveCo();
        }
    }

    protected virtual void Update()
    {
        if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && UMS.CurrentAttackType == AttackType.Particles)
        {
            NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        }

        if (transform.parent == null)
        {

        }


        UMS.HPBar.localScale = new Vector3((1f / 100f) * CharInfo.HealthPerc, 1, 1);

        UMS.StaminaBar.localScale = new Vector3((1f / 100f) * CharInfo.StaminaPerc, 1, 1);
    }

    #region Setup Character
    public virtual void SetupCharacterSide()
    {

        if (UMS.PlayerController.Contains(ControllerType.Enemy))
        {
            if (CharInfo.CharacterID != CharacterNameType.Stage00_BossOctopus &&
            CharInfo.CharacterID != CharacterNameType.Stage00_BossOctopus_Head &&
            CharInfo.CharacterID != CharacterNameType.Stage00_BossOctopus_Tentacles &&
            CharInfo.CharacterID != CharacterNameType.Stage00_BossOctopus_Girl)
                UMS.SelectionIndicator.parent.gameObject.SetActive(false);
        }
        else
        {
            UMS.SelectionIndicator.parent.gameObject.SetActive(true);
        }

        SpineAnimatorsetup();
        UMS.SetupCharacterSide();
        int layer = UMS.Side == SideType.LeftSide ? 9 : 10;
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.gameObject.layer = layer;
        }
    }

    public virtual void StartMoveCo()
    {

    }

    public virtual void StopMoveCo()
    {

    }

    protected void _CharInfo_BaseSpeedChangedEvent(float baseSpeed)
    {
        SpineAnim.SetAnimationSpeed(baseSpeed);
    }

    protected void _CharInfo_DeathEvent()
    {
        if (IsOnField)
        {
            // EventManager.Instance.AddCharacterDeath(this);
            SetCharDead();
        }
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

    public virtual void SetCharDead(bool hasToDisappear = true)
    {

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

        }
        isMoving = false;
        SetAttackReady(false);
        Call_CurrentCharIsDeadEvent();
        shotsLeftInAttack = 0;
        if (hasToDisappear)
        {
            transform.position = new Vector3(100, 100, 100);
            gameObject.SetActive(false);
        }
    }

    protected virtual void Call_CurrentCharIsDeadEvent()
    {
        CurrentCharIsDeadEvent(CharInfo.CharacterID, UMS.PlayerController, UMS.Side);
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

    public void StartAttakCo()
    {
        if (UMS.CurrentAttackType == AttackType.Tile && attackCoroutine == null)
        {
            attackCoroutine = AttackAction(true);
            StartCoroutine(attackCoroutine);
        }
    }

    //Basic attack Action that will start the attack anim every x seconds
    public virtual IEnumerator AttackAction(bool yieldBefore)
    {
        // DOnt do anything until the unit is free to attack(otherwise attack anim gets interupted by the other ones)
        while (SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle.ToString())
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (true)
        {

            //Wait until next attack (if yielding before)
            if (yieldBefore) yield return PauseAttack((CharInfo.SpeedStats.AttackSpeedRatio / 3) * nextAttack.AttackRatioMultiplier);

            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CanAttack || isMoving ||
                (currentAttackPhase != AttackPhasesType.End))
            {
                yield return null;
            }
            yield return AttackSequence();

            while (isMoving)
            {
                yield return null;

            }
            //Wait until next attack
            if (!yieldBefore) yield return PauseAttack((CharInfo.SpeedStats.AttackSpeedRatio / 3) * nextAttack.AttackRatioMultiplier);

        }

    }


    public virtual IEnumerator AttackSequence(ScriptableObjectAttackBase atk = null)
    {
        yield return null;
    }

    public virtual void fireAttackAnimation(Vector3 pos)
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

    public IEnumerator PauseAttack(float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            yield return new WaitForFixedUpdate();
            while ((!VFXTestMode && BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
            {
                yield return new WaitForEndOfFrame();
            }

            while (isSpecialLoading)
            {
                yield return new WaitForEndOfFrame();
                timer = 0;
            }

            timer += Time.fixedDeltaTime;
        }
    }


    List<ScriptableObjectAttackBase> availableAtks = new List<ScriptableObjectAttackBase>();
    List<ScriptableObjectAttackBase> currentTileAtks = new List<ScriptableObjectAttackBase>();
    ScriptableObjectAttackBase atkToCheck;
    public void GetAttack()
    {
        currentTileAtks = CharInfo.CurrentAttackTypeInfo.Where(r => r != null && r.CurrentAttackType == AttackType.Tile).ToList();
        availableAtks.Clear();
        for (int i = 0; i < currentTileAtks.Count; i++)
        {
            atkToCheck = currentTileAtks[i];
            switch (atkToCheck.TilesAtk.StatToCheck)
            {
                case WaveStatsType.Health:
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
                case WaveStatsType.Stamina:
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
                case WaveStatsType.None:
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
        //Debug.Log("Cast");

        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Cast : nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
            SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);
        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);

        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                CharInfo.Stamina -= CharInfo.RapidAttack.Stamina_Cost_Atk;
                EventManager.Instance?.UpdateStamina(this);
            }
            else if (SpineAnim.CurrentAnim.Contains("Atk2"))
            {
                CharInfo.Stamina -= CharInfo.PowerfulAttac.Stamina_Cost_Atk;
                EventManager.Instance?.UpdateStamina(this);
            }
        }
    }

    //Create and set up the basic info for the bullet
    public void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
        // Debug.Log(isSpecialLoading);
        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.BulletEffectTiles = bulletBehaviourInfo.BulletEffectTiles;
        bs.Trajectory_Y = bulletBehaviourInfo.Trajectory_Y;
        bs.Trajectory_Z = bulletBehaviourInfo.Trajectory_Z;
        bs.Facing = UMS.Facing;
        bs.ChildrenExplosionDelay = CharInfo.DamageStats.ChildrenBulletDelay;
        bs.StartingTile = UMS.CurrentTilePos;
        bs.BulletGapStartingTile = bulletBehaviourInfo.BulletGapStartingTile;
        bs.Elemental = CharInfo.DamageStats.CurrentElemental;
        bs.Side = UMS.Side;
        bs.VFXTestMode = VFXTestMode;
        bs.CharInfo = CharInfo;
        bs.attackAudioType = GetAttackAudio();
        bs.EffectChances = 100;
        bs.HitPs = UMS.Side == SideType.LeftSide ? nextAttack.Particles.Left.Hit : nextAttack.Particles.Right.Hit;
        bs.AttackInput = nextAttack.AttackInput;
        bs.AtkType = nextAttack.AttackAnim;
        if (bulletBehaviourInfo.HasEffect)
        {
            bs.BulletEffects = bulletBehaviourInfo.Effects;
        }

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


    public void CreateParticleAttack()
    {

        if (nextAttack.CurrentAttackType == AttackType.Particles)
        {
            foreach (BulletBehaviourInfoClass item in nextAttack.ParticlesAtk.BulletTrajectories)
            {
                CreateBullet(item);
            }
        }
        else
        {
            CreateTileAttack();
        }
    }


    public Vector2Int nextAttackPos;
    public virtual void CreateTileAttack()
    {

        if (nextAttack.CurrentAttackType == AttackType.Tile)
        {
            CharInfo.RapidAttack.DamageMultiplier = nextAttack.DamageMultiplier;
            BaseCharacter charTar = null;
            if (nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget)
            {
                charTar = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList().OrderBy(a => a.CharInfo.HealthPerc).FirstOrDefault();
            }

            foreach (BulletBehaviourInfoClassOnBattleFieldClass item in nextAttack.TilesAtk.BulletTrajectories)
            {
                foreach (BattleFieldAttackTileClass target in item.BulletEffectTiles)
                {
                    Vector2Int res = nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget && charTar != null ? target.Pos + nextAttackPos :
                    nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf ? target.Pos + UMS.CurrentTilePos : target.Pos + nextAttackPos;
                    if (GridManagerScript.Instance.isPosOnField(res))
                    {
                        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(res);
                        if (target.IsEffectOnTile)
                        {
                            bts.SetupEffect(target.Effects, target.DurationOnTile, target.TileParticlesID);
                        }
                        else
                        {
                            if (bts._BattleTileState != BattleTileStateType.Blocked)
                            {
                                if (nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf && bts.WalkingSide == UMS.WalkingSide)
                                {
                                    shotsLeftInAttack++;

                                    bts.BattleTargetScript.SetAttack(item.Delay, res,
                                    0, CharInfo.Elemental, this,
                                    target.Effects, target.EffectChances);
                                }
                                else if (nextAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf && bts.WalkingSide != UMS.WalkingSide)
                                {
                                    shotsLeftInAttack++;

                                    bts.BattleTargetScript.SetAttack(item.Delay, res,
                                    CharInfo.DamageStats.BaseDamage, CharInfo.Elemental, this,
                                    target.Effects, target.EffectChances);
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    #endregion
    #region Defence



    protected float defenceAnimSpeedMultiplier = 5f;

    public void StartDefending()
    {
        if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CharActionlist.Contains( CharacterActionType.Defence))
        {
            return;
        }
        SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
        SpineAnim.SetAnimationSpeed(defenceAnimSpeedMultiplier);

        if (canDefend && CharInfo.Shield >= UniversalGameBalancer.Instance.defenceCost)
        {
            CharInfo.Shield -= UniversalGameBalancer.Instance.defenceCost;
            isDefending = true;
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
            timer = Mathf.Clamp(timer - Time.deltaTime, 0f, (SpineAnim.GetAnimLenght(CharacterAnimationStateType.Defending) / defenceAnimSpeedMultiplier) * 0.25f);
            yield return null;
        }
        SetAnimation(CharacterAnimationStateType.Idle, true);
        yield return null;
    }

    private IEnumerator ReloadDefending_Co()
    {
        StopDefending();
        canDefend = false;
        while (CharInfo.ShieldPerc != 100f)
        {
            yield return null;
        }
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
            DefendingHoldingTimer += Time.deltaTime;
            if (CharInfo.ShieldPerc == 0) StartCoroutine(ReloadDefending_Co());
        }
        DefendingHoldingTimer = 0;
    }

    public void StopDefending()
    {
        if (isDefending)
        {
            isDefending = false;
            SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
        }

    }

    #endregion
    #region Move

    public virtual IEnumerator MoveCharOnDir_Co(InputDirection nextDir)
    {
        if ((CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString() && CharActionlist.Contains(CharacterActionType.Move)) || BattleManagerScript.Instance.VFXScene)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript> CurrentBattleTilesToCheck = new List<BattleTileScript>();

            CharacterAnimationStateType AnimState;
            Vector2Int dir;
            AnimationCurve curve;
            GetDirectionVectorAndAnimationCurve(nextDir, out AnimState, out dir, out curve);

            CurrentBattleTilesToCheck = CheckTileAvailability(dir);

            if (CurrentBattleTilesToCheck.Count > 0 &&
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count && GridManagerScript.Instance.isPosOnField(UMS.CurrentTilePos + dir))
            {
                SetAnimation(AnimState);
                isMoving = true;
                if (prevBattleTile.Count > 1)
                {

                }
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                }
                UMS.CurrentTilePos += dir;
                CharOredrInLayer = 101 + (dir.x * 10) + (UMS.Facing == FacingType.Right ? dir.y - 12 : dir.y);
                if (CharInfo.UseLayeringSystem)
                {
                    SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
                }

                CurrentBattleTiles = CurrentBattleTilesToCheck;
                UMS.Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in CurrentBattleTilesToCheck)
                {
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
                    if (MoveCo != null)
                    {
                        Debug.Log("StopMoveCo");
                        //StopCoroutine(MoveCo);
                    }

                    stopCo = true;

                    if (SpineAnim.CurveType == MovementCurveType.Speed_Time)
                    {
                        MoveCo = MoveByTileSpeed(resbts.transform.position, curve, SpineAnim.GetAnimLenght(AnimState));
                    }
                    else
                    {

                        MoveCo = MoveByTileSpace(resbts.transform.position, curve, SpineAnim.GetAnimLenght(AnimState));
                    }

                   
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

        if (currentAttackPhase == AttackPhasesType.Loading || currentAttackPhase == AttackPhasesType.Cast_Powerful || currentAttackPhase == AttackPhasesType.Bullet_Powerful)
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


    private List<BattleTileScript> CheckTileAvailability(Vector2Int dir)
    {
        List<Vector2Int> nextPos = CalculateNextPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(UMS.Pos, nextPos, UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPos(Vector2Int direction)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        UMS.Pos.ForEach(r => res.Add(r + direction));
        return res;
    }


    bool stopCo = false;
    public virtual IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        Debug.Log("StartMoveCo  " + Time.time);
        float timer = 0;
        stopCo = false;
        float spaceTimer = 0;
        bool isMovCheck = false;
        bool isDefe = false;
        float moveValue = CharInfo.SpeedStats.MovementSpeed * CharInfo.SpeedStats.BaseSpeed;
        Transform spineT = SpineAnim.transform;
        Vector3 offset = spineT.position;
        transform.position = nextPos;
        nextPos = spineT.localPosition;
        spineT.position = offset;
        Vector3 localoffset = spineT.localPosition; 
        while (timer < 1 && !stopCo)
        {

            yield return BattleManagerScript.Instance.PauseUntil();
            timer += (Time.fixedDeltaTime / (animLength / moveValue));
            spaceTimer = curve.Evaluate(timer);
            spineT.localPosition = Vector3.Lerp(localoffset, nextPos, spaceTimer);

            if (timer > 0.7f && !isMovCheck)
            {
                isMovCheck = true;
                isMoving = false;
                if (isDefending && !isDefe)
                {
                    isDefe = true;
                    SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                    SpineAnim.SetAnimationSpeed(5);
                }
                TileMovementCompleteEvent?.Invoke(this);
            }

            if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
            {
                isMoving = false;
                TileMovementCompleteEvent?.Invoke(this);
                MoveCo = null;
                yield break;
            }
        }
        Debug.Log("EndMoveCo");
    }

    //Move the character on the determinated Tile position
    public virtual IEnumerator MoveByTileSpeed(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        float timer = 0;
        float speedTimer = 0;
        Vector3 offset = transform.position;
        bool isMovCheck = false;
        bool isDefe = false;
        float moveValue = CharInfo.SpeedStats.MovementSpeed * CharInfo.SpeedStats.BaseSpeed;
        while (timer < 1)
        {

            yield return BattleManagerScript.Instance.PauseUntil();
            float newAdd = (Time.fixedDeltaTime / (animLength / moveValue));
            timer += (Time.fixedDeltaTime / (animLength / moveValue));
            speedTimer += newAdd * curve.Evaluate(timer + newAdd);
            transform.position = Vector3.Lerp(offset, nextPos, speedTimer);

            if (timer > 0.7f && !isMovCheck)
            {
                isMovCheck = true;
                isMoving = false;
                if (isDefending && !isDefe)
                {
                    isDefe = true;
                    SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                    SpineAnim.SetAnimationSpeed(5);
                }
                TileMovementCompleteEvent?.Invoke(this);
            }

            if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
            {
                isMoving = false;
                TileMovementCompleteEvent?.Invoke(this);
                MoveCo = null;
                yield break;
            }
        }


        if (IsOnField)
        {
            transform.position = nextPos;
        }
        MoveCo = null;
    }
    #endregion
    #region Buff/Debuff


    public void Buff_DebuffCo(Buff_DebuffClass bdClass)
    {
        BuffDebuffClass item = BuffsDebuffsList.Where(r => r.Stat == bdClass.Stat).FirstOrDefault();
        string[] newBuffDebuff = bdClass.Name.Split('_');
        if (item == null)
        {
            Debug.Log(bdClass.Name + "   " + newBuffDebuff.Last());
            item = new BuffDebuffClass(bdClass.Name, bdClass.Stat, Convert.ToInt32(newBuffDebuff.Last()), bdClass, bdClass.Duration);
            item.BuffDebuffCo = Buff_DebuffCoroutine(item);
            BuffsDebuffsList.Add(item);
            StartCoroutine(item.BuffDebuffCo);
        }
        else
        {

            if (item.Level <= Convert.ToInt32(newBuffDebuff.Last()))
            {
                string[] currentBuffDebuff = item.Name.ToString().Split('_');
                item.CurrentBuffDebuff.Stop_Co = true;
                BuffsDebuffsList.Remove(item);
                item = new BuffDebuffClass(bdClass.Name, bdClass.Stat, Convert.ToInt32(newBuffDebuff.Last()), bdClass, bdClass.Duration);
                item.BuffDebuffCo = Buff_DebuffCoroutine(item);
                BuffsDebuffsList.Add(item);
                StartCoroutine(item.BuffDebuffCo);
            }
        }
    }

    //Used to Buff/Debuff the character
    public IEnumerator Buff_DebuffCoroutine(BuffDebuffClass bdClass)
    {
        GameObject ps = null;
        if (bdClass.CurrentBuffDebuff.ParticlesToFire != ParticlesType.None)
        {
            ps = ParticleManagerScript.Instance.GetParticle(bdClass.CurrentBuffDebuff.ParticlesToFire);
            ps.transform.parent = SpineAnim.transform;
            ps.transform.localPosition = Vector3.zero;
            ps.SetActive(true);
        }
        System.Reflection.FieldInfo parentField = null, field = null, B_field = null;
        string[] statToCheck = bdClass.Stat.ToString().Split('_');

        if (bdClass.Stat == BuffDebuffStatsType.ElementalResistance)
        {
            ElementalResistance(bdClass.CurrentBuffDebuff);
        }
        else
        {
            parentField = CharInfo.GetType().GetField(statToCheck[0]);
            field = parentField.GetValue(CharInfo).GetType().GetField(statToCheck[1]);

            B_field = parentField.GetValue(CharInfo).GetType().GetField("B_" + statToCheck[1]);
            if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Perc)
            {
                if (field.FieldType == typeof(Vector2))
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? Vector2.zero : (Vector2)field.GetValue(parentField.GetValue(CharInfo)) +
                        (((Vector2)B_field.GetValue(parentField.GetValue(CharInfo))) / 100) * bdClass.CurrentBuffDebuff.Value);
                }
                else
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? 0 : (float)field.GetValue(parentField.GetValue(CharInfo)) +
                        (((float)B_field.GetValue(parentField.GetValue(CharInfo))) / 100) * bdClass.CurrentBuffDebuff.Value);
                }
            }
            else
            {
                if (field.FieldType == typeof(Vector2))
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? Vector2.zero : new Vector2(((Vector2)field.GetValue(parentField.GetValue(CharInfo))).x + bdClass.CurrentBuffDebuff.Value,
                        ((Vector2)field.GetValue(parentField.GetValue(CharInfo))).y + bdClass.CurrentBuffDebuff.Value));
                }
                else
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? 0 : (float)field.GetValue(parentField.GetValue(CharInfo)) + bdClass.CurrentBuffDebuff.Value);
                }


            }

            if (statToCheck[1].Contains("Health"))
            {
                HealthStatsChangedEvent?.Invoke(bdClass.CurrentBuffDebuff.Value, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, transform);
            }

            if (statToCheck[1] == "BaseSpeed")
            {
                SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.BaseSpeed);
            }
        }


        //SetAnimation(bdClass.CurrentBuffDebuff.AnimToFire);
        int iterator = 0;
        while (bdClass.CurrentBuffDebuff.Timer <= bdClass.Duration && !bdClass.CurrentBuffDebuff.Stop_Co)
        {
            yield return BattleManagerScript.Instance.PauseUntil();

            bdClass.CurrentBuffDebuff.Timer += Time.fixedDeltaTime;

            if (((int)bdClass.CurrentBuffDebuff.Timer) > iterator && statToCheck.Length == 3 && statToCheck[2].Contains("Overtime"))
            {
                iterator++;
                if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Perc)
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? 0 : (float)field.GetValue(parentField.GetValue(CharInfo)) +
                        (((float)B_field.GetValue(parentField.GetValue(CharInfo))) / 100) * bdClass.CurrentBuffDebuff.Value);
                }
                else
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? 0 : (float)field.GetValue(parentField.GetValue(CharInfo)) + bdClass.CurrentBuffDebuff.Value);
                }
                HealthStatsChangedEvent?.Invoke(bdClass.CurrentBuffDebuff.Value, bdClass.CurrentBuffDebuff.Value > 0 ? HealthChangedType.Heal : HealthChangedType.Damage, transform);
            }


        }

        if (bdClass.Stat != BuffDebuffStatsType.ElementalResistance)
        {
            if (bdClass.CurrentBuffDebuff.StatsChecker == StatsCheckerType.Perc)
            {
                if (field.FieldType == typeof(Vector2))
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? (Vector2)B_field.GetValue(parentField.GetValue(CharInfo)) :
                   (Vector2)field.GetValue(parentField.GetValue(CharInfo)) - (((Vector2)B_field.GetValue(parentField.GetValue(CharInfo))) / 100) * bdClass.CurrentBuffDebuff.Value);
                }
                else
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? (float)B_field.GetValue(parentField.GetValue(CharInfo)) :
                    (float)field.GetValue(parentField.GetValue(CharInfo)) - (((float)B_field.GetValue(parentField.GetValue(CharInfo))) / 100) * bdClass.CurrentBuffDebuff.Value);
                }


            }
            else
            {

                if (field.FieldType == typeof(Vector2))
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? (Vector2)B_field.GetValue(parentField.GetValue(CharInfo)) :
                    new Vector2(((Vector2)field.GetValue(parentField.GetValue(CharInfo))).x - bdClass.CurrentBuffDebuff.Value, ((Vector2)field.GetValue(parentField.GetValue(CharInfo))).y - bdClass.CurrentBuffDebuff.Value));
                }
                else
                {
                    field.SetValue(parentField.GetValue(CharInfo), bdClass.CurrentBuffDebuff.Value == 0 ? (float)B_field.GetValue(parentField.GetValue(CharInfo)) :
                    (float)field.GetValue(parentField.GetValue(CharInfo)) - bdClass.CurrentBuffDebuff.Value);
                }
            }

            if (statToCheck[1] == "BaseSpeed")
            {
                SpineAnim.SetAnimationSpeed(CharInfo.SpeedStats.BaseSpeed);
            }
        }
        BuffsDebuffsList.Remove(bdClass);
        ps?.SetActive(false);
    }


    private void ElementalResistance(Buff_DebuffClass bdClass)
    {
        CurrentBuffsDebuffsClass currentBuffDebuff = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
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
        }
    }



    private IEnumerator ElementalBuffDebuffCo(CurrentBuffsDebuffsClass newBuffDebuff)
    {
        float timer = 0;
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

        BuffsDebuffs.Remove(newBuffDebuff);
    }

    #endregion
    #region Animation

    public void ArrivingEvent()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
        if (CharInfo.AudioProfile.ArrivingCry != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.ArrivingCry, AudioBus.MediumPriority, transform);
        }

        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalImpact, AudioBus.MediumPriority, transform);
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
    public IEnumerator PuppetAnimation(string animState, int loops, bool _pauseOnEndFrame = false, float animSpeed = 1f)
    {
        isPuppeting = true;
        puppetAnimCompleteTick = 0;
        int currentAnimPlay = 0;
        while(currentAnimPlay < loops)
        {
            SetAnimation(animState, _pauseOnLastFrame: (currentAnimPlay + 1 == loops && _pauseOnEndFrame));
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

    protected bool pauseOnLastFrame = false;
    public virtual void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        
        if (CharInfo.SpeedStats.BaseSpeed <= 0)
        {
            return;
        }
        //Debug.Log(animState.ToString() + SpineAnim.CurrentAnim.ToString() + CharInfo.CharacterID.ToString());
        if (animState == CharacterAnimationStateType.Arriving.ToString())
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


        if (isMoving && animState.ToString() != CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            return;
        }


        if (SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Arriving.ToString()) || SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Reverse_Arriving.ToString()))
        {
            return;
        }

        float AnimSpeed = 1;

        if (animState == CharacterAnimationStateType.Atk.ToString() || animState == CharacterAnimationStateType.Atk1.ToString())
        {
            AnimSpeed = CharInfo.SpeedStats.AttackSpeed * CharInfo.BaseSpeed;
        }
        else if (animState == CharacterAnimationStateType.DashDown.ToString() ||
            animState == CharacterAnimationStateType.DashUp.ToString() ||
            animState == CharacterAnimationStateType.DashLeft.ToString() ||
            animState == CharacterAnimationStateType.DashRight.ToString())
        {
            AnimSpeed = CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed;
        }
        else if (animState.Contains(CharacterAnimationStateType.Reverse_Arriving.ToString()) || animState.Contains(CharacterAnimationStateType.Arriving.ToString()))
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

            if (SpineAnim.CurveType == MovementCurveType.Space_Time)
            {
                UMS.HpBarContainer.parent = SpineAnim.transform;
                UMS.StaminaBarContainer.parent = SpineAnim.transform;
                UMS.IndicatorContainer.parent = SpineAnim.transform;
            }
        }
    }

    public virtual void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        if (isPuppeting) puppetAnimCompleteTick++;
        if (pauseOnLastFrame) return;
        if (PlayQueuedAnim()) return;

        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() || completedAnim.Contains("Growing"))
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

    public virtual bool SetDamage(float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        return SetDamage(damage, elemental, isCritical);
    }

    public virtual bool SetDamage(float damage, ElementalType elemental, bool isCritical)
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
                damage = 0;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.BasicShield, AudioBus.MediumPriority);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.fullDefenceCost;
                CharInfo.Stamina += UniversalGameBalancer.Instance.staminaRegenOnPerfectBlock;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.full);
            }
            else
            {
                damage = damage - CharInfo.DefenceStats.BaseDefence;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldNormal);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.MegaShield, AudioBus.HighPriority);
                go.transform.position = transform.position;
                CharInfo.Shield -= UniversalGameBalancer.Instance.partialDefenceCost;
                EventManager.Instance.AddBlock(this, BlockInfo.BlockType.partial);

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
            SetAnimation(CharacterAnimationStateType.GettingHit);
            healthCT = isCritical ? HealthChangedType.CriticalHit : HealthChangedType.Damage;
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


        CharInfo.Health -= damage;
        if (CharInfo.Health == 0)
        {
            EventManager.Instance?.AddCharacterDeath(this);
        }
        EventManager.Instance?.UpdateHealth(this);
        EventManager.Instance?.UpdateStamina(this);
        HealthStatsChangedEvent?.Invoke(damage, healthCT, transform);
        return res;
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

    public IEnumerator UsePortal(PortalInfoClass outPortal)
    {
        while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || isMoving)
        {
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine(MoveCo);
        IsUsingAPortal = true;
        transform.position = outPortal.PortalPos.transform.position;

    }

    public IEnumerator Freeze(float duration, float speed)
    {

        while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || isMoving)
        {
            yield return new WaitForEndOfFrame();
        }
        SpineAnim.SetAnimationSpeed(speed);
        float timer = 0;
        while (timer <= duration)
        {
            yield return BattleManagerScript.Instance.PauseUntil();

            timer += Time.fixedDeltaTime;
        }

        SpineAnim.SetAnimationSpeed(CharInfo.BaseSpeed);
    }

    public IEnumerator Trap(PortalInfoClass outPortal)
    {
        while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || isMoving)
        {
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine(MoveCo);
        IsUsingAPortal = true;
        transform.position = outPortal.PortalPos.transform.position;

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
    public string Name;
    public float Duration;
    public float Value;
    public CharacterAnimationStateType AnimToFire;
    public BuffDebuffStatsType Stat;
    public StatsCheckerType StatsChecker;
    public ElementalResistenceClass ElementalResistence;
    public ElementalType ElementalPower;
    public ParticlesType ParticlesToFire;
    public float Timer;
    public bool Stop_Co = false;


    public Buff_DebuffClass(string name, float duration, float value, BuffDebuffStatsType stat,
        StatsCheckerType statsChecker, ElementalResistenceClass elementalResistence, ElementalType elementalPower
        , CharacterAnimationStateType animToFire, ParticlesType particlesToFire)
    {
        Name = name;
        Duration = duration;
        Value = value;
        Stat = stat;
        StatsChecker = statsChecker;
        //AttackT = attackT;
        ElementalResistence = elementalResistence;
        ElementalPower = elementalPower;
        AnimToFire = animToFire;
        ParticlesToFire = particlesToFire;
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



public class BuffDebuffClass
{
    public string Name;
    public Buff_DebuffClass CurrentBuffDebuff;
    public IEnumerator BuffDebuffCo;
    public float Duration;
    public BuffDebuffStatsType Stat;
    public int Level;

    public BuffDebuffClass()
    {

    }
    public BuffDebuffClass(string name, BuffDebuffStatsType stat, int level, Buff_DebuffClass currentCuffDebuff, float duration)
    {
        Name = name;
        Stat = stat;
        Level = level;
        CurrentBuffDebuff = currentCuffDebuff;
        Duration = duration;
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