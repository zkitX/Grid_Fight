using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Switch Character Out For",
                "Switch out the currently selected character for another in their squad")]
[AddComponentMenu("")]
public class CallSwitchCharacterOutFor : Command
{
    public ControllerType playerToSwitch = ControllerType.Player1;
    public bool randomiseSwitch = false;
    [ConditionalField("randomiseSwitch", inverse: true)] public CharacterNameType charToSwitchTo = CharacterNameType.None;

    protected void TheMethod()
    {
        if (BattleManagerScript.Instance.AllCharactersOnField.Where(r => !r.IsOnField).ToArray().Length == 0)
        {
            Debug.LogError("Could not switch character as there are none available");
            return;
        }
        BaseCharacter charSwitch = null;
        if (randomiseSwitch) charSwitch = BattleManagerScript.Instance.AllCharactersOnField.Where(r => !r.IsOnField).ToArray()[Random.Range(0, BattleManagerScript.Instance.AllCharactersOnField.Where(r => !r.IsOnField).ToList().Count)];
        else charSwitch = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == charToSwitchTo).FirstOrDefault();
        if(charSwitch == null)
        {
            Debug.LogError("Could not switch to " + charToSwitchTo.ToString() + " as this character does not exist in the stage or is already selected");
            return;
        }

        CharacterNameType charSwitchID = randomiseSwitch ? charSwitch.CharInfo.CharacterID : charToSwitchTo;
        //BattleManagerScript.Instance.CurrentSelectedCharacters[playerToSwitch].NextSelectionChar.NextSelectionChar = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r)
        BattleManagerScript.Instance.LoadingNewCharacterToGrid(charSwitchID, SideType.LeftSide, playerToSwitch);
    }

    #region Public members

    public override void OnEnter()
    {
        TheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    public override void OnValidate()
    {
        base.OnValidate();
    }
    #endregion
}
