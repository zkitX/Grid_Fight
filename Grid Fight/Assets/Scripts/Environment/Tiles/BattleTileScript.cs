using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the component that take care of all the tile behaviours in the game
/// </summary>
public class BattleTileScript : MonoBehaviour
{

    public BattleTileStateType BattleTileState
    {
        get
        {
            return _BattleTileState;
        }
        set
        {
            //Debug.Log(Pos + "   " + value);
            _BattleTileState = value;
        }
    }
    public Vector2Int Pos;
    public BattleTileStateType _BattleTileState;
    public BattleTileType BattleTileT;
    public ControllerType TileOwner;
    public SpriteRenderer SP;
    public PortalInfoClass PortalInfo;

    //Private
    private bool isMovingEventSubscribed = false;



    #region Tile Variables
    [Header("Buff_Health_Instant")]
    public float Min_Buff_Health_Instant;
    public float Max_Buff_Health_Instant;

    [Header("Buff_Health_OverTime")]
    public float Min_Buff_Health_OverTime;
    public float Max_Buff_Health_OverTime;
    public float Min_Duration_Buff_Health_OverTime;
    public float Max_Duration_Buff_Health_OverTime;

    [Header("Buff_Armor_ForTime")]
    public float Min_Buff_Armor_ForTime;
    public float Max_Buff_Armor_ForTime;
    public float Min_Duration_Buff_Armor_ForTime;
    public float Max_Duration_Buff_Armor_ForTime;

    [Header("Buff_MovementSpeed_ForTime")]
    public float Min_Buff_MovementSpeed_ForTime;
    public float Max_Buff_MovementSpeed_ForTime;
    public float Min_Duration_Buff_MovementSpeed_ForTime;
    public float Max_Duration_Buff_MovementSpeed_ForTime;

    [Header("Buff_Regeneration_ForTime")]
    public float Min_Buff_Regeneration_ForTime;
    public float Max_Buff_Regeneration_ForTime;
    public float Min_Duration_Buff_Regeneration_ForTime;
    public float Max_Duration_Buff_Regeneration_ForTime;

    [Header("Buff_Stamina_ForTime")]
    public float Min_Buff_Stamina_ForTime;
    public float Max_Buff_Stamina_ForTime;
    public float Min_Duration_Buff_Stamina_ForTime;
    public float Max_Duration_Buff_Stamina_ForTime;

    [Header("Buff_StaminaRegeneration_ForTime")]
    public float Min_Buff_StaminaRegeneration_ForTime;
    public float Max_Buff_StaminaRegeneration_ForTime;
    public float Min_Duration_Buff_StaminaRegeneration_ForTime;
    public float Max_Duration_Buff_StaminaRegeneration_ForTime;

    [Header("Buff_AttackSpeed_ForTime")]
    public float Min_Buff_AttackSpeed_ForTime;
    public float Max_Buff_AttackSpeed_ForTime;
    public float Min_Duration_Buff_AttackSpeed_ForTime;
    public float Max_Duration_Buff_AttackSpeed_ForTime;

    [Header("Buff_BulletSpeed_ForTime")]
    public float Min_Buff_BulletSpeed_ForTime;
    public float Max_Buff_BulletSpeed_ForTime;
    public float Min_Duration_Buff_BulletSpeed_ForTime;
    public float Max_Duration_Buff_BulletSpeed_ForTime;

    [Header("Buff_AttackType_ForTime")]
    public float Min_Buff_AttackType_ForTime;
    public float Max_Buff_AttackType_ForTime;
    public float Min_Duration_Buff_AttackType_ForTime;
    public float Max_Duration_Buff_AttackType_ForTime;

    [Header("Buff_Armor_Elemental_Neutral_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Neutral_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Neutral_1_ForTime;

    [Header("Buff_Armor_Elemental_Light_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Light_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Light_1_ForTime;

    [Header("Buff_Armor_Elemental_Dark_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Dark_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Dark_1_ForTime;

    [Header("Buff_Armor_Elemental_Earth_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Earth_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Earth_1_ForTime;

    [Header("Buff_Armor_Elemental_Lightning_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Lightning_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Lightning_1_ForTime;

    [Header("Buff_Armor_Elemental_Water_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Water_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Water_1_ForTime;

    [Header("Buff_Armor_Elemental_Fire_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Fire_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Fire_1_ForTime;

    [Header("Buff_Armor_Elemental_Ice_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Ice_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Ice_1_ForTime;

    [Header("Buff_Armor_Elemental_Wind_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Wind_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Wind_1_ForTime;

    [Header("Buff_Armor_Elemental_Life_1_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Life_1_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Life_1_ForTime;

    [Header("Buff_Armor_Elemental_Neutral_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Neutral_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Neutral_2_ForTime;

    [Header("Buff_Armor_Elemental_Light_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Light_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Light_2_ForTime;

    [Header("Buff_Armor_Elemental_Dark_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Dark_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Dark_2_ForTime;

    [Header("Buff_Armor_Elemental_Earth_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Earth_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Earth_2_ForTime;

    [Header("Buff_Armor_Elemental_Lightning_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Lightning_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Lightning_2_ForTime;

    [Header("Buff_Armor_Elemental_Water_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Water_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Water_2_ForTime;

    [Header("Buff_Armor_Elemental_Fire_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Fire_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Fire_2_ForTime;

    [Header("Buff_Armor_Elemental_Ice_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Ice_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Ice_2_ForTime;

    [Header("Buff_Armor_Elemental_Wind_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Wind_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Wind_2_ForTime;

    [Header("Buff_Armor_Elemental_Life_2_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Life_2_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Life_2_ForTime;

    [Header("Buff_Armor_Elemental_Neutral_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Neutral_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Neutral_3_ForTime;

    [Header("Buff_Armor_Elemental_Light_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Light_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Light_3_ForTime;

    [Header("Buff_Armor_Elemental_Dark_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Dark_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Dark_3_ForTime;

    [Header("Buff_Armor_Elemental_Earth_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Earth_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Earth_3_ForTime;

    [Header("Buff_Armor_Elemental_Lightning_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Lightning_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Lightning_3_ForTime;

    [Header("Buff_Armor_Elemental_Water_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Water_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Water_3_ForTime;

    [Header("Buff_Armor_Elemental_Fire_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Fire_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Fire_3_ForTime;

    [Header("Buff_Armor_Elemental_Ice_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Ice_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Ice_3_ForTime;

    [Header("Buff_Armor_Elemental_Wind_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Wind_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Wind_3_ForTime;

    [Header("Buff_Armor_Elemental_Life_3_ForTime")]
    public float Min_Duration_Buff_Armor_Elemental_Life_3_ForTime;
    public float Max_Duration_Buff_Armor_Elemental_Life_3_ForTime;

    [Header("Debuff_Health_Instant")]
    public float Min_Debuff_Health_Instant;
    public float Max_Debuff_Health_Instant;

    [Header("Debuff_Health_OverTime")]
    public float Min_Debuff_Health_OverTime;
    public float Max_Debuff_Health_OverTime;
    public float Min_Duration_Debuff_Health_OverTime;
    public float Max_Duration_Debuff_Health_OverTime;

    [Header("Debuff_MovementSpeed_ForTime")]
    public float Min_Debuff_MovementSpeed_ForTime;
    public float Max_Debuff_MovementSpeed_ForTime;
    public float Min_Duration_Debuff_MovementSpeed_ForTime;
    public float Max_Duration_Debuff_MovementSpeed_ForTime;

    [Header("Debuff_Regeneration_ForTime")]
    public float Min_Debuff_Regeneration_ForTime;
    public float Max_Debuff_Regeneration_ForTime;
    public float Min_Duration_Debuff_Regeneration_ForTime;
    public float Max_Duration_Debuff_Regeneration_ForTime;

    [Header("Debuff_Stamina_ForTime")]
    public float Min_Debuff_Stamina_ForTime;
    public float Max_Debuff_Stamina_ForTime;
    public float Min_Duration_Debuff_Stamina_ForTime;
    public float Max_Duration_Debuff_Stamina_ForTime;

    [Header("Debuff_StaminaRegeneration_ForTime")]
    public float Min_Debuff_StaminaRegeneration_ForTime;
    public float Max_Debuff_StaminaRegeneration_ForTime;
    public float Min_Duration_Debuff_StaminaRegeneration_ForTime;
    public float Max_Duration_Debuff_StaminaRegeneration_ForTime;

    [Header("Debuff_AttackSpeed_ForTime")]
    public float Min_Debuff_AttackSpeed_ForTime;
    public float Max_Debuff_AttackSpeed_ForTime;
    public float Min_Duration_Debuff_AttackSpeed_ForTime;
    public float Max_Duration_Debuff_AttackSpeed_ForTime;

    [Header("Debuff_BulletSpeed_ForTime")]
    public float Min_Debuff_BulletSpeed_ForTime;
    public float Max_Debuff_BulletSpeed_ForTime;
    public float Min_Duration_Debuff_BulletSpeed_ForTime;
    public float Max_Duration_Debuff_BulletSpeed_ForTime;

    [Header("Debuff_AttackType_ForTime")]
    public float Min_Debuff_AttackType_ForTime;
    public float Max_Debuff_AttackType_ForTime;
    public float Min_Duration_Debuff_AttackType_ForTime;
    public float Max_Duration_Debuff_AttackType_ForTime;

    [Header("Debuff_Armor_Elemental_Neutral_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Neutral_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Neutral_1_ForTime;

    [Header("Debuff_Armor_Elemental_Light_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Light_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Light_1_ForTime;

    [Header("Debuff_Armor_Elemental_Dark_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Dark_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Dark_1_ForTime;

    [Header("Debuff_Armor_Elemental_Earth_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Earth_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Earth_1_ForTime;

    [Header("Debuff_Armor_Elemental_Lightning_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Lightning_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Lightning_1_ForTime;

    [Header("Debuff_Armor_Elemental_Water_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Water_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Water_1_ForTime;

    [Header("Debuff_Armor_Elemental_Fire_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Fire_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Fire_1_ForTime;

    [Header("Debuff_Armor_Elemental_Ice_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Ice_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Ice_1_ForTime;

    [Header("Debuff_Armor_Elemental_Wind_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Wind_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Wind_1_ForTime;

    [Header("Debuff_Armor_Elemental_Life_1_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Life_1_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Life_1_ForTime;

    [Header("Debuff_Armor_Elemental_Neutral_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Neutral_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Neutral_2_ForTime;

    [Header("Debuff_Armor_Elemental_Light_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Light_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Light_2_ForTime;

    [Header("Debuff_Armor_Elemental_Dark_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Dark_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Dark_2_ForTime;

    [Header("Debuff_Armor_Elemental_Earth_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Earth_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Earth_2_ForTime;

    [Header("Debuff_Armor_Elemental_Lightning_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Lightning_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Lightning_2_ForTime;

    [Header("Debuff_Armor_Elemental_Water_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Water_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Water_2_ForTime;

    [Header("Debuff_Armor_Elemental_Fire_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Fire_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Fire_2_ForTime;

    [Header("Debuff_Armor_Elemental_Ice_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Ice_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Ice_2_ForTime;

    [Header("Debuff_Armor_Elemental_Wind_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Wind_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Wind_2_ForTime;

    [Header("Debuff_Armor_Elemental_Life_2_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Life_2_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Life_2_ForTime;

    [Header("Debuff_Armor_Elemental_Neutral_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Neutral_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Neutral_3_ForTime;

    [Header("Debuff_Armor_Elemental_Light_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Light_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Light_3_ForTime;

    [Header("Debuff_Armor_Elemental_Dark_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Dark_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Dark_3_ForTime;

    [Header("Debuff_Armor_Elemental_Earth_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Earth_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Earth_3_ForTime;

    [Header("Debuff_Armor_Elemental_Lightning_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Lightning_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Lightning_3_ForTime;

    [Header("Debuff_Armor_Elemental_Water_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Water_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Water_3_ForTime;

    [Header("Debuff_Armor_Elemental_Fire_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Fire_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Fire_3_ForTime;

    [Header("Debuff_Armor_Elemental_Ice_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Ice_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Ice_3_ForTime;

    [Header("Debuff_Armor_Elemental_Wind_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Wind_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Wind_3_ForTime;

    [Header("Debuff_Armor_Elemental_Life_3_ForTime")]
    public float Min_Duration_Debuff_Armor_Elemental_Life_3_ForTime;
    public float Max_Duration_Debuff_Armor_Elemental_Life_3_ForTime;

    [Header("Debuff_Weapon_Elemental_Neutral_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Neutral_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Neutral_ForTime;

    [Header("Debuff_Weapon_Elemental_Light_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Light_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Light_ForTime;

    [Header("Debuff_Weapon_Elemental_Dark_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Dark_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Dark_ForTime;

    [Header("Debuff_Weapon_Elemental_Earth_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Earth_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Earth_ForTime;

    [Header("Debuff_Weapon_Elemental_Lightning_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Lightning_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Lightning_ForTime;

    [Header("Debuff_Weapon_Elemental_Water_ForTime")]
    public float Min_Duration_Weapon_Elemental_Water_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Water_ForTime;

    [Header("Debuff_Weapon_Elemental_Fire_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Fire_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Fire_ForTime;

    [Header("Debuff_Weapon_Elemental_Ice_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Ice_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Ice_ForTime;

    [Header("Debuff_Weapon_Elemental_Wind_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Wind_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Wind_ForTime;

    [Header("Debuff_Weapon_Elemental_Life_ForTime")]
    public float Min_Duration_Debuff_Weapon_Elemental_Life_ForTime;
    public float Max_Duration_Debuff_Weapon_Elemental_Life_ForTime;

    [Header("Debuff_Freeze_ForTime")]
    public float Min_Duration_Debuff_Freeze_ForTime;
    public float Max_Duration_Debuff_Freeze_ForTime;

    [Header("Debuff_Trap_ForTime")]
    public float Min_Duration_Debuff_Trap_ForTime;
    public float Max_Duration_Debuff_Trap_ForTime;


    #endregion



    private void Awake()
    {
        SP = GetComponent<SpriteRenderer>();
    }
    //Setup tile info
    public void SetupTileFromBattleTileInfo(BattleTileInfo info)
    {
        BattleTileState = info.BattleTileState;
        BattleTileT = info.BattleTileT;
        TileOwner = info.TileOwner;
        if (BattleTileT == BattleTileType.Portal)
        {
            PortalInfo = new PortalInfoClass(this, info.Portal, info.IDPortal);
            GridManagerScript.Instance.Portals.Add(PortalInfo);
        }
        SetupTile();
    }

    //Placeholde for the TileSetup
    public void SetupTile()
    {
        if (BattleTileT == BattleTileType.Base)
        {
            SP.color = TileOwner == ControllerType.Enemy ? Color.blue : Color.yellow;
        }
        else if (BattleTileT.ToString().Contains("Buff"))
        {
            SP.color = Color.white;
        }
        else if (BattleTileT.ToString().Contains("Debuff"))
        {
            SP.color = Color.green;
        }
        else if (BattleTileT.ToString().Contains("Portal"))
        {
            SP.color = Color.red;
        }

        CreateTile();
    }

    //Reset the tile to the default values
    public void ResetTile()
    {
        BattleTileState =  BattleTileStateType.Blocked;
        BattleTileT =  BattleTileType.Base;
        SP.color = Color.white;
    }

    public void CreateTile()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        //If collides with a character and the tile type is not base
        if (other.tag.Contains("Character") && BattleTileT != BattleTileType.Base && BattleTileState == BattleTileStateType.Occupied)
        {
            CharacterBase targetCharacter = other.GetComponentInParent<CharacterBase>();

            if(targetCharacter.Pos.Contains(Pos))
            {
                if (!isMovingEventSubscribed)
                {
                    isMovingEventSubscribed = true;
                    //Subscribe to the TargetCharacter_TileMovementCompleteEvent event
                    targetCharacter.TileMovementCompleteEvent += TargetCharacter_TileMovementCompleteEvent;

                }
            }
        }
    }

    //Setup the Tile effect
    private void TargetCharacter_TileMovementCompleteEvent(CharacterBase movingChar)
    {
        //If is a Portal-In
        if (BattleTileT == BattleTileType.Portal && PortalInfo.Portal == PortalType.In)
        {
            StartCoroutine(movingChar.UsePortal(GridManagerScript.Instance.Portals.Where(r => r.IDPortal == PortalInfo.IDPortal && r.Portal == PortalType.Out).First()));
            return;
        }
        //If is a Portal-Out
        if (BattleTileT == BattleTileType.Portal && PortalInfo.Portal == PortalType.Out && movingChar.IsUsingAPortal)
        {
            //TODO
            return;
        }
        //If is freeze tile
        if (BattleTileT == BattleTileType.Debuff_Freeze_ForTime)
        {
            float freezeDuration = Random.Range((float)this.GetType().GetField("Min_Duration_" + BattleTileT.ToString()).GetValue(this), (float)this.GetType().GetField("Max_Duration_" + BattleTileT.ToString()).GetValue(this));
            StartCoroutine(movingChar.Freeze(freezeDuration, 0));
            return;
        }
        //If is trap tile
        if (BattleTileT == BattleTileType.Debuff_Trap_ForTime)
        {

        }

        //Get Duration from the right variables
        float BuffDebuffDuration = Random.Range((float)this.GetType().GetField("Min_Duration_" + BattleTileT.ToString()).GetValue(this), (float)this.GetType().GetField("Max_Duration_" + BattleTileT.ToString()).GetValue(this));
        //Splitting the Enum name in order to have the right info 
        string[] res = BattleTileT.ToString().Split('_');
        //Getting the buff debuff value
        float BuffDebuffValue = res.Length == 3 ?
        Random.Range((float)this.GetType().GetField("Min_" + BattleTileT.ToString()).GetValue(this), (float)this.GetType().GetField("Max_" + BattleTileT.ToString()).GetValue(this)) : 0;

        //Creation of the Buff/Debuff
        Buff_DebuffClass bdClass = new Buff_DebuffClass();
        bdClass.Duration = BuffDebuffDuration;
        bdClass.Value = BuffDebuffValue;
        bdClass.AnimToFire = (CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), res[0]);
        bdClass.Stat = (BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]);
        if ((BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]) == BuffDebuffStatsType.Armor)
        {
            //deciding the type of elemental + weakness of that elemental
            ElementalWeaknessType ewt = bdClass.AnimToFire == CharacterAnimationStateType.Buff ? (ElementalWeaknessType)System.Convert.ToInt16(res[4]) : (ElementalWeaknessType)(-System.Convert.ToInt16(res[4]));
            bdClass.ElementalResistence = new ElementalResistenceClass((ElementalType)System.Enum.Parse(typeof(ElementalType), res[3]), ewt);
        }
        else if ((BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]) == BuffDebuffStatsType.ElementalPower)
        {
            //Set up the elemental type
            bdClass.ElementalPower = (ElementalType)System.Enum.Parse(typeof(ElementalType), res[1]);
        }
        StartCoroutine(movingChar.Buff_DebuffCoroutine(bdClass));

        if (!BattleTileT.ToString().Contains("_OverTime"))
        {
            movingChar.TileMovementCompleteEvent -= TargetCharacter_TileMovementCompleteEvent;
            BattleTileT = BattleTileType.Base;
            SetupTile();
            isMovingEventSubscribed = false;
        }
       
    }
}


