using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call GameSequenceEvent",
                "Triggers an active event from the Game Sequence Event system")]
[AddComponentMenu("")]
public class CallGameSequenceEvent : Command
{
    public GameSequenceEvent eventToCall;

    protected virtual void CallTheMethod()
    {
        EventManager.Instance.CallEventDirectly(eventToCall.Name);
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}
