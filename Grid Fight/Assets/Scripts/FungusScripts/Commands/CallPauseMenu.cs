using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallPauseMenu",
                "Fires CallPauseMenu")]
[AddComponentMenu("")]
public class CallPauseMenu : Command
{
    #region Public members

    public override void OnEnter()
    {
        InputController.Instance.FireMinus();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

