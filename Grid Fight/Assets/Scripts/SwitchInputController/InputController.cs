using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_SWITCH
using UnityEngine.Switch;
using Rewired.Platforms.Switch;
#endif
public class InputController : MonoBehaviour
{

    #region Button Down
    public delegate void ButtonADown(int player);
    public event ButtonADown ButtonADownEvent;
    public delegate void ButtonBDown(int player);
    public event ButtonBDown ButtonBDownEvent;
    public delegate void ButtonXDown(int player);
    public event ButtonXDown ButtonXDownEvent;
    public delegate void ButtonYDown(int player);
    public event ButtonYDown ButtonYDownEvent;
    public delegate void ButtonUpDown(int player);
    public event ButtonUpDown ButtonUpDownEvent;
    public delegate void ButtonDownDown(int player);
    public event ButtonDownDown ButtonDownDownEvent;
    public delegate void ButtonLeftDown(int player);
    public event ButtonLeftDown ButtonLeftDownEvent;
    public delegate void ButtonRightDown(int player);
    public event ButtonRightDown ButtonRightDownEvent;
    public delegate void ButtonRDown(int player);
    public event ButtonRDown ButtonRDownEvent;
    public delegate void ButtonZRDown(int player);
    public event ButtonZRDown ButtonZRDownEvent;
    public delegate void ButtonLDown(int player);
    public event ButtonLDown ButtonLDownEvent;
    public delegate void ButtonZLDown(int player);
    public event ButtonZLDown ButtonZLDownEvent;
    public delegate void ButtonPlusDown(int player);
    public event ButtonPlusDown ButtonPlusDownEvent;
    public delegate void ButtonMinusDown(int player);
    public event ButtonMinusDown ButtonMinusDownEvent;
    public delegate void ButtonHomeDown(int player);
    public event ButtonHomeDown ButtonHomeDownEvent;
    public delegate void ButtonCaptureDown(int player);
    public event ButtonCaptureDown ButtonCaptureDownEvent;
    public delegate void ButtonLeftStickDown(int player);
    public event ButtonLeftStickDown ButtonLeftStickDownEvent;
    public delegate void ButtonRightStickDown(int player);
    public event ButtonRightStickDown ButtonRightStickDownEvent;
    public delegate void ButtonLeftSLDown(int player);
    public event ButtonLeftSLDown ButtonLeftSLDownEvent;
    public delegate void ButtonLeftSRDown(int player);
    public event ButtonLeftSRDown ButtonLeftSRDownEvent;
    public delegate void ButtonRightSLDown(int player);
    public event ButtonRightSLDown ButtonRightSLDownEvent;
    public delegate void ButtonRightSRDown(int player);
    public event ButtonRightSRDown ButtonRightSRDownEvent;
    #endregion

    #region Button Up
    public delegate void ButtonAUp(int player);
    public event ButtonAUp ButtonAUpEvent;
    public delegate void ButtonBUp(int player);
    public event ButtonBUp ButtonBUpEvent;
    public delegate void ButtonXUp(int player);
    public event ButtonXUp ButtonXUpEvent;
    public delegate void ButtonYUp(int player);
    public event ButtonYUp ButtonYUpEvent;
    public delegate void ButtonUpUp(int player);
    public event ButtonUpUp ButtonUpUpEvent;
    public delegate void ButtonDownUp(int player);
    public event ButtonDownUp ButtonDownUpEvent;
    public delegate void ButtonLeftUp(int player);
    public event ButtonLeftUp ButtonLeftUpEvent;
    public delegate void ButtonRightUp(int player);
    public event ButtonRightUp ButtonRightUpEvent;
    public delegate void ButtonRUp(int player);
    public event ButtonRUp ButtonRUpEvent;
    public delegate void ButtonZRUp(int player);
    public event ButtonZRUp ButtonZRUpEvent;
    public delegate void ButtonLUp(int player);
    public event ButtonLUp ButtonLUpEvent;
    public delegate void ButtonZLUp(int player);
    public event ButtonZLUp ButtonZLUpEvent;
    public delegate void ButtonPlusUp(int player);
    public event ButtonPlusUp ButtonPlusUpEvent;
    public delegate void ButtonMinusUp(int player);
    public event ButtonMinusUp ButtonMinusUpEvent;
    public delegate void ButtonHomePressed(int player);
    public event ButtonHomePressed ButtonHomePressedEvent;
    public delegate void ButtonCapturePressed(int player);
    public event ButtonCapturePressed ButtonCapturePressedEvent;
    public delegate void ButtonLeftStickPressed(int player);
    public event ButtonLeftStickPressed ButtonLeftStickPressedEvent;
    public delegate void ButtonRightStickPressed(int player);
    public event ButtonRightStickPressed ButtonRightStickPressedEvent;
    public delegate void ButtonLeftSLPressed(int player);
    public event ButtonLeftSLPressed ButtonLeftSLPressedEvent;
    public delegate void ButtonLeftSRPressed(int player);
    public event ButtonLeftSRPressed ButtonLeftSRPressedEvent;
    public delegate void ButtonRightSLPressed(int player);
    public event ButtonRightSLPressed ButtonRightSLPressedEvent;
    public delegate void ButtonRightSRPressed(int player);
    public event ButtonRightSRPressed ButtonRightSRPressedEvent;
    #endregion



    #region Button Press
    public delegate void ButtonAPressed(int player);
    public event ButtonAPressed ButtonAPressedEvent;
    public delegate void ButtonBPressed(int player);
    public event ButtonBPressed ButtonBPressedEvent;
    public delegate void ButtonXPressed(int player);
    public event ButtonXPressed ButtonXPressedEvent;
    public delegate void ButtonYPressed(int player);
    public event ButtonYPressed ButtonYPressedEvent;
    public delegate void ButtonUpPressed(int player);
    public event ButtonUpPressed ButtonUpPressedEvent;
    public delegate void ButtonDownPressed(int player);
    public event ButtonDownPressed ButtonDownPressedEvent;
    public delegate void ButtonLeftPressed(int player);
    public event ButtonLeftPressed ButtonLeftPressedEvent;
    public delegate void ButtonRightPressed(int player);
    public event ButtonRightPressed ButtonRightPressedEvent;
    public delegate void ButtonRPressed(int player);
    public event ButtonRPressed ButtonRPressedEvent;
    public delegate void ButtonZRPressed(int player);
    public event ButtonZRPressed ButtonZRPressedEvent;
    public delegate void ButtonLPressed(int player);
    public event ButtonLPressed ButtonLPressedEvent;
    public delegate void ButtonZLPressed(int player);
    public event ButtonZLPressed ButtonZLPressedEvent;
    public delegate void ButtonPlusPressed(int player);
    public event ButtonPlusPressed ButtonPlusPressedEvent;
    public delegate void ButtonMinusPressed(int player);
    public event ButtonMinusPressed ButtonMinusPressedEvent;
    public delegate void ButtonHomeUp(int player);
    public event ButtonHomeUp ButtonHomeUpEvent;
    public delegate void ButtonCaptureUp(int player);
    public event ButtonCaptureUp ButtonCaptureUpEvent;
    public delegate void ButtonLeftStickUp(int player);
    public event ButtonLeftStickUp ButtonLeftStickUpEvent;
    public delegate void ButtonRightStickUp(int player);
    public event ButtonRightStickUp ButtonRightStickUpEvent;
    public delegate void ButtonLeftSLUp(int player);
    public event ButtonLeftSLUp ButtonLeftSLUpEvent;
    public delegate void ButtonLeftSRUp(int player);
    public event ButtonLeftSRUp ButtonLeftSRUpEvent;
    public delegate void ButtonRightSLUp(int player);
    public event ButtonRightSLUp ButtonRightSLUpEvent;
    public delegate void ButtonRightSRUp(int player);
    public event ButtonRightSRUp ButtonRightSRUpEvent;
    #endregion

    #region JoyStick
    public delegate void LeftJoystickUsed(int player, InputDirection dir);
    public event LeftJoystickUsed LeftJoystickUsedEvent;
    public delegate void RightJoystickUsed(int player, InputDirection dir);
    public event RightJoystickUsed RightJoystickUsedEvent;
    #endregion

    
    public void FireMinus()
    {
        ButtonMinusUpEvent?.Invoke(0);
    }

    public static InputController Instance; //Singleton instance

    public int PlayersNumber;
    public int PlayerCount //cus PlayersNumber seems like a cap more than a count
    {
        get
        {
            return players.Where(r => r.controllers.hasKeyboard || r.controllers.joystickCount > 0).ToArray().Length;
        }
    }
    public List<Player> players = new List<Player>(); // The Rewired Player
    public List<Vector2> Joystics = new List<Vector2>(); //Joysticks movement 
    public Vector2 Joystic
    {
        get
        {
            Vector2 average = new Vector2();
            foreach (Vector2 joy in Joystics)
            {
                if (joy.x != 0) average.x = (joy.x + average.x) / 2f;
                if (joy.y != 0) average.y = (joy.y + average.y) / 2f;
            }

            return average;
        }
    }
    
    void Awake()
    {
        bool keyboardAssigned = false;

        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);



        if (ReInput.players.GetPlayer(0).controllers.joystickCount == 0)
        {
            PlayersNumber = 1;
        }
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        for (int i = 0; i < PlayersNumber; i++)
        {
            players.Add(ReInput.players.GetPlayer(i));
            if (players[i].controllers.joystickCount > 0 || keyboardAssigned)
            {
                players[i].controllers.hasKeyboard = false;
            }
            else
            {
                keyboardAssigned = true;
            }
            players[i].AddInputEventDelegate(OnButtonDown, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed);
            players[i].AddInputEventDelegate(OnButtonPress, UpdateLoopType.Update, InputActionEventType.ButtonPressed);
            players[i].AddInputEventDelegate(OnButtonUp, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased);
            players[i].AddInputEventDelegate(OnAxisUpdate, UpdateLoopType.Update, InputActionEventType.AxisActive);
        }

        for (int i = 0; i < PlayerCount; i++)
        {
            Joystics.Add(new Vector2());
        }
        
    }

    void OnAxisUpdate(InputActionEventData data)
    {
        InputButtonType buttonInput = (InputButtonType)System.Enum.Parse(typeof(InputButtonType), data.actionName);
        float x = (buttonInput == InputButtonType.Left_Move_Horizontal || buttonInput == InputButtonType.Right_Move_Horizontal) ? data.GetAxis() : 0;
        float y = (buttonInput == InputButtonType.Left_Move_Vertical || buttonInput == InputButtonType.Right_Move_Vertical) ? data.GetAxis() : 0;
        Joystics[data.playerId] = new Vector2(x,y);
        if (LeftJoystickUsedEvent != null && (x > 0.2f || x < -0.2f || y > 0.2f || y < -0.2f))
        {
            if (Mathf.Abs(Joystics[data.playerId].x) > Mathf.Abs(Joystics[data.playerId].y))
            {
                if (Joystics[data.playerId].x > 0)
                {
                    LeftJoystickUsedEvent(data.playerId, InputDirection.Right);
                }
                else
                {
                    LeftJoystickUsedEvent(data.playerId, InputDirection.Left);
                }
            }
            else
            {
                if (Joystics[data.playerId].y > 0)
                {
                    LeftJoystickUsedEvent(data.playerId, InputDirection.Up);
                }
                else
                {
                    LeftJoystickUsedEvent(data.playerId, InputDirection.Down);
                }
            }
        }
    }


    void OnButtonDown(InputActionEventData data)
    {
        InputButtonType buttonInput = (InputButtonType)System.Enum.Parse(typeof(InputButtonType), data.actionName);

        switch (buttonInput)
        {
            case InputButtonType.A:
                ButtonADownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.B:
                ButtonBDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.X:
                ButtonXDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Y:
                ButtonYDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left:
                ButtonLeftDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right:
                ButtonRightDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Up:
                ButtonUpDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Down:
                ButtonDownDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZL:
                ButtonZLDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.L:
                ButtonLDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZR:
                ButtonZRDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.R:
                ButtonRDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Plus:
                ButtonPlusDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Minus:
                ButtonMinusDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Home:
                ButtonHomeDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Capture:
                ButtonCaptureDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SL:
                ButtonLeftSLDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SL:
                ButtonRightSLDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SR:
                ButtonLeftSRDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SR:
                ButtonRightSRDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_Stick:
                ButtonLeftStickDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_Stick:
                ButtonRightStickDownEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.KeyboardDown:
                LeftJoystickUsedEvent?.Invoke(data.playerId, InputDirection.Down);
                break;
            case InputButtonType.KeyboardLeft:
                LeftJoystickUsedEvent?.Invoke(data.playerId, InputDirection.Left);
                break;
            case InputButtonType.KeyboardRight:
                LeftJoystickUsedEvent?.Invoke(data.playerId, InputDirection.Right);
                break;
            case InputButtonType.KeyboardUp:
                LeftJoystickUsedEvent?.Invoke(data.playerId, InputDirection.Up);
                break;
        }
    }

    void OnButtonPress(InputActionEventData data)
    {
        InputButtonType buttonInput = (InputButtonType)System.Enum.Parse(typeof(InputButtonType), data.actionName);

        switch (buttonInput)
        {
            case InputButtonType.A:
                ButtonAPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.B:
                ButtonBPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.X:
                ButtonXPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Y:
                ButtonYPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left:
                ButtonLeftPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right:
                ButtonRightPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Up:
                ButtonUpPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Down:
                ButtonDownPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZL:
                ButtonZLPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.L:
                ButtonLPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZR:
                ButtonZRPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.R:
                ButtonRPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Plus:
                ButtonPlusPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Minus:
                ButtonMinusPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Home:
                ButtonHomePressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Capture:
                ButtonCapturePressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SL:
                ButtonLeftSLPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SL:
                ButtonRightSLPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SR:
                ButtonLeftSRPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SR:
                ButtonRightSRPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_Stick:
                ButtonLeftStickPressedEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_Stick:
                ButtonRightStickPressedEvent?.Invoke(data.playerId);
                break;
        }
    }

    void OnButtonUp(InputActionEventData data)
    {
        InputButtonType buttonInput = (InputButtonType)System.Enum.Parse(typeof(InputButtonType), data.actionName);
        if(EventManager.Instance != null)
        {
            EventManager.Instance.UpdateButtonPressed(buttonInput);
        }

        switch (buttonInput)
        {
            case InputButtonType.A:
                ButtonAUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.B:
                ButtonBUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.X:
                ButtonXUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Y:
                ButtonYUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left:
                ButtonLeftUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right:
                ButtonRightUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Up:
                ButtonUpUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Down:
                ButtonDownUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZL:
                ButtonZLUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.L:
                ButtonLUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.ZR:
                ButtonZRUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.R:
                ButtonRUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Plus:
                ButtonPlusUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Minus:
                ButtonMinusUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Home:
                ButtonHomeUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Capture:
                ButtonCaptureUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SL:
                ButtonLeftSLUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SL:
                ButtonRightSLUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_SR:
                ButtonLeftSRUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_SR:
                ButtonRightSRUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Left_Stick:
                ButtonLeftStickUpEvent?.Invoke(data.playerId);
                break;
            case InputButtonType.Right_Stick:
                ButtonRightStickUpEvent?.Invoke(data.playerId);
                break;
        }
       

    }

    public void ResetEventSubscription()
    {
        #region Button Down
        ButtonADownEvent = null;
        ButtonBDownEvent = null;
        ButtonXDownEvent = null;
        ButtonYDownEvent = null;
        ButtonUpDownEvent = null;
        ButtonDownDownEvent = null;
        ButtonRightDownEvent = null;
        ButtonLeftDownEvent = null;
        ButtonRDownEvent = null;
        ButtonZRDownEvent = null;
        ButtonLDownEvent = null;
        ButtonZLDownEvent = null;
        ButtonMinusDownEvent = null;
        ButtonPlusDownEvent = null;
        ButtonHomeDownEvent = null;
        ButtonCaptureDownEvent = null;
        ButtonLeftStickDownEvent = null;
        ButtonRightStickDownEvent = null;
        ButtonLeftSLDownEvent = null;
        ButtonRightSLDownEvent = null;
        ButtonLeftSRDownEvent = null;
        ButtonRightSRDownEvent = null;
        #endregion


        #region Button Press
        ButtonAPressedEvent = null;
        ButtonBPressedEvent = null;
        ButtonXPressedEvent = null;
        ButtonYPressedEvent = null;
        ButtonUpPressedEvent = null;
        ButtonDownPressedEvent = null;
        ButtonRightPressedEvent = null;
        ButtonLeftPressedEvent = null;
        ButtonRPressedEvent = null;
        ButtonZRPressedEvent = null;
        ButtonLPressedEvent = null;
        ButtonZLPressedEvent = null;
        ButtonPlusPressedEvent = null;
        ButtonMinusPressedEvent = null;
        ButtonHomePressedEvent = null;
        ButtonCapturePressedEvent = null;
        ButtonLeftStickPressedEvent = null;
        ButtonRightStickPressedEvent = null;
        ButtonLeftSLPressedEvent = null;
        ButtonRightSLPressedEvent = null;
        ButtonRightSRPressedEvent = null;
        ButtonLeftSRPressedEvent = null;
        #endregion

        #region Button Up
        ButtonAUpEvent = null;
        ButtonBUpEvent = null;
        ButtonXUpEvent = null;
        ButtonYUpEvent = null;
        ButtonUpUpEvent = null;
        ButtonDownUpEvent = null;
        ButtonRightUpEvent = null;
        ButtonLeftUpEvent = null;
        ButtonRUpEvent = null;
        ButtonZRUpEvent = null;
        ButtonLUpEvent = null;
        ButtonZLUpEvent = null;
        ButtonPlusUpEvent = null;
        ButtonMinusUpEvent = null;
        ButtonHomeUpEvent = null;
        ButtonCaptureUpEvent = null;
        ButtonRightStickUpEvent = null;
        ButtonLeftStickUpEvent = null;
        ButtonRightSLUpEvent = null;
        ButtonLeftSLUpEvent = null;
        ButtonLeftSRUpEvent = null;
        ButtonRightSRUpEvent = null;
        #endregion

        #region Joystick
        LeftJoystickUsedEvent = null;
        RightJoystickUsedEvent = null;
        #endregion
    }


    /*private void Notification_notificationMessageReceived(Notification.Message obj)
    {
        if (obj == Notification.Message.OperationModeChanged)
        {
            if(Operation.mode == Operation.OperationMode.Console)
            {
                //Performance.SetConfiguration(Performance.PerformanceMode.Boost,Performance.PerformanceConfiguration.Cpu1020MhzGpu768MhzEmc1600Mhz);


                //Screen.SetResolution(640, 480, true);
            }
            Debug.Log("---------------------------------------------" + Operation.mode.ToString());
           // Operation.mode = Operation.OperationMode.Console
        }
    }*/

    //Applet calling
    public IEnumerator Applet(int playersNumber)
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        // Set the options to pass to the Controller Applet
        ControllerAppletOptions options = new ControllerAppletOptions();
        options.playerCountMax = playersNumber;
        options.showColors = true;
        options.showLabels = true;

        if (playersNumber == 1)
        {
            options.players[0].color = Color.red;
            options.players[0].label = "Red Player";
           
        }

        if (playersNumber >= 2)
        {
            options.players[0].color = Color.red;
            options.players[0].label = "Red Player";
            options.players[1].color = Color.green;
            options.players[1].label = "Green Player";
        }

        if(playersNumber == 4)
        {
            options.players[2].color = Color.blue;
            options.players[2].label = "Blue Player";
            options.players[3].color = Color.yellow;
            options.players[3].label = "Yellow Player";
        }

        // Show the controller applet
        UnityEngine.Switch.Applet.Begin(); // See Unity documentation for explanation of this function
        SwitchInput.ControllerApplet.Show(options);
        UnityEngine.Switch.Applet.End();
#endif

        yield return null;
    }


}

