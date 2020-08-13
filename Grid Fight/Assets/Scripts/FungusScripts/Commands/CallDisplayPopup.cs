using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Display Popup",
                "Shows a popup with the provided information to that player")]
[AddComponentMenu("")]
public class CallDisplayPopup : Command
{
    protected PopUpItem popup = null;

    [Header("Popup Info")]
    [SerializeField] protected string popupTitle = "<Title Here>";
    [SerializeField] [TextArea(10, 15)] protected string popupDescription = "<Description Here>";
    [SerializeField] protected Sprite popupImage = null;
    [SerializeField] [Tooltip("Time before the player can skip the popup")] protected float holdTime = 0.0f;
    [SerializeField] protected bool autoEndAfterTime = false;
    [ConditionalField("autoEndAfterTime")] [SerializeField] protected float autoEndTime = 5f;

    [Header("Styling")]
    [SerializeField] protected Vector2 screenPositionOffset = Vector2.zero;
    [SerializeField] protected bool adjustColors = false;
    [ConditionalField("adjustColors")] [SerializeField] protected Color boxColor = Color.black;
    [ConditionalField("adjustColors")] [SerializeField] protected Color titleColor = Color.white;
    [ConditionalField("adjustColors")] [SerializeField] protected Color descriptionColor = Color.white;


    IEnumerator TriggerPopup()
    {
        if(popup == null)
        {
            popup = GetComponentInChildren<PopUpItem>(true);
            if (popup == null)
            {
                Debug.LogError("No popup gameobject in the event manager prefab");
                Continue();
                yield break;
            }

        }
        popup.gameObject.SetActive(true);
        yield return popup.TriggerPopup(
            screenPositionOffset, popupTitle, popupDescription, popupImage, holdTime, autoEndAfterTime, autoEndTime,
            adjustColors ? boxColor : new Color(), adjustColors ? titleColor : new Color(), adjustColors ? descriptionColor : new Color()
            );

        Continue();
    }


    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(TriggerPopup());
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

