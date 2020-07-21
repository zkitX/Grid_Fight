using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class ArenaSideSelectManager : MonoBehaviour
{
    public GameObject selectorPrefab = null;
    public float selectorSpacing = 100f;
    public enum Dimention {horizontal, vertical }
    public Dimention alignment = Dimention.horizontal;
    public Transform Team1Title;
    public Transform Team2Title;
    public Image Team1Banner;
    public Image Team2Banner;
    public GameObject ContinueToCharSelectObj = null;
    public GameObject CannotContinueWarningObj = null;
    protected bool canMoveToCharSelect = false;

    protected List<SelectSideNavBox> sideSelectors = new List<SelectSideNavBox>();

    public void RefreshCanMoveToCharSelect()
    {
        canMoveToCharSelect = sideSelectors.Where(r => r.teamSelected == 1).ToArray().Length > 0 && sideSelectors.Where(r => r.teamSelected == 2).ToArray().Length > 0;
        ContinueToCharSelectObj.SetActive(canMoveToCharSelect);
        CannotContinueWarningObj.SetActive(!canMoveToCharSelect);

        if (!canMoveToCharSelect)
        {
            CannotContinueWarningObj.GetComponent<TextMeshProUGUI>().text =
                (sideSelectors.Where(r => r.teamSelected == 1 || r.teamSelected == 2).ToArray().Length == 0 ? "Teams are" :
                sideSelectors.Where(r => r.teamSelected == 1).ToArray().Length == 0 ? "Team 1 is" :
                sideSelectors.Where(r => r.teamSelected == 2).ToArray().Length == 0 ? "Team 2 is" : "ERROR") + " missing players!";
        }
    }

    void RefreshColors()
    {
        Team1Banner.color = SceneLoadManager.Instance.teamsColor[0];
        Team2Banner.color = SceneLoadManager.Instance.teamsColor[1];
    }

    //Use this to setup side selectors when this screen is to be shown, based on the numebrs of controllers connected
    public void SetupSideSelectors()
    {
        foreach (SelectSideNavBox sideSelector in sideSelectors)
        {
            Destroy(sideSelector.gameObject);
        }

        sideSelectors.Clear();
        RefreshColors();

        for (int i = 0; i < InputController.Instance.PlayerCount; i++)
        {
            sideSelectors.Add(Instantiate(selectorPrefab, new Vector3(
                transform.position.x + (alignment == Dimention.horizontal ? (i * selectorSpacing) - ((InputController.Instance.PlayerCount - 1) * 0.5f * selectorSpacing) : 0f),
                transform.position.y + (alignment == Dimention.vertical ? (i * selectorSpacing) - ((InputController.Instance.PlayerCount - 1) * 0.5f * selectorSpacing) : 0f),
                transform.position.z), Quaternion.identity, transform).GetComponent<SelectSideNavBox>());
            sideSelectors[i].Setup(i);
            sideSelectors[i].manager = this;
        }

        Team1Title.position = new Vector3(
            alignment == Dimention.horizontal ? sideSelectors[0].transform.position.x - (selectorSpacing/2f) : Team1Title.position.x,
            alignment == Dimention.vertical ? sideSelectors[0].transform.position.y + (selectorSpacing/2f) : Team1Title.position.y,
            Team1Title.position.z
            );

        Team2Title.position = new Vector3(
            alignment == Dimention.horizontal ? sideSelectors[sideSelectors.Count - 1].transform.position.x + (selectorSpacing / 2f) : Team2Title.position.x,
            alignment == Dimention.vertical ? sideSelectors[sideSelectors.Count - 1].transform.position.y - (selectorSpacing / 2f) : Team2Title.position.y,
            Team1Title.position.z
            );

        //Auto assign players if there are 2 or more
        if(InputController.Instance.PlayerCount >= 2)
        {
            sideSelectors[0].MoveForPlayer(0, alignment == Dimention.vertical ? InputDirection.Left : InputDirection.Up);
            sideSelectors[1].MoveForPlayer(1, alignment == Dimention.vertical ? InputDirection.Right : InputDirection.Down);
        }
    }

    public void ContinueToCharSelect()
    {
        if (/*canMoveToCharSelect*/true)
        {
            SceneLoadManager.Instance.arenaLoadoutInfo.T1Players.Clear();
            SceneLoadManager.Instance.arenaLoadoutInfo.T2Players.Clear();
            foreach  (SelectSideNavBox sideSelector in sideSelectors.Where(r => r.teamSelected == 1).ToArray())
            {
                SceneLoadManager.Instance.arenaLoadoutInfo.T1Players.Add(sideSelector.playerNum - 1);
            }
            foreach (SelectSideNavBox sideSelector in sideSelectors.Where(r => r.teamSelected == 2).ToArray())
            {
                SceneLoadManager.Instance.arenaLoadoutInfo.T2Players.Add(sideSelector.playerNum - 1);
            }

            Grid_UINavigator.Instance.TriggerUIActivator("MoveToCharSelect");
        }
    }


    public void SetSideSelectorsActive(bool state)
    {
        foreach(SelectSideNavBox sideSelector in sideSelectors)
        {
            sideSelector.SetActiveState(state);
        }
    }
}
