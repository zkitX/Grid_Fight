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
    public Sprite CharacterIcon;
    public BaseCharType BaseCharacterType;
    public CharacterNameType CharacterID;
    public ScriptableObjectAttackType CurrentAttackTypeInfo;
    public AttackParticleTypes ParticleID;
    public ElementalType Elemental;
    public CharacterClassType ClassType;
    public CharacterLevelType CharacterLevel;
    [HideInInspector]
    public CharacterSelectionType CharacterSelection;
    [HideInInspector]
    public List<ControllerType> PlayerController = new List<ControllerType>();
    // public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();

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
        public float Stamina_Cost_Atk;
        public float Stamina_Cost_S_Atk01;
        public float Stamina_Cost_S_Atk02;
        public float Stamina_Cost_S_Atk03;
        public float Stamina_Cost_S_Atk04;
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
        public float LevelMultiplier;
    }


    public DamageStastsClass DamageStats;
    [System.Serializable]
    public class DamageStastsClass
    {
        public float BaseDamage = 10;
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

    public float Special1LoadingDuration;
    public float Special2LoadingDuration;
    public float Special3LoadingDuration;



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
                if (DeathEvent != null)
                {
                    DeathEvent();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        StaminaStats.Stamina = (StaminaStats.Stamina + StaminaStats.Regeneration / 50) > StaminaStats.Base ? StaminaStats.Base : (StaminaStats.Stamina + StaminaStats.Regeneration / 50);
        Health = Health;
    }
}
