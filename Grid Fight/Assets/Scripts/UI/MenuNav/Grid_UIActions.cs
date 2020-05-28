﻿using System.Collections;
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
            case (UI_ActionTypes.SetObjectActive):
                yield return SetObjectActive();
                break;
            case (UI_ActionTypes.SetNavigationSystem):
                yield return SetNavigationSystem();
                break;
            case (UI_ActionTypes.SetBriefingInfo):
                yield return SetBriefingInfo();
                break;
            case (UI_ActionTypes.SetWorldMapFocus):
                yield return SetWorldMapFocus();
                break;
            case (UI_ActionTypes.EnableCollection):
                yield return EnableCollection();
                break;
            case (UI_ActionTypes.SwitchScene):
                yield return SwitchScene();
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
                return "Play " + (animationClipToPlay == null ? "unset animation" : animationClipToPlay.name) + " on " + (thingToAnimate == null ? "unset object" : animateThis ? "this button" : thingToAnimate.name);
            case (UI_ActionTypes.SetButtonSelection):
                if(setSelectionButton != null) return "Set " + (setSelectionForThisButtom ? "this" : setSelectionButton.name) + " button to " + setSelectionType.ToString();
                else return "Set button to " + setSelectionType.ToString();
            case (UI_ActionTypes.SetPanelFocus):
                return "Set " + (setFocusForPreviousPanel ? "previous panel" : setFocusForThisPanel ? "this panel" : "panel: " + setFocusedPanel) + 
                    (setPanelFocusType == SetPanelFocusType.SetFocused ? " to be in focus" : 
                    setPanelFocusType == SetPanelFocusType.SetUnfocused ? " to be out of focus" : " to the opposite focus");
            case (UI_ActionTypes.SetObjectActive):
                return activationObject == null ? "NO OBJECT SET" : "Set " + activationObject.name + " to " + (setActiveState == true ? "active" : "deactivated") +
                    (activationPauseBefore != 0 ? " after " + activationPauseBefore.ToString() + (activationPauseBefore != 1 ? " seconds" : " second") : "") +
                    (activationPauseAfter != 0 ? " and wait " + activationPauseAfter.ToString() + (activationPauseAfter != 1 ? " seconds" : " second") + " afterward" : "");
            case (UI_ActionTypes.SetNavigationSystem):
                return "Set navigation to " + navigationType.ToString();
            case (UI_ActionTypes.SetBriefingInfo):
                return "Set briefing for " + (stageForBriefing != null ? stageForBriefing.Name : "undetermined stage");
            case (UI_ActionTypes.SetWorldMapFocus):
                return thingToFocusMapOn == null ? "Reset Map Focus" : "Set world map to focus on " + thingToFocusMapOn.name +
                    (focusMapZoom != 1 ? " with a zoom of " + focusMapZoom.ToString() + "x" :
                    "") + " over " + focusMapTiming.ToString() + (focusMapTiming == 1 ? " second" : " seconds");
            case (UI_ActionTypes.EnableCollection):
                return enablingCollectionID == "" ? "NO COLLECTION SPECIFIED" : "Set collection: '" + enablingCollectionID + "' to " +
                    (collectionEnableState ? "enabled" : "disabled");
            case (UI_ActionTypes.SwitchScene):
                return "Switch scene to " + sceneSwitchType.ToString();
            default:
                return actionType.ToString();
        }
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.Pause)] public float pauseTime = 1f;
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


    [ConditionalField("actionType", compareValues: UI_ActionTypes.PlayAnimation)] public Animation thingToAnimate = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.PlayAnimation)] public bool animateThis = true;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.PlayAnimation)] public AnimationClip animationClipToPlay = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.PlayAnimation)] public bool holdForAnimation = false;
    IEnumerator PlayAnimation()
    {
        if(thingToAnimate == null || animationClipToPlay == null)
        {
            Debug.LogError("Animation for button not correctly setup");
            yield break;
        }

        if(thingToAnimate.isPlaying) thingToAnimate.Stop();
        thingToAnimate.clip = animationClipToPlay;
        thingToAnimate.Play();
        while (holdForAnimation && thingToAnimate.isPlaying)
        {
            yield return null;
        }
        yield return null;
    }

    public enum SelectionType { Selected, Deselected }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public Grid_UIButton setSelectionButton = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public bool setSelectionForThisButtom = true;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public SelectionType setSelectionType = SelectionType.Selected;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetButtonSelection)] public bool ignoreDeselectEventsForAllOtherButtons = false;
    IEnumerator SetButtonSelection()
    {
        if (setSelectionType == SelectionType.Selected)
        {
            if (Grid_UINavigator.Instance.navType == MenuNavigationType.Cursor) yield break;
            Grid_UINavigator.Instance.SelectButton(setSelectionButton, !ignoreDeselectEventsForAllOtherButtons);
        }
        if (setSelectionType == SelectionType.Deselected) Grid_UINavigator.Instance.DeselectButton(setSelectionButton, Grid_UINavigator.Instance.navType == MenuNavigationType.Cursor ? true : !ignoreDeselectEventsForAllOtherButtons);
        yield return null;
    }

    public enum SetPanelFocusType { SetFocused, SetUnfocused, ToggleBetween }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public Grid_UIPanel setFocusedPanel = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public bool setFocusForPreviousPanel = false;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public bool setFocusForThisPanel = false;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public SetPanelFocusType setPanelFocusType = SetPanelFocusType.ToggleBetween;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetPanelFocus)] public bool setPanelActiveStateAswell = false;
    IEnumerator SetPanelFocus()
    {
        if (setFocusForPreviousPanel)
        {
            setFocusedPanel = parentButton.parentPanel.lastActivateCallPanel;
        }
        else if (setFocusForThisPanel) setFocusedPanel = parentButton.parentPanel;

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
        if (setFocusedPanel.focusState == UI_FocusTypes.Focused)
        {
            setFocusedPanel.lastActivateCallPanel = parentButton.parentPanel;
            Grid_UINavigator.Instance.RefreshCursorCheck();
        }
        if (setPanelActiveStateAswell) setFocusedPanel.gameObject.SetActive(setFocusedPanel.focusState == UI_FocusTypes.Focused ? true : false);
        yield return null;
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetObjectActive)] public float activationPauseBefore = 0f;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetObjectActive)] public float activationPauseAfter = 0f;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetObjectActive)] public GameObject activationObject = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetObjectActive)] public bool setActiveState = false;
    IEnumerator SetObjectActive()
    {
        yield return new WaitForSeconds(activationPauseBefore);
        activationObject.SetActive(setActiveState);
        yield return new WaitForSeconds(activationPauseAfter);
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetNavigationSystem)] public MenuNavigationType navigationType = MenuNavigationType.Relative;
    IEnumerator SetNavigationSystem()
    {
        Grid_UINavigator.Instance.SetNavigation(navigationType);
        yield return null;
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetBriefingInfo)] public Grid_UIBriefing briefing = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetBriefingInfo)] public StageProfile stageForBriefing = null;
    IEnumerator SetBriefingInfo()
    {
        if (briefing == null || stageForBriefing == null) yield break;
        briefing.SetupBriefing(stageForBriefing);
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public WorldMenuExtras worldMenuRef = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public float focusMapTiming = 1f;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public Transform thingToFocusMapOn = null;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public float focusMapZoom = 1f;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public bool focusMapOnScreenCentre = true;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public Vector2 screenPositionOffset;
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SetWorldMapFocus)] public bool pauseTillEndOfFocus = false;
    IEnumerator SetWorldMapFocus()
    {
        if (worldMenuRef == null) yield break;
        if (pauseTillEndOfFocus) yield return worldMenuRef.FocusLerp((focusMapOnScreenCentre ? new Vector2(0.5f, 0.5f) : screenPositionOffset), focusMapTiming, thingToFocusMapOn, focusMapZoom);
        else worldMenuRef.SetFocusToObject((focusMapOnScreenCentre ? new Vector2(0.5f, 0.5f) : screenPositionOffset), focusMapTiming, thingToFocusMapOn, focusMapZoom);
        yield return null;
    }

    [ConditionalField("actionType", compareValues: UI_ActionTypes.EnableCollection)] public string enablingCollectionID = "";
    [ConditionalField("actionType", compareValues: UI_ActionTypes.EnableCollection)] public bool collectionEnableState = false;
    IEnumerator EnableCollection()
    {
        Grid_UINavigator.Instance.EnableCollection(enablingCollectionID, collectionEnableState);
        yield return null;
    }

    public enum SceneSwitchType { MenuScene, BattleScene }
    [ConditionalField("actionType", compareValues: UI_ActionTypes.SwitchScene)] public SceneSwitchType sceneSwitchType = SceneSwitchType.BattleScene;
    IEnumerator SwitchScene()
    {
        if(sceneSwitchType == SceneSwitchType.BattleScene)
        {
            SceneLoadManager.Instance.LoadScene("BattleSceneDefault");
        }
        else
        {
            SceneLoadManager.Instance.LoadScene("MenuScene");
        }
        yield return null;
    }
}
