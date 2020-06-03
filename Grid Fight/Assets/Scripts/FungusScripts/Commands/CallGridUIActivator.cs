using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call GridUI Activator",
                "Trigger Activator from the Grid UI system")]
[AddComponentMenu("")]
public class CallGridUIActivator : Command
{
    public string ActivatorID = "";

    void TheMethod()
    {
        Grid_UINavigator.Instance.TriggerUIActivator(ActivatorID);
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