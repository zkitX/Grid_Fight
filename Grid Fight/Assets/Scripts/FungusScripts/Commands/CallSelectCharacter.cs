using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
                "Call SelectCharacter",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
[AddComponentMenu("")]
public class CallSelectCharacter : Command
{
    public ControllerType playerController;
    public CharacterTypeScript characterToSelect;

    protected virtual void CallTheMethod()
    {
        BattleManagerScript.Instance.SelectCharacter(playerController, characterToSelect);
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
