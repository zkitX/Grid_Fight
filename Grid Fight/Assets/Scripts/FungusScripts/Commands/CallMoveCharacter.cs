using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call Move Character",
                "Move the character in the selected directions by an amount of tiles over a set amount of time")]
[AddComponentMenu("")]
public class CallMoveCharacter : Command
{
    public CharacterNameType characterID;
    public MoveDetailsClass[] moveDetails;
    public bool holdForCompletedMove = true;

    private int comeBackCount;

    

    IEnumerator move()
    {
        BaseCharacter character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if(character == null) character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if(character == null)
        {
            Continue();
            yield break;
        }
        character.TileMovementCompleteEvent += ContinueMoves;

        foreach (MoveDetailsClass moveDetail in moveDetails)
        {
            comeBackCount = 0;
            for (int i = 0; i < moveDetail.amount; i++)
            {
                character.MoveCharOnDirection(moveDetail.nextDir);
                while(comeBackCount == i)
                {
                    yield return null;
                }
            }
        }

        character.TileMovementCompleteEvent -= ContinueMoves;
        if(holdForCompletedMove) Continue();
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

    public override void OnValidate()
    {
        foreach (MoveDetailsClass moveDetail in moveDetails)
        {
            moveDetail.Name = "Move " + moveDetail.nextDir.ToString() + " " + moveDetail.amount.ToString() + (moveDetail.amount == 1 ? " time" : " times");
        }
        base.OnValidate();
    }
    #endregion
}

[System.Serializable]
public class MoveDetailsClass
{
    [HideInInspector] public string Name = "";
    public InputDirection nextDir;
    public int amount = 0;

    public MoveDetailsClass(InputDirection dir, int moves = 1)
    {
        amount = moves;
        nextDir = dir;
    }
}