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
    public CharacterAnimationHub CharacterAnimHub;
    private IEnumerator MoveCo;
    public ControllerType PlayerController;
    [HideInInspector]
    public BattleTileScript CurrentBattleTile;
    private bool isEnemyOrPlayerController;
    private SpineAnimationManager SpineAnim;
    public CharacterAnimationStateType AnimationState;
    
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
                    curve = CharacterAnimHub.UpMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashUp;
                    break;
                case InputDirection.Down:
                    nextPos = new Vector2Int(Pos.x + 1, Pos.y);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = CharacterAnimHub.DownMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashDown;
                    break;
                case InputDirection.Right:
                    nextPos = new Vector2Int(Pos.x, Pos.y + 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = CharacterAnimHub.RightMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashRight;
                    break;
                case InputDirection.Left:
                    nextPos = new Vector2Int(Pos.x, Pos.y - 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    curve = CharacterAnimHub.LeftMovementSpeed;
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
                List<ElementalResistenceClass> ElementalsResistence = CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).ToList();
                if (ElementalsResistence.Count > 0)
                {
                    //CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).First().ElementalWeakness += (int)bdClass.ElementalResistence.ElementalWeakness;
                    ElementalsResistence.First().ElementalWeakness += (int)bdClass.ElementalResistence.ElementalWeakness;
                    if (ElementalsResistence.First().ElementalWeakness > ElementalWeaknessType.ExtremelyResistent)
                    {
                        ElementalsResistence.First().ElementalWeakness = ElementalWeaknessType.ExtremelyResistent;
                    }
                    else if (ElementalsResistence.First().ElementalWeakness < ElementalWeaknessType.ExtremelyWeak)
                    {
                        ElementalsResistence.First().ElementalWeakness = ElementalWeaknessType.ExtremelyWeak;
                    }
                }
                else
                {
                    CharacterInfo.ElementalsResistence.Add(bdClass.ElementalResistence);
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
            case BuffDebuffStatsType.Armor:
                List<ElementalResistenceClass> ElementalsResistence = CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).ToList();
                CharacterInfo.ElementalsResistence.Where(r => r.Elemental == bdClass.ElementalResistence.Elemental).First().ElementalWeakness -= (int)bdClass.ElementalResistence.ElementalWeakness;
                if (ElementalsResistence.First().ElementalWeakness > ElementalWeaknessType.ExtremelyResistent)
                {
                    ElementalsResistence.First().ElementalWeakness = ElementalWeaknessType.ExtremelyResistent;
                }
                else if (ElementalsResistence.First().ElementalWeakness < ElementalWeaknessType.ExtremelyWeak)
                {
                    ElementalsResistence.First().ElementalWeakness = ElementalWeaknessType.ExtremelyWeak;
                }
                break;
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




}




[System.Serializable]
public class AttackClass
{
    public AttackType AttackT;
    public ParticleTypes ParticleType;
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