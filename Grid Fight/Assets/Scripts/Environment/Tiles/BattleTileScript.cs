using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log(value);
            _BattleTileState = value;
        }
    }
    public Vector2Int Pos;
    public BattleTileStateType _BattleTileState;
    public BattleTileType BattleTileT;
    public ControllerType TileOwner;
    public SpriteRenderer SP;
    private float LastTimeCollisionWithCharacter;



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

    [Header("Weapon_Elemental_Neutral_ForTime")]
    public float Min_Duration_Weapon_Elemental_Neutral_ForTime;
    public float Max_Duration_Weapon_Elemental_Neutral_ForTime;

    [Header("Weapon_Elemental_Light_ForTime")]
    public float Min_Duration_Weapon_Elemental_Light_ForTime;
    public float Max_Duration_Weapon_Elemental_Light_ForTime;

    [Header("Weapon_Elemental_Dark_ForTime")]
    public float Min_Duration_Weapon_Elemental_Dark_ForTime;
    public float Max_Duration_Weapon_Elemental_Dark_ForTime;

    [Header("Weapon_Elemental_Earth_ForTime")]
    public float Min_Duration_Weapon_Elemental_Earth_ForTime;
    public float Max_Duration_Weapon_Elemental_Earth_ForTime;

    [Header("Weapon_Elemental_Lightning_ForTime")]
    public float Min_Duration_Weapon_Elemental_Lightning_ForTime;
    public float Max_Duration_Weapon_Elemental_Lightning_ForTime;

    [Header("Weapon_Elemental_Water_ForTime")]
    public float Min_Duration_Weapon_Elemental_Water_ForTime;
    public float Max_Duration_Weapon_Elemental_Water_ForTime;

    [Header("Weapon_Elemental_Fire_ForTime")]
    public float Min_Duration_Weapon_Elemental_Fire_ForTime;
    public float Max_Duration_Weapon_Elemental_Fire_ForTime;

    [Header("Weapon_Elemental_Ice_ForTime")]
    public float Min_Duration_Weapon_Elemental_Ice_ForTime;
    public float Max_Duration_Weapon_Elemental_Ice_ForTime;

    [Header("Weapon_Elemental_Wind_ForTime")]
    public float Min_Duration_Weapon_Elemental_Wind_ForTime;
    public float Max_Duration_Weapon_Elemental_Wind_ForTime;

    [Header("Weapon_Elemental_Life_ForTime")]
    public float Min_Duration_Weapon_Elemental_Life_ForTime;
    public float Max_Duration_Weapon_Elemental_Life_ForTime;






    private void Awake()
    {
        SP = GetComponent<SpriteRenderer>();
    }
    public void SetupTile(BattleTileInfo info)
    {
        BattleTileState = info.BattleTileState;
        BattleTileT = info.BattleTileT;
        TileOwner = info.TileOwner;
        if (BattleTileState == BattleTileStateType.Empty)
        {
            SP.color = Color.red;
        }
        else
        {
            SP.color = Color.white;
        }
        
        CreateTile();
    }

    public void ResetTile()
    {
        BattleTileState =  BattleTileStateType.Blocked;
        BattleTileT =  BattleTileType.Base;
        SP.color = Color.white;
    }

    public void CreateTile()
    {
        switch (BattleTileT)
        {
            case BattleTileType.Base:
                break;
            case BattleTileType.Buff_Health_Instant:
                break;
            case BattleTileType.Buff_Health_OverTime:
                break;
            case BattleTileType.Buff_Armor_ForTime:
                break;
            case BattleTileType.Buff_MovementSpeed_ForTime:
                break;
            case BattleTileType.Buff_Regeneration_ForTime:
                break;
            case BattleTileType.Buff_Stamina_ForTime:
                break;
            case BattleTileType.Buff_StaminaRegeneration_ForTime:
                break;
            case BattleTileType.Buff_AttackSpeed_ForTime:
                break;
            case BattleTileType.Buff_BulletSpeed_ForTime:
                break;
            case BattleTileType.Buff_AttackType_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Neutral_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Light_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Dark_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Earth_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Lightning_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Water_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Fire_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Ice_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Wind_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Life_1_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Neutral_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Light_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Dark_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Earth_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Lightning_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Water_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Fire_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Ice_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Wind_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Life_2_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Neutral_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Light_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Dark_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Earth_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Lightning_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Water_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Fire_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Ice_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Wind_3_ForTime:
                break;
            case BattleTileType.Buff_Armor_Elemental_Life_3_ForTime:
                break;
            case BattleTileType.Debuff_Health_Instant:
                break;
            case BattleTileType.Debuff_Health_OverTime:
                break;
            case BattleTileType.Debuff_Armor_ForTime:
                break;
            case BattleTileType.Debuff_MovementSpeed_ForTime:
                break;
            case BattleTileType.Debuff_Regeneration_ForTime:
                break;
            case BattleTileType.Debuff_Stamina_ForTime:
                break;
            case BattleTileType.Debuff_StaminaRegeneration_ForTime:
                break;
            case BattleTileType.Debuff_AttackSpeed_ForTime:
                break;
            case BattleTileType.Debuff_BulletSpeed_ForTime:
                break;
            case BattleTileType.Debuff_AttackType_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Neutral_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Light_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Dark_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Earth_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Lightning_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Water_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Fire_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Ice_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Wind_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Life_1_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Neutral_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Light_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Dark_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Earth_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Lightning_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Water_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Fire_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Ice_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Wind_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Life_2_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Neutral_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Light_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Dark_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Earth_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Lightning_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Water_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Fire_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Ice_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Wind_3_ForTime:
                break;
            case BattleTileType.Debuff_Armor_Elemental_Life_3_ForTime:
                break;
            case BattleTileType.Trap:
                break;
            case BattleTileType.Freeze:
                break;
            case BattleTileType.Portal:
                break;
            case BattleTileType.Weapon_Elemental_Neutral_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Light_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Dark_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Earth_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Lightning_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Water_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Fire_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Ice_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Wind_ForTime:
                break;
            case BattleTileType.Weapon_Elemental_Life_ForTime:
                break;
            default:
                break;
        }
    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Character" && BattleTileT != BattleTileType.Base)
        {
            if(Time.time - LastTimeCollisionWithCharacter > Time.fixedDeltaTime)
            {
                LastTimeCollisionWithCharacter = Time.time;
                float BuffDebuffDuration = Random.Range((float)this.GetType().GetField("Min_Duration_" + BattleTileT.ToString()).GetValue(this), (float)this.GetType().GetField("Max_Duration_" + BattleTileT.ToString()).GetValue(this));

                string[] res = BattleTileT.ToString().Split('_');
                float BuffDebuffValue = res.Length == 3 ?
                    Random.Range((float)this.GetType().GetField("Min_" + BattleTileT.ToString()).GetValue(this), (float)this.GetType().GetField("Max_" + BattleTileT.ToString()).GetValue(this)) : 0;
                CharacterBase targetCharacter = other.GetComponent<CharacterBase>();


                Buff_DebuffClass t = new Buff_DebuffClass();
                t.Duration = BuffDebuffDuration;
                t.Value = BuffDebuffValue;
                t.Stat = (BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]);
                if ((BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]) == BuffDebuffStatsType.Armor)
                {
                    ElementalWeaknessType ewt = res[0] == "Buff" ? (ElementalWeaknessType)System.Convert.ToInt16(res[4]) : (ElementalWeaknessType)(-System.Convert.ToInt16(res[4]));
                    t.ElementalResistence = new ElementalResistenceClass((ElementalType)System.Enum.Parse(typeof(ElementalType), res[3]), ewt);
                }
                else if ((BuffDebuffStatsType)System.Enum.Parse(typeof(BuffDebuffStatsType), res[1]) == BuffDebuffStatsType.ElementalPower)
                {
                    t.ElementalPower = (ElementalType)System.Enum.Parse(typeof(ElementalType), res[1]);
                }
                StartCoroutine(targetCharacter.Buff_DebuffCoroutine(t));
            }
        }
    }
}


