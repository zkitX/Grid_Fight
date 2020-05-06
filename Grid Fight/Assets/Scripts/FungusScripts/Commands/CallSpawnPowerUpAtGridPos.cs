using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call SpawnPowerUpAtGridPos",
                "Spawns a powerup at the given grid position")]
[AddComponentMenu("")]
public class CallSpawnPowerUpAtGridPos : Command
{
    public ScriptableObjectItemPowerUps powerUp;
    public bool hasGenericDuration = true;
    [ConditionalField("hasGenericDuration", true, false)] public float durationOnField = 10f;
    public bool randomisePosition = true;
    [ConditionalField("randomisePosition", true, false)] public Vector2Int[] spawnGridPos;
    [ConditionalField("randomisePosition", false)] public WalkingSideType sideToSpawnOn = WalkingSideType.Both;
    [ConditionalField("randomisePosition", false)] public int amountToSpawnRandomly = 1;

    protected virtual void CallTheMethod()
    {
        if (randomisePosition)
        {
            for (int i = 0; i < amountToSpawnRandomly; i++)
                ItemSpawnerManagerScript.Instance.SpawnPowerUpAtRandomPointOnSide(powerUp, sideToSpawnOn, hasGenericDuration ? 0f : durationOnField);
        }
        else
        {
            foreach (Vector2Int gridPos in spawnGridPos)
                ItemSpawnerManagerScript.Instance.SpawnPowerUpAtGridPos(powerUp, gridPos, hasGenericDuration ? 0f : durationOnField);
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
