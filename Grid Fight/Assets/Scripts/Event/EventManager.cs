using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[RequireComponent(typeof(Flowchart))]
public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Stage Config")]
    [SerializeField] public StageEventTriggersProfile stageEventTriggersProfile;
    public List<GameSequenceEvent> stageEventTriggers { get; private set; } = new List<GameSequenceEvent>();
    public delegate bool Check();
    protected List<TimedCheck> currentTimedChecks = new List<TimedCheck>();
    IEnumerator timedCheckTicker = null;
    List<GameSequenceEvent> queuedCompleteEvents = new List<GameSequenceEvent>();
    IEnumerator completeEventSequencer = null;

    [Header("Debug Info")]
    [SerializeField] protected List<CharacterEventInfoClass> deadCharacters = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> diedThisFrame = new List<CharacterNameType>();
    [SerializeField] protected List<CharacterEventInfoClass> charactersWhomstHaveArrived = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> arrivedThisFrame = new List<CharacterNameType>();
    [SerializeField] protected List<CharacterEventInfoClass> characterVitalities = new List<CharacterEventInfoClass>();
    protected List<CharacterEventInfoClass> healthChangedLastFrame = new List<CharacterEventInfoClass>();
    protected List<CharacterEventInfoClass> staminaChangedLastFrame = new List<CharacterEventInfoClass>();
    [SerializeField] protected List<string> eventsCalled = new List<string>();
    [SerializeField] protected List<string> eventDirectCallsLastFrame = new List<string>();
    [SerializeField] protected List<string> eventsTriggered = new List<string>();
    protected List<string> eventsTriggeredLastFrame = new List<string>();
    [SerializeField] protected List<CharacterEventInfoClass> charactersSwitched = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> charactersSwitchedLastFrame = new List<CharacterNameType>();
    protected List<InputButtonType> buttonsPressedLastFrame = new List<InputButtonType>();
    [SerializeField] protected List<BlockInfo> blocks = new List<BlockInfo>();
    protected List<BlockInfo> blocksLastFrame = new List<BlockInfo>();
    [SerializeField] protected List<PotionInfoClass> potionsCollected = new List<PotionInfoClass>();
    protected List<PotionInfoClass> potionsCollectedLastFrame = new List<PotionInfoClass>();

    //[Tooltip("How many seconds between checks, increase for performance boost, decrease for accuracy")][SerializeField] protected float timeBetweenChecks = 1f;

    #region Initialize
    private void Awake()
    {
        Instance = this;
    }

    //Initialize the events for the first time
    public void StartEventManager()
    {
        ResetEventsInManager();
    }

    //Reset all event checks, and repopulate current events from the event profile
    public void ResetEventsInManager()
    {
        stageEventTriggers.Clear();
        stageEventTriggers = new List<GameSequenceEvent>();
        if (stageEventTriggersProfile != null)
        {
            foreach (GameSequenceEvent gameSeqEvent in stageEventTriggersProfile.GetAllEventTriggersAndSubTriggers())
            {
                stageEventTriggers.Add(Instantiate(gameSeqEvent));
            }
        }
        InitialiseEvents();
    }

    //Start all events in the event profile
    void InitialiseEvents()
    {
        foreach(GameSequenceEvent gameEvent in stageEventTriggers)
        {
            gameEvent.Initialise();
        }
    }
    #endregion

    #region TimedCheck Handling
    public void AddTimedCheckTicker(TimedCheck checker)
    {
        currentTimedChecks.Add(checker);
        StartStop_TimedCheckTicker();
    }

    public void RemoveTimedCheckTicker(TimedCheck checker)
    {
        if (IsChecking(checker))
        {
            currentTimedChecks.Remove(checker);
            Debug.Log("<i>Stopped Checking for</i> " + checker.Name);
        }
        StartStop_TimedCheckTicker();
    }

    public bool IsChecking(TimedCheck checker)
    {
        foreach (TimedCheck timedCheck in currentTimedChecks)
        {
            if (timedCheck == checker)
            {
                return true;
            }
        }
        return false;
    }

    void StartStop_TimedCheckTicker()
    {
        if (timedCheckTicker == null)
        {
            timedCheckTicker = TimedCheckTick();
            StartCoroutine(timedCheckTicker);
        }
        else if (currentTimedChecks.Count == 0)
        {
            StopCoroutine(timedCheckTicker);
            timedCheckTicker = null;
        }
    }

    IEnumerator TimedCheckTick()
    {
        //Go through each TimedCheck currently active and change their states depending on their delegates
        //Wait a bit between checks to help with performance
        while (true)
        {
            List<GameSequenceEvent> EventsChecking = new List<GameSequenceEvent>();

            foreach (TimedCheck timedCheck in currentTimedChecks.ToArray())
            {
                timedCheck.isHappening = timedCheck.check();
                if (timedCheck.check() && !timedCheck.hasHappened) timedCheck.hasHappened = true;
                if (timedCheck.isHappening)
                {
                    //Add the event checking on this timer to a list of events to update their checks at the end, if it hasnt been added already
                    bool checkerAlreadyChecking = false;
                    foreach (GameSequenceEvent eventChecking in EventsChecking)
                    {
                        if (eventChecking == timedCheck.checker)
                        {
                            checkerAlreadyChecking = true;
                            break;
                        }
                    }
                    if (!checkerAlreadyChecking) EventsChecking.Add(timedCheck.checker);
                }
                if (timedCheck.hasHappened && timedCheck.ceaseOnHappened) timedCheck.CeaseChecking();
            }

            //Start the checks in the gameEvents that have had a timedCheck set to isHappening this frame
            foreach(GameSequenceEvent gameEvent in EventsChecking)
            {
                gameEvent.CheckTimedRequirements();
            }

            // yield return new WaitForSecondsRealtime(timeBetweenChecks);
            yield return null;
        }
    }
    #endregion

    #region Enquiries
    public bool HasHappened(GameSequenceEvent gseqEvent)
    {
        foreach(GameSequenceEvent GSequenceEvent in stageEventTriggers)
        {
            if(GSequenceEvent.hasHappened && GSequenceEvent.Name == gseqEvent.Name)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasConcluded(GameSequenceEvent gseqEvent)
    {
        foreach (GameSequenceEvent GSequenceEvent in stageEventTriggers)
        {
            if (GSequenceEvent.hasHappened && !GSequenceEvent.ceaseOnComplete && GSequenceEvent.Name == gseqEvent.Name)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Queuing Completed Events
    public void AddCompletedGameEvent(GameSequenceEvent gameEvent)
    {
        if (gameEvent.ignoreQueue)
        {
            StartCoroutine(gameEvent.CompleteEventSequence());
            return;
        }

        queuedCompleteEvents.Add(gameEvent);
        if(completeEventSequencer == null)
        {
            completeEventSequencer = CompleteEventSequenceQueue();
            StartCoroutine(completeEventSequencer);
        }
    }

    IEnumerator CompleteEventSequenceQueue()
    {
        while(queuedCompleteEvents.Count > 0)
        {
            yield return queuedCompleteEvents[0].CompleteEventSequence();
            queuedCompleteEvents.Remove(queuedCompleteEvents[0]);
            yield return null;
        }
        completeEventSequencer = null;
    }
    #endregion

    #region Game Stats Updating

    #region Character Deaths Handling
    public void AddCharacterDeath(BaseCharacter character)
    {
        diedThisFrame.Add(character.CharInfo.CharacterID);
        StartCoroutine(ResetCharacterDeathsLastFrame(character.CharInfo.CharacterID));

        foreach(CharacterEventInfoClass deadCharacter in deadCharacters)
        {
            if(deadCharacter.character.CharInfo.CharacterID == character.CharInfo.CharacterID)
            {
                deadCharacter.deaths++;
                return;
            }
        }
        deadCharacters.Add(new CharacterEventInfoClass(character, 1));
    }

    IEnumerator ResetCharacterDeathsLastFrame(CharacterNameType charName)
    {
        yield return null;
        diedThisFrame.Remove(charName);
    }

    public bool HasCharacterDiedThisFrame(CharacterNameType charID)
    {
        foreach(CharacterNameType charWhomstDied in diedThisFrame)
        {
            if (charWhomstDied == charID) return true;
        }
        return false;
    }
    public bool HasCharacterDied(CharacterNameType charID)
    {
        foreach (CharacterEventInfoClass deadChar in deadCharacters)
        {
            if (deadChar.character.CharInfo.CharacterID == charID) return true;
        }
        return false;
    }
    public int CharacterDeathCount(CharacterNameType charID)
    {
        foreach (CharacterEventInfoClass deadChar in deadCharacters)
        {
            if (deadChar.character.CharInfo.CharacterID == charID) return deadChar.deaths;
        }
        return 0;
    }
    #endregion

    #region Character Arrival(Good movie) Handling
    public void AddCharacterArrival(BaseCharacter character)
    {
        arrivedThisFrame.Add(character.CharInfo.CharacterID);
        StartCoroutine(ResetCharacterArrivalsLastFrame(character.CharInfo.CharacterID));

        foreach (CharacterEventInfoClass characterWhomstHasArrived in charactersWhomstHaveArrived)
        {
            if (characterWhomstHasArrived.character.CharInfo.CharacterID == character.CharInfo.CharacterID)
            {
                characterWhomstHasArrived.arrivals++;
                return;
            }
        }
        charactersWhomstHaveArrived.Add(new CharacterEventInfoClass(1, character));
    }

    IEnumerator ResetCharacterArrivalsLastFrame(CharacterNameType charName)
    {
        yield return null;
        arrivedThisFrame.Remove(charName);
    }

    public bool HasCharacterArrivedThisFrame(CharacterNameType charID)
    {
        foreach(CharacterNameType characterWhomstHasArrived in arrivedThisFrame)
        {
            if (characterWhomstHasArrived == charID) return true;
        }
        return false;
    }
    public bool HasCharacterArrived(CharacterNameType charID)
    {
        foreach(CharacterEventInfoClass characterWhomstHasArrived in charactersWhomstHaveArrived)
        {
            if (characterWhomstHasArrived.character.CharInfo.CharacterID == charID) return true;
        }
        return false;
    }
    public int CharacterArrivalCount(CharacterNameType charID)
    {
        foreach(CharacterEventInfoClass characterWhomstHasArrived in charactersWhomstHaveArrived)
        {
            if(characterWhomstHasArrived.character.CharInfo.CharacterID == charID)
            {
                return characterWhomstHasArrived.arrivals;
            }
        }
        return 0;
    }
    #endregion

    #region Health Management
    public void UpdateHealth(BaseCharacter character)
    {
        foreach(CharacterEventInfoClass characterVitality in characterVitalities)
        {
            if(character == characterVitality.character)
            {
                characterVitality.healthPercentage = character.CharInfo.HealthPerc;
                healthChangedLastFrame.Add(characterVitality);
                StartCoroutine(ResetCharacterHealthChangesLastFrame(characterVitality));
                return;
            }
        }
        CharacterEventInfoClass charVitality = new CharacterEventInfoClass(character);
        charVitality.healthPercentage = character.CharInfo.HealthPerc;
        characterVitalities.Add(charVitality);
        healthChangedLastFrame.Add(charVitality);
        StartCoroutine(ResetCharacterHealthChangesLastFrame(charVitality));
    }

    IEnumerator ResetCharacterHealthChangesLastFrame(CharacterEventInfoClass character)
    {
        yield return null;
        healthChangedLastFrame.Remove(character);
    }

    public float GetHealthPercentage(CharacterNameType charID)
    {
        float _healthPercentage = -10000f;
        int numberOfMatchingVitalities = 0;
        foreach (CharacterEventInfoClass characterVitality in characterVitalities)
        {
            if (charID == characterVitality.character.CharInfo.CharacterID)
            {
                numberOfMatchingVitalities++;
                _healthPercentage = characterVitality.healthPercentage;
            }
        }
        if(numberOfMatchingVitalities == 0)
        {
            Debug.Log("No matching vitalities on the board");
            return 0;
        }
        else if(numberOfMatchingVitalities > 1)
        {
            Debug.Log("More than one vitality with a matching name, returning the last value");
        }
        return _healthPercentage;
    }

    public bool GetHealthUpdatedLastFrame(CharacterNameType charID)
    {
        foreach(CharacterEventInfoClass charHealthUpdatedLastFrame in healthChangedLastFrame)
        {
            if (charHealthUpdatedLastFrame.character.CharInfo.CharacterID == charID)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Character Stamina Management
    public void UpdateStamina(BaseCharacter character)
    {
        foreach (CharacterEventInfoClass characterVitality in characterVitalities)
        {
            if (character == characterVitality.character)
            {
                characterVitality.staminaPercentage = character.CharInfo.StaminaPerc;
                staminaChangedLastFrame.Add(characterVitality);
                StartCoroutine(ResetCharacterStaminaChangesLastFrame(characterVitality));
                return;
            }
        }
        CharacterEventInfoClass charVitality = new CharacterEventInfoClass(character);
        charVitality.staminaPercentage = character.CharInfo.StaminaPerc;
        characterVitalities.Add(charVitality);
        staminaChangedLastFrame.Add(charVitality);
        StartCoroutine(ResetCharacterStaminaChangesLastFrame(charVitality));
    }

    IEnumerator ResetCharacterStaminaChangesLastFrame(CharacterEventInfoClass character)
    {
        yield return null;
        staminaChangedLastFrame.Remove(character);
    }

    public float GetStaminaPercentage(CharacterNameType charID)
    {
        float _staminaPercentage = -10000f;
        int numberOfMatchingVitalities = 0;
        foreach (CharacterEventInfoClass characterVitality in characterVitalities)
        {
            if (charID == characterVitality.character.CharInfo.CharacterID)
            {
                numberOfMatchingVitalities++;
                _staminaPercentage = characterVitality.staminaPercentage;
            }
        }
        if (numberOfMatchingVitalities == 0)
        {
            Debug.Log("No matching vitalities on the board");
            return 0;
        }
        else if (numberOfMatchingVitalities > 1)
        {
            Debug.Log("More than one vitality with a matching name, returning the last value");
        }
        return _staminaPercentage;
    }

    public bool GetStaminaUpdatedLastFrame(CharacterNameType charID)
    {
        foreach (CharacterEventInfoClass charStaminaUpdatedLastFrame in staminaChangedLastFrame)
        {
            if (charStaminaUpdatedLastFrame.character.CharInfo.CharacterID == charID)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Call Management
    public void CallEventDirectly(string eventName)
    {
        eventDirectCallsLastFrame.Add(eventName);
        StartCoroutine(ResetEventsCalledLastFrame(eventName));
        if (!eventsCalled.Contains(eventName)) eventsCalled.Add(eventName); 
        foreach (string eventCalledLastFrame in eventDirectCallsLastFrame)
        {
            if (eventCalledLastFrame == eventName) return;
        }
    }

    IEnumerator ResetEventsCalledLastFrame(string eventName)
    {
        yield return null;
        for (int i = 0; i < eventDirectCallsLastFrame.Count; i++)
        {
            if(eventDirectCallsLastFrame[i] == eventName)
            {
                eventDirectCallsLastFrame.RemoveAt(i);
                break;
            }
        }
    }

    public bool EventCalledLastFrame(string eventName)
    {
        foreach(string eventCalled in eventDirectCallsLastFrame)
        {
            if (eventCalled == eventName) return true;
        }
        return false;
    }

    public bool EventCalled(string eventName)
    {
        foreach (string eventCalled in eventsCalled)
        {
            if (eventCalled == eventName) return true;
        }
        return false;
    }
    #endregion

    #region Event Trigger Management
    public void AddTriggeredEvent(string eventName)
    {
        ResetEventsTriggeredLastFrame(eventName);
        if (!eventsCalled.Contains(eventName)) eventsTriggered.Add(eventName);
        foreach (string eventTriggeredLastFrame in eventsTriggeredLastFrame)
        {
            if (eventTriggeredLastFrame == eventName) return;
        }
        eventsTriggeredLastFrame.Add(eventName);
    }

    IEnumerator ResetEventsTriggeredLastFrame(string eventName)
    {
        yield return null;
        eventsTriggeredLastFrame.Remove(eventName);
    }

    public bool EventTriggeredLastFrame(string eventName)
    {
        foreach (string eventTriggered in eventsTriggeredLastFrame)
        {
            if (eventTriggered == eventName) return true;
        }
        return false;
    }

    public bool EventTriggered(string eventName)
    {
        foreach (string eventTriggered in eventsTriggered)
        {
            if (eventTriggered == eventName) return true;
        }
        return false;
    }
    #endregion

    #region Character Switch Management
    public void AddCharacterSwitched(BaseCharacter character)
    {
        charactersSwitchedLastFrame.Add(character.CharInfo.CharacterID);
        StartCoroutine(ResetCharacterSwitchesLastFrame(character.CharInfo.CharacterID));

        foreach (CharacterEventInfoClass characterSwitched in charactersSwitched)
        {
            if (characterSwitched.character.CharInfo.CharacterID == character.CharInfo.CharacterID)
            {
                characterSwitched.switches++;
                return;
            }
        }
        charactersSwitched.Add(new CharacterEventInfoClass(character));
    }

    IEnumerator ResetCharacterSwitchesLastFrame(CharacterNameType charName)
    {
        yield return null;
        charactersSwitchedLastFrame.Remove(charName);
    }

    public bool HasCharacterSwitchedThisFrame(CharacterNameType charID)
    {
        foreach (CharacterNameType characterSwitchedLastFrame in charactersSwitchedLastFrame)
        {
            if (characterSwitchedLastFrame == charID) return true;
        }
        return false;
    }
    public bool HasCharacterSwitched(CharacterNameType charID)
    {
        foreach (CharacterEventInfoClass characterSwitched in charactersSwitched)
        {
            if (characterSwitched.character.CharInfo.CharacterID == charID) return true;
        }
        return false;
    }
    public int CharacterSwitchCount(CharacterNameType charID)
    {
        foreach (CharacterEventInfoClass characterSwitched in charactersSwitched)
        {
            if (characterSwitched.character.CharInfo.CharacterID == charID)
            {
                return characterSwitched.switches;
            }
        }
        return 0;
    }
    #endregion

    #region Input Button Management
    public void UpdateButtonPressed(InputButtonType buttonPressed)
    {
        foreach(InputButtonType button in buttonsPressedLastFrame)
        {
            if (buttonPressed == button) return;
        }
        buttonsPressedLastFrame.Add(buttonPressed);
        StartCoroutine(RemoveButtonsPressedLastFrame(buttonPressed));
    }

    IEnumerator RemoveButtonsPressedLastFrame(InputButtonType buttonPressed)
    {
        yield return null;
        buttonsPressedLastFrame.Remove(buttonPressed);
    }

    public bool GetButtonWasPressedLastFrame(InputButtonType buttonPressed)
    {
        foreach (InputButtonType button in buttonsPressedLastFrame)
        {
            if (buttonPressed == button) return true;
        }
        return false;
    }
    #endregion

    #region Block Management
    public void AddBlock(BaseCharacter charID, BlockInfo.BlockType blockType)
    {
        BlockInfo thisBlock = new BlockInfo(charID, blockType);
        blocks.Add(thisBlock);
        blocksLastFrame.Add(thisBlock);
        StartCoroutine(ResetBlockFromLastFrame(thisBlock));
    }

    IEnumerator ResetBlockFromLastFrame(BlockInfo theBlock)
    {
        yield return null;
        blocksLastFrame.Remove(theBlock);
    }

    public bool GetBlockHappenedLastFrame(BlockInfo.BlockType typeToCheck, int minRequired = 1)
    {
        int number = 0;
        if (blocksLastFrame.Count != 0)
        {
            if(typeToCheck == BlockInfo.BlockType.either) number++;
            else
            {
                foreach(BlockInfo blockInfo in blocksLastFrame)
                {
                    if(blockInfo.blockType == typeToCheck)
                    {
                        number++;
                    }
                }
            }
        }
        if (number < minRequired) return false;
        else return true;
    }

    public bool GetBlocksHappened(BlockInfo.BlockType typeToCheck, int minRequired = 1)
    {
        int number = 0;
        if (blocks.Count != 0)
        {
            if (typeToCheck == BlockInfo.BlockType.either) number = blocks.Count;
            else
            {
                foreach (BlockInfo blockInfo in blocks)
                {
                    if (blockInfo.blockType == typeToCheck)
                    {
                        number++;
                    }
                }
            }
        }
        if (number < minRequired) return false;
        else return true;
    }

    public bool GetCharacterBlockedLastFrame(CharacterNameType charID, BlockInfo.BlockType typeToCheck, int minRequired = 1)
    {
        int number = 0;
        foreach(BlockInfo blockInfo in blocksLastFrame)
        {
            if(blockInfo.characterWhoBlocked.CharInfo.CharacterID == charID)
            {
                if(typeToCheck == BlockInfo.BlockType.either) number++;
                else if(blockInfo.blockType == typeToCheck) number++;
            }
        }
        if (number < minRequired) return false;
        else return true;
    }

    public bool GetCharacterBlocked(CharacterNameType charID, BlockInfo.BlockType typeToCheck, int minRequired = 1)
    {
        int number = 0;
        foreach (BlockInfo blockInfo in blocks)
        {
            if (blockInfo.characterWhoBlocked.CharInfo.CharacterID == charID)
            {
                if (typeToCheck == BlockInfo.BlockType.either) number++;
                else if (blockInfo.blockType == typeToCheck) number++;
            }
        }
        if (number < minRequired) return false;
        else return true;
    }
    #endregion

    #region Potion Management

    public void AddPotionCollected(ItemType potionType)
    {
        potionsCollectedLastFrame.Add(new PotionInfoClass(potionType, 1));
        StartCoroutine(ResetPotionCollectedLastFrame(potionType));

        foreach(PotionInfoClass bdb in potionsCollected)
        {
            if (bdb.type == potionType)
            {
                bdb.count++;
                return;
            }
        }
        potionsCollected.Add(new PotionInfoClass(potionType, 1));
    }

    IEnumerator ResetPotionCollectedLastFrame(ItemType potionNeedingReset)
    {
        yield return null;
        foreach(PotionInfoClass buffdebuff in potionsCollectedLastFrame.ToArray())
        {
            if (buffdebuff.type == potionNeedingReset) potionsCollectedLastFrame.Remove(buffdebuff);
        }
    }

    public bool GetPotionCollected(ItemType potionType, int minAmount)
    {
        if(potionType == ItemType.PowerUp_All)
        {
            if (minAmount <= potionsCollected.Count) return true;
        }
        foreach(PotionInfoClass potion in potionsCollected)
        {
            if(potion.type == potionType)
            {
                if (minAmount <= potion.count) return true;
            }
        }
        return false;
    }

    public bool GetPotionCollectedLastFrame(ItemType potionType, int minAmount)
    {
        if (potionType == ItemType.PowerUp_All)
        {
            if (minAmount <= potionsCollectedLastFrame.Count) return true;
        }
        foreach (PotionInfoClass potion in potionsCollectedLastFrame)
        {
            if (potion.type == potionType)
            {
                if (minAmount <= potion.count) return true;
            }
        }
        return false;
    }

    #endregion

    #endregion
}

[System.Serializable]
public class PotionInfoClass
{
    public ItemType type;
    public int count = 1;

    public PotionInfoClass(ItemType _type, int _count)
    {
        type = _type;
        count = _count;
    }
}

[System.Serializable]
public class CharacterEventInfoClass
{
    public BaseCharacter character;
    public string charName;
    public int deaths = 0;
    public int arrivals = 0;
    public int switches = 1;
    public float healthPercentage = 100f;
    public float staminaPercentage = 100f;

    public CharacterEventInfoClass(BaseCharacter _character, int _deaths)
    {
        character = _character;
        deaths = _deaths;
        SetupOtherStuff();
    }

    public CharacterEventInfoClass(int _arrivals, BaseCharacter _character)
    {
        character = _character;
        arrivals = _arrivals;
        SetupOtherStuff();
    }

    public CharacterEventInfoClass(BaseCharacter _character)
    {
        character = _character;
        SetupOtherStuff();
    }

    public void SetupOtherStuff()
    {
        charName = character.CharInfo.CharacterID.ToString();
        //Debug.Log("Created " + charName);
    }
}

[System.Serializable]
public class BlockInfo
{
    public enum BlockType
    {
        partial,
        full,
        either
    }
    public BlockType blockType;
    public BaseCharacter characterWhoBlocked;
    public float timeBlocked;

    public BlockInfo(BaseCharacter character, BlockType type)
    {
        blockType = type;
        characterWhoBlocked = character;
        timeBlocked = Time.time;
    }
}