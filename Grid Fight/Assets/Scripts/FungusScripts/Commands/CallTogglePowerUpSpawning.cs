using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Fungus;

[CommandInfo("Scripting",
                "Call Toggle Power Up Spawning",
                "Stops/Starts power up spawning")]
[AddComponentMenu("")]
public class CallTogglePowerUpSpawning : Command
{
    public bool toggleSpawningState = true;
    [ConditionalField("toggleSpawningState")] public bool specifyPowerUpSpawns = false;
    [ConditionalField("toggleSpawningState")] public List<ScriptableObjectItemPowerUps> powerUpsReplacementList = new List<ScriptableObjectItemPowerUps>();

    protected virtual void CallTheMethod()
    {
        if (toggleSpawningState) ItemSpawnerManagerScript.Instance.PlaySpawning(specifyPowerUpSpawns ? powerUpsReplacementList.ToArray() : null);
        else ItemSpawnerManagerScript.Instance.PauseSpawning();
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