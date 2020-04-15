using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Audio Profile", menuName = "ScriptableObjects/Audio/Profiles/Character")]
public class CharacterAudioProfileSO : ScriptableObject
{
    [Header("General")]
    public AudioClipInfoClass Footsteps;
    [Header("Attacks and such")]
    public CastLoopImpactAudioClipInfoClass RapidAttack;
    public CastLoopImpactAudioClipInfoClass PowerfulAttack;
    public CastLoopImpactAudioClipInfoClass PowerfulAttackChargingLoop;
    public CastLoopImpactAudioClipInfoClass Skill1;
    public CastLoopImpactAudioClipInfoClass Skill2;
    public CastLoopImpactAudioClipInfoClass Skill3;
    [Header("Arriving/Leaving")]
    public AudioClipInfoClass ArrivingCry;
    public AudioClipInfoClass Death;
    [Header("Shielding")]
    public AudioClipInfoClass MinorShield;
    public AudioClipInfoClass MajorShield;
}

[System.Serializable]
public class CastLoopImpactAudioClipInfoClass
{
    public AudioClipInfoClass Cast;
    public AudioClipInfoClass Loop;
    public AudioClipInfoClass Impact;
}
