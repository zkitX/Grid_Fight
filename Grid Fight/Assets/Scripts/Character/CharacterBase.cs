using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{


    public delegate void TileMovementComplete(CharacterBase movingChar);
    public event TileMovementComplete TileMovementCompleteEvent;
    public Vector2Int Pos
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
    public Vector2Int _Pos;


    public Vector2Int TestAttackPosition;

    public SideType Side;
    public BulletInfoScript BulletInfo;
    public bool isMoving = false;
    public CharacetrBaseInfoClass CharacterInfo;
    private IEnumerator MoveCo;
    public ControllerType PlayerController;
    [HideInInspector]
    public BattleTileScript CurrentBattleTile;
    private bool isEnemyOrPlayerController;
    private SpineAnimationManager SpineAnim;
    public CharacterAnimationStateType AnimationState;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool IsUsingAPortal = false;
    public bool AllowMoreElementalOnWepon_ElementalResistence_Armor = false;
    private FacingType facing;

    public float BulletSpeed = 1;

    public bool shoot = true;

    private void Start()
    {
        StartCoroutine(AttackAction());
    }

    private void Update()
    {
    }

    public void SetupCharacterSide()
    {

        switch (BattleInfoManagerScript.Instance.MatchInfoType)
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
        int layer = Side == SideType.PlayerCharacter ? 9 : 10;
        SpineAnim.gameObject.layer = layer;
    }


    private void EnemyControllerSettings()
    {
        isEnemyOrPlayerController = false;
        gameObject.tag = "EnemyCharacter";
        SpineAnim.gameObject.tag = "EnemyCharacter";
        facing = FacingType.Left;
        Side = SideType.EnemyCharacter;
    }
    private void PlayerControllerSettings()
    {
        isEnemyOrPlayerController = true;
        gameObject.tag = "PlayerCharacter";
        SpineAnim.gameObject.tag = "PlayerCharacter";
        facing = FacingType.Right;
        transform.Rotate(new Vector3(0, 180, 0));
        Side = SideType.PlayerCharacter;
    }


    public void SetupEquipment()
    {

    }

    public IEnumerator AttackAction()
    {
        while (true)
        {
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || isMoving)
            {
                yield return new WaitForEndOfFrame();
            }
            SetAnimation(CharacterAnimationStateType.Atk);
            
            float timer = 0;
            while (timer <= CharacterInfo.AttackSpeed)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
                {
                    yield return new WaitForEndOfFrame();
                }

                timer += Time.fixedDeltaTime;
            }
        }
    }

    public void MoveCharOnDirection(InputDirection nextDir)
    {
        if (CharacterInfo.Health > 0 && !isMoving)
        {
            BattleTileScript prevBattleTile = CurrentBattleTile;
            CharacterAnimationStateType AnimState = CharacterAnimationStateType.Idle;
            AnimationCurve curve = new AnimationCurve();
            Vector2Int nextPos;
            switch (nextDir)
            {
                case InputDirection.Up:
                    nextPos = new Vector2Int(Pos.x - 1, Pos.y);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = SpineAnim.UpMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashUp;
                    break;
                case InputDirection.Down:
                    nextPos = new Vector2Int(Pos.x + 1, Pos.y);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = SpineAnim.DownMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashDown;
                    break;
                case InputDirection.Right:
                    nextPos = new Vector2Int(Pos.x, Pos.y + 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = SpineAnim.RightMovementSpeed;
                    AnimState = facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                    break;
                case InputDirection.Left:
                    nextPos = new Vector2Int(Pos.x, Pos.y - 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = SpineAnim.LeftMovementSpeed;
                    AnimState = facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                    break;
            }

            if (CurrentBattleTile.BattleTileState == BattleTileStateType.Empty)
            {
                GridManagerScript.Instance.SetBattleTileState(Pos, BattleTileStateType.Empty);
                Pos = CurrentBattleTile.Pos;
                GridManagerScript.Instance.SetBattleTileState(Pos, BattleTileStateType.Occupied);
                if (MoveCo != null)
                {
                    StopCoroutine(MoveCo);
                }
                MoveCo = MoveByTile(CurrentBattleTile.transform.position, AnimState, curve);
                StartCoroutine(MoveCo);
            }


            if (prevBattleTile != CurrentBattleTile)
            {

                BattleManagerScript.Instance.OccupiedBattleTiles.Remove(prevBattleTile);
                BattleManagerScript.Instance.OccupiedBattleTiles.Add(CurrentBattleTile);
            }
        }

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
            float newAdd = (Time.fixedDeltaTime / AnimLength);
            timer += (Time.fixedDeltaTime / AnimLength);
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
        transform.position = nextPos;
        MoveCo = null;
    }

    public IEnumerator Buff_DebuffCoroutine(Buff_DebuffClass bdClass)
    {
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

        SetMixAnimation(bdClass.AnimToFire);

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

    public IEnumerator SetAnimationWithFrameDelay(CharacterAnimationStateType animState)
    {
        if(SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
        SpineAnim.SetAnim(CharacterAnimationStateType.Idle, false);
        yield return new WaitForFixedUpdate();
        SpineAnim.SetAnim(animState, animState == CharacterAnimationStateType.Idle ? true : false);
    }

   

    public void SetAnimation(CharacterAnimationStateType animState)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
        SpineAnim.SetAnim(animState, animState == CharacterAnimationStateType.Idle ? true : false);
    }

    public void SpineAnimatorsetup()
    {
        SpineAnim = GetComponentInChildren<SpineAnimationManager>();
        SpineAnim.CharOwner = this;
       
    }

    public void SetMixAnimation(CharacterAnimationStateType animState)
    {
        SpineAnim.SetMixAnim(animState, 0.1f,false);
    }

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

    public void CastAttackParticles()
    {
        ParticleManagerScript.Instance.FireParticlesInPosition(CharacterInfo.AttackParticle, ParticleTypes.Cast, SpineAnim.FiringPoint.position);
    }

    public void CreateBullet()
    {
        if(!shoot)
        {
            return;
        }
        if (BulletInfo == null)
        {
            BulletInfo = this.GetComponentInChildren<BulletInfoScript>();
        }
        GameObject bullet = Instantiate(BattleManagerScript.Instance.BaseBullet, SpineAnim.FiringPoint.position, Quaternion.identity);
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.AType = BulletInfo.AttackT;
        bs.Height = BulletInfo.TrajectoryHeightUp;
        if(Side == SideType.PlayerCharacter)
        {
            // bs.Destination = TestAttackPosition;
            bs.Destination = new Vector2Int(Pos.x, 11);
        }
        else
        {
            bs.Destination = new Vector2Int(Pos.x, 0);
        }
        bs.Elemental = CharacterInfo.ElementalsPower.First();
        bs.Speed = BulletSpeed;
        bs.Damage = 10;
        bs.Side = Side;
        bs.gameObject.SetActive(true);
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharacterInfo.AttackParticle, ParticleTypes.Attack, bullet.transform);
        StartCoroutine(bs.Move());
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
    public AttackType AttackT;
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

