using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class WorldStageSelect : MonoBehaviour
{
    public static WorldStageSelect Instance;

    protected StageButton[] stageBtns;

    private void Awake()
    {
        Instance = this;
        stageBtns = GetComponentsInChildren<StageButton>();
    }

    public Transform GetCurrentFocusStage()
    {
        StageButton curBtn = stageBtns.Where(r => r.stage != null && r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return null;

        return curBtn.transform;
    }

    public void SelectLastFocusedButton()
    {
        StageButton curBtn = stageBtns.Where(r => r.stage != null && r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return;

        Grid_UINavigator.Instance.SelectButton(curBtn.GetComponent<Grid_UIButton>(), true);
    }

    public void RefreshStageButtons()
    {
        foreach(StageButton btn in stageBtns)
        {
            btn.ProcessLockState();
        }
    }

    public void ShowStageButtonsNames(bool state)
    {
        foreach (StageButton btn in stageBtns)
        {
            btn.ShowStageText(state);
        }
    }

    //MOSTLY DEMO SHIT

    public void ShowUnfocusedStageButtons(bool state)
    {
        StageButton[] unfocused = stageBtns.Where(r => r.stage == null || r.stage.ID != Grid_UIBriefing.Instance.curStage.ID).ToArray();
        foreach (StageButton btn in unfocused)
        {
            btn.ShowButton(state);
        }
    }

    public void PressFocusedButtonAnim(bool state)
    {
        StageButton curBtn = stageBtns.Where(r => r.stage != null && r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return;

        Animation anim = curBtn.GetComponent<Animation>();
        anim.clip = null;

        Grid_UINavigator.Instance.DeselectButton(curBtn.GetComponent<Grid_UIButton>(), false);

        if (state)
        {
            anim.clip = anim.GetClip("DV2_StageSelect_PRESS_low");
            if (anim.clip == null) anim.clip = anim.GetClip("DV2_StageSelect_PRESS_high");
        }
        else
        {
            anim.clip = anim.GetClip("DV2_StageSelect_DEPRESS_low");
            if (anim.clip == null) anim.clip = anim.GetClip("DV2_StageSelect_DEPRESS_high");
        }

        anim.Play();
    }
}

