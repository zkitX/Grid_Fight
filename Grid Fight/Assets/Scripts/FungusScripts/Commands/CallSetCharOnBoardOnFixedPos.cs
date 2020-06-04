using System.Collections;
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
                "Call SetCharOnBoardOnFixedPos",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
public class CallSetCharOnBoardOnFixedPos : Command
{
    public ControllerType playerController;
    public CharacterNameType cName;
    public Vector2Int pos;

    protected virtual void CallTheMethod()
    {
        StartCoroutine(callSetChar_Co());
    }

    IEnumerator callSetChar_Co()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == cName).FirstOrDefault();
        if (cb != null)
        {
            if (!cb.IsOnField)
            {
                BaseCharacter cbOnPos = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.Pos.Contains(pos)).FirstOrDefault();
                if (!GridManagerScript.Instance.isPosFree(pos) || cbOnPos != null)
                {
                    yield return BattleManagerScript.Instance.MoveCharOnPos(cbOnPos.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeTilesAdjacentTo(pos, 2, true, cbOnPos.UMS.WalkingSide).First().Pos, true);
                }

                BattleManagerScript.Instance.SetCharOnBoard(playerController, cName, pos);
            }
            else
            {
                yield return BattleManagerScript.Instance.MoveCharOnPos(cName, pos, true);
            }
        }
        else
        {
            cb = BattleManagerScript.Instance.CharsForTalkingPart.Where(r => !r.IsOnField && r.CharInfo.CharacterID == cName).FirstOrDefault();

            if (cb == null)
            {
                cb = BattleManagerScript.Instance.CreateTalkingChar(cName);
            }

            BaseCharacter cbOnPos = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.Pos.Contains(pos)).FirstOrDefault();
            if (!GridManagerScript.Instance.isPosFree(pos) || cbOnPos != null)
            {
                yield return BattleManagerScript.Instance.MoveCharOnPos(cbOnPos.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeTilesAdjacentTo(pos, 1, true, cbOnPos.UMS.WalkingSide).First().Pos, true);
            }

            BattleManagerScript.Instance.SetCharOnBoard(playerController, cName, pos, false);
           
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


