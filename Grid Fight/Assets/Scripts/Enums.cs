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

public enum InputDirectionType
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
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
    SwitchCharacter,
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight
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
    Previous = 1000,
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
    Multiplier,
    Value
}

public enum StatusEffectType
{
    Buff = 0,
    Debuff = 1
}

public enum BuffDebuffStatsType
{
    Regen = 1,
    Drain = 5,
    Zombie = 6,
    BlockTile = 7,
    Bleed = 9,
    AttackChange = 10,
    Legion = 12,
    Invulnerable = 13,
    Rebirth = 14,
    Backfire = 15,
    Rage = 16,
    Bliss = 32,
    SoulCrash = 33,

    Health = 2,
    HealthRegeneration = 17,
    Armour = 18,

    BaseSpeed = 3,
    MovementSpeed = 4,

    Damage = 0,

    Shield = 19,
    ShieldRegeneration = 8,
    ShieldAbsorbtion = 20,
    ShieldInvulnerabilityTime = 21,
    MinionShieldChances = 22,
    MinionPerfectShieldChances = 23,



    Ether = 11,
    EtherRegeneration = 24,


    WeakBulletSpeed = 25,
    WeakAttackDamageMultiplier = 27,
    WeakAttackCriticalChance = 28,
    WeakAttackChances = 29,

    StrongBulletSpeed = 26,
    StrongAttackDamageMultiplier = 30,
    StrongAttackCriticalChance = 31,
    StrongAttackChances = 32,

}

public enum BuffDebuffStackType
{
    Refreshable = 0, //Additional BuffDebuffs of the same kind will refresh the cooldown, if they are of higher level, they will also change the level to the new higher one
    Stackable = 1,//Additional BuffDebuffs of the same kind will upgrade the level and refresh the cooldown
}

public enum StatsCheckType
{
    None,
    Health,
    Ether,
    AttackSpeed,
    MovementSpeed,
    BaseSpeed,
    TeamTotalHpPerc,
    BuffDebuff
}

public enum ModificableStatsType
{
    WeakAttack_CriticalChance = 0,
    WeakAttack_DamageMultiplier = 1,
    StrongfulAttac_CriticalChance = 2,
    StrongfulAttac_DamageMultiplier = 3,
    HealthStats_Health = 13,
    HealthStats_Base = 4,
    HealthStats_BaseHealthRegeneration = 5,
    HealthStats_Regeneration = 6,
    EtherStats_Base = 7,
    EtherStats_BaseEtherRegeneration = 8,
    EtherStats_Regeneration = 9,
    EtherStats_Ether = 10,
    SpeedStats_BaseSpeed = 11,
    SpeedStats_MovementSpeed = 12,
    SpeedStats_WeakBulletSpeed = 15,
    SpeedStats_StrongBulletSpeed = 27,
    DamageStats_BaseDamage = 16,
    ShieldStats_Shield = 17,
    ShieldStats_Base = 18,
    ShieldStats_BaseHealthRegeneration = 19,
    ShieldStats_Regeneration = 20,
    ShieldStats_ShieldOnDefence = 21,
    HealthStats_Armour = 22,
    ShieldStats_MinionShieldChances = 23,
    ShieldStats_MinionPerfectShieldChances = 24,
    ShieldStats_ShieldAbsorbtion = 25,
    ShieldStats_Invulnerability = 26
}

public enum AIType
{
    VeryDefensive = -20,
    Defensive = -10,
    Neutral = 0,
    Aggressive = 10,
    VeryAggressive = 20
}

public enum VisionType
{
    Front_Near = 10,
    Front_Far = 5,
    UpDown_Near = 1,
    UpDown_Far = 0
}


public enum AggroType
{
    Hit_Less0 = 0,
    Hit_1To2 = 3,
    Hit_3To4 = 5,
    Hit_More5 = 10

}

public enum PartyHPType
{
    Over_5 = 20,
    Over_30 = 10,
    Over_50 = 5,
    Over_90 = 0,
}


public enum DeathAnimType
{
    Explosion,
    Defeat,
    Reverse_Arrives
}

public enum ValueCheckerType
{
    LessThan,
    EqualTo,
    MoreThan,
    Between
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
public enum FieldOfViewType
{
    NearRange,
    MidRange,
    LongRange
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
    JumpTransition_OUT,
    DashUp_Intro,
    DashUp_Loop,
    DashUp_End,
    DashDown_Intro,
    DashDown_Loop,
    DashDown_End,
    DashLeft_Intro,
    DashLeft_Loop,
    DashLeft_End,
    DashRight_Intro,
    DashRight_Loop,
    DashRight_End,
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
    Stage01_Boss_Script = 12,
    Stage02_Boss_Script = 13,
    PlayerMinionType_Script = 14,

}

// 0            ->      None
// 1 to 999     ->      Basic
// 1000 to 1999 ->      Bosses extra(leave 100 slots between bosses)
// 2000 to 2999 ->      Mask Skills(leave 100 slots between masks)
// 3000 to 3499 ->      Buff Status effects Buff
// 3500 to 3999 ->      Debuff Status effects 
// 3000 to 3999 ->      Miscelaneous
// 4000+        ->      Test
public enum ParticlesType   
{
    // 0            -> None
    None = 0,

    // 1 to 999     -> Basic
    CharArrivingSmoke = 1,
    ShieldNormal = 2,
    ShieldTotalDefence = 3,

    // 1000 to 1999 ->      Bosses extra(leave 100 slots between bosses)
    Chapter00_CleasTemple_TohoraSea_BossDeathSmoke = 1000,
    Chapter01_TohoraSea_Boss_MoonDrums_Loop = 1100,
    Chapter01_TohoraSea_Boss_MoonDrums_LoopCrumble = 1101,
    Chapter01_TohoraSea_Boss_TeleportationIn = 1102,
    Chapter01_TohoraSea_Boss_TeleportationOut = 1103,
    Chapter01_TohoraSea_Boss_FaceChanging_WarDrums = 1104,
    Chapter01_TohoraSea_Boss_FaceChanging_LifeDrums = 1105,
    Chapter01_TohoraSea_Boss_FaceChanging_MoonDrums = 1106,
    Chapter01_TohoraSea_Boss_CrystalTomb_Effect = 1107,
    Chapter05_AscensoMountain_FlowersSmoke = 5000,

    // 2100 to 2999 ->      Mask Skills(leave 100 slots between masks)
    Skill_Mind_1_Loop = 2100,
    Skill_Mind_2_Loop = 2101,
    Skill_Mind_2_Teleporting = 2102,
    Skill_Mind_3_In = 2103,
    Skill_Mind_3_Loop = 2104,
    Skill_Mind_3_Tentacle = 2105,
    Skill_Might_1_LegionOriginal = 2200,
    Skill_Might_1_LegionClone = 2201,
    Skill_Might_2_Invencible = 2202,

    // 3000 to 3499 -> Buff Status effects Buff
    Status_Buff_Power = 3000,//Atk
    Status_Buff_Regen = 3001,//Health
    Status_Buff_Bliss = 3002,//stamina
    Status_Buff_Haste = 3003,//Speed
    Status_Buff_Armour = 3004,//shield
    Status_Buff_Aim = 3005,
    Status_Buff_Rebirth = 3006,
    Status_Buff_Voice = 3007,
    Status_Buff_Drain = 3008,
    Status_Buff_Piercing = 3009,
    Status_Buff_Push = 3010,

    // 3500 to 3999 -> Debuff Status effects 
    Status_Debuff_Bleed = 3500,
    Status_Debuff_SoulCrash = 3501,
    Status_Debuff_Slow = 3502,
    Status_Debuff_Blind = 3503,
    Status_Debuff_Backfire = 3504,
    Status_Debuff_Stop = 3505,
    Status_Debuff_Shatter = 3506,
    Status_Debuff_Powerless = 3507,
    Status_Debuff_Death_Sentence = 3508,
    Status_Debuff_Silence = 3509,
    Status_Debuff_Rage = 3510,
    Status_Debuff_Chain = 3511,
    Status_Debuff_Zombie = 3512,


    // 4000 to 5999
    AI_Status_VeryAgressive = 4000,
    AI_Status_Scared = 4001,


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
    CriticalHit,
    Invulnerable,
    Rebirth,
    Backfire,
    Miss
}


public enum CharacterNameType
{
    #region CharacterNameType
    None = 0,
    //Stage 00 - Clea's Temple
    CleasTemple_Minion_Mountain_BigHead = 1000,
    CleasTemple_Minion_Desert_MechDog = 1001,
    CleasTemple_Minion_Forest_BeeDoctor = 1002,
    CleasTemple_Minion_Valley_Chander = 1003,
    CleasTemple_Character_Mountain_Bird = 1004,
    CleasTemple_Character_Valley_Donna = 1005,
    CleasTemple_Character_Forest_Koniko = 1006,
    CleasTemple_Character_Desert_Pan = 1007,
    CleasTemple_BossOctopus = 1010,
    CleasTemple_BossOctopus_Head = 1011,
    CleasTemple_BossOctopus_Tentacles = 1012,
    CleasTemple_BossOctopus_Girl = 1013,



    //Stage 01 - Tohora
    Tohora_Minion_Mountain_Toka = 1020,
    Tohora_Minion_Desert_Fishylla = 1021,
    Tohora_Minion_Forest_Crabera = 1022,
    Tohora_Minion_Valley_Mothra = 1023,
    Tohora_Character_Valley_Noiti = 1024,
    Tohora_Character_Desert_MerMer = 1025,
    Tohora_Character_Mountain_Kora = 1026,
    Tohora_Character_Forest_Pai = 1027,
    Tohora_Boss_Tikaka = 1028,


    //Stage 02 - The Burg
    TheBurg_Minion_Mountain_IronTron = 1040,
    TheBurg_Minion_Desert_Robotron = 1041,
    TheBurg_Minion_Forest_HappyBot = 1042,
    TheBurg_Minion_Valley_Ted = 1043,
    TheBurg_Character_Valley_Switch = 1044,
    TheBurg_Character_Desert_Loud = 1045,
    TheBurg_Character_Mountain_Flint = 1046,
    TheBurg_Character_Forest_Deedra = 1047,
    TheBurg_Boss_Mainframe = 1048,
    TheBurg_Boss_Mainframe_Minion = 1049,


    //Stage 03 - Forest Of Kin
    ForestOfKin_Minion_Mountain = 1060,
    ForestOfKin_Minion_Desert_Wendigoat = 1061,
    ForestOfKin_Minion_Forest = 1062,
    ForestOfKin_Minion_Valley = 1063,
    ForestOfKin_Character_Valley_Elu = 1064,
    ForestOfKin_Character_Desert_Valis = 1065,
    ForestOfKin_Character_Mountain_Seke = 1066,
    ForestOfKin_Character_Forest_Balla = 1067,
    ForestOfKin_Boss_Forest_Kin = 1068,


    //Stage 05 - Ascenso Mountains
    AscensoMountains_Minion_Mountain_Bellama = 1080,
    AscensoMountains_Minion_Desert_LadyBush = 1081,
    AscensoMountains_Minion_Forest_JaguarVivern = 1082,
    AscensoMountains_Minion_Valley_Monkeyna = 1083,
    AscensoMountains_Character_Valley_Miranda = 1084,
    AscensoMountains_Character_Desert_Zoila = 1085,
    AscensoMountains_Character_Mountain_Alanda = 1086,
    AscensoMountains_Character_Forest_Zeta = 1087,
    AscensoMountains_BossGirl_Quilla = 1088,
    AscensoMountains_BossMonster_Pachamama = 1089,
    AscensoMountains_BossGirl_Quilla_Minion0 = 1090,
    AscensoMountains_BossGirl_Quilla_Minion1 = 1091,
    AscensoMountains_BossGirl_Quilla_Minion2 = 1092,
    AscensoMountains_BossGirl_Quilla_Minion3 = 1093,
    AscensoMountains_BossMonster_Pachamama_Minion = 1094,
    AscensoMountains_BossMonster_Pachamama_Minion0 = 1095,
    AscensoMountains_BossMonster_Pachamama_Minion1 = 1096,
    AscensoMountains_BossMonster_Pachamama_Minion2 = 1097,
    AscensoMountains_BossMonster_Pachamama_Minion3 = 1098,


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
    DaikiniPeaks_Minion_Mountain_Tantun = 1120,
    DaikiniPeaks_Minion_Desert_Caw = 1121,
    DaikiniPeaks_Minion_Forest_Kabuto = 1122,
    DaikiniPeaks_Minion_Valley_Panchitta = 1123,
    DaikiniPeaks_Character_Valley_Thruthsayer = 1124,
    DaikiniPeaks_Character_Desert_Hoe = 1125,
    DaikiniPeaks_Character_Mountain_Bosha = 1126,
    DaikiniPeaks_Character_Forest_TheTwins = 1127,
    DaikiniPeaks_Boss_Kala = 1128,


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
    MaidenShrine_Minion_Mountain = 1160,
    MaidenShrine_Minion_Desert = 1161,
    MaidenShrine_Minion_Forest = 1162,
    MaidenShrine_Minion_Valley = 1163,
    MaidenShrine_Character_Valley_Dorje = 1164,
    MaidenShrine_Character_Desert_LinSupreme = 1165,
    MaidenShrine_Character_Mountain_Skye = 1166,
    MaidenShrine_Character_Forest_Joja = 1167,
    MaidenShrine_Boss_Geisha = 1168,
    MaidenShrine_Boss_NoFace = 1169,


    //Stage 09
    Stage09_Minion_Mountain = 1180,
    Stage09_Minion_Desert = 1181,
    Stage09_Minion_Forest = 1182,
    Stage09_Minion_Valley = 1183,
    Stage09_Character_Valley = 1184,
    Stage09_Character_Mountain = 1185,
    Stage09_Character_Desert = 1186,
    Stage09_Character_Forest = 1187,
  

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
    PlayerNavBox = 4,
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
    Black = 6,
    DarkRed = 7,
    DarkBlue = 8,
    DarkPurple = 9,
    DarkGreen = 10,
    DarkOrange = 11,
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


public enum OptionBoxAnimType
{
    Active,
    AlreadySelected,
    Hidden
}


public enum CameraMovementType
{
    OnWorldPosition,
    OnCharacter,
    OnPlayer,
    OnTile
}