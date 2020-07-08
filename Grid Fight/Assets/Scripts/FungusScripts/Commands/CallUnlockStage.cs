using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

[CommandInfo("Scripting",
                "Call Unlock Stage",
                "Unlocks the stage specified if locked")]
[AddComponentMenu("")]
public class CallUnlockStage : Command
{
    public StageProfile stageToUnlock;

    protected virtual void CallTheMethod()
    {
        StageLoadInformation info = SceneLoadManager.Instance.loadedStages.Where(r => r.stageProfile.ID == stageToUnlock.ID).FirstOrDefault();
        if (info.lockState == StageUnlockType.locked)
        {
            info.lockState = StageUnlockType.unlocking;
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