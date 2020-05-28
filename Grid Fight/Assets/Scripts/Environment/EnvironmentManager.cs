using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;

    public Vector2Int BattleFieldSize;
    public List<ScriptableObjectGridStructure> GridStructures = new List<ScriptableObjectGridStructure>();
    public bool isChangeGridStructure = false;

    public float defaultTransitionTime = 3f;
    public Transform fightGridMaster;
    public int currentGridIndex = 0;
    public FightGrid[] fightGrids;
    IEnumerator GridLeapSequencer = null;

    public CameraStageInfoScript CameraStage;

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

    public IEnumerator MoveToNewGrid(int gridIndex, float duration, List<TalkingTeamClass> arrivingChar, bool jumpUp = false, bool moveChars = true)
    {
        FightGrid destinationGrid = fightGrids[gridIndex != -1 ? gridIndex : currentGridIndex];

        GridManagerScript.Instance.MoveGrid_ToWorldPosition(destinationGrid.pivot);

        if (duration == 1234.56789f) duration = destinationGrid.hasBaseTransitionTime ? destinationGrid.baseTransitionDuration : defaultTransitionTime;

        if (GridLeapSequencer != null)
        {
            StopCoroutine(GridLeapSequencer);
        }
        currentGridIndex = gridIndex != -1 ? gridIndex : currentGridIndex;
        GridLeapSequencer = GridLeapSequence(duration, CameraStage.CameraInfo.Where(r => r.StageIndex == (gridIndex != -1 ? gridIndex : currentGridIndex)).First().CameraPosition, arrivingChar, jumpUp, moveChars);
        yield return GridLeapSequencer;
    }



    CharacterType_Script[] chars;
    List<CharacterType_Script> jumpingchars = new List<CharacterType_Script>();
    IEnumerator GridLeapSequence(float duration, Vector3 translation, List<TalkingTeamClass> arrivingChar, bool jumpUp = false, bool moveChars = true)
    {
        //Ensure new grid is set and moved to correct position before everything
        CameraInfoClass cic = CameraStage.CameraInfo.Where(r => r.StageIndex == currentGridIndex && !r.used).First();

        if (duration > 0)
        {
            jumpingchars.Clear();
            float jumpHeight = 2f;
            CharacterAnimationStateType jumpAnim = jumpUp ? CharacterAnimationStateType.Reverse_Arriving : CharacterAnimationStateType.JumpTransition_OUT;
            cic.used = true;
            //DONT FORGET TO ADD CAMERA OFFSET ADJUSTMENT
            if (moveChars)
            {
                chars = System.Array.ConvertAll(BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToArray(), item => (CharacterType_Script)item);
            }
            else
            {
                chars = new CharacterType_Script[0];
            }
            Vector3[] charStartPositions = new Vector3[chars != null ? chars.Length : 0];
            Vector3[] charGridPosOffsets = new Vector3[chars != null ? chars.Length : 0];
            for (int i = 0; i < (chars != null ? chars.Length : 0); i++)
            {
                if (false)
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


                    if (translation == Vector3.zero && charGridPosOffsets[i] == Vector3.zero)
                    {
                        continue;
                    }
                }
                jumpingchars.Add(chars[i]);
                chars[i].SetAnimation(jumpAnim);
            }

            while (chars.Where(r => !r.IsOnField).ToList().Count != chars.Length)
            {
                yield return null;

            }
            if (duration > 0)
            {
                CameraManagerScript.Instance.CameraFocusSequence(CameraManagerScript.Instance.DurationOut,
                CameraManagerScript.Instance.TransitionOutZoomValue, CameraManagerScript.Instance.ZoomOut, Vector3.zero);
            }

            List<BaseCharacter> charsToLand = new List<BaseCharacter>();
            for (int i = 0; i < BattleManagerScript.Instance.AllCharactersOnField.Count; i++)
            {
                bool isIn = false;
                BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField[i];
                for (int a = 0; a < BattleManagerScript.Instance.CurrentSelectedCharacters.Count; a++)
                {
                    if (BattleManagerScript.Instance.CurrentSelectedCharacters[(ControllerType)a].Character != null && BattleManagerScript.Instance.CurrentSelectedCharacters[(ControllerType)a].Character == cb)
                    {
                        isIn = true;
                    }
                }

                if (isIn)
                {
                    BattleTileScript nextBts = GridManagerScript.Instance.GetRandomFreeAdjacentTile(cb.UMS.CurrentTilePos, 5, false, cb.UMS.WalkingSide);
                    GridManagerScript.Instance.SetBattleTileState(nextBts.Pos, BattleTileStateType.Occupied);
                    cb.UMS.CurrentTilePos = nextBts.Pos;
                    cb.UMS.Pos.Clear();
                    cb.UMS.Pos.Add(nextBts.Pos);
                    cb.CurrentBattleTiles.Clear();
                    cb.CurrentBattleTiles.Add(nextBts);
                    charsToLand.Add(cb);
                }
            }

            for (int i = 0; i < arrivingChar.Count; i++)
            {
                BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == arrivingChar[i].CharacterId).FirstOrDefault();
                if (cb == null)
                {
                    cb = BattleManagerScript.Instance.CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == arrivingChar[i].CharacterId).FirstOrDefault();
                    if (cb == null)
                    {
                        cb = BattleManagerScript.Instance.CreateTalkingChar(arrivingChar[i].CharacterId);
                    }

                }
                BattleTileScript nextBts = arrivingChar[i].isRandomPos ? GridManagerScript.Instance.GetFreeBattleTile(WalkingSideType.LeftSide) : GridManagerScript.Instance.GetBattleTile(arrivingChar[i].Pos);
                GridManagerScript.Instance.SetBattleTileState(nextBts.Pos, BattleTileStateType.Occupied);
                cb.UMS.CurrentTilePos = nextBts.Pos;
                cb.UMS.Pos.Clear();
                cb.UMS.Pos.Add(nextBts.Pos);
                cb.CurrentBattleTiles.Clear();
                cb.CurrentBattleTiles.Add(nextBts);
                charsToLand.Add(cb);
            }

            float timeRemaining = duration;
            float progress = 0;
            bool hasStarted = false;
            Vector3 cameraStartPos = CameraManagerScript.Instance.transform.position;
            Vector3 cameraFinalPos = BattleManagerScript.Instance.CurrentSelectedCharacters[ControllerType.Player1].Character.CurrentBattleTiles.First().transform.position;
            cameraFinalPos.z = cameraStartPos.z;
            while (timeRemaining >= 0 || !hasStarted)
            {
                hasStarted = true;
                timeRemaining -= Time.deltaTime;
                progress = 1f - (timeRemaining / (duration != 0f ? duration : 1f));
                CameraManagerScript.Instance.transform.position = Vector3.Lerp(cameraStartPos, cameraFinalPos, UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
                // MainCamera.orthographicSize = Mathf.Lerp(cameraStartOrtho, cameraEndOrtho, progress);

                if (jumpUp)
                {
                    /* for (int i = 0; i < (jumpingchars != null ? jumpingchars.Count : 0); i++)
                     {
                         jumpingchars[i].transform.position = Vector3.Lerp(charStartPositions[i], charGridPosOffsets[i], UniversalGameBalancer.Instance.cameraTravelCurve.Evaluate(progress));
                         jumpingchars[i].transform.position += new Vector3(0, jumpHeight * UniversalGameBalancer.Instance.characterJumpCurve.Evaluate(progress), 0);
                         jumpingchars[i].SpineAnim.SetAnimationSpeed(UniversalGameBalancer.Instance.jumpAnimationCurve.Evaluate(progress));
                     }*/
                }

                yield return null;
            }



            yield return CameraManagerScript.Instance.CameraFocusSequence_Co(CameraManagerScript.Instance.DurationIn, CameraManagerScript.Instance.TransitionINZoomValue, CameraManagerScript.Instance.ZoomIn);

            foreach (BaseCharacter item in charsToLand)
            {
                item.SetAnimation(CharacterAnimationStateType.JumpTransition_IN);
            }

            yield return new WaitForSeconds(0.1f);

            foreach (BaseCharacter item in charsToLand)
            {
                item.transform.position = item.CurrentBattleTiles.Last().transform.position;

            }

            while (charsToLand.Where(r => r.IsOnField).ToList().Count != charsToLand.Count)
            {
                yield return null;
            }
        }
       

        CameraManagerScript.Instance.CameraFocusSequence(duration >= 0 ? 1f : 0,
         cic.OrthographicSize, CameraManagerScript.Instance.ZoomOut, cic.CameraPosition);

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
       // CameraManagerScript.Instance.ChangeFocusToNewGrid(CameraStage.CameraInfo.Where(r=> r.StageIndex == (stageIndex != -1 ? stageIndex : currentGridIndex)).First(), cameraChangeDuration, moveCameraInternally);
        GridManagerScript.Instance.SetupGrid(gridStructure);
        
    }
}

