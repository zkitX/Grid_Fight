using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackTypeOnBattleField")]
public class ScriptableObjectAttackTypeOnBattlefield : ScriptableObject
{
    public List<BulletBehaviourInfoClassOnBattleField> BulletTrajectories = new List<BulletBehaviourInfoClassOnBattleField>();
    public WaveStatsType StatToCheck;
    public ValueCheckerType ValueChecker;
    public float PercToCheck;
    public float Chances;
    public float AttackRatioMultiplier = 1;
    public float DamageMultiplier = 1;
    public AttackParticleTypes ParticlesID;
}


[System.Serializable]
public class BulletBehaviourInfoClassOnBattleField
{
    public float Delay;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
}