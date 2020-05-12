using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MyBox;

public class Grid_UINavigator : MonoBehaviour
{
    public static Grid_UINavigator Instance;

    //public bool loopAround = false;
    public bool specificStartingButton = true;
    [ConditionalField("specificStartingButton")] public Grid_UIButton startingButton = null;
    [ConditionalField("specificStartingButton", true)] public InputDirection startingDirection = InputDirection.Up;

    [HideInInspector] public float buttonBufferPercentage = 0.5f;

    protected Grid_UIButton[] buttons = { };
    protected Grid_UIButton[] ActiveButtons
    {
        get
        {
            return buttons.Where(r => r.gameObject.activeInHierarchy && r.ParentPanel.focusState == UI_FocusTypes.Focused).ToArray();
        }
    }

    protected Grid_UIPanel[] panels = { };
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
                if (value.ID == buttons[i].ID)
                {
                    Debug.Log("Current selected button: <b>" + buttons[i].name + "</b> with ID <b>" + buttons[i].ID + "</b> on panel <b>" + buttons[i].ParentPanel.panelID + "</b>");
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
    }

    private void Start()
    {
        StartCoroutine(SelectFirstButton());
    }

    IEnumerator SelectFirstButton()
    {
        while(buttons.Length == 0)
        {
            yield return null;
        }
        if (startingButton != null) SelectButton(startingButton);
        else SelectTopButtonInStartingDirection();
    }

    int IDIndex = 0;
    public int SetupNewButtonInfo(Grid_UIButton btn)
    {
        int btnID = IDIndex;
        IDIndex++;
        List<Grid_UIButton> btns = buttons.ToList();
        btns.Add(btn);
        buttons = btns.ToArray();
        return btnID;
    }

    public void RemoveButtonInfo(Grid_UIButton btn)
    {
        List<Grid_UIButton> btns = buttons.ToList();
        btns.Remove(btn);
        buttons = btns.ToArray();
    }

    public void SetupButtonPanelInfo(Grid_UIPanel panel)
    {
        List<Grid_UIPanel> pnls = panels.ToList();
        pnls.Add(panel);
        panels = pnls.ToArray();
    }

    public void RemoveButtonPanelInfo(Grid_UIPanel panel)
    {
        List<Grid_UIPanel> pnls = panels.ToList();
        pnls.Remove(panel);
        panels = pnls.ToArray();
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




    public void SelectButtonByID(int buttonID)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttonID == buttons[i].ID) SelectButton(i);
        }
    }

    public void SelectTopButtonInStartingDirection()
    {
        SelectButton(GetButtonFurthestInDirection(startingDirection));
    }

    int GetButtonIndex(Grid_UIButton btn)
    {
        if (btn == null) return -1;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].ID == btn.ID)
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

    [Tooltip("The zones scale based on the button image wherein other buttons can be selected inLine")][Range(0.1f, 2f)]public float inLineBuffer = 0.6f;
    public Grid_UIButton GetButtonClosestInDirectionFromSelected(InputDirection direction)
    {
        Grid_UIButton[] activeUnselectedButtons = ActiveButtons.Where(r => r.ID != selectedButton.ID).ToArray();
        if (activeUnselectedButtons.Length == 0) return null;

        Grid_UIButton closestInDirection = null;
        for (int i = 0; i < activeUnselectedButtons.Length; i++)
        {
            if (Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position, direction, selectedButton.buffers) > 0f &&
                Mathf.Abs(Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position,
                (direction == InputDirection.Down || direction == InputDirection.Up) ? InputDirection.Right : InputDirection.Up)) < (
                 (direction == InputDirection.Down || direction == InputDirection.Up) ? selectedButton.DimentionsInScreenSpace.x * 0.5f * inLineBuffer : selectedButton.DimentionsInScreenSpace.y * 0.5f * inLineBuffer))
            {
                if (closestInDirection == null) closestInDirection = activeUnselectedButtons[i];
                else if (Compare.DistanceInDirection(selectedButton.transform.position, activeUnselectedButtons[i].transform.position, direction, selectedButton.buffers) <
                    Compare.DistanceInDirection(selectedButton.transform.position, closestInDirection.transform.position, direction, selectedButton.buffers))
                {
                    if(Compare.IsInlineImage(closestInDirection.transform.position, closestInDirection.DimentionsInScreenSpace, 
                        activeUnselectedButtons[i].transform.position, activeUnselectedButtons[i].DimentionsInScreenSpace,
                        (direction == InputDirection.Up  || direction == InputDirection.Down) ? InputDirection.Right : InputDirection.Up))
                    {
                        if(Vector2.Distance(closestInDirection.transform.position, selectedButton.transform.position) >
                            Vector2.Distance(activeUnselectedButtons[i].transform.position, selectedButton.transform.position))
                        {
                            closestInDirection = activeUnselectedButtons[i];
                        }
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