using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class EventEffect
{
    public string Name;
    public EventEffectTypes effectType = EventEffectTypes.None;

    public IEnumerator PlayEffect()
    {
        switch (effectType)
        {
            case (EventEffectTypes.WaitForSeconds):
                yield return WaitForSeconds();
                break;
            case (EventEffectTypes.DebugLog):
                yield return DebugLog();
                break;
            case (EventEffectTypes.None):
                yield return None();
                break;
            default:
                yield return null;
                break;
        }
    }

    [ConditionalField("effectType", false, EventEffectTypes.WaitForSeconds)] public float secondsToWait = 1f;
    IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(secondsToWait);
    }

    [ConditionalField("effectType", false, EventEffectTypes.DebugLog)] public string debugText = "";
    IEnumerator DebugLog()
    {
        Debug.Log(debugText);
        yield return null;
    }

    IEnumerator None()
    {
        yield return null;
    }

}
