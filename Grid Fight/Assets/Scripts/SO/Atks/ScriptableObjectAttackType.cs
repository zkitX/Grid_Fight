﻿using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   public bool IsRandomPos = true;
//[ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;
/// </summary>

public class ScriptableObjectAttackBase : ScriptableObject
{
    public float AttackRatioMultiplier = 1;
    public Vector2 DamageMultiplier = new Vector2(1,1);
    public AttackAnimType AttackAnim;
}



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackType")]
public class ScriptableObjectAttackType : ScriptableObjectAttackBase
{
    public List<BulletBehaviourInfoClass> BulletTrajectories = new List<BulletBehaviourInfoClass>();
    public CharacterClassType CharacterClass;
}


[System.Serializable]
public class BulletBehaviourInfoClass
{
    public Vector2Int BulletDistanceInTile;
    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
    public Vector2Int BulletGapStartingTile;

    public bool HasEffect = false;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
}

