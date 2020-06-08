using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Universal Audio Profile", menuName = "ScriptableObjects/Audio/Profiles/Universal")]
public class UniversalAudioProfileSO : BaseAudioProfileSO
{
    [Header("Battle General")]
    public AudioClipInfoClass ArrivalSpawn;
    public AudioClipInfoClass ArrivalImpact;
    public AudioClipInfoClass ExitBattleJump;
    public AudioClipInfoClass StageSwitchSound;
    public AudioClipInfoClass SpecialAttackChargingLoop;
    public AudioClipInfoClass SpecialAttackChargingLoopStrong;
    public AudioClipInfoClass SpecialAttackChargingRelease;
    public AudioClipInfoClass Death;

    [Header("Shielding")]
    public AudioClipInfoClass Shield_Full;
    public AudioClipInfoClass Shield_Partial;

    [Header("Power Ups")]
    public AudioClipInfoClass PowerUp_Health;
    public AudioClipInfoClass PowerUp_Speed;
    public AudioClipInfoClass PowerUp_Stamina;
    public AudioClipInfoClass PowerUp_Damage;
    public AudioClipInfoClass PowerUp_Shield;

    [Header("UI")]
    public AudioClipInfoClass Dialogue_Entering;
    public AudioClipInfoClass Dialogue_Exiting;
    public AudioClipInfoClass Dialogue_TextStart;
    public AudioClipInfoClass Dialogue_TextEnd;
    public AudioClipInfoClass Dialogue_CharacterSwap;
    public AudioClipInfoClass Menus_SelectButton;
    public AudioClipInfoClass Menus_PressButton;

    [Header("Loading !!NOT YET IMPLEMENTED!!")]
    public AudioClipInfoClass Loading_Start;
    public AudioClipInfoClass Loading_Stop;

    [Header("Other...")]
    public AudioClipInfoClass GetReadyFight;

    protected override void OnEnable()
    {
        allAudioClips.Add(ArrivalSpawn);
        allAudioClips.Add(ArrivalImpact);
        allAudioClips.Add(SpecialAttackChargingLoop);
        allAudioClips.Add(SpecialAttackChargingRelease);
        allAudioClips.Add(Death);
        allAudioClips.Add(Shield_Full);
        allAudioClips.Add(Shield_Partial);
        allAudioClips.Add(PowerUp_Health);
        allAudioClips.Add(PowerUp_Speed);
        allAudioClips.Add(PowerUp_Stamina);
        allAudioClips.Add(PowerUp_Damage);

        ArrivalSpawn.audioPriority = AudioBus.HighPrio;
        ArrivalImpact.audioPriority = AudioBus.HighPrio;
        ExitBattleJump.audioPriority = AudioBus.HighPrio;
        StageSwitchSound.audioPriority = AudioBus.HighPrio;
        SpecialAttackChargingLoop.audioPriority = AudioBus.MidPrio;
        SpecialAttackChargingRelease.audioPriority = AudioBus.LowPrio;

        Death.audioPriority = AudioBus.HighPrio;
        Shield_Full.audioPriority = AudioBus.MidPrio;
        Shield_Partial.audioPriority = AudioBus.HighPrio;

        PowerUp_Health.audioPriority = AudioBus.HighPrio;
        PowerUp_Speed.audioPriority = AudioBus.HighPrio;
        PowerUp_Stamina.audioPriority = AudioBus.HighPrio;
        PowerUp_Damage.audioPriority = AudioBus.HighPrio;

        Dialogue_Entering.audioPriority = AudioBus.HighPrio;
        Dialogue_Exiting.audioPriority = AudioBus.HighPrio;
        Dialogue_TextStart.audioPriority = AudioBus.HighPrio;
        Dialogue_TextEnd.audioPriority = AudioBus.HighPrio;
        Dialogue_CharacterSwap.audioPriority = AudioBus.HighPrio;
        Menus_SelectButton.audioPriority = AudioBus.HighPrio;
        Menus_PressButton.audioPriority = AudioBus.HighPrio;

        Loading_Start.audioPriority = AudioBus.HighPrio;
        Loading_Stop.audioPriority = AudioBus.HighPrio;

        GetReadyFight.audioPriority = AudioBus.HighPrio;
    }
}
