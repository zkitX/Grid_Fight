using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using MyBox;

public class Grid_UIButton : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI buttonText;
    [HideInInspector] public Image buttonImage;
    public UI_ActionsClass[] PressActions;
    public UI_ActionsClass[] SelectActions;
    public UI_ActionsClass[] DeselectActions;
    [HideInInspector] public bool selected = false;
    [HideInInspector] public Vector2 buffers = Vector2.zero;
    public Vector2 Dimentions
    {
        get
        {
            return new Vector2(buttonImage.rectTransform.rect.width, buttonImage.rectTransform.rect.height);
        }
    }

    private void Start()
    {
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
            buffers = new Vector2((buttonImage.rectTransform.rect.width / 2f) * Grid_UINavigator.Instance.buttonBufferPercentage,
                (buttonImage.rectTransform.rect.height / 2f) * Grid_UINavigator.Instance.buttonBufferPercentage);
        }
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void PressAction()
    {
        StartCoroutine(SequenceEvents(PressActions));
    }

    public virtual void SelectAction()
    {
        //buttonText.color = Color.white;
        //buttonImage.color = Color.red;
        StartCoroutine(SequenceEvents(SelectActions));
        selected = true;
    }

    public virtual void DeselectAction()
    {
        //buttonText.color = Color.black;
        //buttonImage.color = Color.white;
        StartCoroutine(SequenceEvents(DeselectActions));
        selected = false;
    }

    IEnumerator SequenceEvents(UI_ActionsClass[] events)
    {
        yield return null;
        foreach (UI_ActionsClass evento in events)
        {
            if (evento.useStandardUnityEventsInstead)
            {
                yield return new WaitForSeconds(evento.waitBefore);
                evento.events.Invoke();
                yield return new WaitForSeconds(evento.waitAfter);
            }
            else
            {
                foreach(Grid_UIActions action in evento.uiActions)
                {
                    yield return action.PlayAction();
                }
            }
        }
    }



    #region Actions

    public void Action_ChangeButtonColor(Color color)
    {
        buttonImage.color = color;
    }

    public void Action_ChangeButtonTextColor(Color color)
    {
        buttonText.color = color;
    }

    #endregion



    private void OnEnable()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text == "ButtonText" || buttonText.text == "StandardButton") buttonText.text = gameObject.name;
    }

    private void OnValidate()
    {
        if(buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text == "ButtonText" || buttonText.text == "StandardButton") buttonText.text = gameObject.name;
        foreach(UI_ActionsClass uiAcCla in PressActions.Concat(SelectActions).Concat(DeselectActions))
        {
            uiAcCla.Name = (uiAcCla.useStandardUnityEventsInstead ? uiAcCla.events.GetPersistentEventCount().ToString() : uiAcCla.uiActions.Length.ToString()) +
                " " + (uiAcCla.useStandardUnityEventsInstead ? "Unity Events" : "Ui Actions");

            foreach(Grid_UIActions uiAction in uiAcCla.uiActions)
            {
                uiAction.Name = uiAction.GetName();
                //Assign image and text if autofill enabled
                if (uiAction.changeThingColorOnThisObject)
                {
                    uiAction.changeColorText = GetComponentInChildren<TextMeshProUGUI>() == null ? null : GetComponentInChildren<TextMeshProUGUI>();
                    uiAction.changeColorImage = GetComponentInChildren<Image>() == null ? null : GetComponentInChildren<Image>();
                }
            }
        }
    }
}

[System.Serializable]
public class UI_ActionsClass
{
    [HideInInspector] public string Name;
    public Grid_UIActions[] uiActions;
    public bool useStandardUnityEventsInstead = false;
    [ConditionalField("useStandardUnityEventsInstead")] [Tooltip("Time in seconds to wait before triggering the event")] public float waitBefore = 0f;
    [ConditionalField("useStandardUnityEventsInstead")] [Tooltip("Time in seconds to wait after triggering the event")] public float waitAfter = 0f;
    public UnityEvent events;
}