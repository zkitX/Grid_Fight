using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MyBox;

public class Grid_UINavigator : MonoBehaviour
{
    public static Grid_UINavigator Instance;

    public bool loopAround = false;
    public bool specificStartingButton = true;
    [ConditionalField("specificStartingButton")] public Grid_UIButton startingButton = null;
    [ConditionalField("specificStartingButton", true)] public InputDirection startingDirection = InputDirection.Up;

    [HideInInspector] public float buttonBufferPercentage = 0.5f;

    protected Grid_UIButton[] buttons = null;
    protected Grid_UIButton[] ActiveButtons
    {
        get
        {
            return buttons.Where(r => r.gameObject.activeInHierarchy && r.parentPanel.focusState == UI_FocusTypes.Focused).ToArray();
        }
    }

    protected Grid_UIPanel[] panels
    {
        get
        {
            return FindObjectsOfType<Grid_UIPanel>();
        }
    }
    public Grid_UIPanel genericPanel
    {
        get
        {
            if (panels.Where(r => r.isGenericPanel).FirstOrDefault() == null)
            {
                Debug.LogError("No generic fallback panel is setup, please create one");
                return null;
            }
            else if(panels.Where(r => r.isGenericPanel).ToArray().Length != 1)
            {
                Debug.LogError("More than one generic fallback panel is setup, please ensure there is only 1");
                return panels.Where(r => r.isGenericPanel).FirstOrDefault();
            }
            else
            {
                return panels.Where(r => r.isGenericPanel).FirstOrDefault();
            }
        }
    }

    protected int selectedButtonIndex = 0;
    protected Grid_UIButton selectedButton
    {
        get
        {
            return buttons[selectedButtonIndex];
        }
        set
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (value.name == buttons[i].name)
                {
                    Debug.Log("Current selected button: " + buttons[i].name);
                    selectedButtonIndex = i;
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        if(InputController.Instance != null)
        {
            InputController.Instance.LeftJoystickUsedEvent += ButtonChangeInput;
            InputController.Instance.ButtonAUpEvent += ButtonPressInput;
        }
        SetupButtonPanels();
    }

    private void Start()
    {
        buttons = GameObject.FindObjectsOfType<Grid_UIButton>();
        if (startingButton != null) SelectButtonByName(startingButton.name);
        else SelectTopButtonInStartingDirection();
    }


    float offset = 0f;
    public void ButtonChangeInput(int player, InputDirection direction)
    {
        if (offset + 0.2f < Time.time)
        {
            SelectButton(GetButtonClosestInDirectionFromSelected(direction));
            offset = Time.time;
        }
        //if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Menu) return;
    }

    public void ButtonPressInput(int player)
    {
        if (selectedButton != null) selectedButton.PressAction();
    }



    public void SetupButtonPanels()
    {
        foreach (Grid_UIPanel panel in panels)
        {
            panel.SetupChildButtonPanelInfo();
        }
    }




    public void SelectButtonByName(string buttonName)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonName == buttons[i].name) SelectButton(i);
        }
    }

    public void SelectTopButtonInStartingDirection()
    {
        SelectButtonByName(GetButtonFurthestInDirection(startingDirection).name);
    }

    int GetButtonIndex(Grid_UIButton btn)
    {
        if (btn == null) return -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == btn.name)
            {
                return i;
            }
        }
        return -1;
    }

    public void SelectButton(Grid_UIButton btn, bool playDeselectEventsForOtherButtons = true)
    {
        if (btn == null) return;
        SelectButton(GetButtonIndex(btn), playDeselectEventsForOtherButtons);
    }

    public void SelectButton(int index, bool playDeselectEventsForOtherButtons = true)
    {
        if (!buttons[index].SelectAction()) return;

        selectedButton = buttons[index];
        DeselectAllButtons(playDeselectEventsForOtherButtons);
    }

    public void DeselectButton(Grid_UIButton btn, bool playDeselectEvents = true)
    {
        DeselectButton(GetButtonIndex(btn), playDeselectEvents);
    }

    public void DeselectButton(int index, bool playDeselectEvents = true)
    {
        if (!buttons[index].DeselectAction(playDeselectEvents)) return;
    }

    public void DeselectAllButtons(bool playDeselectEventsForOtherButtons = true)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if(!(selectedButtonIndex == i)) DeselectButton(i, playDeselectEventsForOtherButtons);
        }
    }

    public Grid_UIButton GetButtonFurthestInDirection(InputDirection comparison)
    {
        if (ActiveButtons.Length == 0) return null;

        Grid_UIButton furthestInDirection = null;
        for (int i = 0; i < ActiveButtons.Length; i++)
        {
            if (furthestInDirection == null) furthestInDirection = ActiveButtons[i];
            else if (Compare.InDirectionTo(ActiveButtons[i].transform.position, furthestInDirection.transform.position, comparison))
            {
                furthestInDirection = ActiveButtons[i];
            }
        }

        return furthestInDirection;
    }

    public Grid_UIButton GetButtonClosestInDirectionFromSelected(InputDirection direction)
    {
        Grid_UIButton[] activeUnselectedButtons = ActiveButtons.Where(r => r.name != selectedButton.name).ToArray();
        if (activeUnselectedButtons.Length == 0) return null;

        Grid_UIButton closestInDirection = null;
        for (int i = 0; i < activeUnselectedButtons.Length; i++)
        {
            if (Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position, direction, selectedButton.buffers) > 0f &&
                Mathf.Abs(Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position,
                (direction == InputDirection.Down || direction == InputDirection.Up) ? InputDirection.Right : InputDirection.Up)) < (
                 (direction == InputDirection.Down || direction == InputDirection.Up) ? selectedButton.Dimentions.x * 1.3f : selectedButton.Dimentions.y * 1.3f))
            {
                if (closestInDirection == null) closestInDirection = activeUnselectedButtons[i];
                else if (Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position, direction, selectedButton.buffers) <
                    Compare.DistanceInDirection(selectedButton.transform.position, closestInDirection.transform.position, direction, selectedButton.buffers))
                {
                    if(Compare.IsInlineImage(closestInDirection.transform.position, closestInDirection.Dimentions, 
                        activeUnselectedButtons[i].transform.position, activeUnselectedButtons[i].Dimentions,
                        (direction == InputDirection.Up  || direction == InputDirection.Down) ? InputDirection.Right : InputDirection.Up))
                    {
                        //if(direction == InputDirection.Up || direction == InputDirection.Down ?
                        //(Mathf.Abs(selectedButton.transform.position.x - activeUnselectedButtons[i].transform.position.x) <
                        //Mathf.Abs(selectedButton.transform.position.x - closestInDirection.transform.position.x)) :
                        //(Mathf.Abs(selectedButton.transform.position.y - activeUnselectedButtons[i].transform.position.y) <
                        //Mathf.Abs(selectedButton.transform.position.y - closestInDirection.transform.position.y)))
                        //{
                        if(Vector2.Distance(closestInDirection.transform.position, selectedButton.transform.position) >
                            Vector2.Distance(activeUnselectedButtons[i].transform.position, selectedButton.transform.position))
                        closestInDirection = activeUnselectedButtons[i];
                        // }
                    }
                    else closestInDirection = activeUnselectedButtons[i];
                }
            }
        }

        return closestInDirection;
    }
}


public class Compare
{
    public static bool InDirectionTo(Vector2 isObject, Vector2 comparedToObject, InputDirection comparison)
    {
        switch (comparison)
        {
            case (InputDirection.Up):
                return isObject.y > comparedToObject.y;
            case (InputDirection.Down):
                return isObject.y < comparedToObject.y;
            case (InputDirection.Right):
                return isObject.x > comparedToObject.x;
            case (InputDirection.Left):
                return isObject.x < comparedToObject.x;
            default:
                Debug.LogError("input direction not an option");
                break;
        }
        Debug.LogError("Positions of comparison are equal, neither is larger");
        return false;
    }

    public static float DistanceInDirection(Vector2 originObj, Vector2 distanceObj, InputDirection direction)
    {
        return DistanceInDirection(originObj, distanceObj, direction, Vector2.zero);
    }
    public static float DistanceInDirection(Vector2 originObj, Vector2 distanceObj, InputDirection direction, Vector2 offsets)
    {
        switch (direction)
        {
            case (InputDirection.Up):
                return distanceObj.y - originObj.y - offsets.y;
            case (InputDirection.Down):
                return originObj.y - distanceObj.y - offsets.y;
            case (InputDirection.Right):
                return distanceObj.x - originObj.x - offsets.x;
            case (InputDirection.Left):
                return originObj.x - distanceObj.x - offsets.x;
            default:
                Debug.LogError("input direction not an option");
                break;
        }
        Debug.LogError("Returning a default length, please fix the switch error");
        return 0f;
    }

    public static bool IsInlineImage(Vector2 obj1Pos, Vector2 obj1Dim, Vector2 obj2Pos, Vector2 obj2Dim, InputDirection direction, float bufferPercentage = 0.5f)
    {
        Vector2 compareLine = Vector2.zero;
        if(direction == InputDirection.Down || direction == InputDirection.Up) compareLine = Vector2.up;
        else compareLine = Vector2.right;

        if (compareLine == Vector2.up &&
            ((Mathf.Abs(DistanceInDirection(obj1Pos, obj2Pos, InputDirection.Right)) < obj1Dim.y * (bufferPercentage / 2f)) ||
            (Mathf.Abs(DistanceInDirection(obj1Pos, obj2Pos, InputDirection.Right)) < obj2Dim.y * (bufferPercentage / 2f))))
        {
            return true;
        }
        else if(compareLine == Vector2.right &&
            ((Mathf.Abs(DistanceInDirection(obj1Pos, obj2Pos, InputDirection.Up)) < obj1Dim.x * (bufferPercentage / 2f)) ||
            (Mathf.Abs(DistanceInDirection(obj1Pos, obj2Pos, InputDirection.Up)) < obj2Dim.x * (bufferPercentage / 2f))))
        {
            return true;
        }
        return false;
    }
}