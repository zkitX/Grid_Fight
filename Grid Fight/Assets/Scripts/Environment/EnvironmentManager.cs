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
        MainCamera = Camera.main;
        ChangeGridStructure();

    }

    //Setting up the camera position
   public void ChangeScriptableObjectGridStructure(ScriptableObjectGridStructure gridStructure)
   {
        GridManagerScript.Instance.SetupGrid(gridStructure);
        switch (GridStructure)
        {
            case GridStructureType.r2xc4:
                MainCamera.orthographicSize = 2f;
                MainCamera.transform.position = new Vector3(0,2f,-8.5f);
                break;
            case GridStructureType.r4xc8:
                MainCamera.orthographicSize = 3f;
                MainCamera.transform.position = new Vector3(0, 1f, -8.5f);
                break;
            case GridStructureType.r6xc12:
                break;
            case GridStructureType.r6xc12_8x4:
                break;
            case GridStructureType.r5xc8:
                MainCamera.orthographicSize = 3.7f;
                MainCamera.transform.position = new Vector3(0, 1.4f, -8.5f);
                break;
            case GridStructureType.r5xc10:
                MainCamera.orthographicSize = 3.7f;
                MainCamera.transform.position = new Vector3(0, 1.4f, -8.5f);
                break;
        }
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

