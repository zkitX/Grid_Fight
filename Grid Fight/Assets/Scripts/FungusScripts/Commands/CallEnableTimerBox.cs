using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Enable Timer Box",
                "Enables the timer box with the combo counters too")]
[AddComponentMenu("")]
public class CallEnableTimerBox : Command
{
    public bool state = true;

    void TheMethod()
    {
        NewIManager.Instance.EnableTimerBox(state);
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