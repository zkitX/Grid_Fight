using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "GSEQ_EventName", menuName = "ScriptableObjects/Events/Game Sequence Event")]
public class GameSequenceEvent : ScriptableObject
{
    public string Name;
    [HideInInspector] public bool hasHappened = false;

    //requirements as well as their sub divisions (timed/Non-timed)
    public enum RequirementType { and, or }
    [Header("Requirements")]
    [SerializeField] protected RequirementType requirementRule = RequirementType.and;
    [SerializeField] public List<GameSequenceEvent> reqChecks = new List<GameSequenceEvent>();
    [Space(10)]
    [SerializeField] protected RequirementType timedCheckRule = RequirementType.and;
    [SerializeField] public List<TimedCheck> timedChecks = new List<TimedCheck>();

    [Space(10)]
    [Header("Inhibitors")]
    [SerializeField] protected RequirementType inhibitorRule = RequirementType.or;
    [Tooltip("A list of game sequence events that stop the event from being completed")][SerializeField] public List<GameSequenceEvent> inhibitors;

    //

    //Interaction information
    protected int triggerRequests = 0;
    //

    [Space(10)]
    [Header("Completion and stuff")]
    //Effects and triggees
    public bool ignoreQueue = false;
    [SerializeField] public bool ceaseOnComplete = true;
    [SerializeField] protected bool requireComplete = true;
    protected List<GameSequenceEvent> triggees = new List<GameSequenceEvent>();
    [SerializeField] protected EventEffect[] effects;
    //

    public void Trigger()
    {
        if (hasHappened && ceaseOnComplete) return;
        Debug.Log("<i>Event Triggered:</i> " + Name);

        EventManager.Instance.AddTriggeredEvent(Name);
        triggerRequests++;
        //If the event requirements aren't met, stop the method
        if (inhibitors.Count > 0) if (RequirementsFulfilled(inhibitors, inhibitorRule)) return;
        if (reqChecks.Count > 0) if (!RequirementsFulfilled(reqChecks, requirementRule)) return;
        //If they are met, move to start the timed 
        StartAllTimedRequirements();
    }

    //Setup the event
    public void Initialise()
    {
        //Instantiate all the requirement scriptableObjects
        for(int i = 0; i < reqChecks.Count; i++)
        {
            reqChecks[i] = Instantiate(reqChecks[i]);
            //if an error is here, there is a missing requirement (as in the list is larger than the number of requirements in it)
        }

        //Trigger the event if it has no requirements
        if (reqChecks.Count == 0) Trigger();
    }

    //A check to see if all the requirements of any type (inhibitor, standard or otherwise) have been fulfilled based on their rule
    bool RequirementsFulfilled(List<GameSequenceEvent> reqList, RequirementType reqRule)
    {
        //Check if the non-timed event trigger requirements have been fulfilled
        if(reqRule == RequirementType.and)
        {
            foreach (GameSequenceEvent req in reqList)
            {
                if (!EventManager.Instance.HasHappened(req)) return false;
            }
            return true;
        }
        else
        {
            foreach (GameSequenceEvent req in reqList)
            {
                if (EventManager.Instance.HasHappened(req)) return true;
            }
            return false;
        }
    }

    void StartAllTimedRequirements()
    {
        if(timedChecks.Count == 0)
        {
            CompleteEvent();
            return;
        }

        foreach(TimedCheck timedCheck in timedChecks)
        {
            timedCheck.StartChecking(this);
        }
    }

    void PopulateTriggees()
    {
        //GO through all the game events in this stage, see if this event is one of their requirements and if so, add to the list of triggees
        triggees.Clear();
        foreach(GameSequenceEvent gameEvent in EventManager.Instance.stageEventTriggers)
        {
            foreach(GameSequenceEvent reqCheck in gameEvent.reqChecks)
            {
                if(reqCheck.Name == Name)
                {
                    triggees.Add(gameEvent);
                    break;
                }
            }
        }
    }

    public void CheckTimedRequirements()
    {
        //Check if all the timed check requirements are met
        if (timedCheckRule == RequirementType.and)
        {
            foreach (TimedCheck timedCheck in timedChecks)
            {
                if (timedCheck.ceaseOnHappened && !timedCheck.hasHappened) return;
                else if (!timedCheck.isHappening) return;
            }
        }
        else
        {
            bool fulfilled = false;
            foreach (TimedCheck timedCheck in timedChecks)
            {
                if (timedCheck.ceaseOnHappened && timedCheck.hasHappened)
                {
                    fulfilled = true;
                    break;
                }
                else if (timedCheck.isHappening)
                {
                    fulfilled = true;
                    break;
                }
            }
            if (fulfilled == false) return;
        }
        //IF YOU GET TO THIS POINT, ALL REQUIREMENTS HAVE BEEN MET AND YOU CAN GO FORWARD WITH THE EFFECTS OF THE EVENT
        CompleteEvent();
    }

    void TriggerTriggees()
    {
        //Add events-to-be-triggered to the triggees list
        PopulateTriggees();
        //Trigger them all
        foreach (GameSequenceEvent triggee in triggees)
        {
            triggee.Trigger();
        }
    }

    void CompleteEvent()
    {
        //Remove the timedChecks from the checkTicker in case it hasnt been already
        if (inhibitors.Count > 0) if (RequirementsFulfilled(inhibitors, inhibitorRule)) return;
        foreach (TimedCheck timedCheck in timedChecks) timedCheck.CeaseChecking();
        hasHappened = true;
        EventManager.Instance.AddCompletedGameEvent(this);
    }

    public IEnumerator CompleteEventSequence()
    {
        //Sequence through all the effects here
        if (!requireComplete) EventCompletion();


        foreach (EventEffect effect in effects)
        {
            yield return effect.PlayEffect();
        }


        if (requireComplete) EventCompletion();

        Debug.Log("<i>Event Completed:</i> " + Name);
        yield return null;
    }

    //Handle all the event resets and such, do not fuck with the effects in this method
    void EventCompletion()
    {
        EventManager.Instance.AddTriggeredEvent(Name);
        TriggerTriggees();
        if (!ceaseOnComplete) Trigger();
    }

}
