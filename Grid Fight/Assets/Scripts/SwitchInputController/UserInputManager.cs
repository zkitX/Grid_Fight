using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputManager : MonoBehaviour
{
    private void Start()
    {
        #region Button Down
        InputController.Instance.ButtonADownEvent += Instance_ButtonADownEvent;
        InputController.Instance.ButtonBDownEvent += Instance_ButtonBDownEvent;
        InputController.Instance.ButtonXDownEvent += Instance_ButtonXDownEvent;
        InputController.Instance.ButtonYDownEvent += Instance_ButtonYDownEvent;
        InputController.Instance.ButtonUpDownEvent += Instance_ButtonUpDownEvent;
        InputController.Instance.ButtonDownDownEvent += Instance_ButtonDownDownEvent;
        InputController.Instance.ButtonRightDownEvent += Instance_ButtonRightDownEvent;
        InputController.Instance.ButtonLeftDownEvent += Instance_ButtonLeftDownEvent;
        InputController.Instance.ButtonRDownEvent += Instance_ButtonRDownEvent;
        InputController.Instance.ButtonZRDownEvent += Instance_ButtonZRDownEvent;
        InputController.Instance.ButtonLDownEvent += Instance_ButtonLDownEvent;
        InputController.Instance.ButtonZLDownEvent += Instance_ButtonZLDownEvent;
        InputController.Instance.ButtonMinusDownEvent += Instance_ButtonMinusDownEvent;
        InputController.Instance.ButtonPlusDownEvent += Instance_ButtonPlusDownEvent;
        InputController.Instance.ButtonHomeDownEvent += Instance_ButtonHomeDownEvent;
        InputController.Instance.ButtonCaptureDownEvent += Instance_ButtonCaptureDownEvent;
        InputController.Instance.ButtonLeftStickDownEvent += Instance_ButtonLeftStickDownEvent;
        InputController.Instance.ButtonRightStickDownEvent += Instance_ButtonRightStickDownEvent;
        InputController.Instance.ButtonLeftSLDownEvent += Instance_ButtonLeftSLDownEvent;
        InputController.Instance.ButtonRightSLDownEvent += Instance_ButtonRightSLDownEvent;
        InputController.Instance.ButtonLeftSRDownEvent += Instance_ButtonLeftSRDownEvent;
        InputController.Instance.ButtonRightSRDownEvent += Instance_ButtonRightSRDownEvent;
        #endregion


        #region Button Press
        InputController.Instance.ButtonAPressedEvent += Instance_ButtonAPressedEvent;
        InputController.Instance.ButtonBPressedEvent += Instance_ButtonBPressedEvent;
        InputController.Instance.ButtonXPressedEvent += Instance_ButtonXPressedEvent;
        InputController.Instance.ButtonYPressedEvent += Instance_ButtonYPressedEvent;
        InputController.Instance.ButtonUpPressedEvent += Instance_ButtonUpPressedEvent;
        InputController.Instance.ButtonDownPressedEvent += Instance_ButtonDownPressedEvent;
        InputController.Instance.ButtonRightPressedEvent += Instance_ButtonRightPressedEvent;
        InputController.Instance.ButtonLeftPressedEvent += Instance_ButtonLeftPressedEvent;
        InputController.Instance.ButtonRPressedEvent += Instance_ButtonRPressedEvent;
        InputController.Instance.ButtonZRPressedEvent += Instance_ButtonZRPressedEvent;
        InputController.Instance.ButtonLPressedEvent += Instance_ButtonLPressedEvent;
        InputController.Instance.ButtonZLPressedEvent += Instance_ButtonZLPressedEvent;
        InputController.Instance.ButtonPlusPressedEvent += Instance_ButtonPlusPressedEvent;
        InputController.Instance.ButtonMinusPressedEvent += Instance_ButtonMinusPressedEvent;
        InputController.Instance.ButtonHomePressedEvent += Instance_ButtonHomePressedEvent;
        InputController.Instance.ButtonCapturePressedEvent += Instance_ButtonCapturePressedEvent;
        InputController.Instance.ButtonLeftStickPressedEvent += Instance_ButtonLeftStickPressedEvent;
        InputController.Instance.ButtonRightStickPressedEvent += Instance_ButtonRightStickPressedEvent;
        InputController.Instance.ButtonLeftSLPressedEvent += Instance_ButtonLeftSLPressedEvent;
        InputController.Instance.ButtonRightSLPressedEvent += Instance_ButtonRightSLPressedEvent;
        InputController.Instance.ButtonRightSRPressedEvent += Instance_ButtonRightSRPressedEvent;
        InputController.Instance.ButtonLeftSRPressedEvent += Instance_ButtonLeftSRPressedEvent;
        #endregion

        #region Button Up
        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
        InputController.Instance.ButtonBUpEvent += Instance_ButtonBUpEvent;
        InputController.Instance.ButtonXUpEvent += Instance_ButtonXUpEvent;
        InputController.Instance.ButtonYUpEvent += Instance_ButtonYUpEvent;
        InputController.Instance.ButtonUpUpEvent += Instance_ButtonUpUpEvent;
        InputController.Instance.ButtonDownUpEvent += Instance_ButtonDownUpEvent;
        InputController.Instance.ButtonRightUpEvent += Instance_ButtonRightUpEvent;
        InputController.Instance.ButtonLeftUpEvent += Instance_ButtonLeftUpEvent;
        InputController.Instance.ButtonRUpEvent += Instance_ButtonRUpEvent;
        InputController.Instance.ButtonZRUpEvent += Instance_ButtonZRUpEvent;
        InputController.Instance.ButtonLUpEvent += Instance_ButtonLUpEvent;
        InputController.Instance.ButtonZLUpEvent += Instance_ButtonZLUpEvent;
        InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
        InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
        InputController.Instance.ButtonHomeUpEvent += Instance_ButtonHomeUpEvent;
        InputController.Instance.ButtonCaptureUpEvent += Instance_ButtonCaptureUpEvent;
        InputController.Instance.ButtonRightStickUpEvent += Instance_ButtonRightStickUpEvent;
        InputController.Instance.ButtonLeftStickUpEvent += Instance_ButtonLeftStickUpEvent;
        InputController.Instance.ButtonRightSLUpEvent += Instance_ButtonRightSLUpEvent;
        InputController.Instance.ButtonLeftSLUpEvent += Instance_ButtonLeftSLUpEvent;
        InputController.Instance.ButtonLeftSRUpEvent += Instance_ButtonLeftSRUpEvent;
        InputController.Instance.ButtonRightSRUpEvent += Instance_ButtonRightSRUpEvent;
        #endregion

        #region Joystick
        InputController.Instance.LeftJoystickUsedEvent += Instance_LeftJoystickUsedEvent;
        InputController.Instance.RightJoystickUsedEvent += Instance_RightJoystickUsedEvent;
        #endregion


        //InputController.Instance.Applet();
    }
    public int PPPlayer; public InputDirection DDir;
    public bool t = false;
    private void Update()
    {
        if(t)
        {
            t = false;
            UserInputJoystickHandler(PPPlayer, DDir);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instance_ButtonDownDownEvent(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instance_ButtonUpDownEvent(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Instance_ButtonRightDownEvent(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Instance_ButtonLeftDownEvent(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Instance_ButtonDownDownEvent(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Instance_ButtonUpDownEvent(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Instance_ButtonRightDownEvent(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Instance_ButtonLeftDownEvent(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UserInputJoystickHandler(1,InputDirection.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UserInputJoystickHandler(1,InputDirection.Down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UserInputJoystickHandler(1,InputDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UserInputJoystickHandler(1,InputDirection.Left);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            UserInputJoystickHandler(0, InputDirection.Up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UserInputJoystickHandler(0, InputDirection.Down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UserInputJoystickHandler(0, InputDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            UserInputJoystickHandler(0, InputDirection.Left);
        }

    }




    #region Joystick
    private void Instance_RightJoystickUsedEvent(int player, InputDirection dir)
    {
        Debug.Log(player + "    " + dir);
        //UserInputJoystickHandler(PPPlayer, DDir);
        UserInputJoystickHandler(player, dir);
    }

    private void Instance_LeftJoystickUsedEvent(int player, InputDirection dir)
    {
        Debug.Log(player + "    " + dir);
        //UserInputJoystickHandler(PPPlayer, DDir);
        UserInputJoystickHandler(player, dir);
    }

    #endregion
    #region Button Up
    private void Instance_ButtonMinusUpEvent(int player)
    {
        Debug.Log(player + "  " + "Minus Up");
    }

    private void Instance_ButtonPlusUpEvent(int player)
    {
        Debug.Log(player + "  " + "Plus Up");
    }

    private void Instance_ButtonZLUpEvent(int player)
    {
        Debug.Log(player + "  " + "ZL Up");
    }

    private void Instance_ButtonLUpEvent(int player)
    {
        Debug.Log(player + "  " + "L Up");
    }

    private void Instance_ButtonZRUpEvent(int player)
    {
        Debug.Log(player + "  " + "ZR Up");
    }

    private void Instance_ButtonRUpEvent(int player)
    {
        Debug.Log(player + "  " + "R Up");
    }

    private void Instance_ButtonLeftUpEvent(int player)
    {
        Debug.Log(player + "  " + "Left Up");
    }

    private void Instance_ButtonRightUpEvent(int player)
    {
        Debug.Log(player + "  " + "Right Up");
    }

    private void Instance_ButtonDownUpEvent(int player)
    {
        Debug.Log(player + "  " + "Down Up");
    }

    private void Instance_ButtonUpUpEvent(int player)
    {
        Debug.Log(player + "  " + "Up Up");
    }

    private void Instance_ButtonYUpEvent(int player)
    {
        Debug.Log(player + "  " + "Y Up");
    }

    private void Instance_ButtonXUpEvent(int player)
    {
        Debug.Log(player + "  " + "X Up");
    }

    private void Instance_ButtonBUpEvent(int player)
    {
        Debug.Log(player + "  " + "B Up");
        VibrationController.Instance.CustomVibration(player, VibrationType.b);
    }

    private void Instance_ButtonAUpEvent(int player)
    {
        Debug.Log(player + "  " + "A Up");
        VibrationController.Instance.CustomVibration(player, VibrationType.a);

    }

    private void Instance_ButtonRightSRUpEvent(int player)
    {
        Debug.Log(player + "  " + "Right SR Up");
    }

    private void Instance_ButtonLeftSRUpEvent(int player)
    {
        Debug.Log(player + "  " + "Left SR Up");
    }

    private void Instance_ButtonLeftSLUpEvent(int player)
    {
        Debug.Log(player + "  " + "Left SL Up");
    }

    private void Instance_ButtonRightSLUpEvent(int player)
    {
        Debug.Log(player + "  " + "Right SL Up");
    }

    private void Instance_ButtonLeftStickUpEvent(int player)
    {
        Debug.Log(player + "  " + "Left Stick Up");
    }

    private void Instance_ButtonRightStickUpEvent(int player)
    {
        Debug.Log(player + "  " + "Right Stick Up");
    }

    private void Instance_ButtonCaptureUpEvent(int player)
    {
        Debug.Log(player + "  " + "Capture Up");
    }

    private void Instance_ButtonHomeUpEvent(int player)
    {
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        Debug.Log(player + "  " + "Home Up");
    }


    #endregion
    #region Button Press
    private void Instance_ButtonMinusPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Minus Press");
    }

    private void Instance_ButtonPlusPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Plus Press");
    }

    private void Instance_ButtonZLPressedEvent(int player)
    {
        Debug.Log(player + "  " + "ZL Press");
    }

    private void Instance_ButtonLPressedEvent(int player)
    {
        Debug.Log(player + "  " + "L Press");
    }

    private void Instance_ButtonZRPressedEvent(int player)
    {
        Debug.Log(player + "  " + "ZR Press");
    }

    private void Instance_ButtonRPressedEvent(int player)
    {
        Debug.Log(player + "  " + "R Press");
    }

    private void Instance_ButtonLeftPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Left Press");
    }

    private void Instance_ButtonRightPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Right Press");
    }

    private void Instance_ButtonDownPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Down Press");
    }

    private void Instance_ButtonUpPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Up Press");
    }

    private void Instance_ButtonYPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Y Press");
    }

    private void Instance_ButtonXPressedEvent(int player)
    {
        Debug.Log(player + "  " + "X Press");
    }

    private void Instance_ButtonBPressedEvent(int player)
    {
        Debug.Log(player + "  " + "B Press");
    }

    private void Instance_ButtonAPressedEvent(int player)
    {
        Debug.Log(player + "  " + "A Press");
    }

    private void Instance_ButtonLeftSRPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Left SR Press");
    }

    private void Instance_ButtonRightSRPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Right SR Press");
    }

    private void Instance_ButtonRightSLPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Right SL Press");
    }

    private void Instance_ButtonLeftSLPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Left SL Press");
    }

    private void Instance_ButtonRightStickPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Right Stick Press");
    }

    private void Instance_ButtonLeftStickPressedEvent(int player)
    {
        Debug.Log(player + "  " + "Left Stick Press");
    }

    private void Instance_ButtonCapturePressedEvent(int player)
    {
        Debug.Log(player + "  " + "Capture Press");
    }

    private void Instance_ButtonHomePressedEvent(int player)
    {
        Debug.Log(player + "  " + "Home Press");
    }
    #endregion
    #region Button Down
    private void Instance_ButtonPlusDownEvent(int player)
    {
        Debug.Log(player + "  " + "Plus Down");
    }

    private void Instance_ButtonMinusDownEvent(int player)
    {
        Debug.Log(player + "  " + "Minus Down");
    }

    private void Instance_ButtonZLDownEvent(int player)
    {
        Debug.Log(player + "  " + "ZL Down");
    }

    private void Instance_ButtonLDownEvent(int player)
    {
        Debug.Log(player + "  " + "L Down");
    }

    private void Instance_ButtonZRDownEvent(int player)
    {
        Debug.Log(player + "  " + "ZR Down");
    }

    private void Instance_ButtonRDownEvent(int player)
    {
        Debug.Log(player + "  " + "R Down");
    }

    private void Instance_ButtonLeftDownEvent(int player)
    {
        Debug.Log(player + "  " + "Left Down");
        SelectCharacter(CharacterSelectionType.Left, (ControllerType)player);
    }

    private void Instance_ButtonRightDownEvent(int player)
    {
        Debug.Log(player + "  " + "Right Down");
        SelectCharacter(CharacterSelectionType.Right, (ControllerType)player);
    }

    private void Instance_ButtonDownDownEvent(int player)
    {
        Debug.Log(player + "  " + "Down Down");
        SelectCharacter(CharacterSelectionType.Down, (ControllerType)player);
    }

    private void Instance_ButtonUpDownEvent(int player)
    {
        Debug.Log(player + "  " + "Up Down");
        SelectCharacter(CharacterSelectionType.Up, (ControllerType)player);
    }

    private void Instance_ButtonYDownEvent(int player)
    {
        Debug.Log(player + "  " + "Y Down");
        //SelectCharacter(CharacterSelectionType.Y, (ControllerType)player);
    }

    private void Instance_ButtonXDownEvent(int player)
    {
        Debug.Log(player + "  " + "X Down");
        //SelectCharacter(CharacterSelectionType.X, (ControllerType)player);
    }

    private void Instance_ButtonBDownEvent(int player)
    {
        Debug.Log(player + "  " + "B Down");
        //SelectCharacter(CharacterSelectionType.B, (ControllerType)player);
    }

    private void Instance_ButtonADownEvent(int player)
    {
        Debug.Log(player + "  " + "A Down");
        //SelectCharacter(CharacterSelectionType.A, (ControllerType)player);
    }

    private void Instance_ButtonRightSRDownEvent(int player)
    {
        Debug.Log(player + "  " + "Right SR Down");
    }

    private void Instance_ButtonLeftSRDownEvent(int player)
    {
        Debug.Log(player + "  " + "Left SR Down");
    }

    private void Instance_ButtonRightSLDownEvent(int player)
    {
        Debug.Log(player + "  " + "Right SL Down");
    }

    private void Instance_ButtonLeftSLDownEvent(int player)
    {
        Debug.Log(player + "  " + "Left SL Down");
    }

    private void Instance_ButtonRightStickDownEvent(int player)
    {
        Debug.Log(player + "  " + "Right Stick Down");
    }

    private void Instance_ButtonLeftStickDownEvent(int player)
    {
        Debug.Log(player + "  " + "Left Stick Down");
    }

    private void Instance_ButtonCaptureDownEvent(int player)
    {
        Debug.Log(player + "  " + "Capture Down");
    }

    private void Instance_ButtonHomeDownEvent(int player)
    {
        Debug.Log(player + "  " + "Home Down");
    }
    #endregion


    public void UserInputJoystickHandler(int player, InputDirection dir)
    {
        BattleManagerScript.Instance.MoveSelectedCharacterInDirection((ControllerType)player, dir);
    }

    public void SelectCharacter(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        BattleManagerScript.Instance.SetCharacterSelection(characterSelection, controllerType);
    }

}
