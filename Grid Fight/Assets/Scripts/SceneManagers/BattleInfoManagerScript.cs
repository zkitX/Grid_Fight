using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfoManagerScript : MonoBehaviour
{
    public static BattleInfoManagerScript Instance;
   
    public MatchType MatchInfoType;
    public List<CharacetrBaseInfoClass> PlayerBattleInfo = new List<CharacetrBaseInfoClass>();

    private void Awake()
    {
        Instance = this;
    }
}


[System.Serializable]
public class CharacetrBaseInfoClass
{
    public string Name;
    public ControllerType ControllerT;
    public CharacterSelectionType CharacterSelection;
    public CharacterType CT;
    public ArmorClass Armor;
    public WeaponClass Weapon;
    public AttackClass CurrentAttack;
    public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
    public List<ElementalType> ElementalsPower = new List<ElementalType>();
    public float AttackSpeed;
    public float Health;
    public float Regeneration;
    public float MovementSpeed;
    public float Stamina;
    public float StaminaRegeneration;
}


[System.Serializable]
public class ElementalResistenceClass
{
    public ElementalType Elemental;
    public ElementalWeaknessType ElementalWeakness;

    public ElementalResistenceClass(ElementalType elemental, ElementalWeaknessType elementalWeakness)
    {
        Elemental = elemental;
        ElementalWeakness = elementalWeakness;
    }
}