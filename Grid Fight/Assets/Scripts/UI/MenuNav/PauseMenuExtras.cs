using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuExtras : MonoBehaviour
{

    public void CheckOpenPause()
    {
        if(BattleManagerScript.Instance != null)
        {
            Grid_UINavigator.Instance.TriggerUIActivator("PauseGame");
        }
    }

}
