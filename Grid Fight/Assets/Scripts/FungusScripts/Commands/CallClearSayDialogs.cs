using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallClearSayDialogs",
                "CallStartWave")]
[AddComponentMenu("")]
public class CallClearSayDialogs : Command
{
    #region Public members
    GridFightSayDialog setSayLeftDialog;
    GridFightSayDialog setSayRightDialog;
    public override void OnEnter()
    {
        setSayLeftDialog = (GridFightSayDialog)SayDialog.GetSayDialog("LeftDialog");
        setSayRightDialog = (GridFightSayDialog)SayDialog.GetSayDialog("RightDialog");

        setSayLeftDialog.SayDialogAnimatorController.SetBool("InOut", false);
        setSayRightDialog.SayDialogAnimatorController.SetBool("InOut", false);

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

