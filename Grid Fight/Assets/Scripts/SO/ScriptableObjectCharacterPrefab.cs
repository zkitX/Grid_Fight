using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterPrefab/CharacterBasePrefab")]
public class ScriptableObjectCharacterPrefab : ScriptableObject
{
    public CharacterNameType CharacterName;
    public GameObject CharacterPrefab;
    public List<Vector2Int> OccupiedTiles = new List<Vector2Int>();
}
public class ScriptableObjectArmorClass : ScriptableObject 
{
    public ArmorType Armor;
    public float Defence;
    public float MovementSpeed;
    public float Health;
}


public class ScriptableObjectWeaponClass : ScriptableObject 
{
    public WeaponType Weapon;
    public float Damage;
    public float MovementSpeed;
    public float Health;
}