using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Stop Audio",
                "Stop playing a named audio clip")]
[AddComponentMenu("")]
public class CallStopAudio : Command
{
    public string audioID = "";

    protected void TheMethod()
    {
        AudioManagerMk2.Instance.StopNamedSource(audioID);
        Continue();
    }

    #region Public members

    public override void OnEnter()
    {
        TheMethod();
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
