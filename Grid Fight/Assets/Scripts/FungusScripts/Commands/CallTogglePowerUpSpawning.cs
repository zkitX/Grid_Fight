using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call Toggle Power Up Spawning",
                "Stops/Starts power up spawning")]
[AddComponentMenu("")]
public class CallTogglePowerUpSpawning : Command
{
    public bool toggleSpawningState = true;

    protected virtual void CallTheMethod()
    {
        if(toggleSpawningState) ItemSpawnerManagerScript.Instance.PlaySpawning();
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