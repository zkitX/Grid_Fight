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
    GridFightSayDialog RightSay;
    public override void OnEnter()
    {
        setSayDialog = SayDialog.GetSayDialog("LeftDialog");
        RightSay = (GridFightSayDialog)SayDialog.GetSayDialog("RightDialog");
        if (!showAlways && executionCount >= showCount)
        {
            Continue();
            return;
        }

        executionCount++;

        if (setSayDialog == null && RightSay == null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Dialogue_Entering, AudioBus.MidPrio);
        }

        // Override the active say dialog if needed
        if (character != null && character.SetSayDialog != null)
        {
            SayDialog.ActiveSayDialog = character.SetSayDialog;
        }

        if (setSayDialog != null)
        {
            SayDialog.ActiveSayDialog = setSayDialog;
        }

        var sayDialog = SayDialog.GetSayDialog();
        if (sayDialog == null)
        {
            Continue();
            return;
        }

        var flowchart = GetFlowchart();

        sayDialog.SetActive(true);

        sayDialog.SetCharacter(character);
        

        string displayText = storyText;

        var activeCustomTags = CustomTag.activeCustomTags;
        for (int i = 0; i < activeCustomTags.Count; i++)
        {
            var ct = activeCustomTags[i];
            displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
            if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
            {
                displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
            }
        }

        string subbedText = flowchart.SubstituteVariables(displayText);


        StartCoroutine(Say_Continue(sayDialog, subbedText));
    }
    
    private IEnumerator Say_Continue(SayDialog sayDialog, string text)
    {
        RightSay.SayDialogAnimatorController.SetBool("IsSelected", false);
        yield return sayDialog.Say(text, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, character);
        Continue();
    }

}