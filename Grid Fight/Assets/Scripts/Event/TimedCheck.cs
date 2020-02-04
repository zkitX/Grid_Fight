using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
[CreateAssetMenu(fileName = "TIME_EventName", menuName = "ScriptableObjects/Events/Timed Event")]
public class TimedCheck : EventTrigger
{
    [HideInInspector] public bool isHappening = false;

    public EventManager.Check check;
    [HideInInspector] public GameSequenceEvent checker;
    public bool ceaseOnHappened = false;

    public void StartChecking(GameSequenceEvent _checker)
    {
        //Passing a reference to the GameSequenceEvent that requires this to be complete, so we can update it later
        SetCheckFunction();
        checker = _checker;
        Debug.Log("<i>Started Checking for</i> " + Name);
        EventManager.Instance.AddTimedCheckTicker(this);
    }

    public void CeaseChecking()
    {
        EventManager.Instance.RemoveTimedCheckTicker(this);
    }

    //##############################################################################
    //TYPE ASSIGNMENT SECTION
    public TimedCheckTypes TimedCheckType = TimedCheckTypes.None;

    //CHECK ASSIGNMENT
    void SetCheckFunction()
    {
        switch (TimedCheckType)
        {
            case (TimedCheckTypes.CharacterDeath):
                check = CharacterDeath;
                break;
            case (TimedCheckTypes.WaitForButtonPress):
                check = WaitForButtonPress;
                break;

            case (TimedCheckTypes.CharacterArrival):
                check = CharacterArrival;
                break;
            case (TimedCheckTypes.CharacterHealthChange):
                check = CharacterHealthChange;
                break;
            case (TimedCheckTypes.ThisEventCalled):
                check = ThisEventCalled;
                break;
            case (TimedCheckTypes.EventCalled):
                check = EventCalled;
                break;
            default:
                check = Default;
                break;
        }
    }

    //WAIT TYPE METHODS:
    bool Default()
    {
        return true;
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterDeath)] public CharacterNameType deathCharacterID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterDeath)] public bool onlyOnDeath = false;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterDeath)] public int deathsRequired = 1;
    public bool CharacterDeath()
    {
        if (deathsRequired > EventManager.Instance.CharacterDeathCount(deathCharacterID)) return false;

        if (onlyOnDeath)
        {
            return EventManager.Instance.HasCharacterDiedThisFrame(deathCharacterID);
        }
        else
        {
            return EventManager.Instance.HasCharacterDied(deathCharacterID);
        }
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.WaitForButtonPress)] public KeyCode keyToWaitFor = KeyCode.None;
    bool WaitForButtonPress()
    {
        return Input.GetKeyDown(keyToWaitFor);
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterArrival)] public CharacterNameType arrivalCharacterID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterArrival)] public bool onlyOnArrival = false;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterArrival)] public int arrivalsRequired = 1;
    bool CharacterArrival()
    {
        if (arrivalsRequired > EventManager.Instance.CharacterArrivalCount(arrivalCharacterID)) return false;

        if (onlyOnArrival)
        {
            return EventManager.Instance.HasCharacterArrivedThisFrame(arrivalCharacterID);
        }
        else
        {
            return EventManager.Instance.HasCharacterArrived(arrivalCharacterID);
        }
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public CharacterNameType healthChangeCharID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public HealthChangeType healthChange = HealthChangeType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public float healthChangeValue = 50;
    bool CharacterHealthChange()
    {
        if (!EventManager.Instance.GetHealthUpdatedLastFrame(healthChangeCharID)) return false;
        float currentHealthPercentage = EventManager.Instance.GetHealthPercentage(healthChangeCharID);

        switch (healthChange)
        {
            case (HealthChangeType.IsEqualTo):
                if(healthChangeValue == currentHealthPercentage) return true;
                break;
            case (HealthChangeType.LessThan):
                if (healthChangeValue > currentHealthPercentage) return true;
                break;
            case (HealthChangeType.MoreThan):
                if (healthChangeValue < currentHealthPercentage) return true;
                break;
            default:
                Debug.Log("Health Change type is not set on timed check: " + Name);
                break;
        }
        return false;
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.ThisEventCalled)] public bool requireCalledDuringCheck = true;
    bool ThisEventCalled()
    {
        return requireCalledDuringCheck ? EventManager.Instance.EventCalledLastFrame(checker.Name) : EventManager.Instance.EventCalled(checker.Name);
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.EventCalled)] public bool requireEventCalledDuringCheck = true;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.EventCalled)] public GameSequenceEvent EventCallRequired;
    bool EventCalled()
    {
        return requireEventCalledDuringCheck ? EventManager.Instance.EventCalledLastFrame(EventCallRequired.Name) : EventManager.Instance.EventCalled(EventCallRequired.Name);
    }
}
