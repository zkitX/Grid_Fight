using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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