using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave/WaveEvent/KillsNumber")]
public class ScriptableObjectWaveEvent_KillsNumber : ScriptableObjectWaveEvent
{

    public ScriptableObjectWaveEvent_KillsNumber()
    {
        WaveEventType = WaveEventCheckType.KillsNumber;
    }
    public int KillsNum;
}
