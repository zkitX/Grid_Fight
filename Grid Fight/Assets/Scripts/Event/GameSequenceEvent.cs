using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "GSEQ_EventName", menuName = "ScriptableObjects/Events/Game Sequence Event")]
public class GameSequenceEvent : EventTrigger
{
    //requirements as well as their sub divisions (timed/Non-timed)
    public enum RequirementType { and, or }
    [Header("Requirements")]
    [SerializeField] protected RequirementType requirementRule = RequirementType.and;
    [SerializeField] protected RequirementType timedCheckRule = RequirementType.and;
    [Tooltip("A list of game sequence and timed events that enable the event to be completed")] [SerializeField] public EventTrigger[] requirements;

    [Space(10)]
    [Header("Inhibitors")]
    [SerializeField] protected RequirementType inhibitorRule = RequirementType.or;
    [Tooltip("A list of game sequence events that stop the event from being completed")][SerializeField] public List<GameSequenceEvent> inhibitors;

    protected List<TimedCheck> timedChecks = new List<TimedCheck>();
    [HideInInspector] public List<GameSequenceEvent> reqChecks { get; private set; } = new List<GameSequenceEvent>();
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
        if (hasHappened && !ceaseOnComplete) return;
        Debug.Log("<i>Event Triggered:</i> " + Name);

        EventManager.Instance.AddTriggeredEvent(Name);
        triggerRequests++;
        //If the event requirements aren't met, stop the method
        if (inhibitors.Count > 0) if (RequirementsFulfilled(inhibitors, inhibitorRule)) return;
        if (reqChecks.Count > 0) if (!RequirementsFulfilled(reqChecks, requirementRule)) return;
        //If they are met, move to start the timed 
        StartAllTimedRequirements();
    }

    public void Initialise()
    {
        for(int i = 0; i < requirements.Length; i++)
        {
            //Create a copy
            requirements[i] = Instantiate(requirements[i]);
            //Add the requirements to their different groups
            if (requirements[i].GetType() == typeof(GameSequenceEvent)) reqChecks.Add((GameSequenceEvent)requirements[i]);
            else if (requirements[i].GetType() == typeof(TimedCheck)) timedChecks.Add((TimedCheck)requirements[i]);
        }
        if (reqChecks.Count == 0) Trigger();
    }

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
        foreach (TimedCheck timedCheck in timedChecks) timedCheck.CeaseChecking();
        hasHappened = true;
        EventManager.Instance.AddCompletedGameEvent(this);
    }

    public IEnumerator CompleteEventSequence()
    {
        //Sequence through all the effects here
        if (!requireComplete)
        {
            EventManager.Instance.AddTriggeredEvent(Name);
            TriggerTriggees();
        }

        //NEEDS WORK
        foreach(EventEffect effect in effects)
        {
            yield return effect.PlayEffect();
        }

        Debug.Log("<i>Event Completed:</i> " + Name);



        if (requireComplete)
        {
            EventManager.Instance.AddTriggeredEvent(Name);
            TriggerTriggees();
        }

        yield return null;
    }



}
