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
    public Transform cameraToMove;
    public int currentGridIndex = 0;
    public FightGrid[] fightGrids;
    public AnimationCurve cameraTravelCurve;
    public AnimationCurve characterJumpCurve;
    public AnimationCurve jumpAnimationCurve;
    IEnumerator GridFocusSequencer = null;

    private void Awake()
    {
        MainCamera = Camera.main;
        cameraToMove = MainCamera.transform;
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

    public void MoveGridIntoFocus(int destinationGridIndex, CharacterType_Script[] charsToMove, bool smoothMove = true, float transitionDuration = 1234.56789f)
    {
        MoveGridIntoFocus(fightGrids[destinationGridIndex], charsToMove, smoothMove, transitionDuration);
    }

    public void MoveGridIntoFocus(FightGrid destinationGrid, CharacterType_Script[] charsToMove, bool smoothMove = true, float transitionDuration = 1234.56789f)
    {
        if (transitionDuration == 1234.56789f) transitionDuration = destinationGrid.hasBaseTransitionTime ? destinationGrid.baseTransitionDuration : defaultTransitionTime;

        Vector3 translation = -(fightGrids[currentGridIndex].transform.position - destinationGrid.transform.position);
        currentGridIndex = destinationGrid.index;

        if (GridFocusSequencer != null) StopCoroutine(GridFocusSequencer);
        GridFocusSequencer = FocusSequence(transitionDuration, charsToMove, translation);
        StartCoroutine(GridFocusSequencer);
    }

    IEnumerator FocusSequence(float duration, CharacterType_Script[] charsToMove, Vector3 translation)
    {
        Vector3 startPos = cameraToMove.transform.position;
        List<Vector3> playerStartPoses = new List<Vector3>();
        foreach (CharacterType_Script characterToMove in charsToMove) playerStartPoses.Add(characterToMove.transform.position);

        Vector3 acceleration = Vector3.zero;
        float timeRemaining = duration;
        float progress = 0;
        Vector3 newPos = Vector3.zero;
        while (timeRemaining != 0)
        {
            Debug.Log(timeRemaining);
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 9999f);
            progress = cameraTravelCurve.Evaluate(1f - (timeRemaining / duration));
            cameraToMove.transform.position = Vector3.Lerp(startPos, startPos + translation, progress);
            for (int i = 0; i < charsToMove.Length; i++)
            {
                newPos = Vector3.Lerp(playerStartPoses[i], playerStartPoses[i] + translation, progress);
                charsToMove[i].transform.position = new Vector3(newPos.x, charsToMove[i].transform.position.y, newPos.z);
            }
            yield return null;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
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

