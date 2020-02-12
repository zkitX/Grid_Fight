using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManagerScript : MonoBehaviour
{

    public delegate void WaveBossApper(MinionType_Script boss);
    public event WaveBossApper WaveBossApperEvent;

    public delegate void WaveComplete(string startBlockName);
    public event WaveComplete WaveCompleteEvent;




    public static WaveManagerScript Instance;
    public List<WavePhaseClass> WavePhases = new List<WavePhaseClass>();
    public bool isWaveComplete = false;
    public List<BaseCharacter> WaveCharcters = new List<BaseCharacter>();

    private WaveCharClass CurrentWaveChar;
    private List<ScriptableObjectWaveEvent> Events = new List<ScriptableObjectWaveEvent>();
    public int CurrentWave = 0;
    public bool StartWave = false;
    private IEnumerator Wave_Co;
    public Dictionary<string, BaseCharacter> FungusSpawnedChars = new Dictionary<string, BaseCharacter>();



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


    public IEnumerator WaveCharCreator()
    {
        foreach (WavePhaseClass wavePhase in WavePhases)
        {

            foreach (WaveCharClass waveChar in wavePhase.ListOfEnemy)
            {
                CreateChar(waveChar.TypeOfCharacter.CharacterName);
                CreateChar(waveChar.TypeOfCharacter.CharacterName);
            }
        }
        yield return null;
    }


    public void SpawnCharFromGivenWave(string waveName, CharacterNameType characterID, string charIdentifier, bool isRandom, Vector2Int pos)
    {
        WavePhaseClass wpc = WavePhases.Where(r => r.name == waveName).First();
        CurrentWaveChar = wpc.ListOfEnemy.Where(a => a.TypeOfCharacter.CharacterName == characterID).First();
        BaseCharacter newChar = GetWaveCharacter(CurrentWaveChar.TypeOfCharacter);
        FungusSpawnedChars.Add(charIdentifier, newChar);
        SpawChar(newChar, isRandom, pos);
    }



    public BaseCharacter GetWaveCharacter(WaveCharacterInfoClass character)
    {
        BaseCharacter res;
        res = WaveCharcters.Where(r => r.CharInfo.CharacterID == character.CharacterName && !r.IsOnField && !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {
            res = CreateChar(character.CharacterName);
        }
        else
        {
            res.gameObject.SetActive(true);
            res.StartAttakCo();
        }
        res.gameObject.SetActive(true);
        res.CharInfo.HealthStats.Base = Random.Range(character.Health.x, character.Health.y);
        res.CharInfo.HealthStats.Regeneration = Random.Range(character.HealthRegeneration.x, character.HealthRegeneration.y);
        res.CharInfo.StaminaStats.Base = Random.Range(character.Stamina.x, character.Stamina.y);
        res.CharInfo.StaminaStats.Regeneration = Random.Range(character.StaminaRegeneration.x, character.StaminaRegeneration.y);
        res.CharInfo.SpeedStats.BaseSpeed = Random.Range(character.BaseSpeed.x, character.BaseSpeed.y);
        res.CharInfo.SpeedStats.AttackSpeedRatio = Random.Range(character.AttackSpeedRatio.x, character.AttackSpeedRatio.y);
        res.CharInfo.RapidAttack.BaseDamage = Random.Range(character.Damage.x, character.Damage.y);
        res.CharInfo.DamageStats.CurrentDamage = res.CharInfo.RapidAttack.BaseDamage;
        res.CharInfo.Health = res.CharInfo.HealthStats.Base;

        return res;
    }


    private BaseCharacter CreateChar(CharacterNameType characterID)
    {
        BaseCharacter res = BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(characterID.ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, characterID, WalkingSideType.RightSide, AttackType.Tile), transform);
        res.gameObject.SetActive(false);
        WaveCharcters.Add(res);
        return res;
    }


    private IEnumerator WaveCo()
    {
        while (BattleManagerScript.Instance == null || GridManagerScript.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (WavePhaseClass wavePhase in WavePhases)
        {

            while (!StartWave)
            {
                yield return null;
            }
           // WaveCompleteEvent(wavePhase.name);
            StartWave = false;
            yield return Wave(wavePhase);
        }
    }


    private IEnumerator Wave(WavePhaseClass wavePhase)
    {
        float timer = 0;
        BaseCharacter newChar;
        WaveCharacterInfoClass waveCharacterInfoClass;

        if (wavePhase.IsRandom)
        {
            waveCharacterInfoClass = GetAvailableRandomWaveCharacter(wavePhase);
        }
        else
        {
            waveCharacterInfoClass = GetAvailableWaveCharacter(wavePhase);
        }
        while (true)
        {
            timer += Time.fixedDeltaTime;
            yield return BattleManagerScript.Instance.PauseUntil();
            if (timer > CurrentWaveChar.DelayBetweenChars && WaveCharcters.Where(r => r.gameObject.activeInHierarchy).ToList().Count < wavePhase.MaxEnemyOnScreen)
            {
               
                newChar = GetWaveCharacter(waveCharacterInfoClass);
                SpawChar(newChar, CurrentWaveChar.IsFixedSpowiningTile, CurrentWaveChar.SpowningTile.Count > 0 ? CurrentWaveChar.SpowningTile[Random.Range(0, CurrentWaveChar.SpowningTile.Count)] : new Vector2Int());
                timer = 0;

                if (wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList().Count == 0)
                {
                    while (true)
                    {
                        if(WaveCharcters.Where(r => r.gameObject.activeInHierarchy).ToList().Count == 0)
                        {

                            //BattleManagerScript.Instance.RecruitCharFromWave(WaveCharcters[0].CharInfo.CharacterID);
                            yield break;

                        }
                        yield return null;
                    }
                }

                if (wavePhase.IsRandom)
                {
                    waveCharacterInfoClass = GetAvailableRandomWaveCharacter(wavePhase);
                }
                else
                {
                    waveCharacterInfoClass = GetAvailableWaveCharacter(wavePhase);
                }
            }
        }
    }

    private void SpawChar(BaseCharacter newChar, bool isRandom, Vector2Int pos)
    {
        if (!isRandom)
        {
            SetCharInPos(newChar, GridManagerScript.Instance.GetBattleTile(pos));
        }
        else
        {
            SetCharInPos(newChar, GridManagerScript.Instance.GetFreeBattleTile(newChar.UMS.WalkingSide, newChar.UMS.Pos));
        }
    }

    public void SetCharInPos(BaseCharacter currentCharacter, BattleTileScript bts)
    {
        GridManagerScript.Instance.SetBattleTileState(bts.Pos, BattleTileStateType.Occupied);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            GridManagerScript.Instance.SetBattleTileState(currentCharacter.UMS.Pos[i], BattleTileStateType.Occupied);
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }


        currentCharacter.SetUpEnteringOnBattle();
        if(currentCharacter.SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving)
        {

        }

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

    #region WaveEvents

    public void BossArrived(MinionType_Script boss)
    {
        WaveBossApperEvent?.Invoke(boss);
    }

    #endregion

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

