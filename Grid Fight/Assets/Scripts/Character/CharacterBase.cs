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
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            MoveChar(InputDirection.Up);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            MoveChar(InputDirection.Down);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            MoveChar(InputDirection.Right);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            MoveChar(InputDirection.Left);
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

    public void MoveChar(InputDirection nextDir)
    {
        if (CharacterInfo.Health > 0)
        {
            BattleTileScript prevBattleTile = CurrentBattleTile;
            int AnimState = 0;
            Vector2Int nextPos;
            //Debug.Log(nextDir);
            switch (nextDir)
            {
                case InputDirection.Up:
                    nextPos = new Vector2Int(Pos.x - 1, Pos.y);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    AnimState = 2;
                    break;
                case InputDirection.Down:
                    nextPos = new Vector2Int(Pos.x + 1, Pos.y);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    AnimState = 3;
                    break;
                case InputDirection.Right:
                    nextPos = new Vector2Int(Pos.x, Pos.y + 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    AnimState = 2;
                    break;
                case InputDirection.Left:
                    nextPos = new Vector2Int(Pos.x, Pos.y - 1);
                    if (GridManagerScript.Instance.IsBattleTileInControllerArea(nextPos, isEnemyOrPlayerController))
                    {
                        CurrentBattleTile = GridManagerScript.Instance.GetBattleTile(nextPos, isEnemyOrPlayerController);
                    }
                    AnimState = 3;
                    break;
            }

            if (CurrentBattleTile.BattleTileState == BattleTileStateType.Empty)
            {
                isMoving = true;
                GridManagerScript.Instance.SetBattleTileState(Pos, BattleTileStateType.Empty);
                Pos = CurrentBattleTile.Pos;

                if (MoveCo != null)
                {
                    StopCoroutine(MoveCo);
                }
                MoveCo = Move(CurrentBattleTile.transform.position, AnimState);
                StartCoroutine(MoveCo);
            }


            if (prevBattleTile != CurrentBattleTile)
            {

                BattleManagerScript.Instance.OccupiedBattleTiles.Remove(prevBattleTile);
                BattleManagerScript.Instance.OccupiedBattleTiles.Add(CurrentBattleTile);
            }
        }

    }

    private IEnumerator Move(Vector3 nextPos, int animState)
    {
        //Anim.SetInteger("State", 0);
        yield return new WaitForEndOfFrame();
        //Anim.SetInteger("State", animState);
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
            timer += (Time.fixedDeltaTime + (Time.fixedDeltaTime * 0.333f)) * 2;//TODO Movement Speed
            transform.position = Vector3.Lerp(offset, nextPos, timer);
        }
        isMoving = false;
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


        while (timer <= bdClass.Duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("Enter");

            if (bdClass.Stat == BuffDebuffStatsType.HealthOverTime)
            {
                valueOverDuration = (bdClass.Value / bdClass.Duration) / 50f;
            }

            timer += Time.fixedDeltaTime;
        }
        Debug.Log("Enter2");
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
    public BuffDebuffStatsType Stat;
    public AttackType AttackT;
    public ElementalResistenceClass ElementalResistence;
    public ElementalType ElementalPower;

    public Buff_DebuffClass(float duration, float value, BuffDebuffStatsType stat, ElementalResistenceClass elementalResistence, ElementalType elementalPower)
    {
        Duration = duration;
        Value = value;
        Stat = stat;
        //AttackT = attackT;
        ElementalResistence = elementalResistence;
        ElementalPower = elementalPower;
    }

    public Buff_DebuffClass()
    {

    }
}