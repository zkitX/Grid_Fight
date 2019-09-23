using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackParticle")]
public class ScriptableObjectAttackParticle : ScriptableObject
{
    public AttackParticleTypes PSType;
    public GameObject CastPS;
    public GameObject AttackPS;
    public GameObject EffectPS;
}
