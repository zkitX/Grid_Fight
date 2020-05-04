using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            return buttons.Where(r => r.gameObject.activeInHierarchy).ToArray();
        }
        set
        {

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
                if (value.name == buttons[i].name) selectedButtonIndex = i;
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        if(InputController.Instance != null)
        {
            InputController.Instance.LeftJoystickUsedEvent += ButtonChangeInput;
        }
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

    public void SelectButton(Grid_UIButton btn)
    {
        if (btn == null) return;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == btn.name)
            {
                SelectButton(i);
                break;
            }
        }
    }

    public void SelectButton(int index)
    {
        if (buttons[index].selected) return;

        DeselectAllButtons();
        buttons[index].SelectAction();
        selectedButton = buttons[index];
    }

    public void DeselectButton(int index)
    {
        if (!buttons[index].selected) return;
        buttons[index].DeselectAction();
    }

    public void DeselectAllButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            DeselectButton(i);
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
                    closestInDirection = activeUnselectedButtons[i];
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
}