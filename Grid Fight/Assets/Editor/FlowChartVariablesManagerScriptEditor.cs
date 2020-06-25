using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(FlowChartVariablesManagerScript))]
public class FlowChartVariablesManagerScriptEditor : Editor
{
    List<FlowChartVariablesClass> BaseChars = new List<FlowChartVariablesClass>
    {
        new FlowChartVariablesClass("DONNA_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("KONIKO_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("PAN_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("BIRD_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("PAI_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("MERMER_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("KORA_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("DONNA_IN_SQUAD", "OFF"),
        new FlowChartVariablesClass("DONNA_IN_SQUAD", "OFF"),

    };

    public override void OnInspectorGUI()
    {
        FlowChartVariablesManagerScript origin = (FlowChartVariablesManagerScript)target;

        base.OnInspectorGUI();

        foreach (FlowChartVariablesClass item in BaseChars)
        {
            if(origin.Variables.Where(r=> r.Name == item.Name).ToList().Count == 0)
            {
                origin.Variables.Add(item);
            }
        }
        EditorUtility.SetDirty(origin);
    }
}
