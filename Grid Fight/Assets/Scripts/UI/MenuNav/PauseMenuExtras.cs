using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuExtras : MonoBehaviour
{

    public void CheckOpenPause()
    {
        if(BattleManagerScript.Instance != null && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle)
        {
            Grid_UINavigator.Instance.TriggerUIActivator("PauseGame");
        }
    }

}
