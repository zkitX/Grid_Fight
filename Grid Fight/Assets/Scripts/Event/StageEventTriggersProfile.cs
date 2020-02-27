using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_StageEventTriggersProfile", menuName = "ScriptableObjects/Events/Stage Event Triggers Profile")]
public class StageEventTriggersProfile : ScriptableObject
{
    public GameSequenceEvent[] stageEventTriggers;
    public StageEventTriggersProfile[] stageEventTriggerSubProfiles;

    public List<GameSequenceEvent> GetAllEventTriggersAndSubTriggers()
    {
        List<GameSequenceEvent> gangShit = new List<GameSequenceEvent>();
        foreach(GameSequenceEvent gse1 in stageEventTriggers)
        {
            gangShit.Add(gse1);
        }
        foreach(GameSequenceEvent gse2 in GetStageEventSubProfileTriggers())
        {
            gangShit.Add(gse2);
        }
        return gangShit;
    }

    public List<GameSequenceEvent> GetStageEventSubProfileTriggers()
    {
        List<GameSequenceEvent> gangShit = new List<GameSequenceEvent>();
        foreach (StageEventTriggersProfile eventTrigProfile in stageEventTriggerSubProfiles)
        {
            foreach (GameSequenceEvent gse1 in eventTrigProfile.stageEventTriggers)
            {
                gangShit.Add(gse1);
            }
            foreach (StageEventTriggersProfile setp in eventTrigProfile.stageEventTriggerSubProfiles)
            {
                foreach(GameSequenceEvent gse2 in setp.GetStageEventSubProfileTriggers())
                {
                    gangShit.Add(gse2);
                }
            }
        }
        return gangShit;
    }
}
