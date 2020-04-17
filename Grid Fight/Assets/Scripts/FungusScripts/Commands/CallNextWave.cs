using Fungus;
using MyBox;
using System.Collections;
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

    public bool CallAllAlly = true;


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
        BattleManagerScript.Instance.ResetAllActiveChars();
        yield return new WaitForSecondsRealtime(1f);
        BattleManagerScript.Instance.CurrentBattleState = BattleState.FungusPuppets;
        if (CallAllAlly)
        {
            yield return BattleManagerScript.Instance.SetAllNonUsedCharOnBattlefield();

            yield return new WaitForSecondsRealtime(3f);
        }


        if (HasAStageUpdate && HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, FightGridToShow, false, TransitionDuration);
        }
        else if (HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, -1, true, TransitionDuration);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        yield return WaveManagerScript.Instance.SettingUpWave(WaveName);
        if (HasAStageUpdate)
        {
            yield return EnvironmentManager.Instance.MoveToNewGrid(HasAStageUpdate ? FightGridToShow : -1, TransitionDuration);
        }

        if (CallAllAlly)
        {
            yield return new WaitForSecondsRealtime(3f);

            BattleManagerScript.Instance.RemoveAllNonUsedCharFromBoard();
        }
        // yield return new WaitForSecondsRealtime(30f);
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        
        yield return WaveManagerScript.Instance.StartWaveByName(WaveName);

       // yield return new WaitForSecondsRealtime(2);

        SetNextBlockFromName(NextBlockToFire);
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

