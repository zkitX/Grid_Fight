using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid_UIInputActivators : MonoBehaviour
{
    public UIActivator[] activators = new UIActivator[0];
    Grid_UIPanel parentPanel = null;

    private void Awake()
    {
        parentPanel = GetComponentInParent<Grid_UIPanel>();
        if (parentPanel != null) Debug.Log(parentPanel.panelID);
    }

    private void OnEnable()
    {
        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
        InputController.Instance.ButtonBUpEvent += Instance_ButtonBUpEvent;
        InputController.Instance.ButtonXUpEvent += Instance_ButtonXUpEvent;
        InputController.Instance.ButtonYUpEvent += Instance_ButtonYUpEvent;
        InputController.Instance.ButtonUpUpEvent += Instance_ButtonUpUpEvent;
        InputController.Instance.ButtonDownUpEvent += Instance_ButtonDownUpEvent;
        InputController.Instance.ButtonRightUpEvent += Instance_ButtonRightUpEvent;
        InputController.Instance.ButtonLeftUpEvent += Instance_ButtonLeftUpEvent;
        InputController.Instance.ButtonRUpEvent += Instance_ButtonRUpEvent;
        InputController.Instance.ButtonZRUpEvent += Instance_ButtonZRUpEvent;
        InputController.Instance.ButtonLUpEvent += Instance_ButtonLUpEvent;
        InputController.Instance.ButtonZLUpEvent += Instance_ButtonZLUpEvent;
        InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
        InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
        InputController.Instance.ButtonHomeUpEvent += Instance_ButtonHomeUpEvent;
        InputController.Instance.ButtonCaptureUpEvent += Instance_ButtonCaptureUpEvent;
        InputController.Instance.ButtonRightStickUpEvent += Instance_ButtonRightStickUpEvent;
        InputController.Instance.ButtonLeftStickUpEvent += Instance_ButtonLeftStickUpEvent;
        InputController.Instance.ButtonRightSLUpEvent += Instance_ButtonRightSLUpEvent;
        InputController.Instance.ButtonLeftSLUpEvent += Instance_ButtonLeftSLUpEvent;
        InputController.Instance.ButtonLeftSRUpEvent += Instance_ButtonLeftSRUpEvent;
        InputController.Instance.ButtonRightSRUpEvent += Instance_ButtonRightSRUpEvent;
    }

    void Instance_ButtonAUpEvent(int player) { DoActionByButton(InputButtonType.A); }
    void Instance_ButtonBUpEvent(int player) { DoActionByButton(InputButtonType.B); }
    void Instance_ButtonXUpEvent(int player) { DoActionByButton(InputButtonType.X); }
    void Instance_ButtonYUpEvent(int player) { DoActionByButton(InputButtonType.Y); }
    void Instance_ButtonUpUpEvent(int player) { DoActionByButton(InputButtonType.Up); }
    void Instance_ButtonDownUpEvent(int player) { DoActionByButton(InputButtonType.Down); }
    void Instance_ButtonRightUpEvent(int player) { DoActionByButton(InputButtonType.Right); }
    void Instance_ButtonLeftUpEvent(int player) { DoActionByButton(InputButtonType.Left); }
    void Instance_ButtonRUpEvent(int player) { DoActionByButton(InputButtonType.R); }
    void Instance_ButtonZRUpEvent(int player) { DoActionByButton(InputButtonType.ZR); }
    void Instance_ButtonLUpEvent(int player) { DoActionByButton(InputButtonType.L); }
    void Instance_ButtonZLUpEvent(int player) { DoActionByButton(InputButtonType.ZL); }
    void Instance_ButtonPlusUpEvent(int player) { DoActionByButton(InputButtonType.Plus); }
    void Instance_ButtonMinusUpEvent(int player) { DoActionByButton(InputButtonType.Minus); }
    void Instance_ButtonHomeUpEvent(int player) { DoActionByButton(InputButtonType.Home); }
    void Instance_ButtonCaptureUpEvent(int player) { DoActionByButton(InputButtonType.Capture); }
    void Instance_ButtonRightStickUpEvent(int player) { DoActionByButton(InputButtonType.Right_Stick); }
    void Instance_ButtonLeftStickUpEvent(int player) { DoActionByButton(InputButtonType.Left_Stick); }
    void Instance_ButtonRightSLUpEvent(int player) { DoActionByButton(InputButtonType.Right_SL); }
    void Instance_ButtonLeftSLUpEvent(int player) { DoActionByButton(InputButtonType.Left_SL); }
    void Instance_ButtonLeftSRUpEvent(int player) { DoActionByButton(InputButtonType.Left_SR); }
    void Instance_ButtonRightSRUpEvent(int player) { DoActionByButton(InputButtonType.Right_SR); }

    public void DoActionByName(string identifier)
    {
        if (!isActiveAndEnabled || !Grid_UINavigator.Instance.CanNavigate(MenuNavigationType.DirectButton)) return;

        List<UI_ActionsClass> actions = new List<UI_ActionsClass>();

        foreach (UIActivator activ in activators)
        {
            if (activ.ID == identifier)
            {
                foreach (UI_ActionsClass action in activ.actions)
                {
                    actions.Add(action);
                }
            }
        }

        StartCoroutine(SequenceEvents(actions.ToArray()));
    }

    public IEnumerator DoActionByNameCo(string identifier)
    {
        List<UI_ActionsClass> actions = new List<UI_ActionsClass>();

        foreach (UIActivator activ in activators)
        {
            if (activ.ID == identifier)
            {
                foreach (UI_ActionsClass action in activ.actions)
                {
                    actions.Add(action);
                }
            }
        }

        yield return SequenceEvents(actions.ToArray());
    }

    void DoActionByButton(InputButtonType btn)
    {
        //if (!Grid_UINavigator.Instance.CanNavigate(MenuNavigationType.DirectButton)) return; //UNDO AFTER TESTING
       // if (BattleManagerScript.Instance == null) return;
        if (parentPanel.focusState != UI_FocusTypes.Focused) return;

        List<UI_ActionsClass> actions = new List<UI_ActionsClass>();
        foreach(UIActivator act in activators)
        {
            if (act.Activated(btn, false))
            {
                foreach(UI_ActionsClass action in act.actions)
                {
                    actions.Add(action);
                }
            }
        }
        if(actions.Count != 0) StartCoroutine(SequenceEvents(actions.ToArray()));
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
                foreach (Grid_UIActions action in evento.uiActions)
                {
                    yield return action.PlayAction();
                }
            }
        }
    }

    private void OnDisable()
    {
        InputController.Instance.ButtonAUpEvent -= Instance_ButtonAUpEvent;
        InputController.Instance.ButtonBUpEvent -= Instance_ButtonBUpEvent;
        InputController.Instance.ButtonXUpEvent -= Instance_ButtonXUpEvent;
        InputController.Instance.ButtonYUpEvent -= Instance_ButtonYUpEvent;
        InputController.Instance.ButtonUpUpEvent -= Instance_ButtonUpUpEvent;
        InputController.Instance.ButtonDownUpEvent -= Instance_ButtonDownUpEvent;
        InputController.Instance.ButtonRightUpEvent -= Instance_ButtonRightUpEvent;
        InputController.Instance.ButtonLeftUpEvent -= Instance_ButtonLeftUpEvent;
        InputController.Instance.ButtonRUpEvent -= Instance_ButtonRUpEvent;
        InputController.Instance.ButtonZRUpEvent -= Instance_ButtonZRUpEvent;
        InputController.Instance.ButtonLUpEvent -= Instance_ButtonLUpEvent;
        InputController.Instance.ButtonZLUpEvent -= Instance_ButtonZLUpEvent;
        InputController.Instance.ButtonPlusUpEvent -= Instance_ButtonPlusUpEvent;
        InputController.Instance.ButtonMinusUpEvent -= Instance_ButtonMinusUpEvent;
        InputController.Instance.ButtonHomeUpEvent -= Instance_ButtonHomeUpEvent;
        InputController.Instance.ButtonCaptureUpEvent -= Instance_ButtonCaptureUpEvent;
        InputController.Instance.ButtonRightStickUpEvent -= Instance_ButtonRightStickUpEvent;
        InputController.Instance.ButtonLeftStickUpEvent -= Instance_ButtonLeftStickUpEvent;
        InputController.Instance.ButtonRightSLUpEvent -= Instance_ButtonRightSLUpEvent;
        InputController.Instance.ButtonLeftSLUpEvent -= Instance_ButtonLeftSLUpEvent;
        InputController.Instance.ButtonLeftSRUpEvent -= Instance_ButtonLeftSRUpEvent;
        InputController.Instance.ButtonRightSRUpEvent -= Instance_ButtonRightSRUpEvent; 
    }

    private void OnValidate()
    {
        foreach (UIActivator activator in activators)
        {
            int actions = 0;

            foreach (UI_ActionsClass uiAcCla in activator.actions)
            {
                uiAcCla.Name = (uiAcCla.useStandardUnityEventsInstead ? uiAcCla.events.GetPersistentEventCount().ToString() : uiAcCla.uiActions.Length.ToString()) +
                    " " + (uiAcCla.useStandardUnityEventsInstead ? "Unity Events" : "Ui Actions");
                actions += uiAcCla.uiActions.Length + uiAcCla.events.GetPersistentEventCount();

                foreach (Grid_UIActions uiAction in uiAcCla.uiActions)
                {
                    uiAction.parentButton = null;
                    uiAction.Name = uiAction.GetName();
                }
            }
            activator.Name = activator.ID + " >  " + actions.ToString() + " actions triggred by " + activator.input.Length.ToString() + (activator.input.Length != 1 ? " input" : " inputs");
        }
    }
}