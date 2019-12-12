﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{


    public delegate void TileMovementComplete(CharacterBase movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;

    public delegate void CurrentCharIsDead(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsDead CurrentCharIsDeadEvent;


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
    public bool isSpecialLoading = false;
    
    private IEnumerator MoveCo;
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    public SpineAnimationManager SpineAnim;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool IsUsingAPortal = false;
    public bool IsOnField = false;
    public bool AllowMoreElementalOnWepon_ElementalResistence_Armor = false;
    public bool shoot = true;
    public CharacterLevelType NextAttackLevel = CharacterLevelType.Novice;
    public Transform SelectionIndicator;
    public SpriteRenderer SelectionIndicatorSprite;
    public Color SelectionIndicatorColorUnselected;
    public GameObject BaseBullet;
    public UnitManagementScript UMS;
    public List<AttackTypesInfoClass> AttackTypesInfo = new List<AttackTypesInfoClass>();
    public AttackTypesInfoClass CurrentAttackTypeInfo;
    public bool VFXTestMode = false;

    public bool isAttackStarted = false;
    public bool isAttackGoing = false;
    public bool isAttackCompletetd = false;


    #region Unity Life Cycles
    private void Start()
    {
        StartCoroutine(AttackAction());
    }

    private void Update()
    {
      
    }
    #endregion

    #region Setup Character
    public void SetupCharacterSide()
    {
        SpineAnimatorsetup();
        UMS.SetupCharacterSide();
        int layer = UMS.Side == SideType.LeftSide ? 9 : 10;
        SpineAnim.gameObject.layer = layer;
        CurrentAttackTypeInfo = AttackTypesInfo.Where(r => r.CharacterClass == CharInfo.ClassType).First();
    }

    private void _CharInfo_BaseSpeedChangedEvent(float baseSpeed)
    {
        SpineAnim.SetAnimationSpeed(baseSpeed);
    }

    private void _CharInfo_DeathEvent()
    {
        if (IsOnField)
        {
            SetCharDead();
        }
    }


    public void SetCharDead()
    {
        foreach (Vector2Int item in UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Empty);
        }
        IsOnField = false;
        gameObject.SetActive(false);
        if (WaveManagerScript.Instance != null)
        {
            WaveManagerScript.Instance.CurrentNumberOfWaveChars--;
        }
        CurrentCharIsDeadEvent(CharInfo.CharacterID,UMS.PlayerController, UMS.Side);
    }

    #endregion

    #region Attack
    //Basic attack Action that will start the attack anim every x seconds
    public IEnumerator AttackAction()
    {
        while (true)
        {
            while (!IsOnField && !VFXTestMode)
            {
                yield return null;
            }

            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || isMoving || isSpecialLoading))
            {
                yield return null;
            }

            isAttackStarted = false;
            isAttackCompletetd = false;
            isAttackGoing = false;
            while (!isAttackCompletetd)
            {
                if (!isAttackStarted)
                {
                    isAttackStarted = true;
                    isAttackGoing = true;
                    SetAnimation(CharacterAnimationStateType.Atk);
                }

                if (isAttackStarted && !isAttackGoing && !isMoving)
                {
                    isAttackGoing = true;
                    SetAnimation(CharacterAnimationStateType.Atk);
                }
                yield return null;
            }


            float timer = 0;
            while (timer <= CharInfo.AttackSpeedRatio)
            {
                yield return new WaitForFixedUpdate();
                while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
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
    }

    //Load the special attack and fire it if the load is complete
    public IEnumerator LoadSpecialAttack()
    {
        if (CharInfo.StaminaStats.Stamina - CharInfo.StaminaStats.Stamina_Cost_S_Atk01 >= 0 && CharInfo.BaseCharacterType != BaseCharType.Minion)
        {
            isSpecialLoading = true;
            float timer = 0;
            while (isSpecialLoading)
            {
                while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
            }
            if (IsOnField)
            {
                while (isMoving)
                {
                    yield return new WaitForEndOfFrame();
                }
                
                SpecialAttack(CharacterLevelType.Defiant);
            }
        }
    }

    //Set ste special attack
    public void SpecialAttack(CharacterLevelType attackLevel)
    {
        NextAttackLevel = attackLevel;
        SetAnimation(CharacterAnimationStateType.Atk1);
    }

    public void FireCastParticles()
    {
        StartCoroutine(CastAttackParticles(NextAttackLevel));
    }
    //start the casting particlaes foe the attack
    public IEnumerator CastAttackParticles(CharacterLevelType clt)
    {
        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, ParticleTypes.Cast, clt == CharacterLevelType.Novice ? SpineAnim.FiringPoint.position : SpineAnim.SpecialFiringPoint.position, UMS.Side);
        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
        LayerParticleSelection lps = cast.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            lps.Shot = clt;
            if (clt > CharacterLevelType.Novice)
            {
                CharInfo.StaminaStats.Stamina -= CharInfo.StaminaStats.Stamina_Cost_S_Atk01;
            }
            lps.SelectShotLevel();
        }
         while (!isAttackCompletetd)
         {
             if (!isAttackGoing)
             {
                 cast.GetComponentsInChildren<DisableParticleScript>().ToList().ForEach(r => r.ResetParticle());
             }
             yield return null;
         }
    }

    //Create and set up the basic info for the bullet
    public void CreateBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
        if (!shoot)
        {
            return;
        }
        isSpecialLoading = false;
        GameObject bullet = Instantiate(BaseBullet, NextAttackLevel == CharacterLevelType.Novice ? SpineAnim.FiringPoint.position : SpineAnim.SpecialFiringPoint.position, Quaternion.identity);
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
        bs.gameObject.SetActive(true);
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharInfo.ParticleID, ParticleTypes.Attack, bullet.transform, UMS.Side);
        LayerParticleSelection lps = bs.PS.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            bs.attackLevel = NextAttackLevel;
            lps.Shot = NextAttackLevel;
            lps.SelectShotLevel();
        }

        if ((UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x > 5) || (UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x < 0))
        {
            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(UMS.CurrentTilePos.x - bulletBehaviourInfo.BulletDistanceInTile.x, UMS.Side == SideType.LeftSide ? UMS.CurrentTilePos.y + bulletBehaviourInfo.BulletDistanceInTile.y : UMS.CurrentTilePos.y - bulletBehaviourInfo.BulletDistanceInTile.y));
            float h = transform.position.y - bts.transform.position.y;
            bs.DestinationWorld = new Vector3(bts.transform.position.x, (transform.position.y + h), bts.transform.position.z);
            StartCoroutine(bs.MoveToWorldPos());
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
        bs.StartMoveToTile();
    }

    
    public void CreateSingleBullet()
    {
        CreateBullet(CurrentAttackTypeInfo.BulletTrajectories[0]);
    }

    public void CreateMachingunBullets()
    {
        Vector3 offsetRotation = transform.eulerAngles;
        for (int i = 0; i < CharInfo.DamageStats.MultiBulletAttackNumberOfBullets; i++)
        {
            CreateBullet(CurrentAttackTypeInfo.BulletTrajectories[i]);
        }
        if(!VFXTestMode)
        {
            NextAttackLevel = CharacterLevelType.Novice;
        }
        transform.eulerAngles = offsetRotation;
    }

    #endregion

    #region Move
    //used to set the movement of the character on a determinated direction
    public void MoveCharOnDirection(InputDirection nextDir)
    {
        if (CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript>  CurrentBattleTilesToCheck = new List<BattleTileScript>();
            CharacterAnimationStateType AnimState = CharacterAnimationStateType.Idle;
            AnimationCurve curve = new AnimationCurve();
            List<Vector2Int> nextPos;
            Vector2Int dir = Vector2Int.zero;
            switch (nextDir)
            {
                case InputDirection.Up:
                    dir = new Vector2Int(-1, 0);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.UpMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashUp;
                    break;
                case InputDirection.Down:
                    dir = new Vector2Int(1, 0);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.DownMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashDown;
                    break;
                case InputDirection.Right:
                    dir = new Vector2Int(0, 1);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.RightMovementSpeed;
                    AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                    break;
                case InputDirection.Left:
                    dir = new Vector2Int(0, -1);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.LeftMovementSpeed;
                    AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                    break;
            }

            if (CurrentBattleTilesToCheck.Count > 0 && CurrentBattleTilesToCheck.Where(r=>!UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count)
            {
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                }
                UMS.CurrentTilePos += dir;
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
                MoveCo = MoveByTile(CurrentBattleTiles.Where(r=> r.Pos == UMS.CurrentTilePos).First().transform.position, AnimState, curve);
                StartCoroutine(MoveCo);
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

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPos(Vector2Int direction)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        UMS.Pos.ForEach(r => res.Add(r + direction));
        return res;
    }


    public void MoveCharToTargetDestination(Vector3 nextPos, CharacterAnimationStateType animState, float duration)
    {
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
        }
        MoveCo = MoveWorldSpace(nextPos, animState,duration);
        StartCoroutine(MoveCo);
    }

    private IEnumerator MoveWorldSpace(Vector3 nextPos, CharacterAnimationStateType animState, float duration)
    {
        SetAnimation(animState);
        float timer = 0;
        Vector3 offset = transform.position;

        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            //Debug.Log(inum);
            timer += Time.fixedDeltaTime / duration;//TODO Movement Speed
            transform.position = Vector3.Lerp(offset, nextPos, timer);
        }
        transform.position = nextPos;
        MoveCo = null;
    }

    //Move the character on the determinated Tile position
    private IEnumerator MoveByTile(Vector3 nextPos, CharacterAnimationStateType animState, AnimationCurve curve)
    {
        SetAnimation(animState);
        float AnimLength = SpineAnim.GetAnimLenght(animState);
      //  Debug.Log(AnimLength + "  AnimLenght   " + AnimLength / CharInfo.MovementSpeed + " Actual duration" );
        float timer = 0;
        float speedTimer = 0;
        Vector3 offset = transform.position;
        isMoving = true;
        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            float newAdd = (Time.fixedDeltaTime / (AnimLength / CharInfo.MovementSpeed));
            timer += (Time.fixedDeltaTime / (AnimLength / CharInfo.MovementSpeed));
            speedTimer += newAdd * curve.Evaluate(timer + newAdd);
            transform.position = Vector3.Lerp(offset, nextPos, speedTimer);
        }
        isMoving = false;
        if (TileMovementCompleteEvent != null)
        {
            TileMovementCompleteEvent(this);
        }
        transform.position = nextPos;
        MoveCo = null;
    }

    #endregion

    #region Buff/Debuff

    //Used to Buff/Debuff the character
    public IEnumerator Buff_DebuffCoroutine(Buff_DebuffClass bdClass)
    {
        while (SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle)
        {
            yield return new WaitForEndOfFrame();
        }

        float timer = 0;
        float valueOverDuration = 0;
        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.Health:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.Armor:
                
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

                    ElementalWeaknessType newBuffDebuffValue = bdClass.ElementalResistence.ElementalWeakness +  (int)currentBuffDebuff.ElementalResistence.ElementalWeakness > ElementalWeaknessType.ExtremelyResistent ?
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
            case BuffDebuffStatsType.Regeneration:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharInfo.DamageStats.CurrentElemental = bdClass.ElementalPower;
                break;
        }

        SetAnimation(bdClass.AnimToFire);

        while (timer <= bdClass.Duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            if (bdClass.Stat == BuffDebuffStatsType.HealthOverTime)
            {
                valueOverDuration = (bdClass.Value / bdClass.Duration) / 50f;
            }

            timer += Time.fixedDeltaTime;
        }
        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.Regeneration:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharInfo.DamageStats.CurrentElemental = CharInfo.Elemental;
                break;
        }
    }


    
    private IEnumerator ElementalBuffDebuffCo(CurrentBuffsDebuffsClass newBuffDebuff)
    {
        float timer = 0;
        float newDuration = newBuffDebuff.Duration - Mathf.Abs((int)newBuffDebuff.ElementalResistence.ElementalWeakness);
        while (timer <= newDuration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            timer += Time.fixedDeltaTime;
        }

        for (int i = 0; i < Mathf.Abs((int)newBuffDebuff.ElementalResistence.ElementalWeakness); i++)
        {
            timer = 0;
            while (timer <= 1)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
                {
                    yield return new WaitForEndOfFrame();
                }

                timer += Time.fixedDeltaTime;
            }
        }
        BuffsDebuffs.Remove(newBuffDebuff);
    }

    #endregion

    #region Animation

    public void SetAnimation(CharacterAnimationStateType animState)
    {
        float AnimSpeed = 1;
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        if (SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1)
        {
            return;
        }
        
        if (animState != CharacterAnimationStateType.Atk && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk)
        {
            isAttackGoing = false;
        }

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
        else
        {
            AnimSpeed = CharInfo.BaseSpeed;
        }

        SpineAnim.SetAnim(animState, false);
        SpineAnim.SetAnimationSpeed(AnimSpeed);
    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);
        SpineAnim.CharOwner = this;
    }

    #endregion

    
    public void SetDamage(float damage, ElementalType elemental)
    {
        ElementalWeaknessType ElaboratedWeakness;
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
        }
        //Debug.Log(damage);
        CharInfo.Health -= damage;
        SetAnimation(CharacterAnimationStateType.GettingHit);
    }

    public ElementalWeaknessType GetElementalMultiplier(List<ElementalResistenceClass> armorElelmntals, ElementalType elementalToCheck)
    {
        int resVal = 0;

        foreach (ElementalResistenceClass elemental in armorElelmntals)
        {
            
            if(elemental.Elemental != elementalToCheck)
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
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

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


    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, Color selectionIndicatorColorSelected)
    {
        SelectionIndicatorSprite.color = isSelected ? selectionIndicatorColorSelected : SelectionIndicatorColorUnselected;
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

    public Buff_DebuffClass(float duration, float value, BuffDebuffStatsType stat, ElementalResistenceClass elementalResistence, ElementalType elementalPower, CharacterAnimationStateType animToFire)
    {
        Duration = duration;
        Value = value;
        Stat = stat;
        //AttackT = attackT;
        ElementalResistence = elementalResistence;
        ElementalPower = elementalPower;
        AnimToFire = animToFire;
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
public class AttackTypesInfoClass
{
    public List<BulletBehaviourInfoClass> BulletTrajectories = new List<BulletBehaviourInfoClass>();
    public CharacterClassType CharacterClass;
}

[System.Serializable]
public class BulletBehaviourInfoClass
{
    public Vector2Int BulletDistanceInTile;
    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
    public Vector2Int BulletGapStartingTile;
}