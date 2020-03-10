using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/ItemPowerUps")]
public class ScriptableObjectItemPowerUps : ScriptableObjectAttackEffect
{
    public Sprite Icon;
    public float DurationOnField;
    public float EffectDuration;
}
