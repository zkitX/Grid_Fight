using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameTime
{
    [SerializeField] public int hours;
    [SerializeField] public int minutes;
    [SerializeField] public float seconds;
    [HideInInspector] public GameTime startingTime;
    [HideInInspector] public static GameTime zero = new GameTime(0, 0, 0f);
    [HideInInspector] public IEnumerator standardTicker = null;
    [HideInInspector] public IEnumerator standardReverseTicker = null;
    [HideInInspector] public bool counting = false;

    public GameTime()
    {
        hours = 0;
        minutes = 0;
        seconds = 0;
        SetupBasics();
    }

    public GameTime(int _hours, int _minutes, float _seconds)
    {
        hours = Mathf.Clamp(_hours, 0, 9999999);
        minutes = Mathf.Clamp(_minutes, 0, 59);
        seconds = Mathf.Clamp(_seconds, 0f, 59f);
        SetupBasics();
    }

    void SetupBasics()
    {
        if (standardTicker == null) standardTicker = TimeCounter(1f);
        if (standardReverseTicker == null) standardReverseTicker = TimeCounter(-1f);
    }

    public float timeInSeconds
    {
        get
        {
            return (hours * 3600f) + (minutes * 60f) + seconds;
        }
        set
        {
            float inputTime = value;
            hours = Mathf.FloorToInt(inputTime / 3600f);
            minutes = Mathf.FloorToInt((inputTime - (hours * 3600)) / 60f);
            seconds = inputTime - (hours * 3600f) - (minutes * 60);
        }
    }

    public float timeInMinutes
    {
        get
        {
            return (hours * 60f) + (minutes) + (seconds/60f);
        }
        set
        {
            float inputTime = value;
            hours = Mathf.FloorToInt(inputTime / 60f);
            minutes = Mathf.FloorToInt(inputTime - (hours * 60));
            seconds = (inputTime - (hours * 60f) - minutes) * 60f;
        }
    }

    IEnumerator TimeCounter(float rate)
    {
        //Never unsets 
        counting = true;
        startingTime = new GameTime(hours, minutes, seconds);

        while (true)
        {
            if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle) break;
            timeInSeconds = Mathf.Clamp(timeInSeconds + (Time.deltaTime * rate), 0f, 99999999999999999999999999999f);
            yield return null;
        }
    }
    
    public static GameTime TimeDifference(GameTime time1, GameTime time2)
    {
        GameTime timeToReturn = new GameTime();
        timeToReturn.timeInSeconds = TimeDifferenceInSeconds(time1, time2);
        return timeToReturn;
    }

    public static float TimeDifferenceInSeconds(GameTime time1, GameTime time2)
    {
        return Mathf.Abs(time1.timeInSeconds - time2.timeInSeconds);
    }

    public static float TimeDifferenceInMinutes(GameTime time1, GameTime time2)
    {
        return Mathf.Abs(time1.timeInMinutes - time2.timeInMinutes);
    }
}
