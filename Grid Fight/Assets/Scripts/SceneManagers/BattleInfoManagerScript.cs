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
        //DontDestroyOnLoad(this);
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

