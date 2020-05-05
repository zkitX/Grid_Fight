using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid_UIPanel : MonoBehaviour
{
    public bool isGenericPanel = false;
    public string panelID = "Unassigned";
    public UI_FocusTypes focusState = UI_FocusTypes.Focused;
    public Grid_UIButton[] ChildButtons
    {
        get
        {
            return GetComponentsInChildren<Grid_UIButton>();
        }
    }
    [HideInInspector] public bool setup = false;

    private void Awake()
    {
        SetupChildButtonPanelInfo();
    }

    public void SetupChildButtonPanelInfo()
    {
        if (setup) return;

        if (isGenericPanel)
        {
            setup = true;
            return;
        }

        foreach(Grid_UIButton button in ChildButtons)
        {
            if (button.parentPanel == null || button.parentPanel.isGenericPanel)
            {
                button.parentPanel = this;
            }
            else if(button.parentPanel == this)
            {
                Debug.LogWarning("Trying to set parent panel to the same panel the button currently belongs to");
            }
            else
            {
                Debug.LogError("Button parent panel being reassigned, is there more than 1 Grid_UIPanel in its parents?");
            }
        }

        setup = true;
    }

    private void OnValidate()
    {
        if (isGenericPanel)
        {
            panelID = "Universal";
        }
    }
}
