using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call Move Character To Position",
                "Move the specified character to a position in the world if navigation allows it")]
[AddComponentMenu("")]
public class CallMoveCharacterTo : Command
{
    public CharacterNameType characterID;
    public Vector2Int destination;
    public bool holdForCompletedMove = true;
    
    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        yield return BattleManagerScript.Instance.MoveCharOnPos(characterID, destination, holdForCompletedMove);
        if (!holdForCompletedMove) Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}