using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;



[CommandInfo("Narrative",
                "SayRight",
                "Writes text in a dialog box.")]
[AddComponentMenu("")]
public class SayRight : Say
{
    public override void OnEnter()
    {
        setSayDialog = SayDialog.GetSayDialog("RightDialog");
        base.OnEnter();
    }

    public override void SayAndContinue(SayDialog sayDialog, string text)
    {
        sayDialog.Say(text, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {
            Continue();
        }, character);
    }
}
