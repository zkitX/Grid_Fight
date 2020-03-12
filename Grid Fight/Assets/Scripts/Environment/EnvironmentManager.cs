using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    public Vector3 CameraPosition = new Vector3(0, 1f, -8.5f);
    public Vector2Int BattleFieldSize;
    public List<ScriptableObjectGridStructure> GridStructures = new List<ScriptableObjectGridStructure>();
    public Camera MainCamera;
    public bool isChangeGridStructure = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        ChangeGridStructure(GridStructures[0]);

    }

    //Setting up the camera position
   public void ChangeScriptableObjectGridStructure(ScriptableObjectGridStructure gridStructure)
   {
        GridManagerScript.Instance.SetupGrid(gridStructure);
        MainCamera.orthographicSize = gridStructure.OrthographicSize;
        MainCamera.transform.position = gridStructure.CameraPosition;
    }

    public void ChangeGridStructure(ScriptableObjectGridStructure gridStructure)
    {
        GridManagerScript.Instance.ResetGrid();
        isChangeGridStructure = false;
        ChangeScriptableObjectGridStructure(gridStructure);
    }
}

