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