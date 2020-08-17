using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance = null;

    public enum Phase 
    { 
        Started = 0,
        Completed = 1,
        Quit = 2,
        Failed = 3,
    };



    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    //public void TestEvent()
    //{
    //    AnalyticsEvent.Custom("Test_Event", new Dictionary<string, object>
    //    {
    //        { "Test_Time", Time.time },
    //    });
    //}

    Dictionary<string, object> SquadTimeInfo()
    {
        return new Dictionary<string, object>
        {
            { "Time_Started", Time.time },
            { "Team_Used", SceneLoadManager.Instance.squad }
        };
    }






    public void Track_LevelPhase(string LevelName, Phase LevelPhase)
    {
        AnalyticsEvent.Custom(LevelName + "_" + LevelPhase.ToString(), SquadTimeInfo());
    }

    public void Track_WavePhase(string LevelName, string WaveName, Phase WavePhase)
    {
        AnalyticsEvent.Custom(LevelName + "_" + WaveName + "_" + WavePhase.ToString(), SquadTimeInfo());
    }
}
