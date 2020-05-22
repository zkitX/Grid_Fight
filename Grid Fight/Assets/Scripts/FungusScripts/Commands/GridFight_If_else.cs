using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
                "Call GridFight_If_else",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
[AddComponentMenu("")]
public class GridFight_If_else : Command
{
    public List<CheckClass> Checks = new List<CheckClass>();
    public string Pass_BlockName;
    public string NotPass_BlockName;
    protected virtual void CallTheMethod()
    {
        bool pass = true;
        var flowchart = GetFlowchart();
        List<FlowChartVariablesClass> variables = FlowChartVariablesManagerScript.instance.Variables;
        foreach (CheckClass item in Checks)
        {

            FlowChartVariablesClass res = variables.Where(r => r.name == item.varname).First();
            if (res.Value != item.varvalue)
            {
                pass = false;
                break;
            }
        }
        if(pass)
        {
            SetNextBlockFromName(Pass_BlockName);

        }
        else
        {
            SetNextBlockFromName(NotPass_BlockName);
        }
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

[System.Serializable]
public class CheckClass
{
    public string varname;
    public string varvalue;
    
}