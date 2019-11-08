using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{


    public delegate void TileMovementComplete(CharacterBase movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;
    public List<Vector2Int> Pos
    {
        get
        {
            return _Pos;
        }
        set
        {
            _Pos = value;
        }
    }
    public List<Vector2Int> _Pos = new List<Vector2Int>();

    public Vector2Int PhysicalPosOnTile
    {
        get
        {
            return _PhysicalPosOnTile;
        }
        set
        {
            _PhysicalPosOnTile = value;
        }
    }
    public Vector2Int _PhysicalPosOnTile;

    public CharacterInfoScript CharInfo
    {
        get
        {
            if (_CharInfo == null)
            {
                _CharInfo = this.GetComponentInChildren<CharacterInfoScript>(true);
            }
            return _CharInfo;
        }
    }
    public CharacterInfoScript _CharInfo;


    public Vector2Int TestAttackPosition;
    public SideType Side;
    public bool isMoving = false;
    public bool isSpecialLoading = false;
    public CharacterBaseInfoClass CharacterInfo;
    private IEnumerator MoveCo;
    public ControllerType PlayerController;
    [HideInInspector]
    public List<BattleTileScript> CurrentBattleTiles = new List<BattleTileScript>();
    private SpineAnimationManager SpineAnim;
    public CharacterAnimationStateType AnimationState;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool IsUsingAPortal = false;
    public bool IsOnField = false;
    public bool AllowMoreElementalOnWepon_ElementalResistence_Armor = false;
    private FacingType facing;
    public bool shoot = true;
    public CharacterLevelType NextAttackLevel = CharacterLevelType.Novice;
    public Transform SelectionIndicator;

    #region Unity Life Cycles
    private void Start()
    {
        StartCoroutine(AttackAction());
    }

    private void Update()
    {
        if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle)
        {
            CharacterInfo.Stamina = (CharacterInfo.Stamina + CharacterInfo.StaminaRegeneration / 60) > CharacterInfo.StaminaBase ? CharacterInfo.StaminaBase : (CharacterInfo.Stamina + CharacterInfo.StaminaRegeneration / 60);   
        }
    }
    #endregion

    #region Setup Character
    public void SetupCharacterSide()
    {
        SpineAnimatorsetup();
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                if (PlayerController == ControllerType.Enemy)
                {
                    EnemyControllerSettings();
                }
                else
                {
                    PlayerControllerSettings();
                }
                break;
            case MatchType.PvP:
                if (PlayerController == ControllerType.Player2)
                {
                    EnemyControllerSettings();
                }
                else
                {
                    PlayerControllerSettings();
                }
                break;
            case MatchType.PPvE:
                if (PlayerController == ControllerType.Enemy)
                {
                    EnemyControllerSettings();
                }
                else
                {
                    PlayerControllerSettings();
                }
                break;
            case MatchType.PPvPP:
                if (PlayerController == ControllerType.Player3 || PlayerController == ControllerType.Player4)
                {
                    EnemyControllerSettings();
                }
                else
                {
                    PlayerControllerSettings();
                }
                break;
        }
        CharacterInfo.playerController = PlayerController;
        int layer = Side == SideType.LeftSide ? 9 : 10;
        SpineAnim.gameObject.layer = layer;
    }

    private void EnemyControllerSettings()
    {
        facing = FacingType.Left;
        Side = SideType.RightSide;
        gameObject.tag = Side.ToString();
        SpineAnim.gameObject.tag = Side.ToString();
    }

    private void PlayerControllerSettings()
    {
        facing = FacingType.Right;
        transform.Rotate(new Vector3(0, 180, 0));
        Side = SideType.LeftSide;
        gameObject.tag = Side.ToString();
        SpineAnim.gameObject.tag = Side.ToString();
    }

    public void SetupEquipment()
    {

    }

    #endregion

    #region Attack
    public IEnumerator AttackAction()
    {
        Debug.Log("-----Anim");
        while (true)
        {
            while (!IsOnField)
            {
                yield return new WaitForEndOfFrame();
            }

            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || isMoving || isSpecialLoading)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("Anim");
            CastAttackParticles();
            SetAnimation(CharacterAnimationStateType.Atk);

            float timer = 0;
            while (timer <= CharacterInfo.AttackSpeed)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || isMoving || isSpecialLoading)
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

    public IEnumerator LoadSpecialAttack()
    {
        if(CharacterInfo.Stamina - CharacterInfo.StaminaCostSpecial1 >= 0)
        {
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
            CharacterInfo.Stamina -= CharacterInfo.StaminaCostSpecial1;
            SpecialAttack(CharacterLevelType.Defiant);
        }
    }

    public void SpecialAttack(CharacterLevelType attackLevel)
    {
        NextAttackLevel = attackLevel;
        SetAnimation(CharacterAnimationStateType.Atk);
    }



    public void CastAttackParticles()
    {
        ParticleManagerScript.Instance.FireParticlesInPosition(CharacterInfo.AttackParticle, ParticleTypes.Cast, SpineAnim.FiringPoint.position, Side);
    }

    public void CreateSingleBullet(Vector2Int bulletDistanceInTile)
    {
        if (!shoot)
        {
            return;
        }
      
        GameObject bullet = Instantiate(BattleManagerScript.Instance.BaseBullet, SpineAnim.FiringPoint.position, Quaternion.identity);
        BulletScript bs = bullet.GetComponent<BulletScript>();
        LayerParticleSelection lps = bullet.GetComponent<LayerParticleSelection>();
        if(lps != null)
        {
            lps.Shot = NextAttackLevel;
        }

        bs.Elemental = CharacterInfo.ElementalsPower.First();
        bs.Side = Side;
        bs.CharInfo = CharInfo;
        bs.gameObject.SetActive(true);
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharacterInfo.AttackParticle, ParticleTypes.Attack, bullet.transform, Side);
        if ((PhysicalPosOnTile.x + bulletDistanceInTile.x > 5) || (PhysicalPosOnTile.x + bulletDistanceInTile.x < 0))
        {
            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(PhysicalPosOnTile.x - bulletDistanceInTile.x, Side == SideType.LeftSide ? PhysicalPosOnTile.y + bulletDistanceInTile.y : PhysicalPosOnTile.y - bulletDistanceInTile.y));
            float h = transform.position.y - bts.transform.position.y;
            bs.DestinationWorld = new Vector3(bts.transform.position.x, (transform.position.y + h), bts.transform.position.z);
            StartCoroutine(bs.MoveStraight());
            return;
        }

        if (Side == SideType.LeftSide)
        {
            bs.DestinationTile = new Vector2Int(PhysicalPosOnTile.x + bulletDistanceInTile.x, PhysicalPosOnTile.y + bulletDistanceInTile.y > 11 ? 11 : PhysicalPosOnTile.y + bulletDistanceInTile.y);
        }
        else
        {
            bs.DestinationTile = new Vector2Int(PhysicalPosOnTile.x + bulletDistanceInTile.x, PhysicalPosOnTile.y - bulletDistanceInTile.y < 0 ? 0 : PhysicalPosOnTile.y - bulletDistanceInTile.y);
        }
        StartCoroutine(bs.MoveToTile());
    }

    public void CreateMachingunBullets()
    {
        Vector3 offsetRotation = transform.eulerAngles;
        transform.eulerAngles -= new Vector3(0, 0, CharInfo.MultiBulletAttackAngle / 2);
        for (int i = 0; i < CharInfo.MultiBulletAttackNumberOfBullets; i++)
        {
           // transform.eulerAngles += new Vector3(0, 0, CharInfo.MultiBulletAttackAngle / (CharInfo.MultiBulletAttackNumberOfBullets - 1));
            CreateSingleBullet(CharInfo.BulletDistanceInTile[i]);
        }
        transform.eulerAngles = offsetRotation;
    }

    #endregion

    #region Move

    public void MoveCharOnDirection(InputDirection nextDir)
    {
        if (CharacterInfo.Health > 0 && !isMoving)
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
                    nextPos = CalculateNextPos(Pos, dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, Side))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, Side);
                    }
                    curve = SpineAnim.UpMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashUp;
                    break;
                case InputDirection.Down:
                    dir = new Vector2Int(1, 0);
                    nextPos = CalculateNextPos(Pos, dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, Side))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, Side);
                    }
                    curve = SpineAnim.DownMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashDown;
                    break;
                case InputDirection.Right:
                    dir = new Vector2Int(0, 1);
                    nextPos = CalculateNextPos(Pos, dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, Side))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, Side);
                    }
                    curve = SpineAnim.RightMovementSpeed;
                    AnimState = facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                    break;
                case InputDirection.Left:
                    dir = new Vector2Int(0, -1);
                    nextPos = CalculateNextPos(Pos, dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, Side))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, Side);
                    }
                    curve = SpineAnim.LeftMovementSpeed;
                    AnimState = facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                    break;
            }

            if (CurrentBattleTilesToCheck.Count > 0 && CurrentBattleTilesToCheck.Where(r=>!Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                CurrentBattleTilesToCheck.Where(r => !Pos.Contains(r.Pos)).ToList().Count)
            {
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                }
                PhysicalPosOnTile += dir;
                CurrentBattleTiles = CurrentBattleTilesToCheck;
                Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in CurrentBattleTilesToCheck)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    Pos.Add(item.Pos);
                }
                
                if (MoveCo != null)
                {
                    StopCoroutine(MoveCo);
                }
                MoveCo = MoveByTile(CurrentBattleTiles.Where(r=> r.Pos == PhysicalPosOnTile).First().transform.position, AnimState, curve);
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

    public List<Vector2Int> CalculateNextPos(List<Vector2Int> currentPos, Vector2Int direction)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        currentPos.ForEach(r => res.Add(r + direction));
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
        SetAnimation(CharacterAnimationStateType.Idle);
        yield return new WaitForEndOfFrame();
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

    private IEnumerator MoveByTile(Vector3 nextPos, CharacterAnimationStateType animState, AnimationCurve curve)
    {
        SetAnimation(animState);
        SpineAnim.SetAnimationSpeed(CharacterInfo.MovementSpeed);
        float AnimLength = SpineAnim.GetAnimLenght(animState);
        float timer = 0;
        float speedTimer = 0;
        bool IsMovementComplete = false;
        Vector3 offset = transform.position;
        isMoving = true;
        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            float newAdd = (Time.fixedDeltaTime / (AnimLength / CharacterInfo.MovementSpeed));
            timer += (Time.fixedDeltaTime / (AnimLength / CharacterInfo.MovementSpeed));
            speedTimer += newAdd * curve.Evaluate(timer + newAdd);
            transform.position = Vector3.Lerp(offset, nextPos, speedTimer);

            if(timer > 0.9f && !IsMovementComplete)
            {
                isMoving = false;
                IsMovementComplete = true;
                if(TileMovementCompleteEvent != null)
                {
                    TileMovementCompleteEvent(this);
                }
            }
        }
        SpineAnim.SetAnimationSpeed(1);
        transform.position = nextPos;
        MoveCo = null;
    }

    #endregion

    #region Buff/Debuff
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
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.Armor:
                
                CurrentBuffsDebuffsClass currentBuffDebuff = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
                ElementalWeaknessType BaseWeakness = GetElementalMultiplier(CharacterInfo.ElementalsResistence, bdClass.ElementalResistence.Elemental);
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
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharacterInfo.ElementalsPower.Add(bdClass.ElementalPower);
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
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.MovementSpeed:
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.Stamina:
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.StaminaRegeneration:
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackSpeed:
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.BulletSpeed:
                CharacterInfo.Health -= valueOverDuration;
                break;
            case BuffDebuffStatsType.AttackType:
                break;
            case BuffDebuffStatsType.ElementalPower:
                CharacterInfo.ElementalsPower.Remove(bdClass.ElementalPower);
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
           /* BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == newBuffDebuff.ElementalResistence.Elemental).First()
                .ElementalResistence.ElementalWeakness += newBuffDebuff.ElementalResistence.ElementalWeakness > ElementalWeaknessType.Neutral ? 1 : -1;*/
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
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
       
        SetMixAnimation(animState);

    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>(true);
        SpineAnim.CharOwner = this;
    }

    public void SetMixAnimation(CharacterAnimationStateType animState)
    {
        SpineAnim.SetAnim(animState, animState == CharacterAnimationStateType.Idle ? true : false);
    }

    #endregion
    public void SetDamage(float damage, ElementalType elemental)
    {
        ElementalWeaknessType ElaboratedWeakness;
        CurrentBuffsDebuffsClass buffDebuffWeakness = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == elemental).FirstOrDefault();

        ElementalWeaknessType BaseWeakness = GetElementalMultiplier(CharacterInfo.ElementalsResistence, elemental);
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
        CharacterInfo.Health -= damage;
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

        return (ElementalWeaknessType)(resVal / armorElelmntals.Count);
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

        SpineAnim.SetAnimationSpeed(1);

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

