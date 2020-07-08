using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{

}


public enum BattleTileStateType
{
    NonUsable,
    Empty,
    Occupied,
    Blocked
}

public enum FungusDialogType
{
    None,
    Dialog,
    Menu
}

public enum CameraShakeType
{
    None,
    Arrival,
    GettingHit,
    Powerfulattack,
    PowerfulAttackHit,
    Octopus_Tentacle
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

public enum BattleFieldAttackType
{
    OnAreaAttack,
    OnTarget,
    OnItSelf,
    OnRandom
}

public enum CharacterActionType
{
    Move,
    WeakAttack,
    StrongAttack,
    Skill1,
    Skill2,
    Skill3,
    Defence,
    SwitchCharacter
}

public enum BattleState
{
    Initialization = 0,
    FungusPuppets = 1,
    Event = 3,
    Battle = 4,
    Pause = 5,
    Menu = 6,
    End = 7,
    WinLose = 8,
    Tutorial = 9,
    Intro = 10,
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


public enum AttackPhasesType
{
    Start,
    Loading,
    Cast_Weak,
    Cast_Strong,
    Bullet_Weak,
    Bullet_Strong,
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
    PPvPP,
    PPPPvE
}

public enum CharacterSelectionType
{
    Up,
    Down,
    Left,
    Right
}

public enum StatsCheckerType
{
    Perc,
    Value
}

public enum BuffDebuffStatsType
{
    DamageStats_BaseDamage,
    Health_Overtime,
    Health,
    SpeedStats_BaseSpeed,
    SpeedStats_MovementSpeed,
    Drain,
    Zombification,
    BlockTile,
    ShieldStats_BaseShieldRegeneration,
    Drain_Overtime,
    AttackChange,
    StaminaStats_Stamina,
}

public enum StatsCheckType
{
    None,
    Health,
    Stamina,
    AttackSpeed,
    MovementSpeed,
    BaseSpeed,
    TeamTotalHpPerc
}

public enum ModificableStatsType
{
    RapidAttack_CriticalChance,
    RapidAttack_DamageMultiplier,
    RapidAttack_Stamina_Cost_Atk,
    PowerfulAttac_CriticalChance,
    PowerfulAttac_DamageMultiplier,
    PowerfulAttac_Stamina_Cost_Atk,
    HealthStats_Base,
    HealthStats_BaseHealthRegeneration,
    HealthStats_LevelMultiplier,
    HealthStats_Regeneration,
    StaminaStats_Base,
    StaminaStats_BaseStaminaRegeneration,
    StaminaStats_LevelMultiplier,
    StaminaStats_Regeneration,
    StaminaStats_Stamina_Overtime,
    StaminaStats_Stamina,
    SpeedStats_BaseSpeed,
    SpeedStats_MovementSpeed,
    SpeedStats_AttackSpeed,
    SpeedStats_AttackSpeedRatio,
    SpeedStats_BulletSpeed,
    SpeedStats_LeaveSpeed,
    SpeedStats_LevelMultiplier,
    SpeedStats_IdleToAtkDuration,
    SpeedStats_AtkToIdleDuration,
    DamageStats_BaseDamage,
}

public enum AIType
{
    VeryDefensive = -2,
    Defensive = -1,
    Neutral = 0,
    Aggressive = 1,
    VeryAggressive = 2
}

public enum VisionType
{
    Front_Near,
    Front_Far,
    UpDown_Near,
    UpDown_Far
}


public enum AggroType
{
    Hit_0,
    Hit_3,
    Hit_5,
    Hit_10

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


public enum AttackAnimPrefixType
{
    Atk1,
    Atk2,
    Atk3,
    S_Buff,
    S_DeBuff
}


public enum AttackAnimType
{
    Weak_Atk,
    Strong_Atk,
    Buff,
    Debuff,
    Boss_Atk3,
}

public enum AttackInputType
{
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3
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
    Defeat_ReverseArrive,
    Dialogue_Confused,
    Dialogue_Disappointed,
    Dialogue_Angry,
    Dialogue_Happy,
    Dialogue_Sad,
    Dialogue_Standard,
    Dialogue_Surprise,
    Dialogue_To_Idle,
    Idle_To_Dialogue,
	JumpTransition_IN,
    JumpTransition_OUT
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
    TalkingCharacterType_Script = 9999,
    CharacterType_Script = 0,
    MinionType_Script = 1,
    Stage04_BossGirl_Flower_Script = 2,
    Stage04_BossGirl_Script = 3,
    Stage04_BossMonster_Flower_Script = 4,
    Stage04_BossMonster_Script = 5,
    Stage00_BossOctopus_Script = 6,
    Stage00_BossOctopus_Head_Script = 7,
    Stage00_BossOctopus_Tentacles_Script = 8,
    Stage00_BossOctopus_Girl_Script = 9,
    Stage09_Boss_Geisha_Script = 10,
    Stage09_Boss_NoFace_Script = 11,
    Stage01_Boss_Script = 12

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
    ShieldTotalDefence,
    Stage00BossDeathSmoke,
    Stage01_Boss_MoonDrums_Loop,
    Stage01_Boss_MoonDrums_LoopCrumble,
    Stage01_Boss_TeleportationIn,
    Stage01_Boss_TeleportationOut,
    Stage01_Boss_FaceChanging_WarDrums,
    Stage01_Boss_FaceChanging_LifeDrums,
    Stage01_Boss_FaceChanging_MoonDrums,
    Stage01_Boss_CrystalTomb_Effect,
    PowerUp_Shield,
    Skill_Darkening,
    Skill_Mind_1_Loop,
    Skill_Mind_2_Loop,
    Skill_Mind_2_Teleporting,
    Skill_Mind_3_In,
    Skill_Mind_3_Loop,
    Skill_Mind_3_Tentacle

}


public enum AttackParticlePhaseTypes
{
    Cast,
    Bullet,
    Hit,
    Charging,
    CastActivation,
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
    PowerUP_FullRecovery,
    PowerUP_Stamina,
    PowerUp_All,
    PowerUp_Shield,
}


public enum FacingType
{
    Left,
    Right
}

public enum AttackType
{
    Particles,
    Tile,
    Totem
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
    #region CharacterNameType
    None = 0,
    //Stage 00
    Stage00_Minion_Mountain = 1000,
    Stage00_Minion_Desert = 1001,
    Stage00_Minion_Forest = 1002,
    Stage00_Minion_Valley = 1003,
    Stage00_Character_Mountain = 1004,
    Stage00_Character_Valley = 1005,
    Stage00_Character_Forest = 1006,
    Stage00_Character_Desert = 1007,
    Stage00_BossOctopus = 1010,
    Stage00_BossOctopus_Head = 1011,
    Stage00_BossOctopus_Tentacles = 1012,
    Stage00_BossOctopus_Girl = 1013,



    //Stage 01
    Stage01_Minion_Mountain = 1020,
    Stage01_Minion_Desert = 1021,
    Stage01_Minion_Forest = 1022,
    Stage01_Minion_Valley = 1023,
    Stage01_Character_Valley = 1024,
    Stage01_Character_Desert = 1025,
    Stage01_Character_Mountain = 1026,
    Stage01_Character_Forest = 1027,
    Stage01_Boss = 1028,


    //Stage 02
    Stage02_Minion_Mountain = 1040,
    Stage02_Minion_Desert = 1041,
    Stage02_Minion_Forest = 1042,
    Stage02_Minion_Valley = 1043,
    Stage02_Character_Valley = 1044,
    Stage02_Character_Desert = 1045,
    Stage02_Character_Mountain = 1046,
    Stage02_Character_Forest = 1047,


    //Stage 03
    Stage03_Minion_Mountain = 1060,
    Stage03_Minion_Desert = 1061,
    Stage03_Minion_Forest = 1062,
    Stage03_Minion_Valley = 1063,
    Stage03_Character_Valley = 1064,
    Stage03_Character_Desert = 1065,
    Stage03_Character_Mountain = 1066,
    Stage03_Character_Forest = 1067,


    //Stage 04
    Stage04_Minion_Mountain = 1080,
    Stage04_Minion_Desert = 1081,
    Stage04_Minion_Forest = 1082,
    Stage04_Minion_Valley = 1083,
    Stage04_Character_Valley = 1084,
    Stage04_Character_Desert = 1085,
    Stage04_Character_Mountain = 1086,
    Stage04_Character_Forest = 1087,
    Stage04_BossGirl = 1088,
    Stage04_BossMonster = 1089,
    Stage04_BossGirl_Minion0 = 1090,
    Stage04_BossGirl_Minion1 = 1091,
    Stage04_BossGirl_Minion2 = 1092,
    Stage04_BossGirl_Minion3 = 1093,
    Stage04_BossMonster_Minion = 1094,
    Stage04_BossMonster_Minion0 = 1095,
    Stage04_BossMonster_Minion1 = 1096,
    Stage04_BossMonster_Minion2 = 1097,
    Stage04_BossMonster_Minion3 = 1098,


    //Stage 05
    Stage05_Minion_Mountain = 1100,
    Stage05_Minion_Desert = 1101,
    Stage05_Minion_Forest = 1102,
    Stage05_Minion_Valley = 1103,
    Stage05_Character_Mountain = 1104,
    Stage05_Character_Valley = 1105,
    Stage05_Character_Forest = 1106,
    Stage05_Character_Desert = 1107,


    //Stage 06
    Stage06_Minion_Mountain = 1120,
    Stage06_Minion_Desert = 1121,
    Stage06_Minion_Forest = 1122,
    Stage06_Minion_Valley = 1123,
    Stage06_Character_Valley = 1124,
    Stage06_Character_Desert = 1125,
    Stage06_Character_Mountain = 1126,
    Stage06_Character_Forest = 1127,


    //Stage 07
    Stage07_Minion_Mountain = 1140,
    Stage07_Minion_Desert = 1141,
    Stage07_Minion_Forest = 1142,
    Stage07_Minion_Valley = 1143,
    Stage07_Character_Valley = 1144,
    Stage07_Character_Desert = 1145,
    Stage07_Character_Mountain = 1146,
    Stage07_Character_Forest = 1147,


    //Stage 08
    Stage08_Minion_Mountain = 1160,
    Stage08_Minion_Desert = 1161,
    Stage08_Minion_Forest = 1162,
    Stage08_Minion_Valley = 1163,
    Stage08_Character_Valley = 1164,
    Stage08_Character_Desert = 1165,
    Stage08_Character_Mountain = 1166,
    Stage08_Character_Forest = 1167,


    //Stage 09
    Stage09_Minion_Mountain = 1180,
    Stage09_Minion_Desert = 1181,
    Stage09_Minion_Forest = 1182,
    Stage09_Minion_Valley = 1183,
    Stage09_Character_Valley = 1184,
    Stage09_Character_Mountain = 1185,
    Stage09_Character_Desert = 1186,
    Stage09_Character_Forest = 1187,
    Stage09_Boss_Geisha = 1188,
    Stage09_Boss_NoFace = 1189,

    DummyChar = 10001
    #endregion

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

public enum InputActionType
{
    None = 0,
    Weak,
    Strong,
    Skill1,
    Skill2,
    Skill3,
    Defend,
    Defend_Stop,
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
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

public enum AudioSourceType
{
    Any,
    Music,
    Ambience,
    Ui,
    Game,
}

public enum AudioBus
{
    Music = 1000,
    LowPrio = 0,
    MidPrio = 50,
    HighPrio = 100,
    NonSilenced = 500,
}

public enum MovementCurveType
{
    Space_Time,
    Speed_Time
}

public enum MenuNavigationType
{
    Unassigned = 0,
    None = 0,
    Relative = 1,
    Cursor = 2,
    DirectButton = 3,
}

public enum LevelType
{
    Novice,
    Defiant,
    Heroine,
    Goddess
}

public enum PowerUpColorTypes
{
    White = 0,
    Red = 1,
    Blue = 2,
    Purple = 3,
    Green = 4,
    Orange = 5,
}

public enum MaskTypes
{
    None = 0,
    Stage1 = 1,
    Stage2 = 2,
    Stage3 = 3,
    Stage4 = 4,
    Stage5 = 5,
    Stage6 = 6,
    Stage7 = 7,
    Stage8 = 8,
    Stage9 = 9,
    Stage10 = 10,
}

public enum ComboType
{
    None,
    Attack,
    Defence,
    Kill,
}

public enum StageType
{
    Story = 0,
    Pvp = 1,
}
public enum StageUnlockType
{
    locked,
    unlocking,
    unlocked,
};