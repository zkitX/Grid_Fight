using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackTypeOnBattleField")]
public class ScriptableObjectAttackTypeOnBattlefield : ScriptableObject
{
    public bool IsAttackStartingFromCharacter = false;
    public List<BulletBehaviourInfoClassOnBattleField> BulletTrajectories = new List<BulletBehaviourInfoClassOnBattleField>();
}


[System.Serializable]
public class BulletBehaviourInfoClassOnBattleField
{
    public float Delay;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
}