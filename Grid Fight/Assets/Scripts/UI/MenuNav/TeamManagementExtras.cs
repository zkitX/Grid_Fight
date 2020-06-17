using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyBox;
using System.Linq;
using UnityEngine.UI;

public class TeamManagementExtras : MonoBehaviour
{
    public GameObject SquadBox;
    public GameObject CharacterSelectBox;
    public GameObject CharInfoBox;
    public GameObject MaskInfoBox;
    public GameObject MaskSelectBox;
    public GameObject GoalsBox;
    public GameObject OptionsBox;
    public GameObject TabSelect;

    public int currentTabIndex = 0;
    public TeamManagementTabInfoClass[] tabActivatorInformation = new TeamManagementTabInfoClass[]{ };

    public void ResetTeamManagement()
    {
        currentTabIndex = 0;

        SquadBox.SetActive(true);
        CharacterSelectBox.SetActive(true);
        CharInfoBox.SetActive(false);
        MaskInfoBox.SetActive(false);
        MaskSelectBox.SetActive(false);
        GoalsBox.SetActive(false);
        OptionsBox.SetActive(false);

        foreach(Transform trans in TabSelect.GetComponentsInChildren<Transform>())
        {
            trans.localScale = Vector3.one;
        }

        CharSelectBox.Instance.ChangeBoxXSize(1137);
        CharSelectBox.Instance.SetBoxSelectionMode(0);
        CharSelectBox.Instance.UpdateListSizing(5);
    }


    public void ChangeTab(int val)
    {
        if (switchingTabs) return;

        val = val / Mathf.Abs(val);
        int prevTabIndex = currentTabIndex;
        currentTabIndex = Mathf.Clamp(currentTabIndex + val, 0, tabActivatorInformation.Length);

        if (currentTabIndex == prevTabIndex) return;

        int activatorIndex = currentTabIndex < prevTabIndex ? currentTabIndex : prevTabIndex;

        StartCoroutine(SequenceEvents(
            val == -1 ? 
            tabActivatorInformation[activatorIndex].backwardActions :
            tabActivatorInformation[activatorIndex].forwardActions
            ));
    }

    bool switchingTabs = false;
    IEnumerator SequenceEvents(UI_ActionsClass[] events)
    {
        switchingTabs = true;
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
        switchingTabs = false;
    }


    private void OnValidate()
    {
        for (int i = 0; i < tabActivatorInformation.Length; i++)
        {
            tabActivatorInformation[i].Name = (i + 1).ToString() + " <-> " + (i + 2).ToString();

            foreach (UI_ActionsClass uiAcCla in tabActivatorInformation[i].forwardActions.Concat(tabActivatorInformation[i].backwardActions))
            {
                uiAcCla.Name = (uiAcCla.useStandardUnityEventsInstead ? uiAcCla.events.GetPersistentEventCount().ToString() : uiAcCla.uiActions.Length.ToString()) +
                    " " + (uiAcCla.useStandardUnityEventsInstead ? "Unity Events" : "Ui Actions");

                foreach (Grid_UIActions uiAction in uiAcCla.uiActions)
                {
                    //Assign image and text if autofill enabled
                    if (uiAction.changeThingColorOnThisObject)
                    {
                        uiAction.changeColorText = GetComponentInChildren<TextMeshProUGUI>() == null ? null : GetComponentInChildren<TextMeshProUGUI>();
                        uiAction.changeColorImage = GetComponentInChildren<Image>() == null ? null : GetComponentInChildren<Image>();
                    }
                    if (uiAction.setSelectionForThisButtom)
                    {
                        uiAction.setSelectionButton = null;
                    }
                    if (uiAction.animateThis)
                    {
                        uiAction.thingToAnimate = GetComponent<Animation>();
                    }

                    uiAction.parentButton = null;
                    uiAction.Name = uiAction.GetName();
                }
            }
        }
        
    }
}

[System.Serializable]
public class TeamManagementTabInfoClass
{
    [HideInInspector] public string Name = "";
    public UI_ActionsClass[] forwardActions = new UI_ActionsClass[] { };
    public UI_ActionsClass[] backwardActions = new UI_ActionsClass[] { };
}