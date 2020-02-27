using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call RemoveControllerCharacter",
                "Removes and deselects the currently selected character for a given controller")]
[AddComponentMenu("")]
public class CallRemoveControllerCharacter : Command
{
    public ControllerType[] controllers;
    public SideType Side;
    protected virtual void CallTheMethod()
    {
        CharacterType_Script charToRemove = null;
        foreach (ControllerType controller in controllers)
        {
            charToRemove = BattleManagerScript.Instance.CurrentSelectedCharacters[controller].Character;
            if(charToRemove != new CharacterType_Script() || charToRemove != null)
            {
                BattleManagerScript.Instance.RemoveNamedCharacterFromBoard(charToRemove.CharInfo.CharacterID);
                BattleManagerScript.Instance.DeselectCharacter(charToRemove.CharInfo.CharacterID, Side, controller);
            }
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
