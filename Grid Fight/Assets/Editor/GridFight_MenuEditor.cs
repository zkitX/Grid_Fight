// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using Fungus.EditorUtils;
using Fungus;

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
        EditorUtility.SetDirty(origin);
    }
}    
