﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public delegate void CurrentCharIsDead(CharacterNameType cName, List<ControllerType> playerController, SideType side);
    public event CurrentCharIsDead CurrentCharIsDeadEvent;

    public delegate void TileMovementComplete(BaseCharacter movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;

    public delegate void DamageReceived(float damage);
    public event DamageReceived DamageReceivedEvent;

    public delegate void HealReceived(float heal);
    public event HealReceived HealReceivedEvent;

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
    public GameObject BaseBullet;
    public CharacterLevelType NextAttackLevel = CharacterLevelType.Novice;
    public bool isSpecialLoading = false;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool VFXTestMode = false;
    public bool isAttackStarted = false;
    public bool isAttackGoing = false;
    public bool isAttackCompletetd = false;
    public UnitManagementScript UMS;
    public BoxCollider CharBoxCollider;

    

    protected virtual void Start()
    {
        
        BaseBullet = (GameObject)Resources.Load("Prefabs/Bullet/Bullet");
    }

    protected virtual void Update()
    {
        
    }




    #region Setup Character
    public virtual void SetupCharacterSide()
    {
        if(UMS.PlayerController.Contains(ControllerType.Enemy))
        {
            UMS.SelectionIndicator.parent.gameObject.SetActive(false);
        }
        else
        {
            UMS.SelectionIndicator.parent.gameObject.SetActive(true);
        }
        CharBoxCollider = GetComponentInChildren<BoxCollider>(true);
        SpineAnimatorsetup();
        UMS.SetupCharacterSide();
        int layer = UMS.Side == SideType.LeftSide ? 9 : 10;
        SpineAnim.gameObject.layer = layer;
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
            SetCharDead();
        }
    }

    public virtual void SetAttackReady(bool value)
    {
        CharBoxCollider.enabled = value;
        CanAttack = value;
        IsOnField = value;
    }

    public virtual void SetCharDead()
    {
        for (int i = 0; i < UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
            UMS.Pos[i] = Vector2Int.zero;
        }
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
    #endregion
    #region Attack


    public void StartAttakCo()
    {
        UIBattleFieldManager.Instance.SetUIBattleField(this);
        StartCoroutine(AttackAction());
    }

    //Basic attack Action that will start the attack anim every x seconds
    public virtual IEnumerator AttackAction()
    {
        while (true)
        {
            while (!CanAttack && !VFXTestMode)
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
                CharInfo.Stamina -= CharInfo.StaminaStats.Stamina_Cost_S_Atk01;
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
       
        
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharInfo.ParticleID, ParticleTypes.Attack, bullet.transform, UMS.Side,
            CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script ? true : false);


        LayerParticleSelection lps = bs.PS.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            bs.attackLevel = NextAttackLevel;
            lps.Shot = NextAttackLevel;
            lps.SelectShotLevel();
        }

        if ((UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x > 5) || (UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x < 0))
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
        bs.gameObject.SetActive(true);
        bs.StartMoveToTile();
    }


    public void CreateAttack()
    {
        if(UMS.Facing == FacingType.Right)
        {
            foreach (BulletBehaviourInfoClass item in CharInfo.CurrentParticlesAttackTypeInfo.BulletTrajectories)
            {
                CreateBullet(item);
            }
        }
        else
        {
            GridManagerScript.Instance.StartOnBattleFieldAttackCo(CharInfo, CharInfo.CurrentOnBattleFieldAttackTypeInfo, UMS.CurrentTilePos, CharInfo.ParticleID);
        }
    }

  
    #endregion
    #region Move
    public virtual void MoveCharOnDirection(InputDirection nextDir)
    {
        if (CharInfo.Health > 0 && !isMoving && CanAttack && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript> CurrentBattleTilesToCheck = new List<BattleTileScript>();
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

            if (CurrentBattleTilesToCheck.Count > 0 && CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
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
                MoveCo = MoveByTile(CurrentBattleTiles.Where(r => r.Pos == UMS.CurrentTilePos).First().transform.position, AnimState, curve);
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


    //Move the character on the determinated Tile position
    protected virtual IEnumerator MoveByTile(Vector3 nextPos, CharacterAnimationStateType animState, AnimationCurve curve)
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


    public void Buff_DebuffCo(Buff_DebuffClass bdClass)
    {
        StartCoroutine(Buff_DebuffCoroutine(bdClass));
    }

    //Used to Buff/Debuff the character
    public IEnumerator Buff_DebuffCoroutine(Buff_DebuffClass bdClass)
    {
        while (SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle)
        {
            yield return new WaitForEndOfFrame();
        }

        SetAnimation(CharacterAnimationStateType.Buff);
        GameObject ps = null;
        if (bdClass.ParticlesToFire != null)
        {
            ps = Instantiate(bdClass.ParticlesToFire, transform);
            ps.transform.localPosition = Vector3.zero;
        }
        
        float timer = 0;
        float valueOverDuration = bdClass.Value;
        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.Health:
                if(valueOverDuration < 0)
                {
                    DamageReceivedEvent(valueOverDuration);
                }
                else
                {
                    HealReceivedEvent(valueOverDuration);
                }
                CharInfo.Health += valueOverDuration;
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
                CharInfo.DamageStats.CurrentDamage += valueOverDuration;
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
                CharInfo.DamageStats.CurrentDamage -= valueOverDuration;
                break;
        }

        Destroy(ps);
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

    public void ArrivingEvent()
    {
        SFXmanager.Instance.PlayOnce(SFXmanager.Instance.ArrivingImpact);
        UMS.ArrivingParticle.SetActive(true);
    }



    public virtual void SetAnimation(CharacterAnimationStateType animState)
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

        SpineAnim.SetAnim(animState);
        SpineAnim.SetAnimationSpeed(AnimSpeed);
    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);
        SpineAnim.CharOwner = this;
    }

    #endregion


    public virtual void SetDamage(float damage, ElementalType elemental)
    {
        if(!CanAttack)
        {
            return;
        }
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

    /*    switch (ElaboratedWeakness)
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
        DamageReceivedEvent(damage);
        SetAnimation(CharacterAnimationStateType.GettingHit);
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
    public GameObject ParticlesToFire;

    public Buff_DebuffClass(float duration, float value, BuffDebuffStatsType stat
        , ElementalResistenceClass elementalResistence, ElementalType elementalPower
        , CharacterAnimationStateType animToFire, GameObject particlesToFire = null)
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

