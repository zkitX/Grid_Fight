using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public Vector2Int BattleFieldSize;
    public List<ScriptableObjectGridStructure> GridStructures = new List<ScriptableObjectGridStructure>();
    public GridStructureType GridStructure;
    public Camera MainCamera;
    public bool isChangeGridStructure = false;

    // Start is called before the first frame update
    void Start()
    {
        ChangeGridStructure();
    }

    //Setting up the camera position
   public void ChangeScriptableObjectGridStructure(ScriptableObjectGridStructure gridStructure)
   {
        GridManagerScript.Instance.SetupGrid(gridStructure);
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
        if (isChangeGridStructure)
        {
            ChangeGridStructure();
        }
    }

    public void ChangeGridStructure()
    {
        GridManagerScript.Instance.ResetGrid();
        isChangeGridStructure = false;
        ChangeScriptableObjectGridStructure(GridStructures.Where(r => r.GridStructure == GridStructure).First());
    }
}

