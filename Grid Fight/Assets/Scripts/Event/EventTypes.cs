using System.Collections;
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
    BattleTimeCheck,
    BlockCheck,
    CharacterStaminaCheck,
    EventTriggeredCheck,
    PotionCollectionCheck,
    AvailableCharacterCountCheck,
    None = 1000000,
}

public enum CompareType
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

//UI STUFF HERE

public enum UI_ActionTypes
{
    None,
    Pause,
    ChangeColor,
    PlayAnimation,
    SetButtonSelection,
    SetPanelFocus,
    SetObjectActive,
    SetNavigationSystem,
    SetBriefingInfo,
    SetWorldMapFocus,
}

public enum UI_FocusTypes
{
    None,
    Focused,
    Defocused,
}