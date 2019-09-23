using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
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
    public bool isMoving = false;
    public CharacetrBaseInfoClass CharacterInfo;
    [SerializeField]
    private BoxCollider PlayerCharacterCollider;
    [SerializeField]
    private BoxCollider EnemyCollider;
    private IEnumerator MoveCo;
    public ControllerType PlayerController;
    [HideInInspector]
    public BattleTileScript CurrentBattleTile;
    private bool isEnemyOrPlayerController;
    private SpineAnimationManager SpineAnim;
    public CharacterAnimationStateType AnimationState;
    public List<CurrentBuffsDebuffsClass> BuffsDebuffs = new List<CurrentBuffsDebuffsClass>();
    public bool IsUsingAPortal = false;
    public Transform FiringPoint;
   
    public bool AllowMoreElementalOnWepon_ElementalResistence_Armor = false;
    private void Awake()
    {
        
    }

    private void Start()
    {
        if (CharacterInfo.ControllerT == ControllerType.Enemy)
        {
            isEnemyOrPlayerController = false;
            EnemyCollider.enabled = true;
        }
        else
        {
            isEnemyOrPlayerController = true;
            PlayerCharacterCollider.enabled = true;
        }
        StartCoroutine(AttackAction());
        
        //SetAnimation(CharacterAnimationStateType.Arriving);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCharOnDirection(InputDirection.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCharOnDirection(InputDirection.Down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCharOnDirection(InputDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCharOnDirection(InputDirection.Left);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireAttackParticles();
        }
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
            //Anim.SetInteger("State", 0);
            yield return new WaitForEndOfFrame();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForEndOfFrame();
            }
            //Anim.SetInteger("State", 1);
            float Delay = CharacterInfo.AttackSpeed;
            yield return new WaitForSecondsRealtime(Delay);
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
                    AnimState = CharacterAnimationStateType.DashRight;
                    break;
                case InputDirection.Left:
                    nextPos = new Vector2Int(Pos.x, Pos.y - 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = SpineAnim.LeftMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashLeft;
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
        StartCoroutine(SetMoveAnimation(animState));
        float AnimLength = SpineAnim.GetAnimLenght(animState);
        float timer = 0;
        float speedTimer = 0;
        Vector3 offset = transform.position;
        
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
        }
        isMoving = false;
        transform.position = nextPos;
        MoveCo = null;
    }

    public IEnumerator Buff_DebuffCoroutine(Buff_DebuffClass bdClass)
    {
        float timer = 0;
        float valueOverDuration = 0;

        while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause || isMoving)
        {
            yield return new WaitForEndOfFrame();
        }

        switch (bdClass.Stat)
        {
            case BuffDebuffStatsType.Health:
                CharacterInfo.Health += valueOverDuration;
                break;
            case BuffDebuffStatsType.Armor:
                CurrentBuffsDebuffsClass newBuffDebuff = new CurrentBuffsDebuffsClass();
                ElementalResistenceClass elementalsResistence;
                CurrentBuffsDebuffsClass currentBuffDebuff = BuffsDebuffs.Where(r => r.ElementalResistence.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
                if (currentBuffDebuff != null)
                {
                    StopCoroutine(currentBuffDebuff.BuffDebuffCo);
                    BuffsDebuffs.Remove(currentBuffDebuff);
                    newBuffDebuff.ElementalResistence = bdClass.ElementalResistence;
                    newBuffDebuff.Duration = 100 + bdClass.Duration;
                    newBuffDebuff.BuffDebuffCo = ElementalBuffDebuffCo(newBuffDebuff);
                    elementalsResistence = CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
                    if (elementalsResistence != null)
                    {
                        if (newBuffDebuff.ElementalResistence.ElementalWeakness > ElementalWeaknessType.Neutral)
                        {
                            newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness + (int)elementalsResistence.ElementalWeakness + (int)currentBuffDebuff.TotalBuffDebuff > 3 ?
                            3 - (int)elementalsResistence.ElementalWeakness : (int)currentBuffDebuff.TotalBuffDebuff + (int)newBuffDebuff.ElementalResistence.ElementalWeakness ;
                        }
                        else if (newBuffDebuff.ElementalResistence.ElementalWeakness < ElementalWeaknessType.Neutral)
                        {

                            newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness + (int)elementalsResistence.ElementalWeakness + (int)currentBuffDebuff.TotalBuffDebuff < -3 ?
                             -3 - (int)elementalsResistence.ElementalWeakness : (int)currentBuffDebuff.TotalBuffDebuff + (int)newBuffDebuff.ElementalResistence.ElementalWeakness;
                        }
                    }
                    else
                    {
                        newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness;
                    }
                    BuffsDebuffs.Add(newBuffDebuff);
                    StartCoroutine(newBuffDebuff.BuffDebuffCo);
                }
                else
                {
                    newBuffDebuff.ElementalResistence = bdClass.ElementalResistence;
                    newBuffDebuff.Duration = 100 + bdClass.Duration;//TODO
                    newBuffDebuff.BuffDebuffCo = ElementalBuffDebuffCo(newBuffDebuff);
                    elementalsResistence = CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).FirstOrDefault();
                    if (elementalsResistence != null)
                    {                        
                        if (newBuffDebuff.ElementalResistence.ElementalWeakness > ElementalWeaknessType.Neutral)
                        {
                            newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness + (int)elementalsResistence.ElementalWeakness > 3 ?
                            3 - (int)elementalsResistence.ElementalWeakness : (int)newBuffDebuff.ElementalResistence.ElementalWeakness;
                        }
                        else if (newBuffDebuff.ElementalResistence.ElementalWeakness < ElementalWeaknessType.Neutral)
                        {

                            newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness + (int)elementalsResistence.ElementalWeakness < -3 ?
                             -3 - (int)elementalsResistence.ElementalWeakness : (int)newBuffDebuff.ElementalResistence.ElementalWeakness;
                        }                    }
                    else
                    {
                        newBuffDebuff.TotalBuffDebuff = (int)newBuffDebuff.ElementalResistence.ElementalWeakness;
                    }
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
            SpineAnim = GetComponentInChildren<SpineAnimationManager>();
            SpineAnim.CharOwner = this;
        }
        SpineAnim.SetAnim(CharacterAnimationStateType.Idle, false);
        yield return new WaitForFixedUpdate();
        SpineAnim.SetAnim(animState, animState == CharacterAnimationStateType.Idle ? true : false);
    }

    public IEnumerator SetMoveAnimation(CharacterAnimationStateType animState)
    {
        if (SpineAnim == null)
        {
            SpineAnim = GetComponentInChildren<SpineAnimationManager>();
            SpineAnim.CharOwner = this;
        }
        isMoving = true;
        SpineAnim.SetAnim(animState, false);
        yield return new WaitForSecondsRealtime((SpineAnim.GetAnimLenght(animState) / 100) * 90);
        isMoving = false;
    }

    public void SetAnimation(CharacterAnimationStateType animState)
    {
        if (SpineAnim == null)
        {
            SpineAnim = GetComponentInChildren<SpineAnimationManager>();
            SpineAnim.CharOwner = this;
        }
        SpineAnim.SetAnim(animState, animState == CharacterAnimationStateType.Idle ? true : false);
    }

    public void SetMixAnimation(CharacterAnimationStateType animState)
    {
        SpineAnim.SetMixAnim(animState, 0.1f,false);
    }

    public void SetDamage(float damage, List<ElementalType> elelmntals)
    {

    }

    public ElementalWeaknessType GetElementalMultiplier(List<ElementalType> armorElelmntals, List<ElementalType> weaponElementals)
    {
        int resVal = 0;

        foreach (ElementalType elemental in armorElelmntals)
        {
            int res = (int)elemental - (int)weaponElementals[0];

            if(res < 0)
            {
                res *= -1;
            }

            resVal += res;
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

    public void FireAttackParticles()
    {
        ParticleManagerScript.Instance.FireParticlesInPosition(CharacterInfo.AttackParticle, ParticleTypes.Cast, FiringPoint);
    }


}




[System.Serializable]
public class AttackClass
{
    public AttackType AttackT;
    public AttackParticleTypes ParticleType;
    public List<ElementalType> Elemental = new List<ElementalType>();


    public int AttackPower;


    public float BulletSpeed;
    public AnimationCurve Height;


    public int AttackAngle;
    public int NumberOfBullets;
    public AttackClass()
    {
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
    public int TotalBuffDebuff = 0;
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

