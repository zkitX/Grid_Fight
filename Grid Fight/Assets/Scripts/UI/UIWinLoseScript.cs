using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIWinLoseScript : MonoBehaviour
{
    public Button PlayAgain;
    public Button SplashScreen;

    private bool selectedBtn = false; //false play again     true splash screen
    public float CoolDown = 0.5f;
    public float Offset = 0;


    private void OnEnable()
    {
        Invoke("SetEnd", 3);
    }

    private void SetEnd()
    {
        InputController.Instance.ButtonLeftUpEvent += ArrowPressed;
        InputController.Instance.ButtonRightUpEvent += ArrowPressed;
        InputController.Instance.ButtonAUpEvent += Selected;
        InputController.Instance.ButtonPlusUpEvent += Selected;
        InputController.Instance.LeftJoystickUsedEvent += JoystickUsedEvent;
        InputController.Instance.RightJoystickUsedEvent += JoystickUsedEvent;
    }

    private void JoystickUsedEvent(int player, InputDirection dir)
    {
        if ((dir == InputDirection.Left || dir == InputDirection.Right) && Time.time > Offset + CoolDown)
        {
            Offset = Time.time;
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
            SceneManager.LoadScene("SplashPage202004");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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