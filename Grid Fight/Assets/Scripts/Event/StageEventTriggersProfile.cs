using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_StageEventTriggersProfile", menuName = "ScriptableObjects/Events/Stage Event Triggers Profile")]
public class StageEventTriggersProfile : ScriptableObject
{
    public GameSequenceEvent[] stageEventTriggers;
}
