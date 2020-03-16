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

    public float defaultTransitionTime = 3f;
    public Transform fightGridMaster;
    public int currentGridIndex = 0;
    public FightGrid[] fightGrids;
    public AnimationCurve cameraTravelCurve;
    public AnimationCurve characterJumpCurve;
    public AnimationCurve jumpAnimationCurve;
    IEnumerator GridFocusSequencer = null;

    private void Awake()
    {
        PopulateGrids();
        Instance = this;
    }


    void PopulateGrids()
    {
        if (GetComponentInChildren<FightGridMaster>() == null) return;
        fightGridMaster = GetComponentInChildren<FightGridMaster>().transform;
        fightGrids = new FightGrid[GetComponentsInChildren<FightGrid>().Length];
        foreach (FightGrid grid in GetComponentsInChildren<FightGrid>())
        {
            if (!(fightGrids.Length <= grid.index))
            {
                if (fightGrids[grid.index] == null)
                {
                    fightGrids[grid.index] = grid;
                }
                else Debug.LogError("Fight Grid Indexing set up incorrectly at index '" + grid.index + "', please check to ensure there are no double indexes...");
            }
            else Debug.LogError("Indexing for fight grids not done correctly, there are some indexes that excede the range");
        }
        foreach (FightGrid grid in fightGrids)
        {
            if (grid == null)
            {
                Debug.LogError("Error in setting up Fight Grid Indexes, check that all are assigned within the proper range!!! >:(");
                break;
            }
        }
    }

    public void MoveGridIntoFocus(int destinationGridIndex, bool smoothMove = true, float transitionDuration = 1234.56789f)
    {
        MoveGridIntoFocus(fightGrids[destinationGridIndex], smoothMove, transitionDuration);
    }

    public void MoveGridIntoFocus(FightGrid destinationGrid, bool smoothMove = true, float transitionDuration = 1234.56789f)
    {
        if (transitionDuration == 1234.56789f) transitionDuration = destinationGrid.hasBaseTransitionTime ? destinationGrid.baseTransitionDuration : defaultTransitionTime;

        Vector3 translation = fightGrids[currentGridIndex].transform.position - destinationGrid.transform.position;
        currentGridIndex = destinationGrid.index;

        if (GridFocusSequencer != null) StopCoroutine(GridFocusSequencer);
        GridFocusSequencer = FocusSequence(transitionDuration, translation);
        StartCoroutine(GridFocusSequencer);
    }

    IEnumerator FocusSequence(float duration, Vector3 translation)
    {
        Vector3 startPos = fightGridMaster.transform.position;
        Vector3 endPos = startPos + translation;

        Vector3 acceleration = Vector3.zero;
        float timeRemaining = duration;
        float progress = 0;
        while (timeRemaining != 0)
        {
            Debug.Log(timeRemaining);
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 9999f);
            progress = cameraTravelCurve.Evaluate(1f - (timeRemaining / duration));
            fightGridMaster.transform.position = Vector3.Lerp(fightGridMaster.transform.position, endPos, progress);
            yield return null;
        }
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

