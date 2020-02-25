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

public enum FungusDialogType
{
    Nonee,
    Dialog,
    Menu
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
    Enemy,
    None
}

public enum UnitBehaviourType
{
    ControlledByPlayer,
    NPC
}

public enum WalkingSideType
{
    LeftSide,
    RightSide,
    Both
}

public enum BattleState
{
    Initialization,
    FungusPuppets,
    Talk,
    Event,
    Battle,
    Pause,
    Menu,
    End,
    WinLose,
    Tutorial,
    Intro
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

public enum AttackPhasesType
{
    Start,
    Loading,
    Cast_Rapid,
    Bullet_Rapid,
    Cast_Powerful,
    Bullet_Powerful,
    End
    
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
    Right
}

public enum BuffDebuffStatsType
{
    Health,
    HealthOverTime,
    ElementalResistance,
    ElementalPower,
    HealthRegeneration,
    MovementSpeed,
    Stamina,
    StaminaRegeneration,
    AttackSpeed,
    BulletSpeed,
    AttackType,
    BaseSpeed,
    Damage
}

public enum WaveStatsType
{
    None,
    Health,
    Stamina,
    AttackSpeed,
    MovementSpeed,
    BaseSpeed

}

public enum ValueCheckerType
{
    LessThan,
    EqualTo,
    MoreThan
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
    Atk1_AtkToIdle,
    Atk1_IdleToAtk,
    Atk1_Loop,
    Atk1_Charging,
    Atk2_AtkToIdle,
    Atk2_IdleToAtk,
    Atk2_Charging,
    Buff,
    Debuff,
    GettingHit,
    Defending,
    Paralized,
    Arriving,
    Growing,
    Growing1,
    Growing2,
    DashRight,
    DashLeft,
    DashDown,
    DashUp,
    Selection,
    PowerUp,
    Reverse_Arriving,
    Speaking,
    Victory,
    Defeat,
    Death,
    Idle_Disable_Loop,
    Death_Prep,
    Death_Loop,
    Death_Exit,
    Death_Born,
    Idle_Agressive,
    Idle_AtkToIdle,
    Idle_Charging,
    Idle_IdleToAtk,
    Idle_Loop,
}

public enum PortalType
{
    In,
    Out
}


public enum WaveNPCTypes
{
    Minion,
    Recruitable,
    Boss
}


public enum BaseCharType
{

    None = 10000,
    CharacterType_Script = 0,
    MinionType_Script = 1,
    Stage04_BossGirl_Flower_Script = 2,
    Stage04_BossGirl_Script = 3,
    Stage04_BossMonster_Flower_Script = 4,
    Stage04_BossMonster_Script = 5,
    Stage00_BossOctopus = 6,
    Stage00_BossOctopus_Head = 7,
    Stage00_BossOctopus_Tentacles = 8,
    Stage00_BossOctopus_Girl = 9

}


public enum ParticlesType
{
    None,
    CharArrivingSmoke,
    Stage04FlowersSmoke,
    PowerUp_Damage,
    PowerUp_Health,
    PowerUp_Stamina,
    PowerUp_Speed,
    ShieldNormal,
    ShieldTotalDefence
}

public enum AttackParticleTypes
{
    Stage00_Minion_Valley = 13,
    Stage00_Minion_Forest = 14,
    Stage00_Minion_Mountain = 15,
    Stage00_Minion_Desert = 16,
    Stage00_Desert = 17,
    Stage00_Mountain = 18,
    Stage00_Forest = 19,
    Stage00_Valley = 20,
    Stage03_Minion_Desert = 4,
    Stage04_Minion_Mountain = 0,
    Stage04_Minion_Forest = 1,
    Stage04_Minion_Desert = 2,
    Stage04_Minion_Valley = 3,
    Stage04_Desert = 5,
    Stage04_Mountain = 6,
    Stage04_Forest = 7,
    Stage04_Valley = 8,
    Stage04_BossGirl_Minion = 9,
    Stage04_BossGirl = 10,
    Stage04_BossMonster_Minion = 11,
    Stage04_BossMonster = 12,
    Test_Mesh = 1000,
    Stage00_BossOctopus_Tentacle = 21,
    Stage00_BossOctopus_Head = 22,
}


public enum AttackParticlePhaseTypes
{
    CastRight,
    EffectRight,
    CastLeft,
    AttackLeft,
    EffectLeft,
    Charging,
    CastActivation
}

public enum SideType
{
    LeftSide,
    RightSide
}

public enum ItemType
{
    PowerUp_Damage,
    PowerUp_Speed,
    PowerUP_Health,
    PowerUP_FullRecovery
}



public enum GridStructureType
{
    r2xc4,
    r4xc8,
    r6xc12,
    r6xc12_8x4,
    r5xc8,
    r5xc10,
    r5xc10Stage00

}

public enum FacingType
{
    Left,
    Right
}

public enum AttackType
{
    Particles,
    Tile
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

public enum HealthChangedType
{
    Damage,
    Defend,
    Heal,
    CriticalHit
}


public enum CharacterNameType
{
    None = 1000,
    Stage00_Minion_Mountain = 23,
    Stage00_Minion_Desert = 24,
    Stage00_Minion_Forest = 25,
    Stage00_Minion_Valley = 26,
    Stage00_Character_Mountain = 19,
    Stage00_Character_Valley = 20,
    Stage00_Character_Forest = 21,
    Stage00_Character_Desert = 22,
    Stage00_BossOctopus = 27,
    Stage00_BossOctopus_Head = 28,
    Stage00_BossOctopus_Tentacles = 29,
    Stage00_BossOctopus_Girl = 30,
    Stage04_Minion_Mountain = 0,
    Stage04_Minion_Desert = 1,
    Stage04_Minion_Forest = 2,
    Stage04_Minion_Valley = 3,
    Stage04_Character_Valley = 4,
    Stage04_Character_Desert = 5,
    Stage04_Character_Mountain = 6,
    Stage04_Character_Forest = 7,
    Stage04_BossGirl_Minion0 = 8,
    Stage04_BossGirl_Minion1 = 9,
    Stage04_BossGirl_Minion2 = 10,
    Stage04_BossGirl_Minion3 = 11,
    Stage04_BossGirl = 12,
    Stage04_BossMonster_Minion = 13,
    Stage04_BossMonster = 14,
    Stage04_BossMonster_Minion0 = 15,
    Stage04_BossMonster_Minion1 = 16,
    Stage04_BossMonster_Minion2 = 17,
    Stage04_BossMonster_Minion3 = 18,
    Stage09_Character_Valley = 31
}


public enum WavePhaseType
{
    Combat,
    Event,
}

public enum InputAxisType
{
    Left_Move_Horizontal,
    Right_Move_Horizontal,
    Left_Move_Vertical,
    Right_Move_Vertical
}


public enum InputButtonType
{
    A,
    B,
    X,
    Y,
    Left,
    Right,
    Up,
    Down,
    ZL,
    L,
    ZR,
    R,
    Plus,
    Minus,
    Home,
    Capture,
    Left_SL,
    Right_SL,
    Left_SR,
    Right_SR,
    Left_Stick,
    Right_Stick,
    Left_Move_Horizontal,
    Right_Move_Horizontal,
    Left_Move_Vertical,
    Right_Move_Vertical,
    KeyboardDown,
    KeyboardUp,
    KeyboardRight,
    KeyboardLeft

}


public enum MonsterFlowerType
{
    Head1,
    Head2,
    Head3,
    Head4
}




public enum WaveEventCheckType
{
    CharStatsCheckInPerc,
    CharDied,
    KillsNumber

}


public enum SpecialAttackStatus
{
    None,
    Start,
    Move,
    Stop
}


public enum InputControllerType
{
    SelectionOnABXY,
    SelectionOnLR
}

public enum DeathProcessStage
{
    None,
    Start,
    LoopDying,
    End
}