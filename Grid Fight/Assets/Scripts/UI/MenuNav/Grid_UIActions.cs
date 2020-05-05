using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using TMPro;

[System.Serializable]
public class Grid_UIActions
{
    [HideInInspector] public string Name;
    [HideInInspector] public Grid_UIButton parentButton;
    public UI_ActionTypes actionType = UI_ActionTypes.None;

    public IEnumerator PlayAction()
    {
        switch (actionType)
        {
            case (UI_ActionTypes.Pause):
                yield return Pause();
                break;
            case (UI_ActionTypes.ChangeColor):
                yield return ChangeColor();
                break;
            case (UI_ActionTypes.PlayAnimation):
                yield return PlayAnimation();
                break;
            case (UI_ActionTypes.SetButtonSelection):
                yield return SetButtonSelection();
                break;
            case (UI_ActionTypes.SetPanelFocus):
                yield return SetPanelFocus();
                break;
            default:
                break;
        }
    }

    public string GetName()
    {
        switch (actionType)
        {
            case (UI_ActionTypes.None):
                return "!!NO ACTION SELECTED!!";
            case (UI_ActionTypes.Pause):
                return "Pause for " + pauseTime.ToString() + " seconds...";
            case (UI_ActionTypes.ChangeColor):
                return "Change color of the " + changeColorOf.ToString() + (changeThingColorOnThisObject ? " in this gameObject" : " assigned");
            case (UI_ActionTypes.PlayAnimation):
                return "NOT YET IMPLEMENTED";
            case (UI_ActionTypes.SetButtonSelection):
                if(setSelectionButton != null) return "Set " + (setSelectionForThisButtom ? "this" : setSelectionButton.name) + " button to " + setSelectionType.ToString();
                else return "Set button to " + setSelectionType.ToString();
            case (UI_ActionTypes.SetPanelFocus):
                return "NOT YET IMPLEMENTED";
            default:
                return actionType.ToString();
        }
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.Pause)] public float pauseTime = 0f;
    IEnumerator Pause()
    {
        yield return new WaitForSeconds(pauseTime);
    }

    public enum TextOrImage { text, image }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public TextOrImage changeColorOf = TextOrImage.image;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public TextMeshProUGUI changeColorText = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public Image changeColorImage = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public bool changeThingColorOnThisObject = false;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public Color colorToChangeTo = Color.white;
    IEnumerator ChangeColor()
    {
        if (changeColorText != null && changeColorOf == TextOrImage.text) changeColorText.color = colorToChangeTo;
        if (changeColorImage != null && changeColorOf == TextOrImage.image) changeColorImage.color = colorToChangeTo;
        yield return null;
    }

    IEnumerator PlayAnimation()
    {
        yield return null;
    }

    public enum SelectionType { Selected, Deselected }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public Grid_UIButton setSelectionButton = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public bool setSelectionForThisButtom = true;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public SelectionType setSelectionType = SelectionType.Selected;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public bool ignoreDeselectEventsForAllOtherButtons = false;
    IEnumerator SetButtonSelection()
    {
        if (setSelectionType == SelectionType.Selected) Grid_UINavigator.Instance.SelectButton(setSelectionButton, !ignoreDeselectEventsForAllOtherButtons);
        if (setSelectionType == SelectionType.Deselected) Grid_UINavigator.Instance.DeselectButton(setSelectionButton);
        yield return null;
    }

    public enum SetPanelFocusType { SetFocused, SetUnfocused, ToggleBetween }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public Grid_UIPanel setFocusedPanel = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public bool setFocusForThisPanel = false;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public SetPanelFocusType setPanelFocusType = SetPanelFocusType.ToggleBetween;
    IEnumerator SetPanelFocus()
    {
        if (setFocusForThisPanel) setFocusedPanel = parentButton.parentPanel;
        if (setFocusedPanel == null) yield break;
        switch (setPanelFocusType)
        {
            case (SetPanelFocusType.ToggleBetween):
                setFocusedPanel.focusState = setFocusedPanel.focusState == UI_FocusTypes.Focused ? UI_FocusTypes.Defocused : UI_FocusTypes.Focused;
                break;
            case (SetPanelFocusType.SetUnfocused):
                setFocusedPanel.focusState = UI_FocusTypes.Defocused;
                break;
            case (SetPanelFocusType.SetFocused):
                setFocusedPanel.focusState = UI_FocusTypes.Focused;
                break;
        }
        yield return null;
    }
}
