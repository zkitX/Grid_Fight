﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTypes : MonoBehaviour
{
    
}

public enum TimedCheckTypes
{
    WaitForButtonPress,
    CharacterDeath,
    CharacterArrival,
    CharacterSwitchOut,
    CharacterHealthChange,
    ThisEventCalled,
    EventCalled,
    None,
}

public enum HealthChangeType
{
    MoreThan,
    LessThan,
    IsEqualTo,
    None
}

public enum EventEffectTypes
{
    WaitForSeconds,
    DebugLog,
    TriggerFungusEvent,
    TriggerCommand,
    RecruitCharacter,
    None,
}