using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
   
}


public enum BattleTileStateType
{
    Blocked,
    Empty,
    Occupied
}

public enum VibrationType
{
    a,
    b
}

public enum InputDirection
{
    Up,
    Down,
    Left,
    Right
}


public enum ButtonClickStateType
{
    Down,
    Press,
    Up
}

public enum CameraBasePosType
{
    VeryClose,
    Close,
    Mid,
    MidFar,
    Far,
    VeryFar
}


public enum BattleTileType
{
    Base,
    Buff_Health_Instant,
    Buff_Health_OverTime,
    Buff_Armor_ForTime,
    Buff_MovementSpeed_ForTime,
    Buff_Regeneration_ForTime,
    Buff_Stamina_ForTime,
    Buff_StaminaRegeneration_ForTime,
    Buff_AttackSpeed_ForTime,
    Buff_BulletSpeed_ForTime,
    Buff_AttackType_ForTime,
    Buff_Armor_Elemental_Neutral_1_ForTime,
    Buff_Armor_Elemental_Light_1_ForTime,
    Buff_Armor_Elemental_Dark_1_ForTime,
    Buff_Armor_Elemental_Earth_1_ForTime,
    Buff_Armor_Elemental_Lightning_1_ForTime,
    Buff_Armor_Elemental_Water_1_ForTime,
    Buff_Armor_Elemental_Fire_1_ForTime,
    Buff_Armor_Elemental_Ice_1_ForTime,
    Buff_Armor_Elemental_Wind_1_ForTime,
    Buff_Armor_Elemental_Life_1_ForTime,
    Buff_Armor_Elemental_Neutral_2_ForTime,
    Buff_Armor_Elemental_Light_2_ForTime,
    Buff_Armor_Elemental_Dark_2_ForTime,
    Buff_Armor_Elemental_Earth_2_ForTime,
    Buff_Armor_Elemental_Lightning_2_ForTime,
    Buff_Armor_Elemental_Water_2_ForTime,
    Buff_Armor_Elemental_Fire_2_ForTime,
    Buff_Armor_Elemental_Ice_2_ForTime,
    Buff_Armor_Elemental_Wind_2_ForTime,
    Buff_Armor_Elemental_Life_2_ForTime,
    Buff_Armor_Elemental_Neutral_3_ForTime,
    Buff_Armor_Elemental_Light_3_ForTime,
    Buff_Armor_Elemental_Dark_3_ForTime,
    Buff_Armor_Elemental_Earth_3_ForTime,
    Buff_Armor_Elemental_Lightning_3_ForTime,
    Buff_Armor_Elemental_Water_3_ForTime,
    Buff_Armor_Elemental_Fire_3_ForTime,
    Buff_Armor_Elemental_Ice_3_ForTime,
    Buff_Armor_Elemental_Wind_3_ForTime,
    Buff_Armor_Elemental_Life_3_ForTime,
    Debuff_Health_Instant,
    Debuff_Health_OverTime,
    Debuff_Armor_ForTime,
    Debuff_MovementSpeed_ForTime,
    Debuff_Regeneration_ForTime,
    Debuff_Stamina_ForTime,
    Debuff_StaminaRegeneration_ForTime,
    Debuff_AttackSpeed_ForTime,
    Debuff_BulletSpeed_ForTime,
    Debuff_AttackType_ForTime,
    Debuff_Armor_Elemental_Neutral_1_ForTime,
    Debuff_Armor_Elemental_Light_1_ForTime,
    Debuff_Armor_Elemental_Dark_1_ForTime,
    Debuff_Armor_Elemental_Earth_1_ForTime,
    Debuff_Armor_Elemental_Lightning_1_ForTime,
    Debuff_Armor_Elemental_Water_1_ForTime,
    Debuff_Armor_Elemental_Fire_1_ForTime,
    Debuff_Armor_Elemental_Ice_1_ForTime,
    Debuff_Armor_Elemental_Wind_1_ForTime,
    Debuff_Armor_Elemental_Life_1_ForTime,
    Debuff_Armor_Elemental_Neutral_2_ForTime,
    Debuff_Armor_Elemental_Light_2_ForTime,
    Debuff_Armor_Elemental_Dark_2_ForTime,
    Debuff_Armor_Elemental_Earth_2_ForTime,
    Debuff_Armor_Elemental_Lightning_2_ForTime,
    Debuff_Armor_Elemental_Water_2_ForTime,
    Debuff_Armor_Elemental_Fire_2_ForTime,
    Debuff_Armor_Elemental_Ice_2_ForTime,
    Debuff_Armor_Elemental_Wind_2_ForTime,
    Debuff_Armor_Elemental_Life_2_ForTime,
    Debuff_Armor_Elemental_Neutral_3_ForTime,
    Debuff_Armor_Elemental_Light_3_ForTime,
    Debuff_Armor_Elemental_Dark_3_ForTime,
    Debuff_Armor_Elemental_Earth_3_ForTime,
    Debuff_Armor_Elemental_Lightning_3_ForTime,
    Debuff_Armor_Elemental_Water_3_ForTime,
    Debuff_Armor_Elemental_Fire_3_ForTime,
    Debuff_Armor_Elemental_Ice_3_ForTime,
    Debuff_Armor_Elemental_Wind_3_ForTime,
    Debuff_Armor_Elemental_Life_3_ForTime,
    Debuff_Trap_ForTime,
    Debuff_Freeze_ForTime,
    Portal,
    Debuff_Weapon_Elemental_Neutral_ForTime,
    Debuff_Weapon_Elemental_Light_ForTime,
    Debuff_Weapon_Elemental_Dark_ForTime,
    Debuff_Weapon_Elemental_Earth_ForTime,
    Debuff_Weapon_Elemental_Lightning_ForTime,
    Debuff_Weapon_Elemental_Water_ForTime,
    Debuff_Weapon_Elemental_Fire_ForTime,
    Debuff_Weapon_Elemental_Ice_ForTime,
    Debuff_Weapon_Elemental_Wind_ForTime,
    Debuff_Weapon_Elemental_Life_ForTime,
}

public enum ControllerType
{
    Player1,
    Player2,
    Player3,
    Player4,
    Enemy
}

public enum BattleState
{
    Initialization,
    Intro,
    Talk,
    Event,
    Battle,
    Pause,
    Menu,
    End,
    WinLose
}

public enum ElementalType
{
    Neutral,
    Light,
    Dark,
    Earth,
    Lighting,
    Water,
    Fire,
    Ice,
    Wind,
    Life
}


public enum CharacterLevelType
{
    Novice,
    Defiant,
    Heroine,
    Godness
}

public enum RelationshipType
{
    Strangers,
    Acquaintances,
    Friends,
    Sisters
}

public enum CharacterType
{
    c1,
    c2,
    c3,
    c4,
    c5,
    c6,
    c7,
    c8
}

public enum ArmorType
{
    none
}

public enum WeaponType
{
    none
}


public enum MatchType
{
    PvE,
    PvP,
    PPvE,
    PPvPP
}

public enum CharacterSelectionType
{
    Up,
    Down,
    Left,
    Right,
    A,
    B,
    X,
    Y
    
}

public enum BuffDebuffStatsType
{
    Health,
    HealthOverTime,
    Armor,
    ElementalPower,
    Regeneration,
    MovementSpeed,
    Stamina,
    StaminaRegeneration,
    AttackSpeed,
    BulletSpeed,
    AttackType
}

public enum ElementalWeaknessType
{
    ExtremelyWeak = -3,
    VeryWeak = -2,
    Weak = -1,
    Neutral = 0,
    Resistent = 1,
    VeryResistent = 2,
    ExtremelyResistent = 3
}


public enum CharacterAnimationStateType
{
    NoMesh,
    Idle,
    Atk,
    Atk1,
    Buff,
    Debuff,
    Gettinghit,
    Defending,
    Paralized,
    Arriving,
    DashRight,
    DashLeft,
    DashDown,
    DashUp,
    Selection,
    PowerUp,
    Speaking,
    Victory,
    Defeat,
    Death
}

public enum PortalType
{
    In,
    Out
}

public enum AttackParticleTypes
{
    Stage04_Minion_Mountain,
    Stage04_Minion_Forest,
    Stage04_Minion_Desert
}


public enum ParticleTypes
{
    Cast,
    Attack,
    Effect
}

public enum SideType
{
    PlayerCharacter,
    EnemyCharacter
}


public enum FacingType
{
    Left,
    Right
}

public enum RelationshipBetweenElements
{
    Neutral = 0,
    ExtremelyResistent = 1,
    VeryResistent = 2,
    Resistent = 3,
    Weak = 4,
    VeryWeak = 5,
    VeryWeak_1 = 6,
    VeryWeak_2 = 7,
    VeryWeak_3 = 8,
    ExtremelyWeak = 9,
    
   
}


public enum PlayerColorType
{
    _FF0D0D,
    _FF0CDC,
    _0C21FF,
    _0CFF1F
}

public enum CharacterClassType
{
    Valley,
    Mountain,
    Forest,
    Desert
}


public enum CharacterNameType
{
    Balistica,
    LadyBush,
    Jaguar
}