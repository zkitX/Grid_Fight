using PlaytraGamesLtd;
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

    public CameraStageInfoScript CameraStage;
    CharacterType_Script[] chars;
    List<CharacterType_Script> jumpingchars = new List<CharacterType_Script>();


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

    public IEnumerator MoveToNewGrid(int gridIndex, float duration, List<PlayersCurrentSelectedCharClass> playersCurrentSelectedChars, List<TalkingTeamClass> arrivingChar, float jumpAnimSpeed,
    CameraInOutInfoClass camInfo, float windTransitionRotation, bool jumpUp = false, bool moveChars = true, float wt = 0.5f)
    {
        FightGrid destinationGrid = fightGrids[gridIndex != -1 ? gridIndex : currentGridIndex];
        float v = 0;
        if(windTransitionRotation == -1)
        {
            v = Utils.GetAngleInDegToPoint(GridManagerScript.Instance.transform.position - destinationGrid.pivot) + 90;
        }

        GridManagerScript.Instance.MoveGrid_ToWorldPosition(destinationGrid.pivot);
        currentGridIndex = gridIndex != -1 ? gridIndex : currentGridIndex;
        yield return GridLeapSequence(duration, CameraStage.CameraInfo.Where(r => r.StageIndex == (gridIndex != -1 ? gridIndex : currentGridIndex)).First().CameraPosition, playersCurrentSelectedChars, arrivingChar, jumpAnimSpeed,
            camInfo, v, jumpUp, moveChars, wt);
    }

    
    IEnumerator GridLeapSequence(float duration, Vector3 translation, List<PlayersCurrentSelectedCharClass> playersCurrentSelectedChars, List<TalkingTeamClass> arrivingChar,
        float jumpAnimSpeed, CameraInOutInfoClass camInfo, float windTransitionRotation,
        bool jumpUp = false, bool moveChars = true, float wt = 0.5f)
    {
        //Ensure new grid is set and moved to correct position before everything
        CameraInfoClass cic = CameraStage.CameraInfo.Where(r => r.StageIndex == currentGridIndex && !r.used).First();

        if (duration > 0)
        {

            jumpingchars.Clear();
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
                jumpingchars.Add(chars[i]);
                chars[i].SetAnimation(jumpAnim);
                chars[i].SpineAnim.SetAnimationSpeed(jumpAnimSpeed);
            }
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.StageSwitchSound, AudioBus.HighPrio);
            float waitingTime = 0;
            while (waitingTime < wt)
            {
                waitingTime += BattleManagerScript.Instance.DeltaTime;
                yield return null;
            }
            CameraManagerScript.Instance.SetWindTransitionAnim(true, windTransitionRotation == -1 ? 0 : windTransitionRotation);

            if (duration > 0)
            {
                CameraManagerScript.Instance.CameraFocusSequence(camInfo.DurationOut, camInfo.TransitionOutZoomValue, camInfo.ZoomOut, camInfo.MovementCurve, Vector3.zero);
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
                        BattleTileScript nextBts = GridManagerScript.Instance.GetBattleTile(playersCurrentSelectedChars.Where(r=> r.PlayerController == (ControllerType)a).First().Pos);
                        GridManagerScript.Instance.SetBattleTileState(nextBts.Pos, BattleTileStateType.Occupied);
                        cb.UMS.CurrentTilePos = nextBts.Pos;
                        cb.UMS.Pos.Clear();
                        cb.UMS.Pos.Add(nextBts.Pos);
                        charsToLand.Add(cb);
                        break;
                    }
                }
            }

            for (int i = 0; i < arrivingChar.Count; i++)
            {
                if(charsToLand.Where(r=> r.CharInfo.CharacterID == arrivingChar[i].CharacterId).ToList().Count == 0)
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
                    charsToLand.Add(cb);
                }
            }
           
            if (duration > 0)
            {
                yield return CameraManagerScript.Instance.CameraMoveSequence_Co(camInfo.DurationOut, GridManagerScript.Instance.GetBattleTile(BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r=> r.Value.Character != null).First().Value.Character.UMS.CurrentTilePos).transform.position, camInfo.MovementCurve);
            }

            yield return CameraManagerScript.Instance.CameraFocusSequence_Co(camInfo.DurationIn, camInfo.TransitionINZoomValue, camInfo.ZoomIn);
            CameraManagerScript.Instance.TransitionAnimController.SetBool("UIState", false);
            foreach (BaseCharacter item in charsToLand)
            {
                item.SetAnimation(jumpUp ? CharacterAnimationStateType.Arriving : CharacterAnimationStateType.JumpTransition_IN);
                item.SpineAnim.SetAnimationSpeed(jumpAnimSpeed);
            }

            yield return new WaitForSeconds(0.1f);

            foreach (BaseCharacter item in charsToLand)
            {
                item.transform.position = GridManagerScript.Instance.GetBattleTile(item.UMS.CurrentTilePos).transform.position;

            }

            while (charsToLand.Where(r => r.IsOnField).ToList().Count != charsToLand.Count)
            {
                yield return null;
            }
        }

     
        CameraManagerScript.Instance.CameraFocusSequence(duration >= 0 ? 1f : 0,
         cic.OrthographicSize, camInfo.ZoomOut, camInfo.MovementCurve, cic.CameraPosition);

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

