﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
                "Call SetCharOnBoardOnRandomPos",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
public class CallSetCharOnBoardOnRandomPos : Command
{
    public ControllerType playerController;
    public CharacterNameType cName;

    protected virtual void CallTheMethod()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => !r.IsOnField && r.CharInfo.CharacterID == cName).FirstOrDefault();
        if (cb != null)
        {
            BattleManagerScript.Instance.SetCharOnBoardOnRandomPos(playerController, cName);
        }
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}


