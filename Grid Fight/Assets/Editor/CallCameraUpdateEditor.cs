// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using Fungus.EditorUtils;
using Fungus;
using System.Linq;

[CustomEditor (typeof(CallCameraUpdate))]
public class CallCameraUpdateEditor : CommandEditor 
{

    public override void DrawCommandGUI()
    {
        DrawInfo();
        var flowchart = FlowchartWindow.GetFlowchart();
        if (flowchart == null)
        {
            return;
        }
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    public void DrawInfo()
    {
        CallCameraUpdate origin = (CallCameraUpdate)target;
      
        base.OnInspectorGUI();
        switch (origin.CamMovementType)
        {
            case CameraMovementType.OnWorldPosition:
                origin.NextCamPos = EditorGUILayout.Vector3Field("NextCamPos", origin.NextCamPos);
                origin.NextCamPos.z = -8.5f;
                break;
            case CameraMovementType.OnCharacter:
                origin.CharacterId = (CharacterNameType)EditorGUILayout.EnumPopup("CharacterId", origin.CharacterId);
                break;
            case CameraMovementType.OnPlayer:
                origin.PlayerController = (ControllerType)EditorGUILayout.EnumPopup("PlayerController", origin.PlayerController);
                break;
            case CameraMovementType.OnTile:
                origin.TilePos = EditorGUILayout.Vector2IntField("TilePos", origin.TilePos);
                break;
        }

        EditorUtility.SetDirty(origin);
    }
}    
