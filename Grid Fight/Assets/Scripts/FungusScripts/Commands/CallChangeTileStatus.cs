using Fungus;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CommandInfo("Scripting",
                "Call CallChangeTileStatus",
                "CharInfo")]
[AddComponentMenu("")]
public class CallChangeTileStatus : Command
{

    public Vector2Int Pos;
    public BattleTileStateType State;

    #region Public members

    public override void OnEnter()
    {
        GridManagerScript.Instance.SetBattleTileState(Pos, State);
        Continue();
    }

   

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}
