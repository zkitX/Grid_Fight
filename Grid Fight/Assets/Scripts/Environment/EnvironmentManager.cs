using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Vector2Int BattleFieldSize;
    public ScriptableObjectGridStructure GridStructure;
    public Camera MainCamera;
    public TransparencySortMode test;
    public bool boooo = false;
    public Vector3 dir;

    public bool ChangeGridStructure = false;
    // Start is called before the first frame update
    void Start()
    {
        ChangeScriptableObjectGridStructure(GridStructure);
        
    }


    //Setting up the camera position
   public void ChangeScriptableObjectGridStructure(ScriptableObjectGridStructure gridStructure)
   {
        GridStructure = gridStructure;
        GridManagerScript.Instance.SetupGrid(GridStructure);
       /* switch (GridStructure.CameraBasePos)
        {
            case CameraBasePosType.VeryClose:
                MainCamera.orthographicSize = 1.5f;
                break;
            case CameraBasePosType.Close:
                MainCamera.orthographicSize = 2;
                break;
            case CameraBasePosType.Mid:
                MainCamera.orthographicSize = 2.5f;
                break;
            case CameraBasePosType.MidFar:
                MainCamera.orthographicSize = 3;
                break;
            case CameraBasePosType.Far:
                MainCamera.orthographicSize = 4;
                break;
            case CameraBasePosType.VeryFar:
                MainCamera.orthographicSize = 5;
                break;
        }*/
    }


    private void Update()
    {
       /* MainCamera.transparencySortAxis = dir;
        MainCamera.transparencySortMode = test;
        MainCamera.useJitteredProjectionMatrixForTransparentRendering = boooo;*/
        if (ChangeGridStructure)
        {
            GridManagerScript.Instance.ResetGrid();
            ChangeGridStructure = false;
            ChangeScriptableObjectGridStructure(GridStructure);
        }
    }
}

