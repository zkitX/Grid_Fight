// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using Fungus.EditorUtils;
using Fungus;
using System.Linq;

[CustomEditor (typeof(GridFight_Menu))]
public class GridFight_MenuEditor : CommandEditor 
{
    protected SerializedProperty textProp;
    protected SerializedProperty descriptionProp;
    protected SerializedProperty targetBlockProp;
    protected SerializedProperty hideIfVisitedProp;
    protected SerializedProperty interactableProp;
    protected SerializedProperty setMenuDialogProp;
    protected SerializedProperty hideThisOptionProp;
    protected SerializedProperty RelationshipInfoProp;

    public override void OnEnable()
    {
        base.OnEnable();

        textProp = serializedObject.FindProperty("text");
        descriptionProp = serializedObject.FindProperty("description");
        targetBlockProp = serializedObject.FindProperty("targetBlock");
        hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
        interactableProp = serializedObject.FindProperty("interactable");
        setMenuDialogProp = serializedObject.FindProperty("setMenuDialog");
        hideThisOptionProp = serializedObject.FindProperty("hideThisOption");
        RelationshipInfoProp = serializedObject.FindProperty("RelationshipInfo");
    }

    public override void DrawCommandGUI()
    {
        DrawInfo();
        var flowchart = FlowchartWindow.GetFlowchart();
        if (flowchart == null)
        {
            return;
        }

        serializedObject.Update();

        BlockEditor.BlockField(targetBlockProp,
                               new GUIContent("Target Block", "Block to call when option is selected"),
                               new GUIContent("<None>"),
                               flowchart);
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawInfo()
    {
        GridFight_Menu origin = (GridFight_Menu)target;
        var flowchart = FlowchartWindow.GetFlowchart();
        if (flowchart == null)
        {
            return;
        }
        base.OnInspectorGUI();
        if(!origin.Unlockable)
        {
            origin.EnablingBlocksName.Clear();
        }
        if (origin.Parent != null)
        {
            if(string.IsNullOrEmpty(origin.ThisBlockVariableName))
            {
                FlowChartVariablesManagerScript vars = origin.Parent.GetComponent<FlowChartVariablesManagerScript>();
                string vName = origin.Parent.name.Split('_').Last() + "_";
                FlowChartVariablesClass res = vars.Variables.Where(r => r.Name.Contains(vName)).LastOrDefault();

                if (res != null)
                {
                    int id = System.Convert.ToInt16(res.Name.Split('_').Last());
                    res = new FlowChartVariablesClass(vName + (id + 1), "OFF");
                }
                else
                {
                    res = new FlowChartVariablesClass(vName + "0", "OFF");
                }
                origin.ThisBlockVariableName = res.Name;
                vars.Variables.Add(res);
                EditorUtility.SetDirty(origin.Parent);
            }
        }
        else
        {
            Debug.LogError("Vini motha fuka put the parent");
        }
        EditorUtility.SetDirty(origin);
    }
}    
