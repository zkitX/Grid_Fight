using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;



[CommandInfo("Narrative",
                "SayLeft",
                "Writes text in a dialog box.")]
[AddComponentMenu("")]
public class SayLeft : Say
{
    public override void OnEnter()
    {
        setSayDialog = SayDialog.GetSayDialog("LeftDialog");
        base.OnEnter();
    }
}