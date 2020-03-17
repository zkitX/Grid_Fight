using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contains the basic info of the character
/// </summary>
public class CharacterInfoScript : MonoBehaviour
{


    #region Events
    public delegate void BaseSpeedChanged(float baseSpeed);
    public event BaseSpeedChanged BaseSpeedChangedEvent;
    public delegate void Death();
    public event Death DeathEvent;
    #endregion


    public string Name;
    [Tooltip("Should contain 2 for the HealthUI, one for deselected(0) and one for selected(1)")] public Sprite[] CharacterIcons;
    public BaseCharType BaseCharacterType;
    public CharacterNameType CharacterID;
    public List<ScriptableObjectAttackType> CurrentParticlesAttackTypeInfo = new List<ScriptableObjectAttackType>();
    public List<ScriptableObjectAttackTypeOnBattlefield> CurrentOnBattleFieldAttackTypeInfo = new List<ScriptableObjectAttackTypeOnBattlefield>();
    public AttackParticleType ParticleID;
    public ElementalType Elemental;
    public CharacterClassType ClassType;
    public CharacterLevelType CharacterLevel;
    public bool UseLayeringSystem = true;

    [HideInInspector]
    public CharacterSelectionType CharacterSelection;
    // public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();

    public RapidAttackClass RapidAttack;
    [System.Serializable]
    public class RapidAttackClass
    {
        public Vector2 DamageMultiplier = new Vector2(1,1);
        public Vector2 CriticalChance = new Vector2(2,2);
        public float Stamina_Cost_Atk;

        [HideInInspector] public Vector2 B_DamageMultiplier = new Vector2(1, 1);
        [HideInInspector] public Vector2 B_CriticalChance = new Vector2(2, 2);
        [HideInInspector] public float B_Stamina_Cost_Atk;
    }

    public PowerfulAttackClass PowerfulAttac;
    [System.Serializable]
    public class PowerfulAttackClass
    {
        public Vector2 DamageMultiplier = new Vector2(3,3);
        public Vector2 CriticalChance = new Vector2(3, 5);
        public float Stamina_Cost_Atk;

        [HideInInspector] public Vector2 B_DamageMultiplier = new Vector2(3, 3);
        [HideInInspector] public Vector2 B_CriticalChance = new Vector2(3, 5);
        [HideInInspector] public float B_Stamina_Cost_Atk;
    }

    public HealthStastsClass HealthStats;
    [System.Serializable]
    public class HealthStastsClass
    {
        public float Health;
        public float Base;
        public float Regeneration;
        public float BaseHealthRegeneration;
        public float LevelMultiplier;

        [HideInInspector] public float B_Health;
        [HideInInspector] public float B_Base;
        [HideInInspector] public float B_Regeneration;
        [HideInInspector] public float B_BaseHealthRegeneration;
        [HideInInspector] public float B_LevelMultiplier;
    }

    public StaminaStastsClass StaminaStats;
    [System.Serializable]
    public class StaminaStastsClass
    {
        public float Stamina;
        public float Base;
        public float Regeneration;
        public float BaseStaminaRegeneration;
        public float LevelMultiplier;

        [HideInInspector] public float B_Stamina;
        [HideInInspector] public float B_Base;
        [HideInInspector] public float B_Regeneration;
        [HideInInspector] public float B_BaseStaminaRegeneration;
        [HideInInspector] public float B_LevelMultiplier;
    }

    public SpeedStastsClass SpeedStats;
    [System.Serializable]
    public class SpeedStastsClass
    {
        public float BaseSpeed = 1;
        public float MovementSpeed = 1;
        public float AttackSpeed = 1;
        public float AttackSpeedRatio;
        public float BulletSpeed = 5;
        public float LeaveSpeed = 3;
        public float LevelMultiplier;
        public float IdleToAtkDuration = 0.2f;
        public float AtkToIdleDuration = 0.2f;

        [HideInInspector] public float B_BaseSpeed = 1;
        [HideInInspector] public float B_MovementSpeed = 1;
        [HideInInspector] public float B_AttackSpeed = 1;
        [HideInInspector] public float B_AttackSpeedRatio;
        [HideInInspector] public float B_BulletSpeed = 5;
        [HideInInspector] public float B_LeaveSpeed = 3;
        [HideInInspector] public float B_LevelMultiplier;
        [HideInInspector] public float B_IdleToAtkDuration = 0.2f;
        [HideInInspector] public float B_AtkToIdleDuration = 0.2f;
    }


    public DamageStastsClass DamageStats;
    [System.Serializable]
    public class DamageStastsClass
    {
        public float BaseDamage = 10;
        [HideInInspector]
        public int MultiBulletAttackNumberOfBullets = 3;
        public float ChildrenBulletDelay;
        [HideInInspector]
        public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
        [HideInInspector]
        public ElementalType CurrentElemental;
        public float LevelMultiplier;
    }

    public DefenceStastsClass DefenceStats;
    [System.Serializable]
    public class DefenceStastsClass
    {
        public float BaseDefence = 10;
        public float Invulnerability = 0.2f;
    }

    public float Special1LoadingDuration;
    public float Special2LoadingDuration;
    public float Special3LoadingDuration;

    public Vector2 MovementTimer = new Vector2(5, 8);

    public float HealthPerc
    {
        get
        {
            return (Health * 100) / HealthStats.Base;
        }
    }

    public float StaminaPerc
    {
        get
        {
            return (StaminaStats.Stamina * 100) / StaminaStats.Base;
        }
    }

    public float BaseSpeed
    {
        get
        {
            return SpeedStats.BaseSpeed;
        }
        set
        {
            if(BaseSpeedChangedEvent != null)
            {
                BaseSpeedChangedEvent(value);
            }
            SpeedStats.BaseSpeed = value;
        }
    }

    public float Health
    {
        get
        {
            return HealthStats.Health;
        }
        set
        {
            HealthStats.Health = value;
            if (HealthStats.Health <= 0)
            {
                HealthStats.Health = HealthStats.Health <= 0 ? 0 : HealthStats.Health;
                if (DeathEvent != null)
                {
                    Invoke("SetCharDeath", 0.2f);
                }
            }
            if(HealthStats.Health > HealthStats.Base)
            {
                Health = HealthStats.Base;
            }
        }
    }

    private void SetCharDeath()
    {
        DeathEvent();
    }

    public float Stamina
    {
        get
        {
            return StaminaStats.Stamina <= 0 ? 0 : StaminaStats.Stamina;
        }
        set
        {
            StaminaStats.Stamina = value;
            if (StaminaStats.Stamina > StaminaStats.Base)
            {
                Stamina = StaminaStats.Base;
            }
        }
    }


    private void Awake()
    {

        //RapidAttack
        RapidAttack.B_CriticalChance = RapidAttack.CriticalChance;
        RapidAttack.B_DamageMultiplier = RapidAttack.DamageMultiplier;
        RapidAttack.B_Stamina_Cost_Atk = RapidAttack.Stamina_Cost_Atk;

        //PowerfulAttac
        PowerfulAttac.B_CriticalChance = PowerfulAttac.CriticalChance;
        PowerfulAttac.B_DamageMultiplier = PowerfulAttac.DamageMultiplier;
        PowerfulAttac.B_Stamina_Cost_Atk = PowerfulAttac.Stamina_Cost_Atk;

        //HealthStats
        HealthStats.B_Base = HealthStats.Base;
        HealthStats.B_BaseHealthRegeneration = HealthStats.BaseHealthRegeneration;
        HealthStats.B_Health = HealthStats.Health;
        HealthStats.B_LevelMultiplier = HealthStats.LevelMultiplier;
        HealthStats.B_Regeneration = HealthStats.Regeneration;

        //StaminaStats
        StaminaStats.B_Base = StaminaStats.Base;
        StaminaStats.B_BaseStaminaRegeneration = StaminaStats.BaseStaminaRegeneration;
        StaminaStats.B_LevelMultiplier = StaminaStats.LevelMultiplier;
        StaminaStats.B_Regeneration = StaminaStats.Regeneration;
        StaminaStats.B_Stamina = StaminaStats.Stamina;

        //SpeedStats
        SpeedStats.B_BaseSpeed = SpeedStats.BaseSpeed;
        SpeedStats.B_MovementSpeed = SpeedStats.MovementSpeed;
        SpeedStats.B_AttackSpeed = SpeedStats.AttackSpeed;
        SpeedStats.B_AttackSpeedRatio = SpeedStats.AttackSpeedRatio;
        SpeedStats.B_BulletSpeed = SpeedStats.BulletSpeed;
        SpeedStats.B_LeaveSpeed = SpeedStats.LeaveSpeed;
        SpeedStats.B_LevelMultiplier = SpeedStats.LevelMultiplier;
        SpeedStats.B_IdleToAtkDuration = SpeedStats.IdleToAtkDuration;
        SpeedStats.B_AtkToIdleDuration = SpeedStats.AtkToIdleDuration;
    }


    private void FixedUpdate()
    {
        if(Health > 0)
        {
            Stamina = StaminaStats.Stamina + StaminaStats.Regeneration / 50;
            Health = HealthStats.Health + HealthStats.Regeneration / 50;
        }
    }

    public bool IsCritical(bool rapidOrPowerful)
    {
        float chance = Random.Range(0, 100);
        if (chance <= Random.Range(rapidOrPowerful ? RapidAttack.CriticalChance.x : PowerfulAttac.CriticalChance.x,
            rapidOrPowerful ? RapidAttack.CriticalChance.y : PowerfulAttac.CriticalChance.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
