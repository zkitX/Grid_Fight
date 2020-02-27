using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System;

public class GridFightSayDialog : SayDialog
{
    private Character LastCharacter = null;
    [SerializeField]
    private Animator SayDialogAnimatorController;

    protected override void Awake()
    {
        closeOtherDialogs = false;
        base.Awake();
    }

    public override IEnumerator DoSay(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, Action onComplete, Character nextChar)
    {
        while (BattleManagerScript.Instance == null)
        {
            yield return null;
        }

        if(LastCharacter != null && LastCharacter.name != nextChar.name)
        {
            SayDialogAnimatorController.SetBool("InOut", false);
            yield return null; 
        }

        SayDialogAnimatorController.SetBool("InOut", true);




        SayDialogAnimatorController.SetBool("IsSelected", true);
        LastCharacter = nextChar;


        BattleManagerScript.Instance.FungusState = FungusDialogType.Dialog;
        yield return base.DoSay(text, clearPrevious, waitForInput, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, onComplete);

        SayDialogAnimatorController.SetBool("IsSelected", false);
    }
}
