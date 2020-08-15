using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Tooltip("Should contain 2 for the HealthUI, one for deselected(0) and one for selected(1)")]
    public Sprite CharacterIcon;
    public ScriptableObjectSkillMask Mask;
    public CharacterAudioProfileSO AudioProfile;
    public BaseCharType BaseCharacterType;
    public CharacterNameType CharacterID;
    public List<AttackSequence> attackSequences = new List<AttackSequence>();
    public ScriptableObjectAttackBase[] NextAttackSequence
    {
        get
        {
            AttackSequence atkSq = attackSequences.Where(r => r.CheckTrigger(this)).FirstOrDefault();
            return atkSq != null ? atkSq.GetAttackSequence() : null;
        }
    }
    [Tooltip("Attacks must follow this sequence (particles attack)  WEAK/STRONG/Skills (enemy attack) no order")]
    public List<ScriptableObjectAttackBase> _CurrentAttackTypeInfo = new List<ScriptableObjectAttackBase>();

    public List<ScriptableObjectAttackBase> CurrentAttackTypeInfo
    {
        get
        {
            List<ScriptableObjectAttackBase> res = _CurrentAttackTypeInfo.ToList();
            res.AddRange(AddedAttackTypeInfo);
            return res;
        }
        set
        {
            _CurrentAttackTypeInfo = value;
        }
    }
    public List<ScriptableObjectAttackBase> AddedAttackTypeInfo = new List<ScriptableObjectAttackBase>();
    public ElementalType Elemental;
    public CharacterClassType ClassType;
    public LevelType CharaterLevel;
    public bool UseLayeringSystem = true;
    public Transform Head;

    public List<ScriptableObjectAI> AIs = new List<ScriptableObjectAI>();

    public List<LevelsInfoClass> Levels = new List<LevelsInfoClass>
    {
        new LevelsInfoClass(LevelType.Novice, 0),
        new LevelsInfoClass(LevelType.Defiant, 1000),
        new LevelsInfoClass(LevelType.Heroine, 3600),
        new LevelsInfoClass(LevelType.Goddess, 9850)
    };

    [Tooltip("Length of time, in seconds, that the character will take to respawn once killed")]
    public float CharacterRespawnLength = 180f;

    [HideInInspector]
    public CharacterSelectionType CharacterSelection;
    // public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();




    public HealthStastsClass HealthStats;
    [System.Serializable]
    public class HealthStastsClass
    {
        public float Health;
        public float Base;
        public float Regeneration;
        public float BaseHealthRegeneration;
        public float Armour = 1;
        public float LevelMultiplier;

        [HideInInspector] public float B_Health;
        [HideInInspector] public float B_Base;
        [HideInInspector] public float B_Regeneration;
        [HideInInspector] public float B_BaseHealthRegeneration;
        [HideInInspector] public float B_Armour = 1;

    }

    public ShieldStastsClass ShieldStats;
    [System.Serializable]
    public class ShieldStastsClass
    {
        public float Shield = 50;
        public float Base = 50;
        public float Regeneration = 8;
        public float BaseShieldRegeneration = 8;
        [Range(0, 1)]
        public float ShieldAbsorbtion = 0.5f;
        public float Invulnerability = 0.2f;
        public float MinionShieldChances = 20;
        public float MinionPerfectShieldChances = 5;


        public float LevelMultiplier = 50;


        [HideInInspector] public float B_Shield;
        [HideInInspector] public float B_Base;
        [HideInInspector] public float B_Regeneration;
        [HideInInspector] public float B_BaseShieldRegeneration;
        [HideInInspector] public float B_ShieldAbsorbtion;
        [HideInInspector] public float B_Invulnerability = 0.2f;
        [HideInInspector] public float B_MinionShieldChances = 20;
        [HideInInspector] public float B_MinionPerfectShieldChances = 5;
    }

 

    public EtherStastsClass EtherStats;
    [System.Serializable]
    public class EtherStastsClass
    {
        public float Ether = 40;
        public float Base = 40;
        public float Regeneration = 1;
        public float BaseEtherRegeneration = 1;
        public float LevelMultiplier = 30;

        [HideInInspector] public float B_Stamina;
        [HideInInspector] public float B_Base;
        [HideInInspector] public float B_Regeneration;
        [HideInInspector] public float B_BaseStaminaRegeneration;
    }

   
    public SpeedStastsClass SpeedStats;
    [System.Serializable]
    public class SpeedStastsClass
    {
        public float BaseSpeed = 1;
        [Range(0, 2)]
        public float TileMovementTime = 1;

        public float MovementSpeed = 1;
        public float CuttingPerc = 0.85f;

        public float IntroPerc = 0.15f;
        public float LoopPerc = 0.70f;
        public float EndPerc = 0.15f;

        [Range(0, 1)]
        public float AttackLoopDuration = 0.5f;
        [Range(0, 1)]
        public float IdleToAttackDuration = 0.01f;
        public float LeaveSpeed = 3;
        public float BaseSpeed_LevelMultiplier;
        public float MovementSpeed_LevelMultiplier;
        public float IdleToAtkDuration = 0.2f;
        public float AtkToIdleDuration = 0.2f;

        public float WeakBulletSpeed = 5;
        public float StrongBulletSpeed = 5;


        [HideInInspector] public float B_BaseSpeed = 1;
        [HideInInspector] public float B_MovementSpeed = 1;
        [HideInInspector] public float B_AttackSpeed = 1;
        [HideInInspector] public float B_LeaveSpeed = 3;
        [HideInInspector] public float B_IdleToAtkDuration = 0.2f;
        [HideInInspector] public float B_AtkToIdleDuration = 0.2f;
        [HideInInspector] public float B_WeakBulletSpeed = 5;
        [HideInInspector] public float B_StrongBulletSpeed = 5;

    }

    public DamageStastsClass DamageStats;
    [System.Serializable]
    public class DamageStastsClass
    {
        public float BaseDamage = 10;
        [HideInInspector] public float B_BaseDamage = 10f;
        [HideInInspector] public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
        [HideInInspector] public ElementalType CurrentElemental;
        public float LevelMultiplier;
    }


    public WeakAttackClass WeakAttack;
    [System.Serializable]
    public class WeakAttackClass
    {
        public Vector2 DamageMultiplier = new Vector2(1, 1);
        public Vector2 CriticalChance = new Vector2(2, 2);
        public Vector2 Chances = new Vector2(1,1);
        public float LevelMultiplier;

        [HideInInspector] public Vector2 B_DamageMultiplier = new Vector2(1, 1);
        [HideInInspector] public Vector2 B_CriticalChance = new Vector2(2, 2);
        [HideInInspector] public Vector2 B_Chances = new Vector2(2, 2);
    }

    public StrongfulAttackClass StrongAttack;
    [System.Serializable]
    public class StrongfulAttackClass
    {
        public Vector2 DamageMultiplier = new Vector2(3, 3);
        public Vector2 CriticalChance = new Vector2(3, 5);
        public Vector2 Chances = new Vector2(1, 1);

        public float LevelMultiplier;

        [HideInInspector] public Vector2 B_DamageMultiplier = new Vector2(3, 3);
        [HideInInspector] public Vector2 B_CriticalChance = new Vector2(3, 5);
        [HideInInspector] public Vector2 B_Chances = new Vector2(2, 2);

    }



    [HideInInspector] public float ExperienceValue;
    [HideInInspector] public Vector2 MovementTimer = new Vector2(5, 8);
    [HideInInspector] public Vector2 B_MovementTimer = new Vector2(5, 8);


    [Header("Relationship")]
    public List<RelationshipClass> RelationshipList = new List<RelationshipClass>();


    public float HealthPerc
    {
        get
        {
            return (Health * 100) / HealthStats.Base;
        }
    }

    public float EtherPerc
    {
        get
        {
            return (Ether * 100) / EtherStats.Base;
        }
    }

    public float ShieldPerc
    {
        get
        {
            return (ShieldStats.Shield * 100) / ShieldStats.Base;
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
            if (BaseSpeedChangedEvent != null)
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
            if (HealthStats.Health > HealthStats.Base)
            {
                HealthStats.Health = HealthStats.Base;
            }
        }
    }

    private void SetCharDeath()
    {
        DeathEvent();
    }

    public float Ether
    {
        get
        {
            return EtherStats.Ether <= 0 ? 0 : EtherStats.Ether;
        }
        set
        {
            EtherStats.Ether = value;
            if (value < 0)
            {
                Ether = Ether < 0 ? 0 : Ether;
            }
            if (EtherStats.Ether > EtherStats.Base)
            {
                EtherStats.Ether = EtherStats.Base;
            }
        }
    }

    public float Shield
    {
        get
        {
            return ShieldStats.Shield <= 0 ? 0 : ShieldStats.Shield;
        }
        set
        {
            ShieldStats.Shield = Mathf.Clamp(value, 0f, ShieldStats.Base);
            if (ShieldStats.Shield > ShieldStats.Base)
            {
                Shield = ShieldStats.Base;
            }
        }
    }


    private void FixedUpdate()
    {
        if (Health > 0)
        {
            Ether = BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle ? EtherStats.Ether + EtherStats.Regeneration / 50 : EtherStats.Ether;
            Health = BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle ? HealthStats.Health + HealthStats.Regeneration / 50 : HealthStats.Health;
            Shield = BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle ? ShieldStats.Shield + ShieldStats.Regeneration / 50 : ShieldStats.Shield;
        }
    }

    public bool IsCritical(bool rapidOrPowerful)
    {
        float chance = Random.Range(0, 100);
        if (chance <= Random.Range(rapidOrPowerful ? WeakAttack.CriticalChance.x : StrongAttack.CriticalChance.x,
            rapidOrPowerful ? WeakAttack.CriticalChance.y : StrongAttack.CriticalChance.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void SetupChar(float strengthScaler = 1f)
    {
        for (int i = 0; i < (int)CharaterLevel; i++)
        {
            HealthStats.Base *= HealthStats.LevelMultiplier * strengthScaler;
            HealthStats.Health *= HealthStats.LevelMultiplier * strengthScaler;
            HealthStats.Regeneration *= HealthStats.LevelMultiplier * strengthScaler;
            HealthStats.BaseHealthRegeneration *= HealthStats.LevelMultiplier * strengthScaler;

            EtherStats.Base *= EtherStats.LevelMultiplier * strengthScaler;
            EtherStats.Regeneration *= EtherStats.LevelMultiplier * strengthScaler;
            EtherStats.Ether *= EtherStats.LevelMultiplier * strengthScaler;
            EtherStats.BaseEtherRegeneration *= EtherStats.LevelMultiplier * strengthScaler;

            ShieldStats.Base *= ShieldStats.LevelMultiplier * strengthScaler;
            ShieldStats.Regeneration *= ShieldStats.LevelMultiplier * strengthScaler;
            ShieldStats.Shield *= ShieldStats.LevelMultiplier * strengthScaler;
            ShieldStats.BaseShieldRegeneration *= ShieldStats.LevelMultiplier * strengthScaler;
            ShieldStats.ShieldAbsorbtion *= ShieldStats.LevelMultiplier * strengthScaler;


            SpeedStats.BaseSpeed *= SpeedStats.BaseSpeed_LevelMultiplier;
            SpeedStats.TileMovementTime /= 1 + SpeedStats.MovementSpeed_LevelMultiplier;

            HealthStats.Armour *= HealthStats.LevelMultiplier * strengthScaler;

            DamageStats.BaseDamage *= DamageStats.LevelMultiplier * strengthScaler;
        }


        //RapidAttack
        WeakAttack.B_CriticalChance = WeakAttack.CriticalChance;
        WeakAttack.B_DamageMultiplier = WeakAttack.DamageMultiplier;
        WeakAttack.B_Chances = WeakAttack.Chances;

        //PowerfulAttac
        StrongAttack.B_CriticalChance = StrongAttack.CriticalChance;
        StrongAttack.B_DamageMultiplier = StrongAttack.DamageMultiplier;
        StrongAttack.B_Chances = StrongAttack.Chances;

        //HealthStats
        HealthStats.B_Base = HealthStats.Base;
        HealthStats.Health = HealthStats.Base;
        HealthStats.B_BaseHealthRegeneration = HealthStats.BaseHealthRegeneration;
        HealthStats.B_Health = HealthStats.Health;
        HealthStats.B_Regeneration = HealthStats.Regeneration;
        HealthStats.B_Armour = HealthStats.Armour;

        //StaminaStats
        EtherStats.B_Base = EtherStats.Base;
        EtherStats.B_BaseStaminaRegeneration = EtherStats.BaseEtherRegeneration;
        EtherStats.B_Regeneration = EtherStats.Regeneration;
        EtherStats.B_Stamina = EtherStats.Ether;

        //Shield
        ShieldStats.B_Base = ShieldStats.Base;
        ShieldStats.B_BaseShieldRegeneration = ShieldStats.BaseShieldRegeneration;
        ShieldStats.B_Regeneration = ShieldStats.Regeneration;
        ShieldStats.B_Shield = ShieldStats.Shield;
        ShieldStats.B_ShieldAbsorbtion = ShieldStats.ShieldAbsorbtion;
        ShieldStats.B_Invulnerability = ShieldStats.Invulnerability;
        ShieldStats.B_MinionShieldChances = ShieldStats.MinionShieldChances;
        ShieldStats.B_MinionPerfectShieldChances = ShieldStats.MinionPerfectShieldChances;

        //SpeedStats
        SpeedStats.B_BaseSpeed = SpeedStats.BaseSpeed;
        SpeedStats.B_MovementSpeed = SpeedStats.MovementSpeed;
        SpeedStats.B_LeaveSpeed = SpeedStats.LeaveSpeed;
        SpeedStats.B_IdleToAtkDuration = SpeedStats.IdleToAtkDuration;
        SpeedStats.B_AtkToIdleDuration = SpeedStats.AtkToIdleDuration;
        SpeedStats.B_WeakBulletSpeed = SpeedStats.WeakBulletSpeed;
        SpeedStats.B_WeakBulletSpeed = SpeedStats.WeakBulletSpeed;
        SpeedStats.B_StrongBulletSpeed = SpeedStats.StrongBulletSpeed;

        DamageStats.B_BaseDamage = DamageStats.BaseDamage;
       
    }

    public ScriptableObjectAI GetCurrentAI(List<AggroInfoClass> enemies, Vector2Int currentPos, BaseCharacter bChar, ref BaseCharacter target)
    {
        target = null;
        List<AIInfoCLass> aisInfo = new List<AIInfoCLass>();

        int charTargeting = 0;
        foreach (ScriptableObjectAI item in AIs)
        {
            aisInfo.Add(new AIInfoCLass(item, item.CheckAvailability(bChar, enemies, currentPos, ref target)));
            charTargeting += Mathf.Abs(aisInfo.Last().Score);
        }

        foreach (AIInfoCLass item in aisInfo)
        {
            int resI = Random.Range(0, charTargeting);

            if (resI <= Mathf.Abs(item.Score))
            {
                return item.AI;
            }
            else
            {
                charTargeting -= Mathf.Abs(item.Score);
            }
        }


        return AIs.First();
    }


    private void OnValidate()
    {
        foreach (AttackSequence atkSeq in attackSequences)
        {
            atkSeq.GenerateName();
        }
    }

}


public class LevelsInfoClass
{
    public LevelType Level;
    public float ExpNeeded;

    public LevelsInfoClass()
    {

    }

    public LevelsInfoClass(LevelType level, float expNeeded)
    {
        Level = level;
        ExpNeeded = expNeeded;
    }
}


public class AIInfoCLass
{
    public ScriptableObjectAI AI;
    public int Score;

    public AIInfoCLass()
    {

    }

    public AIInfoCLass(ScriptableObjectAI ai, int score)
    {
        AI = ai;
        Score = score;
    }
}
