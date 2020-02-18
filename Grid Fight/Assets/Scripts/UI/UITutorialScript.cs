using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialScript : MonoBehaviour
{
    private BattleState previousBattleState;    
    private bool isSetup = false;
    // Update is called once per frame
    void Update()
    {
        if(!isSetup && InputController.Instance != null)
        {
            isSetup = true;

            InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
            InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
            gameObject.SetActive(false);
        }
    }

    private void Instance_ButtonMinusUpEvent(int player)
    {
        if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.Pause)
        {
            return;
        }
        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        }
        else
        {
            BattleManagerScript.Instance.CurrentBattleState = BattleState.Pause;
        }
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void Instance_ButtonPlusUpEvent(int player)
    {
        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.RestartScene();
        }
    }
}
