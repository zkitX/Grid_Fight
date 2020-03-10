using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/AttackEffect")]
public class ScriptableObjectAttackEffect : ScriptableObject
{
    public string Name;
    public bool HasEffect = false;
    public BuffDebuffStatsType StatsToAffect;
    public StatsCheckerType StatsChecker;
    public Vector2 Value;
    public Vector2 Duration;
    public CharacterAnimationStateType AnimToFire;
    public ParticlesType Particles;
}