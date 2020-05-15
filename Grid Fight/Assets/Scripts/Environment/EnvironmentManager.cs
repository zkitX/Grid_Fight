﻿using System.Collections;
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
    IEnumerator GridLeapSequencer = null;

    public CameraStageInfoScript CameraStage;

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

    public IEnumerator MoveToNewGrid(int gridIndex, float duration, bool moveChars = true)
    {
        FightGrid destinationGrid = fightGrids[gridIndex != -1 ? gridIndex : currentGridIndex];

        GridManagerScript.Instance.MoveGrid_ToWorldPosition(destinationGrid.pivot);

        if (duration == 1234.56789f) duration = destinationGrid.hasBaseTransitionTime ? destinationGrid.baseTransitionDuration : defaultTransitionTime;

        if (GridLeapSequencer != null)
        {
            StopCoroutine(GridLeapSequencer);
        }
        currentGridIndex = gridIndex != -1 ? gridIndex : currentGridIndex;
        GridLeapSequencer = GridLeapSequence(duration, CameraStage.CameraInfo.Where(r => r.StageIndex == (gridIndex != -1 ? gridIndex : currentGridIndex)).First().CameraPosition, moveChars);
        yield return GridLeapSequencer;
    }



    CharacterType_Script[] chars;
    List<CharacterType_Script> jumpingchars = new List<CharacterType_Script>();
    IEnumerator GridLeapSequence(float duration, Vector3 translation, bool moveChars = true)
    {
        //Ensure new grid is set and moved to correct position before everything
        jumpingchars.Clear();
        float jumpHeight = 2f;
        translation.z = -8.5f;
        CharacterAnimationStateType jumpAnim = CharacterAnimationStateType.DashUp;

       // CharacterAnimationStateType jumpAnim = CharacterAnimationStateType.JumpTransition_OUT;

        Vector3 cameraStartPos = MainCamera.transform.position;
        CameraInfoClass cic = CameraStage.CameraInfo.Where(r => r.StageIndex == currentGridIndex && !r.used).First();

        float cameraStartOrtho = MainCamera.orthographicSize;
        float cameraEndOrtho = cic.OrthographicSize;
        cic.used = true;
        //DONT FORGET TO ADD CAMERA OFFSET ADJUSTMENT
        if (moveChars)
        {
            chars = System.Array.ConvertAll(BattleManagerScript.Instance.AllCharactersOnField.Where(r=> r.IsOnField).ToArray(), item => (CharacterType_Script)item);
        }
        else
        {
            chars = new CharacterType_Script[0];
        }
        Vector3[] charStartPositions = new Vector3[chars != null ? chars.Length : 0];
        Vector3[] charGridPosOffsets = new Vector3[chars != null ? chars.Length : 0];
        for (int i = 0; i < (chars != null ? chars.Length : 0); i++)
        {
            charStartPositions[i] = chars[i].transform.position;
            BattleTileScript bts = GridManagerScript.Instance.BattleTiles.Where(r => r.Pos == chars[i].UMS.CurrentTilePos).FirstOrDefault();
            if (bts.BattleTileState != BattleTileStateType.Empty
                || chars[i].UMS.WalkingSide != bts.WalkingSide)
            {
                BattleTileScript newGridTile = GridManagerScript.Instance.GetRandomFreeAdjacentTile(chars[i].UMS.CurrentTilePos, 5, false, chars[i].UMS.WalkingSide);
                Debug.Log(newGridTile.Pos);
                charGridPosOffsets[i] = GridManagerScript.Instance.BattleTiles.Where(r => r.Pos == newGridTile.Pos).First().transform.position;
                GridManagerScript.Instance.SetBattleTileState(newGridTile.Pos, BattleTileStateType.Occupied);
                chars[i].CurrentBattleTiles = new List<BattleTileScript>() { newGridTile };
                chars[i].UMS.CurrentTilePos = newGridTile.Pos;
                chars[i].UMS.Pos = new List<Vector2Int>() { newGridTile.Pos };
            }
            else
            {
                GridManagerScript.Instance.SetBattleTileState(bts.Pos, BattleTileStateType.Occupied);
                charGridPosOffsets[i] = bts.transform.position;
            }
            chars[i].SetAttackReady(false);

            if (translation == Vector3.zero && charGridPosOffsets[i] == Vector3.zero)
            {
                continue;
            }
            jumpingchars.Add(chars[i]);
            chars[i].SetAnimation(jumpAnim);

        }

       // yield return new WaitForSecondsRealtime(2);

        float timeRemaining = duration;
        float progress = 0;
        bool hasStarted = false;

        while (timeRemaining != 0 || !hasStarted)
        {
            hasStarted = true;
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 9999f);
            progress = 1f - (timeRemaining / (duration != 0f ? duration : 1f));

            MainCamera.transform.position = Vector3.Lerp(cameraStartPos, translation, UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
            MainCamera.orthographicSize = Mathf.Lerp(cameraStartOrtho, cameraEndOrtho, progress);


            for (int i = 0; i < (jumpingchars != null ? jumpingchars.Count : 0); i++)
            {
                jumpingchars[i].transform.position = Vector3.Lerp(charStartPositions[i], charGridPosOffsets[i], UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
                jumpingchars[i].transform.position += new Vector3(0, jumpHeight * UniversalGameBalancer.Instance.characterJumpCurve.Evaluate(progress), 0);
                jumpingchars[i].SpineAnim.SetAnimationSpeed(UniversalGameBalancer.Instance.jumpAnimationCurve.Evaluate(progress));
            }
            yield return null;
        }

     /*   for (int i = 0; i < (jumpingchars != null ? jumpingchars.Count : 0); i++)
        {
            chars[i].transform.position = chars[i].CurrentBattleTiles[0].transform.position;
            chars[i].SetAnimation(CharacterAnimationStateType.JumpTransition_IN);
        }
        */
        if (chars.Length != 0)
        {
            foreach (CharacterType_Script character in BattleManagerScript.Instance.PlayerControlledCharacters)
            {
                character.SetAttackReady(true);
            }
        }
    }

    public void MoveCharactersToFitNewGrid(float duration)
    {
        if (GridLeapSequencer != null) StopCoroutine(GridLeapSequencer);
        GridLeapSequencer = MoveCharactersToFitNewGridSequence(duration);
        StartCoroutine(GridLeapSequencer);
    }

    IEnumerator MoveCharactersToFitNewGridSequence(float duration)
    {
        float jumpHeight = 2f;
        CharacterAnimationStateType jumpAnim = CharacterAnimationStateType.DashUp;

        CharacterType_Script[] chars = BattleManagerScript.Instance.PlayerControlledCharacters;
        List<CharacterType_Script> charsToMove = new List<CharacterType_Script>();
        List<Vector3> charStartPositions = new List<Vector3>();
        List<Vector3> charGridPosOffsets = new List<Vector3>();
        for (int i = 0; i < (chars != null ? chars.Length : 0); i++)
        {
            if(GridManagerScript.Instance.BattleTiles.Where(r => r.Pos == chars[i].UMS.CurrentTilePos).FirstOrDefault().BattleTileState == BattleTileStateType.Blocked ||
                GridManagerScript.Instance.BattleTiles.Where(r => r.Pos == chars[i].UMS.CurrentTilePos).FirstOrDefault().WalkingSide != chars[i].UMS.WalkingSide)
            {
                BattleTileScript newGridTile = GridManagerScript.Instance.GetRandomFreeAdjacentTile(chars[i].UMS.CurrentTilePos, 5, false, chars[i].UMS.WalkingSide);
                GridManagerScript.Instance.SetBattleTileState(newGridTile.Pos, BattleTileStateType.Occupied);

                charsToMove.Add(chars[i]);
                charStartPositions.Add(chars[i].transform.position);
                charGridPosOffsets.Add(GridManagerScript.GetTranslationBetweenTiles(GridManagerScript.Instance.BattleTiles.Where(r => r.Pos == chars[i].UMS.CurrentTilePos).First(), newGridTile));

                chars[i].CurrentBattleTiles = new List<BattleTileScript>() { newGridTile };
                chars[i].UMS.CurrentTilePos = newGridTile.Pos;
                chars[i].UMS.Pos = new List<Vector2Int>() { newGridTile.Pos };

                chars[i].SetAnimation(jumpAnim);
            }
            chars[i].SetAttackReady(false);
        }

        float timeRemaining = duration;
        float progress = 0;
        float xPos = 0f;
        float yPos = 0f;
        bool hasStarted = false;
        while (timeRemaining != 0 || !hasStarted)
        {
            hasStarted = true;
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 9999f);
            progress = 1f - (timeRemaining / (duration != 0f ? duration : 1f));

            for (int i = 0; i < (charsToMove != null ? charsToMove.Count : 0); i++)
            {
                xPos = Mathf.Lerp(charStartPositions[i].x, charStartPositions[i].x + charGridPosOffsets[i].x, UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
                yPos = Mathf.Lerp(charStartPositions[i].y, charStartPositions[i].y + charGridPosOffsets[i].y, UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
                yPos += jumpHeight * UniversalGameBalancer.Instance.characterJumpCurve.Evaluate(progress);
                charsToMove[i].transform.position = new Vector3(xPos, yPos, charsToMove[i].transform.position.z);
                charsToMove[i].SpineAnim.SetAnimationSpeed(UniversalGameBalancer.Instance.jumpAnimationCurve.Evaluate(progress));
            }
            yield return null;
        }
        if (chars != null)
        {
            foreach (CharacterType_Script character in chars)
            {
                character.SetAttackReady(true);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
      //  ChangeGridStructure(GridStructures[0], true);
    }

    //Setting up the camera position and new grid stuff
    public void ChangeGridStructure(ScriptableObjectGridStructure gridStructure,int stageIndex, bool moveCameraInternally, float cameraChangeDuration = 0f)
    {
        GridManagerScript.Instance.ResetGrid();
        isChangeGridStructure = false;
        CameraManagerScript.Instance.ChangeFocusToNewGrid(CameraStage.CameraInfo.Where(r=> r.StageIndex == (stageIndex != -1 ? stageIndex : currentGridIndex)).First(), cameraChangeDuration, moveCameraInternally);
        GridManagerScript.Instance.SetupGrid(gridStructure);
        
    }
}

