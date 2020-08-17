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
    public delegate void BattleTimerComplete(string blockToTrigger);
    public event BattleTimerComplete OnBattleTimerComplete;

    public static WaveManagerScript Instance;
    public List<WavePhaseClass> WavePhases = new List<WavePhaseClass>();
    public bool isWaveComplete = false;
    public List<BaseCharacter> WaveCharcters = new List<BaseCharacter>();

    private WaveCharClass CurrentWaveChar;
    public bool StartWave = false;
    public Dictionary<string, BaseCharacter> FungusSpawnedChars = new Dictionary<string, BaseCharacter>();

    public List<StartingCharactersForWaveClass> StartingCharInWave = new List<StartingCharactersForWaveClass>();
    WavePhaseClass currentWavePhase;
    bool isWaveOn = false;

    BaseCharacter newChar;
    bool leadCharDie = false;

    private void Awake()
    {
        Instance = this;
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            if (wavePhase.AbsoluteMaxEnemyOnScreen < wavePhase.MaxEnemyOnScreen) wavePhase.AbsoluteMaxEnemyOnScreen = Mathf.RoundToInt((float)wavePhase.MaxEnemyOnScreen * UniversalGameBalancer.Instance.difficulty.enemySpawnScaler);
        }
    }

    public void ToggleBattleTimer(bool timerState, bool countDown = true)
    {
        if (battleTime.standardReverseTicker != null) StopCoroutine(battleTime.standardReverseTicker);
        if (battleTime.standardTicker != null) StopCoroutine(battleTime.standardTicker);

        if (battleTime.standardTicker == null) battleTime.SetupBasics();

        if (countDown)
        {
            if (timerState) StartCoroutine(battleTime.standardReverseTicker);
            else StopCoroutine(Instance.battleTime.standardReverseTicker);
        }
        else
        {
            if (timerState) StartCoroutine(battleTime.standardTicker);
            else StopCoroutine(Instance.battleTime.standardTicker);
        }
    }

    public void TriggerBattleTimerEnded(string block)
    {
        OnBattleTimerComplete?.Invoke(block);
    }

    public void SetBattleTimer(bool countDown, bool changeTime, int hours, int minutes, float seconds, bool start = true, string blockToTriggerOnComplete = "")
    {
        battleTime = changeTime ? new GameTime(hours, minutes, seconds): battleTime;
        battleTime.blockToTriggerOnComplete = blockToTriggerOnComplete;
        ToggleBattleTimer(start, countDown);
    }

    public IEnumerator WaveCharCreator()
    {
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            foreach (WaveCharClass waveChar in wavePhase.ListOfEnemy)
            {

                if (waveChar.NPCType == WaveNPCTypes.Recruitable)
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
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            if (Mathf.RoundToInt((float)wavePhase.MaxEnemyOnScreen * UniversalGameBalancer.Instance.difficulty.enemySpawnScaler) > mostEnemiesPossible) mostEnemiesPossible = Mathf.RoundToInt((float)wavePhase.MaxEnemyOnScreen * UniversalGameBalancer.Instance.difficulty.enemySpawnScaler);
        }
        return mostEnemiesPossible;
    }

    public IEnumerator SpawnCharFromGivenWave(string waveName, CharacterNameType characterID, string charIdentifier, bool isRandom, Vector2Int pos, bool removeFromWave)
    {
        WavePhaseClass wpc = WavePhases.Where(r => r.name == waveName).First();
        CurrentWaveChar = wpc.ListOfEnemy.Where(a => a.TypeOfCharacter.CharacterName == characterID).First();
        BaseCharacter newChar = GetWaveCharacter(CurrentWaveChar.TypeOfCharacter);
        if (removeFromWave)
        {
            CurrentWaveChar.NumberOfCharacter--;
        }
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
        if(character.KillWaveIfCharDie)
        {
            res.CurrentCharIsDeadEvent += Res_CurrentCharIsDeadEvent;
        }
        
        res.CharInfo.HealthStats.Base = Random.Range(character.Health.x, character.Health.y) * UniversalGameBalancer.Instance.difficulty.enemyHealthScaler;
        res.CharInfo.HealthStats.Regeneration = Random.Range(character.HealthRegeneration.x, character.HealthRegeneration.y);
        res.CharInfo.EtherStats.Base = Random.Range(character.Ether.x, character.Ether.y);
        res.CharInfo.EtherStats.Regeneration = Random.Range(character.EtherRegeneration.x, character.EtherRegeneration.y);
        res.CharInfo.SpeedStats.BaseSpeed = Random.Range(character.BaseSpeed.x, character.BaseSpeed.y);
        res.CharInfo.DamageStats.BaseDamage = Random.Range(character.BaseDamage.x, character.BaseDamage.y);
        res.CharInfo.WeakAttack.DamageMultiplier = character.WeakAttackMultiplier;
        res.CharInfo.StrongAttack.DamageMultiplier = character.StrongAttackMultiplier;
        res.CharInfo.Health = res.CharInfo.HealthStats.Base;
        res.CharInfo.Ether = res.CharInfo.EtherStats.Base;
        res.CharInfo.SpeedStats.MovementSpeed = Random.Range(character.MovementSpeed.x, character.MovementSpeed.y);
        res.CharInfo.HealthStats.Armour = Random.Range(character.Armour.x, character.Armour.y);
        res.CharInfo.ShieldStats.MinionShieldChances = Random.Range(character.MinionShieldChances.x, character.MinionShieldChances.y);
        res.CharInfo.ShieldStats.MinionPerfectShieldChances = Random.Range(character.MinionPerfectShieldChances.x, character.MinionPerfectShieldChances.y);

        res.CharInfo.ExperienceValue = character.Exp;
        res.DeathAnim = character.DeathAnim;
        res.CharActionlist.Add(CharacterActionType.Move);
        res.CharInfo.AIs = character.AIs;
        if (character.AddAttacks)
        {
            res.CharInfo.AddedAttackTypeInfo.AddRange(character.AttacksToAdd);
        }
        else
        {
            res.CharInfo.AddedAttackTypeInfo.Clear();
        }
        res.CharInfo.SetupChar();
        return res;
    }

    private void Res_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        leadCharDie = true;
        foreach (WaveCharClass item in currentWavePhase.ListOfEnemy)
        {
            item.NumberOfCharacter = 0;
        }

        foreach (BaseCharacter item in WaveCharcters.Where(r => r.gameObject.activeInHierarchy && r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script).ToList())
        {
            item.CharInfo.Health = -50;
        }
    }

    private BaseCharacter CreateChar(CharacterNameType characterID, BaseCharType bCharType = BaseCharType.None)
    {
        BaseCharacter res = BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(characterID.ToString(), CharacterSelectionType.Up,
        new List<ControllerType> { ControllerType.Enemy }, characterID, WalkingSideType.RightSide, SideType.RightSide, FacingType.Left, bCharType, new List<CharacterActionType>(), LevelType.Novice), transform);
        if (characterID != CharacterNameType.CleasTemple_BossOctopus &&
            characterID != CharacterNameType.CleasTemple_BossOctopus_Head &&
            characterID != CharacterNameType.CleasTemple_BossOctopus_Tentacles &&
            characterID != CharacterNameType.CleasTemple_BossOctopus_Girl) res.gameObject.SetActive(false);
        WaveCharcters.Add(res);
        return res;
    }

    public void RemoveWaveCharacterFromBoard(BaseCharacter character)
    {
        if (WaveCharcters.Where(r => r.CharInfo.Name == character.CharInfo.Name).FirstOrDefault() != null)
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


    public IEnumerator StartWaveByName(string waveName, string variableName, float duration = 0)
    {
        yield return duration == 0 ? Wave(WavePhases.Where(r => r.name == waveName).First()) : Wave(WavePhases.Where(r => r.name == waveName).First(), duration, variableName);
    }

    private IEnumerator Wave(WavePhaseClass wavePhase)
    {
        isWaveOn = true;
        leadCharDie = false;
        float timer = 0;
        currentWavePhase = wavePhase;

        //Wave scaling
        currentWavePhase.MaxEnemyOnScreen = Mathf.Clamp(Mathf.RoundToInt((float)currentWavePhase.MaxEnemyOnScreen * UniversalGameBalancer.Instance.difficulty.enemySpawnScaler), 0, currentWavePhase.AbsoluteMaxEnemyOnScreen);
        foreach (WaveCharClass waveChar in currentWavePhase.ListOfEnemy)
        {
            if(waveChar.NPCType == WaveNPCTypes.Minion)
            {
                waveChar.NumberOfCharacter = Mathf.RoundToInt((float)waveChar.NumberOfCharacter * UniversalGameBalancer.Instance.difficulty.enemySpawnScaler);
            }
        }
        //

        WaveCharacterInfoClass waveCharacterInfoClass;
        while (!leadCharDie)
        {
            timer += BattleManagerScript.Instance.DeltaTime;
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
            if (timer > wavePhase.DelayBetweenChars &&
                WaveCharcters.Where(r => r.gameObject.activeInHierarchy && r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script /*&&
                r.CharInfo.HealthPerc > 0*/ && r.died == false).ToList().Count < wavePhase.MaxEnemyOnScreen)
            {
                if (wavePhase.IsRandom)
                {
                    waveCharacterInfoClass = GetAvailableRandomWaveCharacter(wavePhase);
                }
                else
                {
                    waveCharacterInfoClass = GetAvailableWaveCharacter(wavePhase);
                }

                if(waveCharacterInfoClass != null)
                {
                    newChar = GetWaveCharacter(waveCharacterInfoClass);
                    yield return SpawChar(newChar, CurrentWaveChar.IsRandomSpowiningTile,
                        CurrentWaveChar.IsRandomSpowiningTile ? new Vector2Int() : CurrentWaveChar.SpowningTile[Random.Range(0, CurrentWaveChar.SpowningTile.Count)], true);
                    timer = 0;
                }


                if (wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList().Count == 0)
                {
                    List<BaseCharacter> test;
                    while (true)
                    {
                        test = WaveCharcters.Where(r =>
                            (r.gameObject.activeInHierarchy && (r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script || (r.IsOnField && r.CharInfo.BaseCharacterType != BaseCharType.MinionType_Script)))).ToList();

                        if (leadCharDie || (test.Count == 0 || test.Where(r => r.CharInfo.HealthPerc > 0).ToList().Count == 0))
                        {
                            isWaveOn = false;
                            for (int i = 0; i < test.Count; i++)
                            {
                                for (int a = 0; a < test[i].UMS.Pos.Count; a++)
                                {
                                    if(test[i].DeathAnim == DeathAnimType.Defeat)
                                    {
                                        GridManagerScript.Instance.SetBattleTileState(test[i].UMS.Pos[a], BattleTileStateType.Empty);
                                        test[i].UMS.Pos[a] = Vector2Int.zero;
                                    }
                                }
                            }
                            yield break;
                        }
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public bool WaveStillHasEnemies()
    {
        return currentWavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList().Count > 0 ? true : false;
    }

    private IEnumerator Wave(WavePhaseClass wavePhase, float duration, string variableName)
    {
        IEnumerator wave = Wave(wavePhase);
        StartCoroutine(wave);

        yield return BattleManagerScript.Instance.WaitFor(duration * UniversalGameBalancer.Instance.difficulty.waveDurationScaler, ()=> BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle, ()=> !isWaveOn);

        if(isWaveOn)
        {
            FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == variableName).First().Value = "OFF";
        }
        else
        {
            FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == variableName).First().Value = "ON";
        }
    }

    private IEnumerator SpawChar(BaseCharacter newChar, bool isRandom, Vector2Int pos, bool withArrivingAnim)
    {
        BattleTileScript bts = null;
        while (bts == null)
        {
            if (isRandom)
            {
                bts = GridManagerScript.Instance.GetFreeBattleTile(newChar.UMS.WalkingSide, newChar.UMS.Pos);
            }
            else
            {
                bts = GridManagerScript.Instance.GetBattleTile(pos);
            }
            yield return null;
        }
        yield return SetCharInPos(newChar, bts, withArrivingAnim);

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

        yield return BattleManagerScript.Instance.MoveCharToBoardWithDelay(withArrivingAnim ? 0.2f : 0, currentCharacter, bts.transform.position);

        while (currentCharacter.isActiveAndEnabled && !currentCharacter.IsOnField)
        {
            yield return null;
        }
    }

    private WaveCharacterInfoClass GetAvailableRandomWaveCharacter(WavePhaseClass wavePhase)
    {
        List<WaveCharClass> ListOfEnemy = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList();
        if(ListOfEnemy.Count > 0)
        {
            CurrentWaveChar = ListOfEnemy[Random.Range(0, ListOfEnemy.Count)];
            CurrentWaveChar.NumberOfCharacter--;
            return CurrentWaveChar.TypeOfCharacter;
        }

        return null;
       
    }

    private WaveCharacterInfoClass GetAvailableWaveCharacter(WavePhaseClass wavePhase)
    {
        CurrentWaveChar = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).FirstOrDefault();
        if (CurrentWaveChar != null)
        {
            CurrentWaveChar.NumberOfCharacter--;
            return CurrentWaveChar.TypeOfCharacter;
        }
        return null;
    }

    #region WaveEvents

    public void BossArrived(MinionType_Script boss)
    {
        WaveBossApperEvent?.Invoke(boss);
    }

    #endregion



    private void Update()
    {
        if(Input.GetKey(KeyCode.G) && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle)
        {
            foreach (WaveCharClass item in currentWavePhase.ListOfEnemy)
            {
                item.NumberOfCharacter = 0;
            }

            foreach (BaseCharacter item in WaveCharcters.Where(r => r.gameObject.activeInHierarchy && (r.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script || (r.IsOnField && r.CharInfo.BaseCharacterType != BaseCharType.MinionType_Script))).ToList())
            {
                item.CharInfo.Health = -50;
            }
            leadCharDie = true;
        }
    }

    public List<CharacterNameType> GetAllIdOfType(WaveNPCTypes NPCtype)
    {
        List<CharacterNameType> res = new List<CharacterNameType>();
        foreach (WavePhaseClass wave in WavePhases)
        {
            foreach (WaveCharClass wavechar in wave.ListOfEnemy)
            {
                if(wavechar.NPCType == NPCtype)
                {
                    res.Add(wavechar.TypeOfCharacter.CharacterName);
                }
            }
        }

        return res.Distinct().ToList();
    }

    public float GetCurrentPartyHPPerc()
    {
        float BaseHp = 0;
        float CurrentHp = 0;
        WaveCharcters.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList().ForEach(a =>
        {
            BaseHp += a.CharInfo.HealthStats.Base;
            CurrentHp += a.CharInfo.HealthStats.Health;
        });
        return (CurrentHp / BaseHp) * 100;
    }

}

[System.Serializable]
public class WavePhaseClass
{
    public string name;
    public bool IsRandom = false;
    public int MaxEnemyOnScreen;
    public int AbsoluteMaxEnemyOnScreen = 0;
    public float DelayBetweenChars;

    public List<WaveCharClass> ListOfEnemy = new List<WaveCharClass>();
}

[System.Serializable]
public class WaveCharacterInfoClass
{
    public CharacterNameType CharacterName;
    public bool KillWaveIfCharDie = false;
    public Vector2 Health;
    public Vector2 HealthRegeneration;
    public Vector2 BaseDamage;
    public Vector2 WeakAttackMultiplier;
    public Vector2 StrongAttackMultiplier;
    public Vector2 BaseSpeed;
    public Vector2 Ether;
    public Vector2 EtherRegeneration;
    public Vector2 MovementSpeed;
    public Vector2 Armour;
    public Vector2 MinionShieldChances;
    public Vector2 MinionPerfectShieldChances;
    public DeathAnimType DeathAnim;
    public float Exp;

    public List<ScriptableObjectAI> AIs = new List<ScriptableObjectAI>();
    public bool AddAttacks = false;
    public List<ScriptableObjectAttackBase> AttacksToAdd = new List<ScriptableObjectAttackBase>();
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


