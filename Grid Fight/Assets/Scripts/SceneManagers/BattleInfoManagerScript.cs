using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfoManagerScript : MonoBehaviour
{
    public static BattleInfoManagerScript Instance;
   
    public MatchType MatchInfoType;
    public List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    /*
      PvE,
    PvP,
    PPvE,
    PPvPP
         */
    public void SetupMatchInfoType(int v)
    {
        MatchInfoType = (MatchType)v;
    }

}


[System.Serializable]
public class CharacterBaseInfoClass
{
    public string Name;
    public CharacterSelectionType CharacterSelection;
    public CharacterLevelType CharacterLevel;
    public ControllerType playerController;
    public CharacterNameType CharacterName;
    public AttackParticleTypes AttackParticle;
    public ArmorClass Armor;
    public WeaponClass Weapon;
    public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
    public List<ElementalType> ElementalsPower = new List<ElementalType>();
    public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();
    public float AttackTimeRatio;
    public float Special2LoadingDuration;
    public float Special3LoadingDuration; 
    public float Health;
    public float HealthBase;
    public float Regeneration;
    public float BaseSpeed = 1;
    public float Stamina;
    public float StaminaBase;
    public float StaminaRegeneration;
    public float StaminaCostSpecial1;
    public float StaminaCostSpecial2;
    public float StaminaCostSpecial3;


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
}


[System.Serializable]
public class ElementalResistenceClass
{

    public ElementalWeaknessType ElementalWeakness
    {
        get
        {
            return _ElementalWeakness;
        }
        set
        {
            _ElementalWeakness = value;
        }
    }

    public ElementalType Elemental;
    public ElementalWeaknessType _ElementalWeakness;

    public ElementalResistenceClass()
    {

    }
    public ElementalResistenceClass(ElementalType elemental, ElementalWeaknessType elementalWeakness)
    {
        Elemental = elemental;
        ElementalWeakness = elementalWeakness;
    }
}

[System.Serializable]
public class CharactersRelationshipClass
{
    public RelationshipType Relationship;
    public CharacterNameType CharacterName;

    public CharactersRelationshipClass()
    {
    }

    public CharactersRelationshipClass(RelationshipType relationship, CharacterNameType characterName)
    {
        Relationship = relationship;
        CharacterName = characterName;
    }
}

