using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectWaveEvent : ScriptableObject
{
    [HideInInspector]
    public WaveEventCheckType WaveEventType;
    public string FungusBlockName;
    [HideInInspector]
    public bool isUsed;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave/WaveEvent/CharStatsCheckInPerc")]
public class ScriptableObjectWaveEvent_CharStatsCheckInPerc : ScriptableObjectWaveEvent
{
    public ScriptableObjectWaveEvent_CharStatsCheckInPerc()
    {
        WaveEventType = WaveEventCheckType.CharStatsCheckInPerc;
    }

    public List<CharacterNameType> CharactersID;

    public WaveStatsType StatToCheck;
    public ValueCheckerType ValueChecker;
    public float PercToCheck;
    
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave/WaveEvent/CharDied")]
public class ScriptableObjectWaveEvent_CharDied : ScriptableObjectWaveEvent
{
    public ScriptableObjectWaveEvent_CharDied()
    {
        WaveEventType = WaveEventCheckType.CharDied;
    }
    public List<CharacterNameType> CharactersID;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave/WaveEvent/KillsNumber")]
public class ScriptableObjectWaveEvent_KillsNumber : ScriptableObjectWaveEvent
{

    public ScriptableObjectWaveEvent_KillsNumber()
    {
        WaveEventType = WaveEventCheckType.KillsNumber;
    }
    public int KillsNum;
}


public enum WaveEventCheckType
{
    CharStatsCheckInPerc,
    CharDied,
    KillsNumber

}