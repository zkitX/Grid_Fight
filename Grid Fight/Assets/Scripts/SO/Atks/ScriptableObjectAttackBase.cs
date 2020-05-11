using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   public bool IsRandomPos = true;
//[ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/atkbase")]
public class ScriptableObjectAttackBase : ScriptableObject
{
    public int Chances = 100;
    public AttackType CurrentAttackType;
    public float AttackRatioMultiplier = 1;
    public Vector2 DamageMultiplier = new Vector2(1, 1);
    public AttackInputType AttackInput;
    public AttackAnimType AttackAnim;
    [HideInInspector] public AttackAnimPrefixType PrefixAnim;

    public AttackParticlesClass Particles;
    [System.Serializable]
    public class AttackParticlesClass
    {
        public AttackParticlesSideClass Left;
        public AttackParticlesSideClass Right;
        [System.Serializable]
        public class AttackParticlesSideClass
        {
            public GameObject Cast;
            public GameObject Bullet;
            public GameObject Hit;
        }

        public GameObject CastLoopPS;
        public GameObject CastActivationPS;
    }


    public int TrajectoriesNumber;
    [HideInInspector] public ParticlesAttackTypeClass ParticlesAtk;
    [HideInInspector] public TilesAttackTypeClass TilesAtk;


    private void OnEnable()
    {
        switch (AttackAnim)
        {
            case AttackAnimType.Weak_Atk:
                PrefixAnim = AttackAnimPrefixType.Atk1;
                break;
            case AttackAnimType.Strong_Atk:
                PrefixAnim = AttackAnimPrefixType.Atk2;
                break;
            case AttackAnimType.Boss_Atk3:
                PrefixAnim = AttackAnimPrefixType.Atk3;
                break;
            case AttackAnimType.Buff:
                PrefixAnim = AttackAnimPrefixType.S_Buff;
                break;
            case AttackAnimType.Debuff:
                PrefixAnim = AttackAnimPrefixType.S_DeBuff;
                break;
            default:
                break;
        }
    }
}


#region Tiles


[System.Serializable]
public class TilesAttackTypeClass
{
    public BattleFieldAttackType AtkType;
    public List<BulletBehaviourInfoClassOnBattleFieldClass> BulletTrajectories = new List<BulletBehaviourInfoClassOnBattleFieldClass>();
    public WaveStatsType StatToCheck;
    public ValueCheckerType ValueChecker;
    public float PercToCheck;
    public int Chances;
}


[System.Serializable]
public class BulletBehaviourInfoClassOnBattleFieldClass
{
    public float Delay;
    public bool Show = true;
    [HideInInspector] public List<BattleFieldAttackTileClass> BulletEffectTiles = new List<BattleFieldAttackTileClass>();
}
[System.Serializable]
public class BattleFieldAttackTileClass
{
    [HideInInspector] public Vector2Int Pos;

    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;


    public bool HasEffect = false;
    public bool showImpact = true;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    [ConditionalField("HasEffect", false)] public float EffectChances = 100;
    public bool IsEffectOnTile = false;
    [ConditionalField("IsEffectOnTile", false)] public ParticlesType TileParticlesID;
    [ConditionalField("IsEffectOnTile", false)] public float DurationOnTile;

    public BattleFieldAttackTileClass(Vector2Int pos)
    {
        Pos = pos;
    }

    public BattleFieldAttackTileClass(Vector2Int pos, bool hasEffect, List<ScriptableObjectAttackEffect> effects, bool isEffectOnTile,
        ParticlesType tileParticlesID, float durationOnTile)
    {
        Pos = pos;
        HasEffect = hasEffect;
        Effects = effects;
        IsEffectOnTile = isEffectOnTile;
        TileParticlesID = tileParticlesID;
        DurationOnTile = durationOnTile;
    }
}

#endregion

#region Particles

[System.Serializable]
public class ParticlesAttackTypeClass
{
    public List<BulletBehaviourInfoClass> BulletTrajectories = new List<BulletBehaviourInfoClass>();
    public CharacterClassType CharacterClass;
}


[System.Serializable]
public class BulletBehaviourInfoClass
{
    [HideInInspector] public bool Show;
    public Vector2Int BulletDistanceInTile;
    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
    public Vector2Int BulletGapStartingTile;

    public bool HasEffect = false;
    public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    public float EffectChances = 100;

}
#endregion

