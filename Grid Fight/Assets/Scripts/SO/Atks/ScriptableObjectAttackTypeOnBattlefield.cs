using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackTypeOnBattleField")]
public class ScriptableObjectAttackTypeOnBattlefield : ScriptableObjectAttackBase
{
    public BattleFieldAttackType AtkType;
    public List<BulletBehaviourInfoClassOnBattleFieldClass> BulletTrajectories = new List<BulletBehaviourInfoClassOnBattleFieldClass>();
    public WaveStatsType StatToCheck;
    public ValueCheckerType ValueChecker;
    public float PercToCheck;
    public float Chances;
}


[System.Serializable]
public class BulletBehaviourInfoClassOnBattleFieldClass
{
    public float Delay;
    public List<BattleFieldAttackTileClass> BulletEffectTiles = new List<BattleFieldAttackTileClass>();
}
[System.Serializable]
public class BattleFieldAttackTileClass
{

    [HideInInspector] public Vector2Int Pos;
    public bool HasEffect = false;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    public bool HasDifferentParticles = false;
    [ConditionalField("HasDifferentParticles", false)] public AttackParticleType ParticlesID;

    public BattleFieldAttackTileClass(Vector2Int pos)
    {
        Pos = pos;
    }
}