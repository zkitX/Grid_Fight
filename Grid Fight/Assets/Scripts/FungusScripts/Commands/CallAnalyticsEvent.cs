using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Analytics Event",
                "Track a custom event in analytics")]
[AddComponentMenu("")]
public class CallAnalyticsEvent : Command
{
    public string CustomEventName = "FUNGUS_EVENT_NAME";

    protected void TheMethod()
    {
        AnalyticsManager.Instance.Track_FungusEventReached(CustomEventName);
    }

    #region Public members

    public override void OnEnter()
    {
        TheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    public override void OnValidate()
    {
        base.OnValidate();
    }
    #endregion
}