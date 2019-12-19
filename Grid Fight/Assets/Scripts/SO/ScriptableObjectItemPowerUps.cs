using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemPowerUps")]
public class ScriptableObjectItemPowerUps : ScriptableObject
{
    public BuffDebuffStatsType StatsToAffect;
    public Vector2 Value;
    public Sprite Icon;
    public CharacterAnimationStateType AnimToFire;
    public float DurationOnField;
    public float EffectDuration;
}
