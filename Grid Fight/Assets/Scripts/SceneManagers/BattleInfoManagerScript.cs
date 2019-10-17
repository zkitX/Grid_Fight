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
    public ControllerType playerController;
    public CharacterType CT;
    public AttackParticleTypes AttackParticle;
    public ArmorClass Armor;
    public WeaponClass Weapon;
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