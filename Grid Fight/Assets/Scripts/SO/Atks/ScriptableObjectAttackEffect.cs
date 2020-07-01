﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/AttackEffect")]
public class ScriptableObjectAttackEffect : ScriptableObject
{

    public float Duration
    {
        get
        {
            return UnityEngine.Random.Range(_Duration.x, _Duration.y);
        }
    }

    public string Name;
    public BuffDebuffStatsType StatsToAffect;
    public StatsCheckerType StatsChecker;
    public Vector2 Value;
    public Vector2 _Duration;
    public CharacterAnimationStateType AnimToFire;
    public ParticlesType Particles;
    [HideInInspector] public ScriptableObjectAttackBase Atk;
}