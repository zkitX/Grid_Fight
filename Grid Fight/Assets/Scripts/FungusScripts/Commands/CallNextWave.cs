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

    public bool HasADifferentStage = false;
    [ConditionalField("HasADifferentStage", true)] public int StageToShow;
    [ConditionalField("HasADifferentStage", true)] public float TransitionDuration = 2;

    public bool HasADifferentGrid = false;
    [ConditionalField("HasADifferentGrid", true)] public ScriptableObjectGridStructure Grid;
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

        if (HasADifferentStage)
        {
            BattleManagerScript.Instance.MoveToNewGrid(StageToShow, TransitionDuration);
        }

        if (HasADifferentGrid)
        {
            EnvironmentManager.Instance.ChangeGridStructure(Grid);
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

