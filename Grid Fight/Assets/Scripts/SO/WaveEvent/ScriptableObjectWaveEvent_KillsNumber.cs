using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Wave/WaveEvent/CharDied")]
public class ScriptableObjectWaveEvent_CharDied : ScriptableObjectWaveEvent
{
    public ScriptableObjectWaveEvent_CharDied()
    {
        WaveEventType = WaveEventCheckType.CharDied;
    }
    public List<CharacterNameType> CharactersID;
}
