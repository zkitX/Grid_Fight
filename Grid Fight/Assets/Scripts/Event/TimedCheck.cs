using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class TimedCheck
{
    public string Name;
    [HideInInspector] public bool hasHappened = false;
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
            case (TimedCheckTypes.CharacterSwitchOut):
                check = CharacterSwitchOut;
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
            case (TimedCheckTypes.BattleTimeCheck):
                check = BattleTimeCheck;
                break;
            case (TimedCheckTypes.BlockCheck):
                check = BlockCheck;
                break;
            case (TimedCheckTypes.CharacterStaminaCheck):
                check = CharacterStaminaCheck;
                break;
            case (TimedCheckTypes.EventTriggeredCheck):
                check = EventTriggeredCheck;
                break;
            case (TimedCheckTypes.PotionCollectionCheck):
                check = PotionCollectionCheck;
                break;
            case (TimedCheckTypes.AvailableCharacterCountCheck):
                check = AvailableCharacterCountCheck;
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

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.WaitForButtonPress)] public InputButtonType buttonToWaitFor;
    bool WaitForButtonPress()
    {
        return EventManager.Instance.GetButtonWasPressedLastFrame(buttonToWaitFor);
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

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterSwitchOut)] public CharacterNameType switchCharacterID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterSwitchOut)] public bool onlyOnSwitch = false;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterSwitchOut)] public float switchesRequired = 1;
    bool CharacterSwitchOut()
    {
        if (switchesRequired > EventManager.Instance.CharacterSwitchCount(switchCharacterID)) return false;

        if (onlyOnSwitch)
        {
            return EventManager.Instance.HasCharacterSwitchedThisFrame(switchCharacterID);
        }
        else
        {
            return EventManager.Instance.HasCharacterSwitched(switchCharacterID);
        }
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public CharacterNameType healthChangeCharID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public CompareType healthChange = CompareType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterHealthChange)] public float healthChangeValue = 50;
    bool CharacterHealthChange()
    {
        //if (!EventManager.Instance.GetHealthUpdatedLastFrame(healthChangeCharID)) return false;
        float currentHealthPercentage = EventManager.Instance.GetHealthPercentage(healthChangeCharID);

        switch (healthChange)
        {
            case (CompareType.IsEqualTo):
                if(healthChangeValue == currentHealthPercentage) return true;
                break;
            case (CompareType.LessThan):
                if (healthChangeValue > currentHealthPercentage) return true;
                break;
            case (CompareType.MoreThan):
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

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BattleTimeCheck)] public CompareType timeCompareType = CompareType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BattleTimeCheck)] public int hoursToCompare;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BattleTimeCheck)] public int minutesToCompare;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BattleTimeCheck)] public float secondsToCompare;
    bool BattleTimeCheck()
    {
        GameTime time = new GameTime(hoursToCompare, minutesToCompare, secondsToCompare);
        float timeComparing = WaveManagerScript.Instance.battleTime.timeInSeconds;
        switch (timeCompareType)
        {
            case (CompareType.IsEqualTo):
                if (timeComparing == time.timeInSeconds) return true;
                break;
            case (CompareType.LessThan):
                if (timeComparing < time.timeInSeconds) return true;
                break;
            case (CompareType.MoreThan):
                if (timeComparing > time.timeInSeconds) return true;
                break;
            default:
                Debug.Log("Health Change type is not set on timed check: " + Name);
                break;
        }
        return false;
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BlockCheck)] public BlockInfo.BlockType blockTypeToCheck = BlockInfo.BlockType.either;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BlockCheck)] public bool checkBlockForAnyCharacter = false;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BlockCheck)] public CharacterNameType characterToCheckWhichBlocked = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BlockCheck)] public int numberOfBlocksRequired = 1;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.BlockCheck)] public bool requireBlockHappenedLastFrame = true;
    bool BlockCheck()
    {
        if (requireBlockHappenedLastFrame)
        {
            if(characterToCheckWhichBlocked == CharacterNameType.None || checkBlockForAnyCharacter)
            {
                return EventManager.Instance.GetBlockHappenedLastFrame(blockTypeToCheck, numberOfBlocksRequired);
            }
            else
            {
                return EventManager.Instance.GetCharacterBlockedLastFrame(characterToCheckWhichBlocked, blockTypeToCheck, numberOfBlocksRequired);
            }
        }
        else
        {
            if (characterToCheckWhichBlocked == CharacterNameType.None || checkBlockForAnyCharacter)
            {
                return EventManager.Instance.GetBlocksHappened(blockTypeToCheck, numberOfBlocksRequired);
            }
            else
            {
                return EventManager.Instance.GetCharacterBlocked(characterToCheckWhichBlocked, blockTypeToCheck, numberOfBlocksRequired);
            }
        }
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterStaminaCheck)] public CharacterNameType staminaChangeCharID = CharacterNameType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterStaminaCheck)] public CompareType staminaChange = CompareType.None;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.CharacterStaminaCheck)] public float staminaChangeValue = 50;
    bool CharacterStaminaCheck()
    {
        if (!EventManager.Instance.GetStaminaUpdatedLastFrame(staminaChangeCharID)) return false;
        float currentStaminaPercentage = EventManager.Instance.GetStaminaPercentage(staminaChangeCharID);

        switch (staminaChange)
        {
            case (CompareType.IsEqualTo):
                if (staminaChangeValue == currentStaminaPercentage) return true;
                break;
            case (CompareType.LessThan):
                if (staminaChangeValue > currentStaminaPercentage) return true;
                break;
            case (CompareType.MoreThan):
                if (staminaChangeValue < currentStaminaPercentage) return true;
                break;
            default:
                Debug.Log("Stamina Change type is not set on timed check: " + Name);
                break;
        }
        return false;
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.EventTriggeredCheck)] public bool requireEventTriggerDuringCheck = true;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.EventTriggeredCheck)] public GameSequenceEvent EventTriggerRequired;
    bool EventTriggeredCheck()
    {
        return requireEventTriggerDuringCheck ? EventManager.Instance.EventTriggeredLastFrame(EventTriggerRequired.Name) : EventManager.Instance.EventTriggered(EventTriggerRequired.Name);
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.PotionCollectionCheck)] public ItemType potionType = ItemType.PowerUp_All;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.PotionCollectionCheck)] public int potionsRequired = 1;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.PotionCollectionCheck)] public bool potionCollectedLastFrame = true;
    bool PotionCollectionCheck()
    {
        if (potionCollectedLastFrame)
        {
            return EventManager.Instance.GetPotionCollectedLastFrame(potionType, potionsRequired);
        }
        else
        {
            return EventManager.Instance.GetPotionCollected(potionType, potionsRequired);
        }
    }

    [ConditionalField("TimedCheckType", false, TimedCheckTypes.AvailableCharacterCountCheck)] public int availableCharactersToCheckFor = 0;
    [ConditionalField("TimedCheckType", false, TimedCheckTypes.AvailableCharacterCountCheck)] public CompareType avilableCharacterCheckRule = CompareType.IsEqualTo;
    bool AvailableCharacterCountCheck()
    {
        int charsAlive = 0;
        foreach(CharacterType_Script character in BattleManagerScript.Instance.AllCharactersOnField)
        {
            if (character.CharInfo.Health > 0) charsAlive++;
        }

        switch (avilableCharacterCheckRule)
        {
            case (CompareType.IsEqualTo):
                if (charsAlive == availableCharactersToCheckFor) return true;
                break;
            case (CompareType.LessThan):
                if (charsAlive > availableCharactersToCheckFor) return true;
                break;
            case (CompareType.MoreThan):
                if (charsAlive < availableCharactersToCheckFor) return true;
                break;
            default:
                Debug.Log("Health Change type is not set on timed check: " + Name);
                break;
        }
        return false;
    }
}
