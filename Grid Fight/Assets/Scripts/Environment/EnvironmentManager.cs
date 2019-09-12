using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Vector2Int BattleFieldSize;
    public ScriptableObjectGridStructure GridStructure;
    public Camera MainCamera;


    public bool ChangeGridStructure = false;
    // Start is called before the first frame update
    void Start()
    {
        ChangeScriptableObjectGridStructure(GridStructure);
    }

   public void ChangeScriptableObjectGridStructure(ScriptableObjectGridStructure gridStructure)
   {
        GridStructure = gridStructure;
        GridManagerScript.Instance.SetupGrid(GridStructure);
        switch (GridStructure.CameraBasePos)
        {
            case CameraBasePosType.VeryClose:
                MainCamera.orthographicSize = 2;
                break;
            case CameraBasePosType.Close:
                MainCamera.orthographicSize = 2.75f;
                break;
            case CameraBasePosType.Mid:
                MainCamera.orthographicSize = 3.5f;
                break;
            case CameraBasePosType.MidFar:
                MainCamera.orthographicSize = 4.5f;
                break;
            case CameraBasePosType.Far:
                MainCamera.orthographicSize = 5;
                break;
            case CameraBasePosType.VeryFar:
                MainCamera.orthographicSize = 7;
                break;
        }
    }


    private void Update()
    {
        if(ChangeGridStructure)
        {
            GridManagerScript.Instance.ResetGrid();
            ChangeGridStructure = false;
            ChangeScriptableObjectGridStructure(GridStructure);
        }
    }
}

