using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance = null;

    public enum PhaseEvent 
    { 
        Started = 0,
        Completed = 1,
        Quit = 2,
        Failed = 3,
    };

    public enum CharEvent
    {
        Selected = 0, //For when they are used in your squad
        Defeated = 1,
        Recruited = 2,
        Encountered = 3,
    }



    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }




    Dictionary<string, object> SquadTimeStageInfo() //Only use in story mode, NOT FOR PVP
    {
        return new Dictionary<string, object>
        {
            { "Time_Started", Time.time },
            { "Squadie_1", SceneLoadManager.Instance.squad[0].characterID },
            { "Squadie_2", SceneLoadManager.Instance.squad[1].characterID },
            { "Squadie_3", SceneLoadManager.Instance.squad[2].characterID },
            { "Squadie_4", SceneLoadManager.Instance.squad[3].characterID },
            { "Stage", SceneLoadManager.Instance.stagePrimedToLoad != null ? SceneLoadManager.Instance.stagePrimedToLoad.Name : "No particular stage" },
            { "Player_Count", BattleManagerScript.Instance != null ? BattleManagerScript.Instance.maxPlayersUsed : 0 },
            { "Co-op_Used", BattleManagerScript.Instance != null ? BattleManagerScript.Instance.maxPlayersUsed > 1 : false },
        };
    }




    public void Track_CharacterEvent(CharacterNameType charName, CharEvent charEvent)
    {
        AnalyticsEvent.Custom("Character_" + charEvent.ToString(), new Dictionary<string, object> { { "CharacterID", charName } });
    }

    public void Track_LevelPhase(PhaseEvent LevelPhase)
    {
        if (SceneLoadManager.Instance.stagePrimedToLoad == null || !SceneLoadManager.Instance.stagePrimedToLoad.trackPhases) return;

        AnalyticsEvent.Custom("Level_" + LevelPhase.ToString(), SquadTimeStageInfo());
    }

    public void Track_WavePhase(string WaveName, PhaseEvent WavePhase)
    {
        AnalyticsEvent.Custom("Wave_" + WavePhase.ToString(), 
            new Dictionary<string, object>
            {
                { "Stage", SceneLoadManager.Instance.stagePrimedToLoad != null ? SceneLoadManager.Instance.stagePrimedToLoad.Name : "No particular stage" },
                { "Wave", WaveName },
                { "Squadie_1", SceneLoadManager.Instance.squad[0].characterID },
                { "Squadie_2", SceneLoadManager.Instance.squad[1].characterID },
                { "Squadie_3", SceneLoadManager.Instance.squad[2].characterID },
                { "Squadie_4", SceneLoadManager.Instance.squad[3].characterID },
            }
        );
    }

    public void Track_FungusEventReached(string EventName)
    {
        AnalyticsEvent.Custom("FungusEvent_Reached",
            new Dictionary<string, object>
            {
                { "Stage", SceneLoadManager.Instance.stagePrimedToLoad != null ? SceneLoadManager.Instance.stagePrimedToLoad.Name : "No particular stage" },
                { "Event", EventName },
                { "Squadie_1", SceneLoadManager.Instance.squad[0].characterID },
                { "Squadie_2", SceneLoadManager.Instance.squad[1].characterID },
                { "Squadie_3", SceneLoadManager.Instance.squad[2].characterID },
                { "Squadie_4", SceneLoadManager.Instance.squad[3].characterID },
            }
        );
    }

}
