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

    private int comeBackCount;

    IEnumerator move()
    {
        Debug.Log("Meant to move");
        BaseCharacter character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if(character == null) character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if(character == null)
        {
            Continue();
            yield break;
        }
        Debug.Log("Meant to move");
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
        Continue();
    }

    private void ContinueMoves(BaseCharacter movingChar)
    {
        comeBackCount++;
    }

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(move());
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

[System.Serializable]
public class MoveDetailsClass
{
    public InputDirection nextDir;
    public int amount = 0;
}