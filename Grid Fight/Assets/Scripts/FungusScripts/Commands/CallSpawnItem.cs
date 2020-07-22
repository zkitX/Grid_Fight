using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallSpawnItem",
                "CallSpawnItem")]
[AddComponentMenu("")]
public class CallSpawnItem : Command
{
    public float WaitingTime = 1;
    public ScriptableObjectItemPowerUps ItemEffect;
    public bool isRandomPos = false;
    [ConditionalField("isRandomPos", true)] public Vector2Int Pos;
    [ConditionalField("isRandomPos", false)] public WalkingSideType WalkingSide;
    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(WaitFor());
    }

    IEnumerator WaitFor()
    {
        yield return BattleManagerScript.Instance.WaitFor(WaitingTime, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        if(isRandomPos)
        {
            ItemSpawnerManagerScript.Instance.SpawnItemRandomPos(ItemEffect, WalkingSide);
        }
        else
        {
            ItemSpawnerManagerScript.Instance.SpawnItem(ItemEffect, GridManagerScript.Instance.GetBattleTile(Pos));
        }
        
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

