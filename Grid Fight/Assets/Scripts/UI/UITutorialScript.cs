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
            InputController.Instance.ButtonXUpEvent += Instance_ButtonXUpEvent;
            InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
            gameObject.SetActive(false);
        }
    }

    private void Instance_ButtonMinusUpEvent(int player)
    {
        BattleManagerScript.Instance.CurrentBattleState = previousBattleState;
        gameObject.SetActive(false);
    }

    private void Instance_ButtonXUpEvent(int player)
    {
        if(BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            return;
        }
        previousBattleState = BattleManagerScript.Instance.CurrentBattleState;
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Pause;
        gameObject.SetActive(true);
    }
    private void Instance_ButtonPlusUpEvent(int player)
    {
        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.RestartScene();
        }
    }
}
