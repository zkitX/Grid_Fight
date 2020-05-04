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
            default:
                break;
        }
    }

    public string GetName()
    {
        switch (actionType)
        {
            case (UI_ActionTypes.Pause):
                return "Pause for " + pauseTime.ToString() + " seconds...";
            case (UI_ActionTypes.ChangeColor):
                return "Change color of the " + changeColorOf.ToString() + (changeThingColorOnThisObject ? " in this gameObject" : " assigned");
            case (UI_ActionTypes.PlayAnimation):
                return "NOT YET IMPLEMENTED";
            default:
                return "NONE";
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
    [ConditionalField("actionType", compareValues: UI_ActionTypes.ChangeColor)] public bool changeThingColorOnThisObject = false; //NEEDS TO UPDATE ABOVE VALUES
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

}
