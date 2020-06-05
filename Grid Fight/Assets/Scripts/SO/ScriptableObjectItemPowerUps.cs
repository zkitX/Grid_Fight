using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/ItemPowerUps")]
public class ScriptableObjectItemPowerUps : ScriptableObjectAttackEffect
{
    public GameObject prefab;
    public string powerUpText = "D";
    public PowerUpColorTypes color = PowerUpColorTypes.White;
    public GameObject activeParticles = null;
    public GameObject terminationParticles = null;
    public float DurationOnField;
    [HideInInspector] public float EffectDuration = 1f; //TODO Remove later

}