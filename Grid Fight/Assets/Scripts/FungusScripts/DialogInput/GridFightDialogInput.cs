using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;


public class GridFightDialogInput : DialogInput
{

    private void Start()
    {
        StartCoroutine(LookingForRewired());
    }

    IEnumerator LookingForRewired()
    {
        while (InputController.Instance == null)
        {
            yield return null;
        }

        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
    }

    private void Instance_ButtonAUpEvent(int player)
    {
        if (BattleManagerScript.Instance == null || BattleManagerScript.Instance.FungusState == FungusDialogType.Dialog)
        {
            SetNextLineFlag();
        }
    }
}
