using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using MyBox;

[RequireComponent(typeof(Animation), typeof(Image))]
public class Grid_UIButton : MonoBehaviour
{
    [HideInInspector] public int ID;

    public Grid_UIPanel parentPanel = null;
    public Grid_UIPanel ParentPanel
    {
        get
        {
            if (parentPanel == null) parentPanel = Grid_UINavigator.Instance.genericPanel;
            return parentPanel;
        }
        set
        {
            parentPanel = value;
        }
    }

    [HideInInspector] public TextMeshProUGUI buttonText;
    [HideInInspector] public Image buttonImage;

    public UI_ActionsClass[] PressActions;
    public UI_ActionsClass[] SelectActions;
    public UI_ActionsClass[] DeselectActions;

    //public List<UI_ActionsClass> PressActionsL = new List<UI_ActionsClass>();
    //public List<UI_ActionsClass> SelectActionsL = new List<UI_ActionsClass>();
    //public List<UI_ActionsClass> DeselectActionsL = new List<UI_ActionsClass>();

    public bool Active
    {
        get
        {
            return ParentPanel.focusState == UI_FocusTypes.Focused && gameObject.activeInHierarchy;
        }
    }
    [HideInInspector] public bool selected = false;
    [HideInInspector] public bool visuallySelected = false;

    [HideInInspector] public Vector2 buffers = Vector2.zero;
    public Vector2 DimentionsInScreenSpace
    {
        get
        {
            return new Vector2(buttonImage.rectTransform.rect.width / transform.localScale.x, buttonImage.rectTransform.rect.height / transform.localScale.y);
        }
    }
    public Vector2 DimentionsInWorldSpace
    {
        get
        {
            Vector2 topRight = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position) + new Vector3(buttonImage.rectTransform.rect.width / transform.localScale.x * 0.5f, buttonImage.rectTransform.rect.height / transform.localScale.y * 0.5f, 0f));
            Vector2 botLeft = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(transform.position) - new Vector3(buttonImage.rectTransform.rect.width / transform.localScale.x * 0.5f, buttonImage.rectTransform.rect.height / transform.localScale.y * 0.5f, 0f));
            return topRight - botLeft;
        }
    }

    private void Awake()
    {
        ID = Grid_UINavigator.Instance.SetupNewButtonInfo(this);
        GetComponent<BoxCollider2D>().size = GetComponent<Image>().rectTransform.rect.size;

        /*PressActionsL = PressActions.ToList();
        SelectActionsL = SelectActions.ToList();
        DeselectActionsL = DeselectActions.ToList();
        foreach(UI_ActionsClass uac in PressActionsL)
        {
            foreach(Grid_UIActions act in uac.uiActions)
            {
                uac.uiActionsL.Add(act);
            }
        }
        foreach (UI_ActionsClass uac in SelectActionsL)
        {
            foreach (Grid_UIActions act in uac.uiActions)
            {
                uac.uiActionsL.Add(act);
            }
        }
        foreach (UI_ActionsClass uac in DeselectActionsL)
        {
            foreach (Grid_UIActions act in uac.uiActions)
            {
                uac.uiActionsL.Add(act);
            }
        }*/
    }

    private void OnDestroy()
    {
        Grid_UINavigator.Instance.RemoveButtonInfo(this);
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
        if (PressEventsSequencer != null) StopCoroutine(PressEventsSequencer);
        PressEventsSequencer = SequenceEvents(PressActions);
        StartCoroutine(PressEventsSequencer);
    }

    public virtual bool SelectAction()
    {
        if (selected) return false;

        if (!visuallySelected)
        {
            if (SelectionEventsSequencer != null) StopCoroutine(SelectionEventsSequencer);
            SelectionEventsSequencer = SequenceEvents(SelectActions);
            StartCoroutine(SelectionEventsSequencer);

            visuallySelected = true;
        }


        selected = true;
        return true;
    }

    public virtual bool DeselectAction(bool playDeselectEffects)
    {
        if (!selected) return false;

        if (playDeselectEffects && visuallySelected)
        {
            if (SelectionEventsSequencer != null) StopCoroutine(SelectionEventsSequencer);
            SelectionEventsSequencer = SequenceEvents(DeselectActions);
            StartCoroutine(SelectionEventsSequencer);

            visuallySelected = false;
        }

        selected = false;
        return true;
    }

    IEnumerator PressEventsSequencer = null;
    IEnumerator SelectionEventsSequencer = null;
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

    public void RefreshCursorCheck()
    {
        if (Grid_UINavigator.Instance.navType != MenuNavigationType.Cursor) return;

        if (GetComponent<Collider2D>().IsTouching(Grid_UINavigator.Instance.cursor.GetComponent<Collider2D>()))
        {
            if (!selected) Grid_UINavigator.Instance.SelectButtonByID(ID);
        }
        else if (selected)
        {
            Grid_UINavigator.Instance.DeselectButton(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("UICursor")) CursorEnter(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("UICursor")) CursorEnter(false);
    }

    void CursorEnter(bool state)
    {
        if (Grid_UINavigator.Instance.navType != MenuNavigationType.Cursor || !Active) return;

        if (state)
        {
            Grid_UINavigator.Instance.SelectButtonByID(ID);
        }
        else
        {
            Grid_UINavigator.Instance.DeselectButton(this);
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
                //Assign image and text if autofill enabled
                if (uiAction.changeThingColorOnThisObject)
                {
                    uiAction.changeColorText = GetComponentInChildren<TextMeshProUGUI>() == null ? null : GetComponentInChildren<TextMeshProUGUI>();
                    uiAction.changeColorImage = GetComponentInChildren<Image>() == null ? null : GetComponentInChildren<Image>();
                }
                if (uiAction.setSelectionForThisButtom)
                {
                    uiAction.setSelectionButton = this;
                }
                if (uiAction.animateThis)
                {
                    uiAction.thingToAnimate = GetComponent<Animation>();
                }

                uiAction.parentButton = this;
                uiAction.Name = uiAction.GetName();
            }
        }
    }
}

[System.Serializable]
public class UI_ActionsClass
{
    [HideInInspector] public string Name;
    public Grid_UIActions[] uiActions;
    //public List<Grid_UIActions> uiActionsL = new List<Grid_UIActions>();

    public bool useStandardUnityEventsInstead = false;
    [ConditionalField("useStandardUnityEventsInstead")] [Tooltip("Time in seconds to wait before triggering the event")] public float waitBefore = 0f;
    [ConditionalField("useStandardUnityEventsInstead")] [Tooltip("Time in seconds to wait after triggering the event")] public float waitAfter = 0f;
    public UnityEvent events;
}