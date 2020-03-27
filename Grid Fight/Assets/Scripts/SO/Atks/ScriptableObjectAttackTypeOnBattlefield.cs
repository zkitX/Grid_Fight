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
    public bool Show = true;
    [HideInInspector]public List<BattleFieldAttackTileClass> BulletEffectTiles = new List<BattleFieldAttackTileClass>();
}
[System.Serializable]
public class BattleFieldAttackTileClass
{
    [HideInInspector] public Vector2Int Pos;
    public bool HasEffect = false;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    [ConditionalField("HasEffect", false)] public float EffectChances = 100;
    public bool HasDifferentParticles = false;
    [ConditionalField("HasDifferentParticles", false)] public AttackParticleType ParticlesID;
    [ConditionalField("HasDifferentParticles", false)] public bool IsEffectOnTile = false;
    [ConditionalField("IsEffectOnTile", false)] public ParticlesType TileParticlesID;
    [ConditionalField("IsEffectOnTile", false)] public float DurationOnTile;

    public BattleFieldAttackTileClass(Vector2Int pos)
    {
        Pos = pos;
    }

    public BattleFieldAttackTileClass(Vector2Int pos, bool hasEffect, List<ScriptableObjectAttackEffect> effects, bool hasDifferentParticles, AttackParticleType particlesID, bool isEffectOnTile,
        ParticlesType tileParticlesID, float durationOnTile)
    {
        Pos = pos;
        HasEffect = hasEffect;
        Effects = effects;
        HasDifferentParticles = hasDifferentParticles;
        ParticlesID = particlesID;
        IsEffectOnTile = isEffectOnTile;
        TileParticlesID = tileParticlesID;
        DurationOnTile = durationOnTile;
    }
}