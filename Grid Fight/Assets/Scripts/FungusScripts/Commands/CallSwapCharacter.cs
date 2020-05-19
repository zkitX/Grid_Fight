using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
                "Call SwapCharacter",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
[AddComponentMenu("")]
public class CallSwapCharacter : Command
{

    public ControllerType playerController;
    public bool isRandomChar = false;
    [ConditionalField("isRandomChar", true)] public CharacterNameType characterID;
    //Call the swap of the selected player character with the char we set or with a random one
    protected virtual void CallTheMethod()
    {
        if(isRandomChar)
        {
            BattleManagerScript.Instance.Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType.Down, playerController, isRandomChar, true);
        }
        else
        {
            BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).First();
            BattleManagerScript.Instance.SetNextChar(true, cb, cb.UMS.Side, playerController, cb.CharInfo.CharacterSelection, true);
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
