﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWinLoseScript : MonoBehaviour
{
    public Button PlayAgain;
    public Button SplashScreen;

    private bool selectedBtn = false; //false play again     true splash screen


    private void OnEnable()
    {
        InputController.Instance.ButtonLeftUpEvent += ArrowPressed;
        InputController.Instance.ButtonRightUpEvent += ArrowPressed;
        InputController.Instance.ButtonAUpEvent += Selected;
        InputController.Instance.LeftJoystickUsedEvent += JoystickUsedEvent;
        InputController.Instance.RightJoystickUsedEvent += JoystickUsedEvent;
    }

    private void JoystickUsedEvent(int player, InputDirection dir)
    {
        if (dir == InputDirection.Left || dir == InputDirection.Right)
        {
            selectedBtn = !selectedBtn;
            UpdateBtn();
        }
    }

    private void ArrowPressed(int player)
    {
        selectedBtn = !selectedBtn;
        UpdateBtn();
    }

    private void Selected(int player)
    {
        if (selectedBtn)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SplashScreenDemo");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateBtn()
    {
        if(selectedBtn)
        {

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(SplashScreen.gameObject);
            SplashScreen.Select();
        }
        else
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(PlayAgain.gameObject);
            PlayAgain.Select();
        }

    }
}