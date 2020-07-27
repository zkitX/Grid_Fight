using Fungus;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CommandInfo("Scripting",
                "Call CallNextWave",
                "CallStartWave")]
[AddComponentMenu("")]
public class CallNextWave : Command
{
    public string WaveName;

    public float TransitionDuration = 2;

    public bool HasAStageUpdate = false;
    [ConditionalField("HasAStageUpdate", false)] public int FightGridToShow;
    public CameraInOutInfoClass CamInfo;

    public bool HasADifferentGrid = false;
    [ConditionalField("HasADifferentGrid", false)] public ScriptableObjectGridStructure Grid;

    public bool CallPool = true;
    public List<PlayersCurrentSelectedCharClass> PlayersCurrentSelectedChars = new List<PlayersCurrentSelectedCharClass>
    {
        new PlayersCurrentSelectedCharClass(ControllerType.Player1, Vector2Int.zero),
        new PlayersCurrentSelectedCharClass(ControllerType.Player2, Vector2Int.zero),
        new PlayersCurrentSelectedCharClass(ControllerType.Player3, Vector2Int.zero),
        new PlayersCurrentSelectedCharClass(ControllerType.Player4, Vector2Int.zero)
    };
    public List<TalkingTeamClass> TalkingTeam = new List<TalkingTeamClass>();

    public bool JumpUp = false;
    [Range(0, 5)]
    public float JumpAnimSpeed = 1;
    public float DelayBeforeJump = 0.5f;
    public float PoolDelay = 3;
    public bool UseWave = true;
    [ConditionalField("UseWave", false)] public bool UseTimer;
    [ConditionalField("UseTimer", false)] public float Timer;
    [ConditionalField("UseTimer", false)] public string VariableName;


    public string NextBlockToFire;

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(Wave());
    }

    private IEnumerator Wave()
    {
        if(WaveManagerScript.Instance == null)
        {
            yield return null;
        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;
        if (HasAStageUpdate)
        {
            CameraManagerScript.Instance.CameraFocusSequence(CamInfo.DurationIn, CamInfo.TransitionINZoomValue, CamInfo.ZoomIn, CamInfo.ZoomInCamMovement, BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r=> r.Value.Character != null).First().Value.Character.transform.position);
        }

        if (CallPool)
        {
            yield return BattleManagerScript.Instance.SetAllNonUsedCharOnBattlefield();

            yield return new WaitForSecondsRealtime(PoolDelay);
        }


        if (HasAStageUpdate && HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, FightGridToShow, false, TransitionDuration);
        }
        else if (HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, -1, true, TransitionDuration);

        }


        if (UseWave)
            yield return WaveManagerScript.Instance.SettingUpWave(WaveName);
        if (HasAStageUpdate)
        {
            yield return EnvironmentManager.Instance.MoveToNewGrid(HasAStageUpdate ? FightGridToShow : -1, TransitionDuration, PlayersCurrentSelectedChars, TalkingTeam, JumpAnimSpeed, CamInfo, JumpUp, wt: DelayBeforeJump);
        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;

        if (UseWave)
        {
            if (UseTimer)
            {
                yield return WaveManagerScript.Instance.StartWaveByName(WaveName, VariableName, Timer);

            }
            else
            {
                yield return WaveManagerScript.Instance.StartWaveByName(WaveName ,"");
            }

        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;
        SetNextBlockFromName(NextBlockToFire);

        Debug.Log("<b>CONTINUE FROM WAVE</b>");
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

[System.Serializable]
public class TalkingTeamClass
{
    public CharacterNameType CharacterId;
    public bool isRandomPos = true;
    [ConditionalField("isRandomPos", true)] public Vector2Int Pos;

    public TalkingTeamClass()
    {
            
    }
}

[System.Serializable]
public class PlayersCurrentSelectedCharClass
{
    public ControllerType PlayerController;
    public Vector2Int Pos;

    public PlayersCurrentSelectedCharClass(ControllerType playerController, Vector2Int pos)
    {
        PlayerController = playerController;
        Pos = pos;
    }
}

[System.Serializable]
public class CameraInOutInfoClass
{
    public float TransitionINZoomValue = 0;
    public float DurationIn = 0;
    public AnimationCurve ZoomIn;
    public AnimationCurve ZoomInCamMovement;

    public float TransitionOutZoomValue = 0;
    public float DurationOut = 0;
    public AnimationCurve ZoomOut;

    public AnimationCurve MovementCurve;

    public CameraInOutInfoClass()
    {
            
    }
}