using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contains the basic info of the character
/// </summary>
public class CharacterInfoScript : MonoBehaviour
{
    public delegate void BaseSpeedChanged(float baseSpeed);
    public event BaseSpeedChanged BaseSpeedChangedEvent;


    public Sprite CharacterIcon;
    public CharacterClassType ClassType;
  //  public AnimationCurve Trajectory_Y;
  //  public AnimationCurve Trajectory_Z;
    public float _BulletSpeed = 5;
   // public List<Vector2Int> BulletDistanceInTile = new List<Vector2Int>();
    public float Damage = 10;
    public int MultiBulletAttackNumberOfBullets = 3;
    public float ChildrenExplosionDelay;
    public string Name;
    public CharacterSelectionType CharacterSelection;
    public CharacterLevelType CharacterLevel;
    public ControllerType PlayerController;
    public CharacterNameType CharacterName;
    public AttackParticleTypes AttackParticle;
    public ArmorClass Armor;
    public WeaponClass Weapon;
    public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
    public List<ElementalType> ElementalsPower = new List<ElementalType>();
    // public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();
    public float _AttackTimeRatio;
    public float Special2LoadingDuration;
    public float Special3LoadingDuration;
    public float Health;
    public float HealthBase;
    public float Regeneration;
    private float _BaseSpeed = 1;
    public float Stamina;
    public float StaminaBase;
    public float StaminaRegeneration;
    public float StaminaCostSpecial1;
    public float StaminaCostSpecial2;
    public float StaminaCostSpecial3;


    public float testBaseSpeed = 1;

    public float HealthPerc
    {
        get
        {
            return (Health * 100) / HealthBase;
        }
    }

    public float StaminaPerc
    {
        get
        {
            return (Stamina * 100) / StaminaBase;
        }
    }

    public float AttackTimeRatio
    {
        get
        {
            return _AttackTimeRatio / BaseSpeed;
        }
        set
        {
            _AttackTimeRatio = value;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return _BulletSpeed / BaseSpeed;
        }
        set
        {
            _BulletSpeed = value;
        }
    }

    public float BaseSpeed
    {
        get
        {
            return _BaseSpeed;
        }
        set
        {
            if(BaseSpeedChangedEvent != null)
            {
                BaseSpeedChangedEvent(value);
            }
            _BaseSpeed = value;
        }
    }


    private void Update()
    {
        BaseSpeed = testBaseSpeed;
    }
}
