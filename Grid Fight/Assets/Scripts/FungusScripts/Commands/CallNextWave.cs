using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallNextWave",
                "CallStartWave")]
[AddComponentMenu("")]
public class CallNextWave : Command
{
    public string WaveName;

    public float TransitionDuration = 2;
    public bool HasADifferentStage = false;
    [ConditionalField("HasADifferentStage", false)] public int FightGridToShow;

    public bool HasADifferentGrid = false;
    [ConditionalField("HasADifferentGrid", false)] public ScriptableObjectGridStructure Grid;
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

        if (HasADifferentStage && HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, false, TransitionDuration);
            EnvironmentManager.Instance.MoveToNewGrid(FightGridToShow, TransitionDuration);
        }
        else if (HasADifferentStage)
        {
            EnvironmentManager.Instance.MoveToNewGrid(FightGridToShow, TransitionDuration);
        }
        else if (HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid, true, TransitionDuration);
            EnvironmentManager.Instance.MoveCharactersToFitNewGrid(TransitionDuration);
        }

        yield return WaveManagerScript.Instance.StartWaveByName(WaveName);
        SetNextBlockFromName(NextBlockToFire);
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

