using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public class WaveManagerScript : MonoBehaviour
{

    public delegate void WaveBossApper(MinionType_Script boss);
    public event WaveBossApper WaveBossApperEvent;

    public delegate void WaveComplete(string startBlockName);
    public event WaveComplete WaveCompleteEvent;

    public GameTime battleTime = GameTime.zero;

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

    public List<StartingCharactersForWaveClass> StartingCharInWave = new List<StartingCharactersForWaveClass>();



    BaseCharacter newChar;


    private void Awake()
    {
        Instance = this;
    }

    public void ToggleBattleTimer(bool timerState)
    {
        if (battleTime.standardTicker == null) battleTime.SetupBasics();
        if (timerState) StartCoroutine(battleTime.standardReverseTicker);
        else StopCoroutine(Instance.battleTime.standardReverseTicker);
    }

    public IEnumerator WaveCharCreator()
    {
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            foreach (WaveCharClass waveChar in wavePhase.ListOfEnemy)
            {
                
                if(waveChar.NPCType == WaveNPCTypes.Recruitable)
                {
                    CreateChar(waveChar.TypeOfCharacter.CharacterName, BaseCharType.MinionType_Script);
                }
                else if (waveChar.NPCType == WaveNPCTypes.Boss)
                {
                    CreateChar(waveChar.TypeOfCharacter.CharacterName);
                }
                else
                {
                    CreateChar(waveChar.TypeOfCharacter.CharacterName);
                    CreateChar(waveChar.TypeOfCharacter.CharacterName);
                }
                
            }
            yield return null;

        }
    }

    public int GetMaxEnemiesOnScreenAcrossAllWaves()
    {
        int mostEnemiesPossible = 0;
        foreach(WavePhaseClass wavePhase in WavePhases)
        {
            if (wavePhase.MaxEnemyOnScreen > mostEnemiesPossible) mostEnemiesPossible = wavePhase.MaxEnemyOnScreen;
        }
        return mostEnemiesPossible;
    }

    public IEnumerator SpawnCharFromGivenWave(string waveName, CharacterNameType characterID, string charIdentifier, bool isRandom, Vector2Int pos)
    {
        WavePhaseClass wpc = WavePhases.Where(r => r.name == waveName).First();
        CurrentWaveChar = wpc.ListOfEnemy.Where(a => a.TypeOfCharacter.CharacterName == characterID).First();
        BaseCharacter newChar = GetWaveCharacter(CurrentWaveChar.TypeOfCharacter);
        FungusSpawnedChars.Add(charIdentifier, newChar);
        yield return SpawChar(newChar, isRandom, pos, true);
    }

    public BaseCharacter GetWaveCharacter(WaveCharacterInfoClass character)
    {
        BaseCharacter res;
        res = WaveCharcters.Where(r => r.CharInfo.CharacterID == character.CharacterName && !r.IsOnField && ((r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script && !r.gameObject.activeInHierarchy)
        || (r.CharInfo.BaseCharacterType != BaseCharType.MinionType_Script))).FirstOrDefault();
        if (res == null)
        {
            res = CreateChar(character.CharacterName);
        }
        else
        {
            res.gameObject.SetActive(true);
        }
        res.gameObject.SetActive(true);
        res.CharInfo.HealthStats.Base = Random.Range(character.Health.x, character.Health.y);
        res.CharInfo.HealthStats.Regeneration = Random.Range(character.HealthRegeneration.x, character.HealthRegeneration.y);
        res.CharInfo.StaminaStats.Base = Random.Range(character.Stamina.x, character.Stamina.y);
        res.CharInfo.StaminaStats.Regeneration = Random.Range(character.StaminaRegeneration.x, character.StaminaRegeneration.y);
        res.CharInfo.SpeedStats.BaseSpeed = Random.Range(character.BaseSpeed.x, character.BaseSpeed.y);
        res.CharInfo.SpeedStats.AttackSpeedRatio = Random.Range(character.AttackSpeedRatio.x, character.AttackSpeedRatio.y);
        res.CharInfo.DamageStats.BaseDamage = Random.Range(character.BaseDamage.x, character.BaseDamage.y);
        res.CharInfo.RapidAttack.DamageMultiplier = character.RapidAttackMultiplier;
        res.CharInfo.PowerfulAttac.DamageMultiplier = character.PowerfulAttackMultiplier;
        res.CharInfo.Health = res.CharInfo.HealthStats.Base;
        res.CharInfo.SpeedStats.MovementSpeed = Random.Range(character.MovementSpeed.x, character.MovementSpeed.y);
        res.CharInfo.MovementTimer = character.MovementTimer;
        ((MinionType_Script)res).UpDownPerc = character.UpDownPerc;
        return res;
    }

    private BaseCharacter CreateChar(CharacterNameType characterID, BaseCharType bCharType = BaseCharType.None)
    {
        BaseCharacter res = BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(characterID.ToString(), CharacterSelectionType.Up,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, characterID, WalkingSideType.RightSide, AttackType.Tile, bCharType), transform);
        if(characterID != CharacterNameType.Stage00_BossOctopus &&
            characterID != CharacterNameType.Stage00_BossOctopus_Head &&
            characterID != CharacterNameType.Stage00_BossOctopus_Tentacles &&
            characterID != CharacterNameType.Stage00_BossOctopus_Girl) res.gameObject.SetActive(false);
        WaveCharcters.Add(res);
        return res;
    }

    public void RemoveWaveCharacterFromBoard(BaseCharacter character)
    {
        if(WaveCharcters.Where(r => r.CharInfo.Name == character.CharInfo.Name).FirstOrDefault() != null)
        {
            WaveCharcters.Where(r => r.CharInfo.Name == character.CharInfo.Name).FirstOrDefault().IsOnField = false;
            WaveCharcters.Where(r => r.CharInfo.Name == character.CharInfo.Name).FirstOrDefault().gameObject.SetActive(false);
        }
    }


    public IEnumerator SettingUpWave(string waveName)
    {
        WavePhaseClass wavePhase = WavePhases.Where(r => r.name == waveName).First();
        StartingCharInWave = new List<StartingCharactersForWaveClass>();
        foreach (WaveCharClass waveChar in wavePhase.ListOfEnemy)
        {
            if (waveChar.AreThereStartingEnemy)
            {
                waveChar.NumberOfCharacter -= waveChar.StartingEnemyTiles.Count;
                StartingCharInWave.Add(new StartingCharactersForWaveClass(waveChar.inRandomPos, waveChar.StartingEnemyTiles, waveChar.TypeOfCharacter));
            }
        }

        foreach (StartingCharactersForWaveClass startingCharacters in StartingCharInWave)
        {
            for (int i = 0; i < startingCharacters.Pos.Count; i++)
            {
                newChar = GetWaveCharacter(startingCharacters.TypeOfCharacter);
                yield return SpawChar(newChar, startingCharacters.InRandomPos, startingCharacters.Pos[i], false);
            }
        }
    }


    public IEnumerator StartWaveByName(string waveName)
    {
        yield return Wave(WavePhases.Where(r=> r.name == waveName).First());
    }

    private IEnumerator Wave(WavePhaseClass wavePhase)
    {
        float timer = 0;
        WaveCharacterInfoClass waveCharacterInfoClass;
        while (true)
        {
            timer += Time.fixedDeltaTime;
            yield return BattleManagerScript.Instance.PauseUntil();
            if (timer > wavePhase.DelayBetweenChars && 
                WaveCharcters.Where(r => r.gameObject.activeInHierarchy && r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script).ToList().Count < wavePhase.MaxEnemyOnScreen)
            {
        yield return new WaitForSeconds(0.5f);
                if (wavePhase.IsRandom)
                {
                    waveCharacterInfoClass = GetAvailableRandomWaveCharacter(wavePhase);
                }
                else
                {
                    waveCharacterInfoClass = GetAvailableWaveCharacter(wavePhase);
                }
                newChar = GetWaveCharacter(waveCharacterInfoClass);
                yield return SpawChar(newChar, CurrentWaveChar.IsRandomSpowiningTile, 
                    CurrentWaveChar.IsRandomSpowiningTile ? new Vector2Int() : CurrentWaveChar.SpowningTile[Random.Range(0, CurrentWaveChar.SpowningTile.Count)], true);
                timer = 0;

                if (wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList().Count == 0)
                {
                    while (true)
                    {
                        if(WaveCharcters.Where(r => r.gameObject.activeInHierarchy && (r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script || (r.IsOnField && r.CharInfo.BaseCharacterType != BaseCharType.MinionType_Script))).ToList().Count == 0)
                        {
                            yield break;
                        }
                        yield return null;
                    }
                }
            }
        }
    }

    private IEnumerator SpawChar(BaseCharacter newChar, bool isRandom, Vector2Int pos, bool withArrivingAnim)
    {
        if (isRandom)
        {
            yield return SetCharInPos(newChar, GridManagerScript.Instance.GetFreeBattleTile(newChar.UMS.WalkingSide, newChar.UMS.Pos), withArrivingAnim);
        }
        else
        {
            yield return SetCharInPos(newChar, GridManagerScript.Instance.GetBattleTile(pos), withArrivingAnim);
        }
        EventManager.Instance?.AddCharacterArrival(newChar);
    }

    public IEnumerator SetCharInPos(BaseCharacter currentCharacter, BattleTileScript bts, bool withArrivingAnim)
    {
        GridManagerScript.Instance.SetBattleTileState(bts.Pos, BattleTileStateType.Occupied);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        currentCharacter.CurrentBattleTiles = new List<BattleTileScript>();
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

        if (withArrivingAnim)
        {
            currentCharacter.SetUpEnteringOnBattle();
        }
        else
        {
            currentCharacter.CharArrivedOnBattleField();
        }
        if (currentCharacter.SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString())
        {

        }

        yield return BattleManagerScript.Instance.MoveCharToBoardWithDelay(withArrivingAnim  ? 0.2f : 0, currentCharacter, bts.transform.position);


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
    public float DelayBetweenChars;

    public List<WaveCharClass> ListOfEnemy = new List<WaveCharClass>();
}

[System.Serializable]
public class WaveCharacterInfoClass
{
    public CharacterNameType CharacterName;
    public Vector2 Health;
    public Vector2 HealthRegeneration;
    public CharacterLevelType CharacterClass;
    public Vector2 BaseDamage;
    public Vector2 RapidAttackMultiplier;
    public Vector2 PowerfulAttackMultiplier;
    public Vector2 BaseSpeed;
    public Vector2 AttackSpeedRatio;
    public Vector2 Stamina;
    public Vector2 StaminaRegeneration;
    public Vector2 MovementSpeed;
    public Vector2 MovementTimer;

    public float UpDownPerc = 18;


    public List<ScriptableObjectWaveEvent> Events = new List<ScriptableObjectWaveEvent>();

}


[System.Serializable]
public class WaveCharClass
{
    public string name;
    public WaveNPCTypes NPCType = WaveNPCTypes.Minion;
    public int NumberOfCharacter;
    public WaveCharacterInfoClass TypeOfCharacter;
    [Header("STARTING CHARACTERS")]
    public bool AreThereStartingEnemy = false;
    [ConditionalField("AreThereStartingEnemy", false)] public bool inRandomPos = true;
    public List<Vector2Int> StartingEnemyTiles = new List<Vector2Int>();

    [Header("WAVE CHARACTERS")]
    [Tooltip("Random spawnning for wave characters")]
    public bool IsRandomSpowiningTile = true;
    public List<Vector2Int> SpowningTile = new List<Vector2Int>();

}

[System.Serializable]
public class WaveEventClass
{
    public string name;
    public WaveCharacterInfoClass TypeOfCharacter;
}

public class StartingCharactersForWaveClass
{
    public List<Vector2Int> Pos = new List<Vector2Int>();
    public WaveCharacterInfoClass TypeOfCharacter;
    public bool InRandomPos;

    public StartingCharactersForWaveClass(bool inRandomPos, List<Vector2Int> pos, WaveCharacterInfoClass typeOfCharacter)
    {
        InRandomPos = inRandomPos;
        Pos = pos;
        TypeOfCharacter = typeOfCharacter;
    }
}


