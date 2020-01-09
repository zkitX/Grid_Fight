using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    
    public static InputController Instance; //Singleton instance

    public int PlayersNumber;
    public List<Player> players = new List<Player>(); // The Rewired Player
    public Vector2 LeftJoystic, RightJoystic; //Joysticks movement 
    void Awake()
    {
        Instance = this;
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        for (int i = 0; i < PlayersNumber; i++)
        {
            players.Add(ReInput.players.GetPlayer(i));
            if (players[i].controllers.joystickCount > 0)
            {
                players[i].controllers.hasKeyboard = false;
            }
        }
        
    }

    private void Update()
    {
        //Looking for all the possible players input
        foreach (Player item in players)
        {
            ButtonsUp(item);
            ButtonsPress(item);
            ButtonsDown(item);
            JoystickMovement(item);
        }
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

    //Check for Joysticks movement
    private void JoystickMovement(Player currentPlayer)
    {
        LeftJoystic = new Vector2(currentPlayer.GetAxis("Left Move Horizontal"), currentPlayer.GetAxis("Left Move Vertical"));
        if (LeftJoystic != Vector2.zero)
        {
            //Debug.Log(player.GetButtonDown("Left Joystic"));
            if (LeftJoystickUsedEvent != null)
            {
                if (Mathf.Abs(LeftJoystic.x) > Mathf.Abs(LeftJoystic.y))
                {
                    if (LeftJoystic.x > 0)
                    {
                        LeftJoystickUsedEvent(currentPlayer.id ,InputDirection.Right);
                    }
                    else
                    {
                        LeftJoystickUsedEvent(currentPlayer.id ,InputDirection.Left);
                    }
                }
                else
                {
                    if (LeftJoystic.y > 0)
                    {
                        LeftJoystickUsedEvent(currentPlayer.id ,InputDirection.Up);
                    }
                    else
                    {
                        LeftJoystickUsedEvent(currentPlayer.id ,InputDirection.Down);
                    }
                }
            }
        }
        RightJoystic = new Vector2(currentPlayer.GetAxis("Right Move Horizontal"), currentPlayer.GetAxis("Right Move Vertical"));
        if (RightJoystic != Vector2.zero)
        {
            //Debug.Log(player.GetButtonDown("Right Joystic"));
            if (RightJoystickUsedEvent != null)
            {

                if (Mathf.Abs(RightJoystic.x) > Mathf.Abs(RightJoystic.y))
                {
                    if (RightJoystic.x > 0)
                    {
                        RightJoystickUsedEvent(currentPlayer.id, InputDirection.Right);
                    }
                    else
                    {
                        RightJoystickUsedEvent(currentPlayer.id, InputDirection.Left);
                    }
                }
                else
                {
                    if (RightJoystic.y > 0)
                    {
                        RightJoystickUsedEvent(currentPlayer.id, InputDirection.Up);
                    }
                    else
                    {
                        RightJoystickUsedEvent(currentPlayer.id, InputDirection.Down);
                    }
                }
            }
        }
    }
    //Check for Buttons Up
    private void ButtonsUp(Player currentPlayer)
    {
        if (currentPlayer.GetButtonUp("A"))
        {
            // Debug.Log("Up A");
            if (ButtonAUpEvent != null)
            {
                ButtonAUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("B"))
        {
            //Debug.Log(player.GetButtonDown("B"));
            if (ButtonBUpEvent != null)
            {
                ButtonBUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("X"))
        {
            //Debug.Log(player.GetButtonDown("X"));
            if (ButtonXUpEvent != null)
            {
                ButtonXUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Y"))
        {
            //Debug.Log(player.GetButtonDown("Y"));
            if (ButtonYUpEvent != null)
            {
                ButtonYUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("R"))
        {
            //Debug.Log(player.GetButtonDown("R"));
            if (ButtonRUpEvent != null)
            {
                ButtonRUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("ZR"))
        {
            //Debug.Log(player.GetButtonDown("ZR"));
            if (ButtonZRUpEvent != null)
            {
                ButtonZRUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("L"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonLUpEvent != null)
            {
                ButtonLUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("ZL"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonZLUpEvent != null)
            {
                ButtonZLUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Right"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightUpEvent != null)
            {
                ButtonRightUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Up"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonUpUpEvent != null)
            {
                ButtonUpUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Left"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftUpEvent != null)
            {
                ButtonLeftUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Down"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonDownUpEvent != null)
            {
                ButtonDownUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Plus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusUpEvent != null)
            {
                ButtonPlusUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Minus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusUpEvent != null)
            {
                ButtonMinusUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Home"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonHomeUpEvent != null)
            {
                ButtonHomeUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Capture"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonCaptureUpEvent != null)
            {
                ButtonCaptureUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Right Stick"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightStickUpEvent != null)
            {
                ButtonRightStickUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Left Stick"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonLeftStickUpEvent != null)
            {
                ButtonLeftStickUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Left SL"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftSLUpEvent != null)
            {
                ButtonLeftSLUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Right SL"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonRightSLUpEvent != null)
            {
                ButtonRightSLUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Left SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonLeftSRUpEvent != null)
            {
                ButtonLeftSRUpEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonUp("Right SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonRightSRUpEvent != null)
            {
                ButtonRightSRUpEvent(currentPlayer.id);
            }
        }
    }
    //Check for Buttons Press
    private void ButtonsPress(Player currentPlayer)
    {
        if (currentPlayer.GetButton("A"))
        {
            // Debug.Log("A");
            if (ButtonAPressedEvent != null)
            {
                ButtonAPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("B"))
        {
            //Debug.Log(player.GetButtonDown("B"));
            if (ButtonBPressedEvent != null)
            {
                ButtonBPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("X"))
        {
            //Debug.Log(player.GetButtonDown("X"));
            if (ButtonXPressedEvent != null)
            {
                ButtonXPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Y"))
        {
            //Debug.Log(player.GetButtonDown("Y"));
            if (ButtonYPressedEvent != null)
            {
                ButtonYPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("R"))
        {
            //Debug.Log(player.GetButtonDown("R"));
            if (ButtonRPressedEvent != null)
            {
                ButtonRPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("ZR"))
        {
            //Debug.Log(player.GetButtonDown("ZR"));
            if (ButtonZRPressedEvent != null)
            {
                ButtonZRPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("L"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonLPressedEvent != null)
            {
                ButtonLPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("ZL"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonZLPressedEvent != null)
            {
                ButtonZLPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Right"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightPressedEvent != null)
            {
                ButtonRightPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Up"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonUpPressedEvent != null)
            {
                ButtonUpPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Left"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftPressedEvent != null)
            {
                ButtonLeftPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Down"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonDownPressedEvent != null)
            {
                ButtonDownPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Plus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusPressedEvent != null)
            {
                ButtonPlusPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Minus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusPressedEvent != null)
            {
                ButtonMinusPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Home"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonHomePressedEvent != null)
            {
                ButtonHomePressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Capture"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonCapturePressedEvent != null)
            {
                ButtonCapturePressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Right Stick"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightStickPressedEvent != null)
            {
                ButtonRightStickPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Left Stick"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonLeftStickPressedEvent != null)
            {
                ButtonLeftStickPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Left SL"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftSLPressedEvent != null)
            {
                ButtonLeftSLPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Right SL"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonRightSLPressedEvent != null)
            {
                ButtonRightSLPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Left SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonLeftSRPressedEvent != null)
            {
                ButtonLeftSRPressedEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButton("Right SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonRightSRPressedEvent != null)
            {
                ButtonRightSRPressedEvent(currentPlayer.id);
            }
        }


    }
    //Check for Buttons Down
    private void ButtonsDown(Player currentPlayer)
    {
        if (currentPlayer.GetButtonDown("A"))
        {
            // Debug.Log("Down A");
            if (ButtonADownEvent != null)
            {
                //ListOfButtons.Add(new ButtonStateClass("A", ButtonClickStateType.Down));
                ButtonADownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("B"))
        {
            //Debug.Log(player.GetButtonDown("B"));
            if (ButtonBDownEvent != null)
            {
                ButtonBDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("X"))
        {
            //Debug.Log(player.GetButtonDown("X"));
            if (ButtonXDownEvent != null)
            {
                ButtonXDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Y"))
        {
            //Debug.Log(player.GetButtonDown("Y"));
            if (ButtonYDownEvent != null)
            {
                ButtonYDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("R"))
        {
            //Debug.Log(player.GetButtonDown("R"));
            if (ButtonRDownEvent != null)
            {
                ButtonRDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("ZR"))
        {
            //Debug.Log(player.GetButtonDown("ZR"));
            if (ButtonZRDownEvent != null)
            {
                ButtonZRDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("L"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonLDownEvent != null)
            {
                ButtonLDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("ZL"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonZLDownEvent != null)
            {
                ButtonZLDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Right"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightDownEvent != null)
            {
                ButtonRightDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Up"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonUpDownEvent != null)
            {
                ButtonUpDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Left"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftDownEvent != null)
            {
                ButtonLeftDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Down"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonDownDownEvent != null)
            {
                ButtonDownDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Plus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusDownEvent != null)
            {
                ButtonPlusDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Minus"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonPlusDownEvent != null)
            {
                ButtonMinusDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Home"))
        {
            //Debug.Log(player.GetButtonDown("L"));
            if (ButtonHomeDownEvent != null)
            {
                ButtonHomeDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Capture"))
        {
            //Debug.Log(player.GetButtonDown("ZL"));
            if (ButtonCaptureDownEvent != null)
            {
                ButtonCaptureDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Right Stick"))
        {
            //Debug.Log(player.GetButtonDown("Right"));
            if (ButtonRightStickDownEvent != null)
            {
                ButtonRightStickDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Left Stick"))
        {
            //Debug.Log(player.GetButtonDown("Up"));
            if (ButtonLeftStickDownEvent != null)
            {
                ButtonLeftStickDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Left SL"))
        {
            //Debug.Log(player.GetButtonDown("Left"));
            if (ButtonLeftSLDownEvent != null)
            {
                ButtonLeftSLDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Right SL"))
        {
            //Debug.Log(player.GetButtonDown("Down"));
            if (ButtonRightSLDownEvent != null)
            {
                ButtonRightSLDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Left SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonLeftSRDownEvent != null)
            {
                ButtonLeftSRDownEvent(currentPlayer.id);
            }
        }
        if (currentPlayer.GetButtonDown("Right SR"))
        {
            //Debug.Log(player.GetButtonDown("Plus"));
            if (ButtonRightSRDownEvent != null)
            {
                ButtonRightSRDownEvent(currentPlayer.id);
            }
        }

    }

    //Applet calling
    public void Applet(int playersNumber)
    {
#if UNITY_SWITCH
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
    }


}

