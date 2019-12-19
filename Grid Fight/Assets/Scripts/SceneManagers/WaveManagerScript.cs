using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManagerScript : MonoBehaviour
{
    public static WaveManagerScript Instance;
    public List<WavePhaseClass> WavePhases = new List<WavePhaseClass>();
    public bool isWaveComplete = false;
    public List<BaseCharacter> WaveCharcters = new List<BaseCharacter>();

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
        BaseCharacter target = null;
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



    public BaseCharacter GetWaveCharacter(WaveCharacterInfoClass character, Transform parent)
    {
        BaseCharacter res;
        res = WaveCharcters.Where(r => r.CharInfo.CharacterID == character.CharacterName && !r.IsOnField && !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {

            res = BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(character.CharacterName.ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, character.CharacterName, WalkingSideType.RightSide), parent);
            BattleManagerScript.Instance.AllCharactersOnField.Add(res);

        }
        res.CharInfo.HealthStats.Health = Random.Range(character.Health.x, character.Health.y);
        res.CharInfo.HealthStats.Base = res.CharInfo.HealthStats.Health;
       
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

            BaseCharacter newChar = GetWaveCharacter(wavePhase.IsRandom ? GetAvailableRandomWaveCharacter(wavePhase) : GetAvailableWaveCharacter(wavePhase), transform);
            SpawChar(newChar);
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

                if(timer > CurrentWaveChar.DelayBetweenChars && WaveCharcters.Where(r=> r.gameObject.activeInHierarchy).ToList().Count < wavePhase.MaxEnemyOnScreen)
                {
                    if (wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList().Count == 0)
                    {
                        isWaveComplete = true;
                        break;
                    }
                    yield return new WaitForSecondsRealtime(0.1f);
                    newChar = GetWaveCharacter(wavePhase.IsRandom ? GetAvailableRandomWaveCharacter(wavePhase) : GetAvailableWaveCharacter(wavePhase), transform);
                    SpawChar(newChar);
                    timer = 0;
                   
                }
            }

            while (isWaveComplete)
            {
                yield return null;
                if(WaveCharcters.Where(r=> r.IsOnField).ToList().Count == 0)
                {
                    isWaveComplete = false;
                }
            }
        }
    }

    private void SpawChar(BaseCharacter newChar)
    {
        if (CurrentWaveChar.IsFixedSpowiningTile)
        {
            SetCharInPos(newChar, GridManagerScript.Instance.GetBattleTile(CurrentWaveChar.SpowningTile[Random.Range(0, CurrentWaveChar.SpowningTile.Count)]));
        }
        else
        {
            SetCharInPos(newChar, GridManagerScript.Instance.GetFreeBattleTile(newChar.UMS.WalkingSide, newChar.UMS.Pos));
        }
    }

    public void SetCharInPos(BaseCharacter currentCharacter, BattleTileScript bts)
    {
        currentCharacter.gameObject.SetActive(true);
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
        currentCharacter.SetUpEnteringOnBattle();
        StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.2f, currentCharacter, bts.transform.position));
    }


    private WaveCharacterInfoClass GetAvailableRandomWaveCharacter(WavePhaseClass wavePhase)
    {
        List<WaveCharClass> ListOfEnemy = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList();
        CurrentWaveChar = ListOfEnemy[Random.Range(0, ListOfEnemy.Count)];
        CurrentWaveChar.NumberOfCharacter--;
        Events.AddRange(CurrentWaveChar.TypeOfCharacter.Events);
        Events = Events.Distinct().ToList();
        return CurrentWaveChar.TypeOfCharacter;
    }

    private WaveCharacterInfoClass GetAvailableWaveCharacter(WavePhaseClass wavePhase)
    {
        CurrentWaveChar = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).First();
        CurrentWaveChar.NumberOfCharacter--;
        Events.AddRange(CurrentWaveChar.TypeOfCharacter.Events);
        Events = Events.Distinct().ToList();
        return CurrentWaveChar.TypeOfCharacter;
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
    public bool IsFixedSpowiningTile = false;
    public List<Vector2Int> SpowningTile = new List<Vector2Int>();
    public float DelayBetweenChars;

}

[System.Serializable]
public class WaveEventClass
{
    public string name;
    public WaveCharacterInfoClass TypeOfCharacter;
}

