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
    public AudioClipInfoClass SpecialAttackChargingRelease;

    [Header("Shielding")]
    public AudioClipInfoClass Shield_Full;
    public AudioClipInfoClass Shield_Partial;

    [Header("Power Ups")]
    public AudioClipInfoClass PowerUp_Health;
    public AudioClipInfoClass PowerUp_Speed;
    public AudioClipInfoClass PowerUp_Stamina;
    public AudioClipInfoClass PowerUp_Damage;

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
        allAudioClips.Add(Shield_Full);
        allAudioClips.Add(Shield_Partial);
        allAudioClips.Add(PowerUp_Health);
        allAudioClips.Add(PowerUp_Speed);
        allAudioClips.Add(PowerUp_Stamina);
        allAudioClips.Add(PowerUp_Damage);

        ArrivalSpawn.audioBus = AudioBus.HighPrio;
        ArrivalImpact.audioBus = AudioBus.HighPrio;
        ExitBattleJump.audioBus = AudioBus.HighPrio;
        StageSwitchSound.audioBus = AudioBus.HighPrio;
        SpecialAttackChargingLoop.audioBus = AudioBus.MidPrio;
        SpecialAttackChargingRelease.audioBus = AudioBus.LowPrio;

        Shield_Full.audioBus = AudioBus.MidPrio;
        Shield_Partial.audioBus = AudioBus.HighPrio;

        PowerUp_Health.audioBus = AudioBus.MidPrio;
        PowerUp_Speed.audioBus = AudioBus.MidPrio;
        PowerUp_Stamina.audioBus = AudioBus.MidPrio;
        PowerUp_Damage.audioBus = AudioBus.MidPrio;

        Dialogue_Entering.audioBus = AudioBus.HighPrio;
        Dialogue_Exiting.audioBus = AudioBus.HighPrio;
        Dialogue_TextStart.audioBus = AudioBus.HighPrio;
        Dialogue_TextEnd.audioBus = AudioBus.HighPrio;
        Dialogue_CharacterSwap.audioBus = AudioBus.HighPrio;
        Menus_SelectButton.audioBus = AudioBus.HighPrio;
        Menus_PressButton.audioBus = AudioBus.HighPrio;

        Loading_Start.audioBus = AudioBus.HighPrio;
        Loading_Stop.audioBus = AudioBus.HighPrio;

        GetReadyFight.audioBus = AudioBus.HighPrio;
    }
}
