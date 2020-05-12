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
    List<MoveDetailsClass> moveDetails = new List<MoveDetailsClass>();
    public bool holdForCompletedMove = true;

    private int comeBackCount;

    IEnumerator move()
    {
        BaseCharacter character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null) character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null)
        {
            Continue();
            yield break;
        }

        Vector2Int[] path = GridManagerScript.Pathfinding.GetPathTo(destination, character.UMS.CurrentTilePos, GridManagerScript.Instance.GetWalkableTilesLayout(character.UMS.WalkingSide));
        Vector2Int curPos = character.UMS.CurrentTilePos;
        foreach(Vector2Int movePos in path)
        {
            InputDirection direction = InputDirection.Down;
            Vector2Int move = movePos - curPos;
            if (move == new Vector2Int(1, 0)) direction = InputDirection.Down;
            else if (move == new Vector2Int(-1, 0)) direction = InputDirection.Up;
            else if (move == new Vector2Int(0, 1)) direction = InputDirection.Right;
            else if (move == new Vector2Int(0, -1)) direction = InputDirection.Left;
            moveDetails.Add(new MoveDetailsClass(direction));
            curPos = movePos;
        }

        character.TileMovementCompleteEvent += ContinueMoves;

        foreach (MoveDetailsClass moveDetail in moveDetails)
        {
            comeBackCount = 0;
            for (int i = 0; i < moveDetail.amount; i++)
            {
                character.MoveCharOnDirection(moveDetail.nextDir);
                while (comeBackCount == i)
                {
                    yield return null;
                }
            }
        }

        character.TileMovementCompleteEvent -= ContinueMoves;
        if (holdForCompletedMove) Continue();
    }

    private void ContinueMoves(BaseCharacter movingChar)
    {
        comeBackCount++;
    }

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(move());
        if (!holdForCompletedMove) Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}