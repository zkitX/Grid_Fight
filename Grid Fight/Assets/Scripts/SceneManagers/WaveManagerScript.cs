using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManagerScript : MonoBehaviour
{
    public static WaveManagerScript Instance;
    public List<WavePhaseClass> WavePhases = new List<WavePhaseClass>();
    public bool isWaveComplete = false;
    public int CurrentNumberOfWaveChars = 0;
    public List<CharacterBase> WaveCharcters = new List<CharacterBase>();

    private WaveCharClass CurrentWaveChar;
    private List<ScriptableObjectWaveEvent> Events = new List<ScriptableObjectWaveEvent>();

    private IEnumerator Wave_Co;
    private IEnumerator Event_Co;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Wave_Co = WaveCo();
        StartCoroutine(Wave_Co);
    }

    private IEnumerator EventCo()
    {
        while (true)
        {
            yield return null;
            foreach (ScriptableObjectWaveEvent item in Events.Where(r=> !r.isUsed).ToList())
            {
                switch (item.WaveEventType)
                {
                    case WaveEventCheckType.CharStatsCheckInPerc:
                        if(CharStatsCheckInPerc((ScriptableObjectWaveEvent_CharStatsCheckInPerc)item))
                        {
                            Debug.Log(item.FungusBlockName);
                            item.isUsed = true;
                        }
                        break;
                    case WaveEventCheckType.CharDied:
                        break;
                    case WaveEventCheckType.KillsNumber:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #region Events

    private bool CharStatsCheckInPerc(ScriptableObjectWaveEvent_CharStatsCheckInPerc so)
    {
        CharacterBase target = null;
        foreach (CharacterNameType item in so.CharactersID)
        {
            switch (so.StatToCheck)
            {
                case WaveStatsType.Health:
                    switch (so.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.HealthPerc < so.PercToCheck).FirstOrDefault();

                            break;
                        case ValueCheckerType.EqualTo:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.HealthPerc == so.PercToCheck).FirstOrDefault();

                            break;
                        case ValueCheckerType.MoreThan:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.HealthPerc > so.PercToCheck).FirstOrDefault();

                            break;
                    }
                    return target != null ? true : false;
                case WaveStatsType.Stamina:
                    switch (so.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.StaminaPerc < so.PercToCheck).FirstOrDefault();

                            break;
                        case ValueCheckerType.EqualTo:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.StaminaPerc == so.PercToCheck).FirstOrDefault();

                            break;
                        case ValueCheckerType.MoreThan:
                            target = WaveCharcters.Where(r => r.CharInfo.CharacterID == item && r.CharInfo.StaminaPerc > so.PercToCheck).FirstOrDefault();

                            break;
                    }
                    return target != null ? true : false;
            }
        }

        return true;
    }

    #endregion



    public CharacterBase GetWaveCharacter(CharacterNameType characterName, Transform parent)
    {
        CharacterBase res;
        res = WaveCharcters.Where(r => r.CharInfo.CharacterID == characterName && !r.IsOnField).FirstOrDefault();
        if (res == null)
        {

            res = BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(characterName.ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, characterName, WalkingSideType.RightSide), parent);

        }
        WaveCharcters.Add(res);
        return res;
    }


    private IEnumerator WaveCo()
    {
        float timer = 0;
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            while (BattleManagerScript.Instance == null)
            {
                yield return new WaitForEndOfFrame();
            }

            CharacterBase newChar = GetWaveCharacter(wavePhase.IsRandom ? GetAvailableRandomWaveCharacter(wavePhase) : GetAvailableWaveCharacter(wavePhase), transform);
            SetCharInRandomPos(newChar);
            isWaveComplete = false;

            if(Event_Co != null)
            {
                StopCoroutine(Event_Co);
            }
            Event_Co = EventCo();
            StartCoroutine(Event_Co);
            while (!isWaveComplete)
            {
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForEndOfFrame();
                }

                if(timer > CurrentWaveChar.DelayBetweenChars && CurrentNumberOfWaveChars < wavePhase.MaxEnemyOnScreen)
                {
                    newChar = GetWaveCharacter(wavePhase.IsRandom ? GetAvailableRandomWaveCharacter(wavePhase) : GetAvailableWaveCharacter(wavePhase), transform);
                    SetCharInRandomPos(newChar);
                    timer = 0;
                }
            }
        }
    }

    public void SetCharInRandomPos(CharacterBase currentCharacter)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(currentCharacter.UMS.WalkingSide, currentCharacter.UMS.Pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
        StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.2f, currentCharacter, bts.transform.position));
        CurrentNumberOfWaveChars++;
    }


    private CharacterNameType GetAvailableRandomWaveCharacter(WavePhaseClass wavePhase)
    {
        List<WaveCharClass> ListOfEnemy = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList();
        CurrentWaveChar = ListOfEnemy[Random.Range(0, ListOfEnemy.Count)];
        CurrentWaveChar.NumberOfCharacter--;
        Events.AddRange(CurrentWaveChar.TypeOfCharacter.Events);
        Events = Events.Distinct().ToList();
        return CurrentWaveChar.TypeOfCharacter.CharacterName;
    }

    private CharacterNameType GetAvailableWaveCharacter(WavePhaseClass wavePhase)
    {
        CurrentWaveChar = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).First();
        CurrentWaveChar.NumberOfCharacter--;
        Events.AddRange(CurrentWaveChar.TypeOfCharacter.Events);
        Events = Events.Distinct().ToList();
        return CurrentWaveChar.TypeOfCharacter.CharacterName;
    }
}

[System.Serializable]
public class WavePhaseClass
{
    public string name;
    public bool IsRandom = false;

    public int MaxEnemyOnScreen;
    public List<WaveCharClass> ListOfEnemy = new List<WaveCharClass>();
}

[System.Serializable]
public class WaveCharacterInfoClass
{
    public CharacterNameType CharacterName;
    public CharacterLevelType CharacterClass;
    public Vector2 Health;
    public Vector2 HealthRegeneration;
    public Vector2 Damage;
    public Vector2 BaseSpeed;
    public Vector2 AttackSpeedRatio;
    public Vector2 Stamina;
    public Vector2 StaminaRegeneration;

    public List<ScriptableObjectWaveEvent> Events = new List<ScriptableObjectWaveEvent>();

}


[System.Serializable]
public class WaveCharClass
{
    public string name;
    public int NumberOfCharacter;
    public WaveCharacterInfoClass TypeOfCharacter;
    public float DelayBetweenChars;

}

[System.Serializable]
public class WaveEventClass
{
    public string name;
    public WaveCharacterInfoClass TypeOfCharacter;
}

