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
    public Sprite[] CharacterIcons;
    public BaseCharType BaseCharacterType;
    public CharacterNameType CharacterID;
    public List<ScriptableObjectAttackType> CurrentParticlesAttackTypeInfo = new List<ScriptableObjectAttackType>();
    public List<ScriptableObjectAttackTypeOnBattlefield> CurrentOnBattleFieldAttackTypeInfo = new List<ScriptableObjectAttackTypeOnBattlefield>();
    public AttackParticleTypes ParticleID;
    public ElementalType Elemental;
    public CharacterClassType ClassType;
    public CharacterLevelType CharacterLevel;
    [HideInInspector]
    public CharacterSelectionType CharacterSelection;
    // public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();

    public RapidAttackClass RapidAttack;
    [System.Serializable]
    public class RapidAttackClass
    {
        public float BaseDamage = 10;
        public float Stamina_Cost_Atk;
    }

    public PowerfulAttackClass PowerfulAttac;
    [System.Serializable]
    public class PowerfulAttackClass
    {
        public float BaseDamage = 10;
        public float Stamina_Cost_Atk;
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
    }


    public DamageStastsClass DamageStats;
    [System.Serializable]
    public class DamageStastsClass
    {
       
        [HideInInspector]
        public float CurrentDamage = 10;
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

    public float AttackSpeedRatio
    {
        get
        {
            return SpeedStats.AttackSpeedRatio;
        }
        set
        {
            SpeedStats.AttackSpeedRatio = value;
        }
    }

    public float LeaveSpeed
    {
        get
        {
            return SpeedStats.LeaveSpeed;
        }
        set
        {
            SpeedStats.LeaveSpeed = value;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return SpeedStats.BulletSpeed;
        }
        set
        {
            SpeedStats.BulletSpeed = value;
        }
    }

    public float MovementSpeed
    {
        get
        {
            return SpeedStats.MovementSpeed * BaseSpeed;
        }
        set
        {
            SpeedStats.MovementSpeed = value;
        }
    }

    public float AttackSpeed
    {
        get
        {
            return SpeedStats.AttackSpeed * BaseSpeed;
        }
        set
        {
            SpeedStats.AttackSpeed = value;
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
                    DeathEvent();
                }
            }
            if(HealthStats.Health > HealthStats.Base)
            {
                Health = HealthStats.Base;
            }
        }
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
        DamageStats.CurrentDamage = RapidAttack.BaseDamage;
    }

    private void FixedUpdate()
    {
        if(Health > 0)
        {
            Stamina += StaminaStats.Regeneration / 50;
            Health += HealthStats.Regeneration / 50;
        }
    }
}
