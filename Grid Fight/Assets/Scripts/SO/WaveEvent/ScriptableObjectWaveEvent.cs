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
