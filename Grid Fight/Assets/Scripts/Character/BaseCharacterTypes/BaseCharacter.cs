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

    public CharacterInfoScript CharInfo
    {
        get
        {
            if (_CharInfo == null)
            {
                _CharInfo = this.GetComponentInChildren<CharacterInfoScript>(true);
                _CharInfo.BaseSpeedChangedEvent += _CharInfo_BaseSpeedChangedEvent;
                _CharInfo.DeathEvent += _CharInfo_DeathEvent;
            }
            return _CharInfo;
        }
    }
    public CharacterInfoScript _CharInfo;
    public bool isMoving = false;
    public bool IsUsingAPortal = false;
    protected IEnumerator MoveCo;
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    public SpineAnimationManager SpineAnim;
    public bool IsOnField = false;
    public bool CanAttack = false;
    public CharacterLevelType NextAttackLevel = CharacterLevelType.Novice;
    public bool isSpecialLoading = false;
    public bool isSpecialQueueing = false;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool VFXTestMode = false;
    public UnitManagementScript UMS;
    public BoxCollider CharBoxCollider;
    public ScriptableObjectAttackBase nextAttack = null;
    public AttackPhasesType currentAttackPhase = AttackPhasesType.End;
    public DeathProcessStage currentDeathProcessPhase = DeathProcessStage.None;
    protected IEnumerator attackCoroutine = null;
    public SpecialAttackStatus StopPowerfulAtk;
    private float DefendingHoldingTimer = 0;
    public bool IsSwapping = false;
    public bool SwapWhenPossible = false;
    public GameObject chargeParticles = null;

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

    // Temp variables to allow the minions without proper animations setup to charge attacks
    public bool sequencedAttacker = false;
    [HideInInspector]
    public bool Attacking = false;
    private int OredrInLayer = 0;

    protected virtual void Start()
    {
        if(VFXTestMode || UMS.CurrentAttackType == AttackType.Tile)
        {
            StartAttakCo();
            StartMoveCo();
        }
    }

    protected virtual void Update()
    {
        if (CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && UMS.CurrentAttackType == AttackType.Particles)
        {
            NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo);
        }

        if(transform.parent == null)
        {

        }


        UMS.HPBar.localScale = new Vector3((1f / 100f) * CharInfo.HealthPerc,1,1);

        UMS.StaminaBar.localScale = new Vector3((1f / 100f) * CharInfo.StaminaPerc, 1, 1);
    }

    #region Setup Character
    public virtual void SetupCharacterSide()
    {
        EventManager.Instance.UpdateHealth(this);
        if (UMS.PlayerController.Contains(ControllerType.Enemy))
        {
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

    private void _CharInfo_BaseSpeedChangedEvent(float baseSpeed)
    {
        SpineAnim.SetAnimationSpeed(baseSpeed);
    }

    private void _CharInfo_DeathEvent()
    {
        if (IsOnField)
        {
           // EventManager.Instance.AddCharacterDeath(this);
            SetCharDead();
        }
    }

    public virtual void SetAttackReady(bool value)
    {
        if(CharBoxCollider != null)
        {
            CharBoxCollider.enabled = value;
        }
        CanAttack = value;
        IsOnField = value;
        currentAttackPhase = AttackPhasesType.End;
        OredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.SetSkeletonOrderInLayer(OredrInLayer);
        }
    }

    public virtual void SetCharDead()
    {
        for (int i = 0; i < UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
            UMS.Pos[i] = Vector2Int.zero;
        }
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;

        }
        isMoving = false;
        SetAttackReady(false);
        Call_CurrentCharIsDeadEvent();
        transform.position = new Vector3(100,100,100);
        gameObject.SetActive(false);
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

    public virtual void SpecialAttackImpactEffects()
    {

    }

    public void StartAttakCo()
    {
        if(UMS.CurrentAttackType == AttackType.Tile && attackCoroutine == null)
        {
            attackCoroutine = AttackAction(true);
            StartCoroutine(attackCoroutine);
        }
    }

    //Basic attack Action that will start the attack anim every x seconds
    public virtual IEnumerator AttackAction(bool yieldBefore)
    {
        if (nextAttack == null)
        {
            GetAttack(CharacterAnimationStateType.Atk);
        }

        // DOnt do anything until the unit is free to attack(otherwise attack anim gets interupted by the other ones)
        while (SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle)
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (true)
        {

            //Wait until next attack (if yielding before)
            if (yieldBefore) yield return PauseAttack(CharInfo.AttackSpeedRatio * nextAttack.AttackRatioMultiplier);

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
            GetAttack(CharacterAnimationStateType.Atk);

            //Wait until next attack
            if (!yieldBefore) yield return PauseAttack(CharInfo.AttackSpeedRatio * nextAttack.AttackRatioMultiplier);

        }

    }


    public virtual IEnumerator AttackSequence()
    {
        yield return null;
    }

    public virtual void fireAttackAnimation(Vector3 pos)
    {

    }

    public int GetHowManyAttackAreOnBattleField(List<BulletBehaviourInfoClassOnBattleField> bulTraj)
    {
        int res = 0;
        Vector2Int basePos = new Vector2Int(UMS.CurrentTilePos.x, GridManagerScript.Instance.YGridSeparator);
        foreach (BulletBehaviourInfoClassOnBattleField item in bulTraj)
        {
            foreach (Vector2Int target in item.BulletEffectTiles)
            {

                Vector2Int posToCheck = basePos - target;
                if (GridManagerScript.Instance.isPosOnField(posToCheck))
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

    public void GetAttack(CharacterAnimationStateType anim = CharacterAnimationStateType.NoMesh)
    {
        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            switch (anim)
            {
                case CharacterAnimationStateType.Atk:
                    nextAttack = CharInfo.CurrentParticlesAttackTypeInfo[0];
                    break;
                case CharacterAnimationStateType.Atk1:
                    nextAttack = CharInfo.CurrentParticlesAttackTypeInfo[1];
                    break;
            }
        }
        else
        {
            foreach (ScriptableObjectAttackTypeOnBattlefield atk in CharInfo.CurrentOnBattleFieldAttackTypeInfo)
            {
                int chances = UnityEngine.Random.Range(0, 101);

                switch (atk.StatToCheck)
                {
                    case WaveStatsType.Health:
                        switch (atk.ValueChecker)
                        {
                            case ValueCheckerType.LessThan:
                                if (CharInfo.HealthPerc < atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                            case ValueCheckerType.EqualTo:
                                if (CharInfo.HealthPerc == atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                            case ValueCheckerType.MoreThan:
                                if (CharInfo.HealthPerc > atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                        }
                        break;
                    case WaveStatsType.Stamina:
                        switch (atk.ValueChecker)
                        {
                            case ValueCheckerType.LessThan:
                                if (CharInfo.StaminaPerc < atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                            case ValueCheckerType.EqualTo:
                                if (CharInfo.StaminaPerc == atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                            case ValueCheckerType.MoreThan:
                                if (CharInfo.StaminaPerc > atk.PercToCheck && chances < atk.Chances)
                                {
                                    nextAttack = atk;
                                }
                                break;
                        }
                        break;
                    case WaveStatsType.None:
                        nextAttack = atk;
                        break;
                }
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
        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, UMS.CurrentAttackType == AttackType.Particles ? AttackParticlePhaseTypes.CastLeft : AttackParticlePhaseTypes.CastRight,
            NextAttackLevel == CharacterLevelType.Novice ? SpineAnim.FiringPoint.position : SpineAnim.SpecialFiringPoint.position, UMS.Side);
        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
        LayerParticleSelection lps = cast.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            lps.Shot = NextAttackLevel;
            lps.SelectShotLevel();
        }

        if(UMS.CurrentAttackType == AttackType.Particles)
        {
            if (SpineAnim.CurrentAnim.ToString().Contains("Atk1"))
            {
                CharInfo.Stamina -= CharInfo.RapidAttack.Stamina_Cost_Atk;
            }
            else if (SpineAnim.CurrentAnim.ToString().Contains("Atk2"))
            {
                CharInfo.Stamina -= CharInfo.PowerfulAttac.Stamina_Cost_Atk;
            }
        }

        
    }

    //Create and set up the basic info for the bullet
    public void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
       // Debug.Log(isSpecialLoading);
        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = NextAttackLevel == CharacterLevelType.Novice ? SpineAnim.FiringPoint.position : SpineAnim.SpecialFiringPoint.position;
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
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharInfo.ParticleID, AttackParticlePhaseTypes.AttackLeft, bullet.transform, UMS.Side,
            CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script ? true : false);

        LayerParticleSelection lps = bs.PS.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            bs.attackLevel = NextAttackLevel;
            lps.Shot = NextAttackLevel;
            lps.SelectShotLevel();
        }
        if(CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
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
        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            foreach (BulletBehaviourInfoClass item in CharInfo.CurrentParticlesAttackTypeInfo[SpineAnim.CurrentAnim.ToString().Contains("1") ? 0 : 1].BulletTrajectories)
            {
                CreateBullet(item);
            }
        }
       
    }

    public void CreateTileAttack()
    {
        if (UMS.CurrentAttackType == AttackType.Tile)
        {
            CharInfo.RapidAttack.DamageMultiplier = ((ScriptableObjectAttackTypeOnBattlefield)nextAttack).DamageMultiplier;
            GridManagerScript.Instance.StartOnBattleFieldAttackCo(CharInfo, ((ScriptableObjectAttackTypeOnBattlefield)nextAttack), UMS.CurrentTilePos, UMS, this);
        }
    }


    #endregion
    #region Defence
    public void StartDefending()
    {
        SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
        SpineAnim.SetAnimationSpeed(5);
        defe = true;
        DefendingHoldingTimer = 0;
        StartCoroutine(Defending_Co());
    }
    bool defe = false;
    private IEnumerator Defending_Co()
    {
        while (defe)
        {
            if(SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle)
            {
                SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                SpineAnim.SetAnimationSpeed(5);
            }

            yield return null;
            DefendingHoldingTimer += Time.deltaTime;
        }
        DefendingHoldingTimer = 0;
    }

    public void StopDefending()
    {
        defe = false;
        SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
    }


    #endregion
    #region Move
    public virtual void MoveCharOnDirection(InputDirection nextDir)
    {
        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving || SpineAnim.CurrentAnim == CharacterAnimationStateType.Arriving ||
            SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle || IsSwapping || SwapWhenPossible)
        {
            return;
        }

        if ((CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving) || BattleManagerScript.Instance.VFXScene)
        {
            /*if(StopPowerfulAtk >= SpecialAttackStatus.Start)
            {
                StopPowerfulAtk++;
            }*/

            if (currentAttackPhase == AttackPhasesType.Loading || currentAttackPhase == AttackPhasesType.Cast_Powerful || currentAttackPhase == AttackPhasesType.Bullet_Powerful)
            {
                return;
            }

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
                OredrInLayer = 101 + (dir.x * 10) + (UMS.Facing == FacingType.Right ? dir.y - 12 : dir.y);
                if (CharInfo.UseLayeringSystem)
                {
                    SpineAnim.SetSkeletonOrderInLayer(OredrInLayer);
                }

                CurrentBattleTiles = CurrentBattleTilesToCheck;
                UMS.Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in CurrentBattleTilesToCheck)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    UMS.Pos.Add(item.Pos);
                }

                if (MoveCo != null)
                {
                    StopCoroutine(MoveCo);
                }
                MoveCo = MoveByTile(CurrentBattleTiles.Where(r => r.Pos == UMS.CurrentTilePos).First().transform.position, curve, SpineAnim.GetAnimLenght(AnimState));
                StartCoroutine(MoveCo);
            }
            else
            {
                if (TileMovementCompleteEvent != null && TileMovementCompleteEvent.Target != null) TileMovementCompleteEvent(this);
            }

            if (CurrentBattleTiles.Count > 0)
            {
                foreach (BattleTileScript item in prevBattleTile)
                {
                    BattleManagerScript.Instance.OccupiedBattleTiles.Remove(item);
                }
                BattleManagerScript.Instance.OccupiedBattleTiles.AddRange(CurrentBattleTiles);
            }
        }
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
                curve = SpineAnim.UpMovementSpeed;
                AnimState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirection.Down:
                dir = new Vector2Int(1, 0);
                curve = SpineAnim.DownMovementSpeed;
                AnimState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirection.Right:
                dir = new Vector2Int(0, 1);
                curve = SpineAnim.RightMovementSpeed;
                AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                break;
            case InputDirection.Left:
                dir = new Vector2Int(0, -1);
                curve = SpineAnim.LeftMovementSpeed;
                AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                break;
        }
    }


    private List<BattleTileScript> CheckTileAvailability(Vector2Int dir)
    {
        List<Vector2Int> nextPos = CalculateNextPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
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


    //Move the character on the determinated Tile position
    public virtual IEnumerator MoveByTile(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        float timer = 0;
        float speedTimer = 0;
        Vector3 offset = transform.position;
        bool isMovCheck = false;
        bool isDefe = false;
        while (timer < 1)
        {
          
            yield return BattleManagerScript.Instance.PauseUntil();
            float newAdd = (Time.fixedDeltaTime / (animLength / CharInfo.MovementSpeed));
            timer += (Time.fixedDeltaTime / (animLength / CharInfo.MovementSpeed));
            speedTimer += newAdd * curve.Evaluate(timer + newAdd);
            transform.position = Vector3.Lerp(offset, nextPos, speedTimer);

            if(timer > 0.7f && !isMovCheck)
            {
                isMovCheck = true;
                isMoving = false;
                if(defe && !isDefe)
                {
                    isDefe = true;
                    SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                    SpineAnim.SetAnimationSpeed(5);
                }
                if (TileMovementCompleteEvent != null)
                {
                    TileMovementCompleteEvent(this);
                }
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
        StartCoroutine(Buff_DebuffCoroutine(bdClass));
    }

    //Used to Buff/Debuff the character
    public IEnumerator Buff_DebuffCoroutine(Buff_DebuffClass bdClass)
    {
        SetAnimation(CharacterAnimationStateType.Buff);
        GameObject ps = null;
        if (bdClass.ParticlesToFire != ParticlesType.None)
        {
            ps = ParticleManagerScript.Instance.GetParticle(bdClass.ParticlesToFire);
            ps.transform.parent = transform;
            ps.transform.localPosition = Vector3.zero;
            ps.SetActive(true);
        }
        float timer = 0;
        float valueOverDuration = bdClass.Value;
        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.Health:
                if(valueOverDuration < 0)
                {
                    HealthStatsChangedEvent?.Invoke(valueOverDuration, HealthChangedType.Damage, transform);
                }
                else
                {
                    HealthStatsChangedEvent?.Invoke(valueOverDuration, HealthChangedType.Heal, transform);
                }
                CharInfo.Health += valueOverDuration;
                EventManager.Instance.UpdateHealth(this);
                break;
            case BuffDebuffStatsType.ElementalResistance:
                CurrentBuffsDebuffsClass currentBuffDebuff = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
                ElementalWeaknessType BaseWeakness = GetElementalMultiplier(CharInfo.DamageStats.ElementalsResistence, bdClass.ElementalResistence.Elemental);
                CurrentBuffsDebuffsClass newBuffDebuff = new CurrentBuffsDebuffsClass();
                newBuffDebuff.ElementalResistence = bdClass.ElementalResistence;
                newBuffDebuff.Duration = 100 + bdClass.Duration;
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
                break;
            case BuffDebuffStatsType.HealthRegeneration:
                CharInfo.HealthStats.Regeneration += valueOverDuration;
                EventManager.Instance.UpdateHealth(this);
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharInfo.MovementSpeed += valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharInfo.Stamina += valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharInfo.StaminaStats.Regeneration += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharInfo.AttackSpeed += valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharInfo.SpeedStats.BulletSpeed += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharInfo.DamageStats.CurrentElemental = bdClass.ElementalPower;
                break;
            case BuffDebuffStatsType.BaseSpeed:
                CharInfo.SpeedStats.BaseSpeed += valueOverDuration;
                break;
            case BuffDebuffStatsType.Damage:
                EventManager.Instance.UpdateHealth(this);
                CharInfo.DamageStats.BaseDamage += valueOverDuration;
                break;
        }

        SetAnimation(bdClass.AnimToFire);

        while (timer <= bdClass.Duration)
        {
            yield return BattleManagerScript.Instance.PauseUntil();

            if (bdClass.Stat == BuffDebuffStatsType.HealthOverTime)
            {
                valueOverDuration = (bdClass.Value / bdClass.Duration) / 50f;
            }

            timer += Time.fixedDeltaTime;
        }
        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.HealthRegeneration:
                CharInfo.HealthStats.Regeneration -= valueOverDuration;
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharInfo.MovementSpeed -= valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharInfo.Stamina -= valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharInfo.StaminaStats.Regeneration -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharInfo.AttackSpeed -= valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharInfo.SpeedStats.BulletSpeed -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharInfo.DamageStats.CurrentElemental = CharInfo.Elemental;
                break;
            case BuffDebuffStatsType.BaseSpeed:
                CharInfo.SpeedStats.BaseSpeed -= valueOverDuration;
                break;
            case BuffDebuffStatsType.Damage:
                CharInfo.DamageStats.BaseDamage -= valueOverDuration;
                break;
        }

        ps.SetActive(false);
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
        CameraManagerScript.Instance.CameraShake(1);
        UMS.ArrivingParticle.transform.position = transform.position;
        UMS.ArrivingParticle.SetActive(true);
    }



    public virtual void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
         //Debug.Log(animState.ToString() + SpineAnim.CurrentAnim.ToString() + NextAttackLevel.ToString());


        if(animState == CharacterAnimationStateType.GettingHit && currentAttackPhase != AttackPhasesType.End)
        {
            return;
        }

        if (animState == CharacterAnimationStateType.GettingHit && Attacking)
        {
            return;
        }

        if (isMoving)
        {
            return;
        }

        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle)
        {
            return;
        }

        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Arriving || SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving)
        {
            return;
        }

        float AnimSpeed = 1;

        if (animState == CharacterAnimationStateType.Atk || animState == CharacterAnimationStateType.Atk1)
        {
            AnimSpeed = CharInfo.AttackSpeed * CharInfo.BaseSpeed;
        }
        else if (animState == CharacterAnimationStateType.DashDown ||
            animState == CharacterAnimationStateType.DashUp ||
            animState == CharacterAnimationStateType.DashLeft ||
            animState == CharacterAnimationStateType.DashRight)
        {
            AnimSpeed = CharInfo.MovementSpeed * CharInfo.BaseSpeed;
        }
        else if(animState == CharacterAnimationStateType.Reverse_Arriving)
        {
            AnimSpeed = CharInfo.LeaveSpeed;
        }
        else
        {
            AnimSpeed = CharInfo.BaseSpeed;
        }

        SpineAnim.SetAnim(animState, loop, transition);
        SpineAnim.SetAnimationSpeed(AnimSpeed);
    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);
        SpineAnim.CharOwner = this;
    }

    #endregion


    public virtual bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        if(!IsOnField)
        {
            return false;
        }
        HealthChangedType healthCT = HealthChangedType.Damage;
        bool res;
        if(SpineAnim.CurrentAnim == CharacterAnimationStateType.Defending)
        {
            GameObject go;
            if(DefendingHoldingTimer < CharInfo.DefenceStats.Invulnerability)
            {
                damage = 0;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
                go.transform.position = transform.position;
                
            }
            else
            {
                damage = damage - CharInfo.DefenceStats.BaseDefence;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldNormal);
                go.transform.position = transform.position;

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
                go.transform.localScale = new Vector3(-1,1,1);
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
            EventManager.Instance.AddCharacterDeath(this);
        }
        EventManager.Instance.UpdateHealth(this);
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
    public float Duration;
    public float Value;
    public CharacterAnimationStateType AnimToFire;
    public BuffDebuffStatsType Stat;
    public ElementalResistenceClass ElementalResistence;
    public ElementalType ElementalPower;
    public ParticlesType ParticlesToFire;

    public Buff_DebuffClass(float duration, float value, BuffDebuffStatsType stat
        , ElementalResistenceClass elementalResistence, ElementalType elementalPower
        , CharacterAnimationStateType animToFire, ParticlesType particlesToFire)
    {
        Duration = duration;
        Value = value;
        Stat = stat;
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

