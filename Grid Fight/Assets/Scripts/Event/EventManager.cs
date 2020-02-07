using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [Header("Stage Config")]
    [SerializeField] protected StageEventTriggersProfile stageEventTriggersProfile;
    public List<GameSequenceEvent> stageEventTriggers { get; private set; } = new List<GameSequenceEvent>();
    public delegate bool Check();
    protected List<TimedCheck> currentTimedChecks = new List<TimedCheck>();
    IEnumerator timedCheckTicker = null;
    List<GameSequenceEvent> queuedCompleteEvents = new List<GameSequenceEvent>();
    IEnumerator completeEventSequencer = null;

    [Header("Current Info")]
    [SerializeField] protected List<CharacterEventInfoClass> deadCharacters = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> diedThisFrame = new List<CharacterNameType>();
    [SerializeField] protected List<CharacterEventInfoClass> charactersWhomstHaveArrived = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> arrivedThisFrame = new List<CharacterNameType>();
    [SerializeField] protected List<CharacterEventInfoClass> characterVitalities = new List<CharacterEventInfoClass>();
    protected List<CharacterEventInfoClass> healthChangedLastFrame = new List<CharacterEventInfoClass>();
    [SerializeField] protected List<string> eventsCalled = new List<string>();
    protected List<string> eventDirectCallsLastFrame = new List<string>();
    [SerializeField] protected List<CharacterEventInfoClass> charactersSwitched = new List<CharacterEventInfoClass>();
    protected List<CharacterNameType> charactersSwitchedLastFrame = new List<CharacterNameType>();
    protected List<InputButtonType> buttonsPressedLastFrame = new List<InputButtonType>();

    //[Tooltip("How many seconds between checks, increase for performance boost, decrease for accuracy")][SerializeField] protected float timeBetweenChecks = 1f;

    #region Initialize
    private void Awake()
    {
        Instance = this;
        ResetEventsInManager();
    }

    public void ResetEventsInManager()
    {
        if (!BattleManagerScript.Instance.usingFungus) return;
        stageEventTriggers.Clear();
        stageEventTriggers = new List<GameSequenceEvent>();
        if (stageEventTriggersProfile != null)
        {
            foreach (GameSequenceEvent gameSeqEvent in stageEventTriggersProfile.stageEventTriggers)
            {
                stageEventTriggers.Add(Instantiate(gameSeqEvent));
            }
        }
        InitialiseEvents();
    }

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
    #endregion

    #region Queuing Completed Events
    public void AddCompletedGameEvent(GameSequenceEvent gameEvent)
    {
        queuedCompleteEvents.Add(gameEvent);
        if(completeEventSequencer == null)
        {
            completeEventSequencer = CompleteEventSequence();
            StartCoroutine(completeEventSequencer);
        }
    }

    IEnumerator CompleteEventSequence()
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

    #region Call Management
    public void CallEventDirectly(string eventName)
    {
        ResetEventsCalledLastFrame(eventName);
        if (!eventsCalled.Contains(eventName)) eventsCalled.Add(eventName); 
        foreach (string eventCalledLastFrame in eventDirectCallsLastFrame)
        {
            if (eventCalledLastFrame == eventName) return;
        }
        eventDirectCallsLastFrame.Add(eventName);
    }

    IEnumerator ResetEventsCalledLastFrame(string eventName)
    {
        yield return null;
        eventDirectCallsLastFrame.Remove(eventName);
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

    #endregion


}

[System.Serializable]
public class CharacterEventInfoClass
{
    public BaseCharacter character;
    public int deaths = 0;
    public int arrivals = 0;
    public int switches = 0;
    public float healthPercentage = 100f;

    public CharacterEventInfoClass(BaseCharacter _character, int _deaths)
    {
        character = _character;
        deaths = _deaths;
    }

    public CharacterEventInfoClass(int _arrivals, BaseCharacter _character)
    {
        character = _character;
        arrivals = _arrivals;
    }

    public CharacterEventInfoClass(BaseCharacter _character)
    {
        character = _character;
    }
}