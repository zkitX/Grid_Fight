using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Fungus;

[System.Serializable]
public class EventEffect
{
    public string Name;
    public EventEffectTypes effectType = EventEffectTypes.None;

    public delegate void FungusEventTriggerAction(string blockName);
    public static event FungusEventTriggerAction OnFungusEventTrigger;

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
            case (EventEffectTypes.TriggerFungusEvent):
                yield return TriggerFungusEvent();
                break;
            case (EventEffectTypes.TriggerCommand):
                yield return TriggerCommand();
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

    [ConditionalField("effectType", false, EventEffectTypes.TriggerFungusEvent)] public string blockName = "";
    IEnumerator TriggerFungusEvent()
    {
        if(OnFungusEventTrigger != null && OnFungusEventTrigger.Target != null) OnFungusEventTrigger(blockName);
        yield return null;
    }

    [ConditionalField("effectType", false, EventEffectTypes.TriggerCommand)] public Command command;
    IEnumerator TriggerCommand()
    {
        command.OnEnter();
        yield return null;
    }

    IEnumerator None()
    {
        yield return null;
    }


}
