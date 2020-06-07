using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialScript : MonoBehaviour
{
    private BattleState previousBattleState = BattleState.Battle;    
    private bool isSetup = false;
    // Update is called once per frame
    void Update()
    {
        if (!isSetup && InputController.Instance != null && UserInputManager.Instance != null && UserInputManager.Instance.IsReadyToBeSetUp)
        {
            isSetup = true;
            InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
            InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
        }
    }

    private void Instance_ButtonMinusUpEvent(int player)
    {
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Menus_SelectButton, AudioBus.MidPrio);

        if (BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)
        {
            return;
        }
        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.CurrentBattleState = previousBattleState;
        }
        else
        {
            previousBattleState = BattleManagerScript.Instance.CurrentBattleState;
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
