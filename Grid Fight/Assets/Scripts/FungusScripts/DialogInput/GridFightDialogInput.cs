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

        InputController.Instance.ButtonADownEvent += Instance_ButtonADownEvent;
    }

    private void Instance_ButtonADownEvent(int player)
    {
        if(BattleManagerScript.Instance.FungusState == FungusDialogType.Dialog)
        {
            SetNextLineFlag();
        }
    }
}
