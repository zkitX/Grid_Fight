using Fungus;
using MyBox;
using System.Collections;
using System.Collections.Generic;
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
    public bool HasADifferentGrid = false;
    [ConditionalField("HasADifferentGrid", false)] public ScriptableObjectGridStructure Grid;


    public bool CallPool = true;
    public List<TalkingTeamClass> TalkingTeam = new List<TalkingTeamClass>();

    public bool JumpUp = false;
    [Range(0, 5)]
    public float JumpAnimSpeed = 1;
    public bool UseWave = true;

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

        //GridManagerScript.Instance.BattleTiles.ForEach(r=> r.BattleTargetScript.StopAllCoroutines());
        if(TransitionDuration == 0)
        {
            BattleManagerScript.Instance.ResetAllActiveChars();
            yield return new WaitForSecondsRealtime(1f);
        }
        
        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;
        if (HasAStageUpdate)
        {
            CameraManagerScript.Instance.CameraFocusSequence(CameraManagerScript.Instance.DurationIn,
                CameraManagerScript.Instance.TransitionINZoomValue, CameraManagerScript.Instance.ZoomIn, BattleManagerScript.Instance.CurrentSelectedCharacters[ControllerType.Player1].Character.transform.position);
            //CameraManagerScript.Instance.CameraJumpInOut(2);
        }

        if (CallPool)
        {
            yield return BattleManagerScript.Instance.SetAllNonUsedCharOnBattlefield();

            yield return new WaitForSecondsRealtime(3f);
        }


        if (HasAStageUpdate && HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, FightGridToShow, false, TransitionDuration);
            yield return new WaitForSecondsRealtime(0.5f);

        }
        else if (HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, -1, true, TransitionDuration);
            yield return new WaitForSecondsRealtime(0.5f);

        }


        if (UseWave)
            yield return WaveManagerScript.Instance.SettingUpWave(WaveName);
        if (HasAStageUpdate)
        {
            yield return EnvironmentManager.Instance.MoveToNewGrid(HasAStageUpdate ? FightGridToShow : -1, TransitionDuration, TalkingTeam, JumpAnimSpeed, JumpUp);
        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;

        if (UseWave)  
        yield return WaveManagerScript.Instance.StartWaveByName(WaveName);

        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;

        yield return new WaitForSecondsRealtime(.5f);

        SetNextBlockFromName(NextBlockToFire);
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