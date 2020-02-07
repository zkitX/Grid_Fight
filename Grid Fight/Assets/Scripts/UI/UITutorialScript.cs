using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialScript : MonoBehaviour
{
    private bool isSetup = false;
    // Update is called once per frame
    void Update()
    {
        if(!isSetup && InputController.Instance != null)
        {
            isSetup = true;

            InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
        }
    }

    private void Instance_ButtonMinusUpEvent(int player)
    {
        if(BattleManagerScript.Instance != null && BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            InputController.Instance.ButtonMinusUpEvent -= Instance_ButtonMinusUpEvent;
            if (BattleManagerScript.Instance.usingFungus)
            {
                BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;
            }
            else
            {
                BattleManagerScript.Instance.CurrentBattleState = BattleState.Intro;
            }

            gameObject.SetActive(false);
        }
    }
}
