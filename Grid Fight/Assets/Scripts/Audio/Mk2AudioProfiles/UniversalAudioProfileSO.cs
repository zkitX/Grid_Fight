using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Universal Audio Profile", menuName = "ScriptableObjects/Audio/Profiles/Universal")]
public class UniversalAudioProfileSO : BaseAudioProfileSO
{
    [Header("Battle General")]
    public AudioClipInfoClass ArrivalSpawn;
    public AudioClipInfoClass ArrivalImpact;
    public AudioClipInfoClass SpecialAttackChargingLoop;
    public AudioClipInfoClass SpecialAttackChargingRelease;

    [Header("Shielding")]
    public AudioClipInfoClass BasicShield;
    public AudioClipInfoClass MegaShield;

    [Header("Power Ups")]
    public AudioClipInfoClass PowerUp_Health;
    public AudioClipInfoClass PowerUp_Speed;
    public AudioClipInfoClass PowerUp_Stamina;
    public AudioClipInfoClass PowerUp_Damage;

    protected override void OnEnable()
    {
        allAudioClips.Add(ArrivalSpawn);
        allAudioClips.Add(ArrivalImpact);
        allAudioClips.Add(SpecialAttackChargingLoop);
        allAudioClips.Add(SpecialAttackChargingRelease);
        allAudioClips.Add(BasicShield);
        allAudioClips.Add(MegaShield);
        allAudioClips.Add(PowerUp_Health);
        allAudioClips.Add(PowerUp_Speed);
        allAudioClips.Add(PowerUp_Stamina);
        allAudioClips.Add(PowerUp_Damage);
    }
}
