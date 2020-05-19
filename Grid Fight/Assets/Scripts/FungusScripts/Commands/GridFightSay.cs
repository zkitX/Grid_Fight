using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;



[CommandInfo("Narrative",
                "GridFight Say",
                "Writes text in a dialog box.")]
[AddComponentMenu("")]
public class GridFightSay : Say
{
    public SideType Side;
    GridFightSayDialog GridFight_Say;
    public override void OnEnter()
    {
        GridFight_Say = (GridFightSayDialog)SayDialog.GetSayDialog(Side == SideType.LeftSide ? "LeftDialog" : "RightDialog");
        if (!showAlways && executionCount >= showCount)
        {
            Continue();
            return;
        }

        executionCount++;

        if (GridFight_Say == null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Dialogue_Entering, AudioBus.MidPrio);
        }

        var flowchart = GetFlowchart();

        GridFight_Say.SetCharacter(character);

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


        StartCoroutine(Say_Continue(subbedText));
    }

    private IEnumerator Say_Continue(string text)
    {
        GridFight_Say.SayDialogAnimatorController.SetBool("IsSelected", false);
        yield return GridFight_Say.Say(text, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, character);
        Continue();
    }
}
