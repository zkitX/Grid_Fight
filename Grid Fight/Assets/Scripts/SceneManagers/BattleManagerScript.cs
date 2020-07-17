using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{

    public delegate void CurrentBattleStateChanged(BattleState currentBattleState);
    public event CurrentBattleStateChanged CurrentBattleStateChangedEvent;

    public delegate void CurrentBattleSpeedChanged(float currentBattleSpeed);
    public event CurrentBattleSpeedChanged CurrentBattleSpeedChangedEvent;

    public UniversalAudioProfileSO AudioProfile;

    public delegate void CurrentFungusStateChanged(FungusDialogType currentFungusState);
    public event CurrentFungusStateChanged CurrentFungusStateChangedEvent;

    public delegate void MatchLost();
    public event MatchLost MatchLostEvent;


    public float MovementMultiplier = 1;

    public BattleState CurrentBattleState
    {
        get
        {
            return _CurrentBattleState;
        }
        set
        {
            CurrentBattleStateChangedEvent?.Invoke(value);
            if(value == BattleState.FungusPuppets)
            {
                ResetAllActiveChars();
            }
            else if(value == BattleState.Pause)
            {
                BattleSpeed = 0;
            }
            else if (value == BattleState.Battle)
            {
                BattleSpeed = 1;
            }


            _CurrentBattleState = value;
        }
    }

    public FungusDialogType FungusState
    {
        get
        {
            return _FungusState;
        }
        set
        {
            CurrentFungusStateChangedEvent?.Invoke(value);
            _FungusState = value;
        }
    }

    public CharacterType_Script[] PlayerControlledCharacters
    {
        get
        {
            List<CharacterType_Script> chars = new List<CharacterType_Script>();
            for (int i = 0; i < CurrentSelectedCharacters.Count; i++)
            {
                if (CurrentSelectedCharacters[(ControllerType)i].Character != null)
                {
                    chars.Add(CurrentSelectedCharacters[(ControllerType)i].Character);
                }
            }
            if (chars.Count != 0) return chars.ToArray();
            else return null;
        }
    }

    public float BattleSpeed
    {
        get
        {
            return _BattleSpeed;
        }
        set
        {
            _BattleSpeed = value;
            CurrentBattleSpeedChangedEvent?.Invoke(_BattleSpeed);
            if(_BattleSpeed != 1)
            {
                AllCharactersOnField.ForEach(r => StartCoroutine(r.SlowDownAnimation(_BattleSpeed, () => _BattleSpeed != 1)));
                WaveManagerScript.Instance.WaveCharcters.Where(a => a.gameObject.activeInHierarchy).ToList().ForEach(r => StartCoroutine(r.SlowDownAnimation(_BattleSpeed, () => _BattleSpeed != 1)));
            }
        }
    }
    public float _BattleSpeed;


    public float FixedDeltaTime
    {
        get
        {
            return Time.fixedDeltaTime * BattleSpeed;
        }
    }

    public float DeltaTime
    {
        get
        {
            return Time.deltaTime * BattleSpeed;
        }
    }



    public static BattleManagerScript Instance;
    public BattleState _CurrentBattleState;
    public FungusDialogType _FungusState;
    public List<BattleTileScript> OccupiedBattleTiles = new List<BattleTileScript>();
    public GameObject CharacterBasePrefab;
    public Dictionary<ControllerType, CurrentSelectedCharacterClass> CurrentSelectedCharacters = new Dictionary<ControllerType, CurrentSelectedCharacterClass>()
    {
        { ControllerType.Player1, new CurrentSelectedCharacterClass() },
        { ControllerType.Player2, new CurrentSelectedCharacterClass() },
        { ControllerType.Player3, new CurrentSelectedCharacterClass() },
        { ControllerType.Player4, new CurrentSelectedCharacterClass() }
    };
    public int maxPlayersUsed = 0;

    public List<ScriptableObjectCharacterPrefab> ListOfScriptableObjectCharacterPrefab = new List<ScriptableObjectCharacterPrefab>();
    public List<BaseCharacter> AllCharactersOnField = new List<BaseCharacter>();
    public List<BaseCharacter> AllPlayersMinionOnField = new List<BaseCharacter>();
    public List<BaseCharacter> CharsForTalkingPart = new List<BaseCharacter>();

    public List<CharacterLoadingInfoClass> CurrentCharactersLoadingInfo = new List<CharacterLoadingInfoClass>();
    [SerializeField]
    private Transform CharactersContainer;
    private List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
    public List<Color> playerColors = new List<Color>();
    public List<Color> playersColor
    {
        get
        {
            if(SceneLoadManager.Instance != null)
            {
                return SceneLoadManager.Instance.playersColor;
            }
            return playerColors;
        }
    }
    public List<Sprite> playersNumberBig = new List<Sprite>();
    public List<Sprite> playersNumberSmall = new List<Sprite>();
    private MatchType matchType;
    public Camera MCam;
    public bool VFXScene = false;
    [SerializeField] private bool singleUnitControls = true;
    bool matchStarted = false;
    public InputControllerType InputControllerT;
    [SerializeField]
    [HideInInspector] public bool usingFungus = false;


    [Header("CharsRelationship")]
    public List<RelationshipClass> TeamRelationship = new List<RelationshipClass>();

    public void SetupBattleState()
    {
        if (matchStarted) return;
        matchStarted = true;
        ConfigureUsingFungus();
        UIBattleManager.Instance.StartMatch.gameObject.SetActive(false);
    }

    void ConfigureUsingFungus()
    {
        EventManager.Instance?.StartEventManager();
    }

    #region Unity Life Cycle
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Events


    #endregion

    #region Waves


    #endregion

    #region SetCharacterOnBoard
    //Used to set the already created char on a random Position in the battlefield
    public CharacterType_Script SetCharOnBoardOnRandomPos(ControllerType playerController, CharacterNameType cName)
    {
        bool isPlayer = true;
        BaseCharacter currentCharacter = AllCharactersOnField.Where(r => r.UMS.PlayerController.Contains(playerController) && r.CharInfo.CharacterID == cName && !r.IsOnField).FirstOrDefault();
        if(currentCharacter == null)
        {
            currentCharacter = CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == cName).FirstOrDefault();
            isPlayer = false;
        }
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(currentCharacter.UMS.WalkingSide, currentCharacter.UMS.Pos);
       
        if (currentCharacter != null && bts != null)
        {
            return SetCharOnBoard(playerController, cName, bts.Pos, isPlayer);
        }

        return null;
    }


    public IEnumerator SetAllNonUsedCharOnBattlefield()
    {
        List<BaseCharacter> res = new List<BaseCharacter>();
        BattleTileScript bts;

        res.AddRange(AllCharactersOnField.Where(r => !r.IsOnField).ToList());
        foreach (BaseCharacter currentCharacter in res)
        {
            bts = GridManagerScript.Instance.GetFreeTilesAdjacentTo(CurrentSelectedCharacters[ControllerType.Player1].Character.UMS.CurrentTilePos, 2, true, WalkingSideType.LeftSide).First();

            SetCharOnBoard(currentCharacter.UMS.PlayerController[0], currentCharacter.CharInfo.CharacterID, bts.Pos);
        }

        
        while (res.Where(r => !r.IsOnField && r.CharInfo.HealthPerc > 0).ToList().Count != 0)
        {
            yield return null;
        }
    }

    public CharacterType_Script SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterNameType cName, Vector2Int pos)
    {
        if (CurrentSelectedCharacters[playerController].Character != null && (!CurrentSelectedCharacters[playerController].Character.IsOnField
          || (CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle.ToString())))
        {
            return null;
        }

        return SetCharOnBoard(playerController, cName, pos);
    }

    public CharacterType_Script SetCharOnBoard(ControllerType playerController, CharacterNameType cName, Vector2Int pos, bool isPlayer = true)
    {
        using (BaseCharacter currentCharacter = isPlayer ? AllCharactersOnField.Where(r => r.UMS.PlayerController.Contains(playerController) && r.CharInfo.CharacterID == cName).First() :
            CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == cName).First())
        {
            if (currentCharacter.CharInfo.Health <= 0 && isPlayer)
            {
                return null;
            }
            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
            currentCharacter.UMS.CurrentTilePos = bts.Pos;
            currentCharacter.CurrentBattleTiles = new List<BattleTileScript>();
            currentCharacter.UMS.Pos = new List<Vector2Int>();
            ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == currentCharacter.CharInfo.CharacterID).First();
            foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
            {
                currentCharacter.UMS.Pos.Add(item);
            }
            for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
            {
                currentCharacter.UMS.Pos[i] += bts.Pos;
                //Debug.Log(currentCharacter.UMS.Pos[i].ToString());
                GridManagerScript.Instance.SetBattleTileState(currentCharacter.UMS.Pos[i], BattleTileStateType.Occupied);
                BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
                currentCharacter.CurrentBattleTiles.Add(cbts);
            }
            currentCharacter.SetUpEnteringOnBattle();
            StartCoroutine(MoveCharToBoardWithDelay(0.1f, currentCharacter, bts.transform.position));

            UIBattleManager.Instance.isLeftSidePlaying = true;
            return (CharacterType_Script)currentCharacter;
        }
    }

    public IEnumerator RemoveCharacterFromBaord(BaseCharacter currentCharacter, bool leaveEmpty)
    {
        if (leaveEmpty)
        {
            for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
            {
                if(GridManagerScript.Instance.isPosOnField(currentCharacter.UMS.Pos[i]))
                {
                    GridManagerScript.Instance.SetBattleTileState(currentCharacter.UMS.Pos[i], BattleTileStateType.Empty);
                }
                currentCharacter.UMS.Pos[i] = Vector2Int.zero;
            }
        }

        List<Vector2Int> newPoses = new List<Vector2Int>();
        foreach (Vector2Int pos in currentCharacter.UMS.Pos)
        {
            newPoses.Add(Vector2Int.zero);
        }
        currentCharacter.UMS.Pos = newPoses;
        currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", false);
        currentCharacter.SetUpLeavingBattle();


        yield return null;
    }



    public IEnumerator RemoveZombieFromBaord(BaseCharacter zombie)
    {
        for (int i = 0; i < zombie.UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(zombie.UMS.Pos[i], BattleTileStateType.Empty);
            zombie.UMS.Pos[i] = Vector2Int.zero;
        }

        zombie.SetUpLeavingBattle();
        yield return MoveCharToBoardWithDelay(0.2f, zombie, new Vector3(100f, 100f, 100f));

    }




    public IEnumerator MoveCharToBoardWithDelay(float delay, BaseCharacter cb, Vector3 nextPos)
    {
        yield return WaitFor(delay, () => CurrentBattleState == BattleState.Pause);
        cb.transform.position = nextPos;
    }

    #endregion

    #region Create Character

    public BaseCharacter CreateTalkingChar(CharacterNameType characterID)
    {
        CharsForTalkingPart.Add(CreateChar(new CharacterBaseInfoClass(characterID.ToString(), CharacterSelectionType.Up,
        new List<ControllerType> { ControllerType.Player1 }, characterID, WalkingSideType.LeftSide, SideType.LeftSide, FacingType.Right, AttackType.Tile, BaseCharType.TalkingCharacterType_Script, 
        new List<CharacterActionType> {
            CharacterActionType.Defence,
            CharacterActionType.Move,
            CharacterActionType.WeakAttack,
            CharacterActionType.StrongAttack,
            CharacterActionType.Skill1,
            CharacterActionType.Skill2,
            CharacterActionType.Skill3,
            CharacterActionType.SwitchCharacter}, LevelType.Novice), transform));

        return CharsForTalkingPart.Last();
    }



    public IEnumerator InstanciateAllChar()
    {
        //Temporary way of loading characters into the squad from the char selection menu system
        if (SceneLoadManager.Instance != null)
        {
            for (int i = 0; i < BattleInfoManagerScript.Instance.PlayerBattleInfo.Count; i++)
            {
                BattleInfoManagerScript.Instance.PlayerBattleInfo[i].CharacterName = SceneLoadManager.Instance.squad[i].characterID;
            }
        }
        PlayerBattleInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo;
        //

        foreach (CharacterBaseInfoClass item in PlayerBattleInfo)
        {
            AllCharactersOnField.Add(CreateChar(item, CharactersContainer));
        }
        switch (matchType)
        {
            case MatchType.PvE:
                UIBattleManager.Instance.UICharacterSelectionLeft.gameObject.SetActive(true);
                break;
            case MatchType.PvP:
                UIBattleManager.Instance.UICharacterSelectionLeft.gameObject.SetActive(true);
                UIBattleManager.Instance.UICharacterSelectionRight.gameObject.SetActive(true);
                break;
            case MatchType.PPvE:
                UIBattleManager.Instance.UICharacterSelectionLeft.gameObject.SetActive(true);
                break;
            case MatchType.PPvPP:
                UIBattleManager.Instance.UICharacterSelectionLeft.gameObject.SetActive(true);
                UIBattleManager.Instance.UICharacterSelectionRight.gameObject.SetActive(true);
                break;
            case MatchType.PPPPvE:
                UIBattleManager.Instance.UICharacterSelectionLeft.gameObject.SetActive(true);
                break;
        }
        foreach (BaseCharacter playableCharOnScene in AllCharactersOnField)
        {
            StatisticInfoManagerScript.Instance.CharaterStats.Add(new StatisticInfoClass(playableCharOnScene.CharInfo.CharacterID, playableCharOnScene.UMS.PlayerController));
            NewIManager.Instance.SetUICharacterToButton((CharacterType_Script)playableCharOnScene, BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r => r.CharacterName == playableCharOnScene.CharInfo.CharacterID).FirstOrDefault().CharacterSelection);
            if (FlowChartVariablesManagerScript.instance != null && FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == (playableCharOnScene.CharInfo.Name.ToUpper() + "_IN_SQUAD")).FirstOrDefault() != null)
            {
                FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == (playableCharOnScene.CharInfo.Name.ToUpper() + "_IN_SQUAD")).First().Value = "ON";
            }
        }
        SetUICharacterSelectionIcons();

     /*   foreach (CharacterNameType item in WaveManagerScript.Instance.GetAllIdOfType(WaveNPCTypes.Recruitable))
        {
            foreach (BaseCharacter mainChars in AllCharactersOnField)
            {
                TeamRelationship.Add(new RelationshipClass(mainChars.CharInfo.CharacterID, item, mainChars.CharInfo.RelationshipList.Where(r => r.CharacterId == item).FirstOrDefault().CurrentValue));
            }
        }*/

        yield return null;
    }
    //Creation of the character with the basic info
    public BaseCharacter CreateChar(CharacterBaseInfoClass charInfo, Transform parent)
    {
        GameObject characterBasePrefab = null;
       // Debug.LogError(charInfo.CharacterName);
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == charInfo.CharacterName).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100, 100, 100), Quaternion.identity, parent);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        BaseCharacter currentCharacter = (BaseCharacter)characterBasePrefab.AddComponent(System.Type.GetType(charInfo.BCharType == BaseCharType.None ? child.GetComponentInChildren<CharacterInfoScript>().BaseCharacterType.ToString() : charInfo.BCharType.ToString()));
        currentCharacter.UMS = currentCharacter.GetComponent<UnitManagementScript>();
        currentCharacter.UMS.CharOwner = currentCharacter;
        currentCharacter.UMS.PlayerController = charInfo.PlayerController;
        currentCharacter.CharActionlist = charInfo.CharActionlist;
        currentCharacter.UMS.WalkingSide = charInfo.WalkingSide;
        currentCharacter.UMS.Side = charInfo.Side;
        currentCharacter.UMS.Facing = charInfo.Facing;
        currentCharacter.CharInfo.BaseCharacterType = charInfo.BCharType == BaseCharType.None ? child.GetComponentInChildren<CharacterInfoScript>().BaseCharacterType : charInfo.BCharType;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.UMS.Pos.Add(item);
        }
        currentCharacter.CharInfo.CharaterLevel = charInfo.CharaterLevel;
        currentCharacter.SetupCharacterSide();
        if (EventManager.Instance != null)
        {
            EventManager.Instance.UpdateHealth(currentCharacter);
            EventManager.Instance.UpdateStamina(currentCharacter);
        }
        currentCharacter.CharInfo.CharacterSelection = charInfo.CharacterSelection;
        if(SceneLoadManager.Instance != null)
        {
            CharacterLoadInformation squadLoadInfo = SceneLoadManager.Instance.squad.Values.Where(r => r.characterID == currentCharacter.CharInfo.CharacterID).FirstOrDefault();
            if (squadLoadInfo != null)
            {
                currentCharacter.CharInfo.Mask = squadLoadInfo.heldMask != MaskTypes.None ? SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == squadLoadInfo.heldMask).First().maskSkills : null;
            }
        }

        currentCharacter.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        currentCharacter.CharBoxCollider = currentCharacter.GetComponentInChildren<BoxCollider>(true);
        if (currentCharacter.CharBoxCollider != null) currentCharacter.CharBoxCollider.enabled = false;
        UIBattleFieldManager.Instance.SetupCharListener(currentCharacter);
        return currentCharacter;
    }

    private void CurrentCharacter_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Death, AudioBus.HighPrio, transform);

        if (!playerController.Contains(ControllerType.Enemy))
        {

            if (AllCharactersOnField.Where(r => r.CharInfo.Health > 0).ToList().Count == 0)
            {
                MatchLostEvent();
                CurrentBattleState = BattleState.End;
                return;
            }

            /* if (res.Where(r => r.isUsed).ToList().Count == res.Where(r => !r.isAlive).ToList().Count)
             {
                 UIBattleManager.Instance.StartTimeUp(15, side);
             }*/

            if (CurrentSelectedCharacters.Where(r => r.Value.Character != null && r.Value.Character.CharInfo.CharacterID == cName && r.Value.Character.UMS.Side == side).ToList().Count > 0)
            {
                KeyValuePair<ControllerType, CurrentSelectedCharacterClass> currentPlayer = CurrentSelectedCharacters.Where(r => r.Value.Character != null && r.Value.Character.CharInfo.CharacterID == cName && r.Value.Character.UMS.Side == side).First();
                List<BaseCharacter> cbs = AllCharactersOnField.Where(r => r.CharInfo.HealthPerc > 0 && !r.IsOnField && r.UMS.IsCharControllableByPlayers(playerController)).ToList();
                if(cbs.Count > 0)
                {
                    foreach (BaseCharacter item in cbs)
                    {
                        List<KeyValuePair<ControllerType, CurrentSelectedCharacterClass>> controllers = CurrentSelectedCharacters.Where(r => playerController.Contains(r.Key) && r.Value.Character != null && r.Value.NextSelectionChar.NextSelectionChar == item.CharInfo.CharacterSelection).ToList();
                        if (controllers.Count == 0)
                        {
                            SetCharOnBoard(currentPlayer.Key, item.CharInfo.CharacterID, GridManagerScript.Instance.GetFreeBattleTile(item.UMS.WalkingSide, item.UMS.Pos).Pos);
                            SelectCharacter(currentPlayer.Key, (CharacterType_Script)item);

                            CurrentSelectedCharacters[currentPlayer.Key].NextSelectionChar.NextSelectionChar = item.CharInfo.CharacterSelection;
                            CurrentSelectedCharacters[currentPlayer.Key].NextSelectionChar.Side = item.UMS.Side;
                            ((CharacterType_Script)item).SetCharSelected(true, currentPlayer.Key);
                            return;
                        }
                    }
                }
                else
                {
                    currentPlayer.Value.Character = null;
                }
            }
        }
    }

    #endregion

    #region Loading_Selection Character
    public List<MinionType_Script> zombiesList = new List<MinionType_Script>();
    public void Zombification(BaseCharacter zombie, float duration, List<ScriptableObjectAI> ais)
    {
        if(zombie.CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
        {
            List<BaseCharacter> res = AllCharactersOnField.Where(r => !r.IsOnField && r.CharInfo.HealthPerc > 0 && r.BuffsDebuffsList.Where(a => a.Stat == BuffDebuffStatsType.Zombification).ToList().Count == 0).ToList();
            if (res.Count > 0)
            {
                StartCoroutine(CharacterType_Zombification_Co(zombie, duration, ais));
            }
            else
            {
                MatchLostEvent();
                CurrentBattleState = BattleState.End;
                return;
            }
        }
        else if (zombie.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script)
        {
            if(WaveManagerScript.Instance.WaveCharcters.Where(r=> r.IsOnField && r.gameObject.activeInHierarchy).ToList().Count > 1 || WaveManagerScript.Instance.WaveStillHasEnemies())
            {
                StartCoroutine(MinionType_Zombification_Co(zombie, duration));
            }
        }

    }

    IEnumerator CharacterType_Zombification_Co(BaseCharacter zombie, float duration, List<ScriptableObjectAI> ais)
    {
        ControllerType playerController = CurrentSelectedCharacters.Where(r => r.Value.Character == zombie).First().Key;
        CurrentSelectedCharacters[playerController].Character = null;
        DeselectCharacter(zombie.CharInfo.CharacterID, zombie.UMS.Side, playerController);
        Switch_LoadingNewCharacterInRandomPosition(zombie.CharInfo.CharacterSelection, playerController, true);
        zombie.IsOnField = false;

        GameObject zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_MoonDrums_Loop);
        zombiePs.SetActive(true);
        zombiePs.transform.parent = zombie.SpineAnim.transform;
        zombiePs.transform.localPosition = Vector3.zero;
        zombiePs.transform.localRotation = Quaternion.Euler(zombie.UMS.Side == SideType.LeftSide ? Vector3.zero : zombiePs.transform.eulerAngles);

        

        yield return RemoveCharacterFromBaord(zombie, true);
        zombie.CharActionlist.Remove(CharacterActionType.SwitchCharacter);
        zombiePs.transform.parent = null;
        zombiePs.SetActive(false);

        MinionType_Script zombiefied = zombiesList.Where(r => r.CharInfo.CharacterID == zombie.CharInfo.CharacterID).FirstOrDefault();
        if(zombiefied == null)
        {
            zombiefied = (MinionType_Script)CreateChar(new CharacterBaseInfoClass(zombie.CharInfo.CharacterID.ToString(), CharacterSelectionType.Up,
        new List<ControllerType> { ControllerType.Enemy }, zombie.CharInfo.CharacterID, 
        zombie.UMS.WalkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide,
        zombie.UMS.Side == SideType.LeftSide ? SideType.RightSide : SideType.LeftSide,
        zombie.UMS.Facing == FacingType.Left ? FacingType.Right : FacingType.Left,
        AttackType.Tile, BaseCharType.MinionType_Script, new List<CharacterActionType>(), LevelType.Novice), transform);
            zombiesList.Add(zombiefied);
        }
        zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_MoonDrums_Loop);
        zombiePs.transform.parent = zombiefied.SpineAnim.transform;
        zombiePs.transform.localPosition = Vector3.zero;
        zombiePs.SetActive(true);
        zombiefied.CharInfo.AIs = ais;

        yield return WaveManagerScript.Instance.SetCharInPos(zombiefied, GridManagerScript.Instance.GetFreeBattleTile(zombiefied.UMS.WalkingSide, zombiefied.UMS.Pos), true);
        zombiefied.CharActionlist.Add(CharacterActionType.Move);
        while (zombie.BuffsDebuffsList.Where(r=> r.Stat == BuffDebuffStatsType.Zombification).ToList().Count > 0)
        {
            yield return null;
        }
        zombiefied.IsOnField = false;
        while (zombiefied.isMoving || zombiefied.currentAttackPhase != AttackPhasesType.End)
        {
            yield return null;
        }

        zombiePs.transform.parent = null;
        zombiePs.SetActive(false);
        zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_MoonDrums_LoopCrumble);
        zombiePs.SetActive(true);
        zombiePs.transform.parent = zombiefied.SpineAnim.transform;
        zombiePs.transform.localPosition = Vector3.zero;

        yield return RemoveZombieFromBaord(zombiefied);
        while (zombiefied.IsOnField)
        {
            yield return null;
        }

        zombiePs.transform.parent = null;
        zombiePs.SetActive(false);
        zombie.CharActionlist.Add(CharacterActionType.SwitchCharacter);

    }


    IEnumerator MinionType_Zombification_Co(BaseCharacter zombie, float duration)
    {
        //Set up the PS and reset the anim
        GameObject zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Skill_Mind_2_Teleporting);
        zombiePs.SetActive(true);
        zombiePs.transform.position = zombie.SpineAnim.transform.position;
        zombie.SpineAnim.SetAnim(CharacterAnimationStateType.Idle);
        zombie.SetAttackReady(false);

        yield return MoveCharToBoardWithDelay(0.1f, zombie, new Vector3(-100, -100, -100));

        //Emptying the occupied tiles
        foreach (Vector2Int item in zombie.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Empty);
        }

        //Set up animation to be clear and with the idle anim
        zombie.SpineAnim.SpineAnimationState.ClearTracks();
        zombie.SpineAnim.SetAnim(CharacterAnimationStateType.Idle);

        //Set up attack
        zombie.Attacking = false;
        zombie.shotsLeftInAttack = 0;
        zombie.BuffsDebuffsList.ForEach(r =>
        {
            if(r.Stat != BuffDebuffStatsType.Zombification)
            {
                r.Duration = 0;
                r.CurrentBuffDebuff.Stop_Co = true;
            }
        }
        );


        //Delete subscription to events
        zombie.SpineAnim.SpineAnimationState.Event -= zombie.SpineAnimationState_Event;
        zombie.SpineAnim.SpineAnimationState.Complete -= zombie.SpineAnimationState_Complete;
        zombie.CharInfo.BaseSpeedChangedEvent -= zombie._CharInfo_BaseSpeedChangedEvent;
        zombie.CharInfo.DeathEvent -= zombie._CharInfo_DeathEvent;

        // remove char from Wave
        WaveManagerScript.Instance.WaveCharcters.Remove(zombie);


        //Getting char
        PlayerMinionType_Script playerZombie = zombie.GetComponent<PlayerMinionType_Script>();
        if(playerZombie == null)
        {
            playerZombie = zombie.gameObject.AddComponent<PlayerMinionType_Script>();
        }
        
        AllPlayersMinionOnField.Add(playerZombie);
        zombie.enabled = false;

        //Set up charinfo and events
        playerZombie._CharInfo = zombie.CharInfo;
        playerZombie.CharInfo.BaseSpeedChangedEvent += playerZombie._CharInfo_BaseSpeedChangedEvent;
        playerZombie.CharInfo.DeathEvent += playerZombie._CharInfo_DeathEvent;
        playerZombie.CharInfo.CharacterSelection = (CharacterSelectionType)AllCharactersOnField.Count - 1;
        playerZombie.CharInfo.BaseCharacterType = BaseCharType.MinionType_Script;

        //Set up UMS and side
        playerZombie.UMS = playerZombie.GetComponent<UnitManagementScript>();
        playerZombie.UMS.CharOwner = playerZombie;
        playerZombie.UMS.Side = zombie.UMS.Side == SideType.LeftSide ? SideType.RightSide : SideType.LeftSide;
        playerZombie.UMS.WalkingSide = zombie.UMS.WalkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide;
        playerZombie.UMS.Facing = zombie.UMS.Facing == FacingType.Left ? FacingType.Right : FacingType.Left;


       
        playerZombie.CharActionlist.Add(CharacterActionType.Move);
        playerZombie.SetupCharacterSide();

        //Set up playerzombie position
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(playerZombie.UMS.WalkingSide);
        playerZombie.UMS.Pos.Clear();
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == playerZombie.CharInfo.CharacterID).First();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            playerZombie.UMS.Pos.Add(item);
        }

        zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Skill_Mind_2_Teleporting);
        zombiePs.SetActive(true);
        zombiePs.transform.position = bts.transform.position;
        yield return WaveManagerScript.Instance.SetCharInPos(playerZombie, bts, false);

        //Duration on field 
        yield return WaitFor(duration, () => CurrentBattleState != BattleState.Battle, ()=> playerZombie.CharInfo.HealthPerc <= 0);


        //Set up previous sides and charinfo
        playerZombie.UMS.CharOwner = zombie;
        playerZombie.UMS.Side = playerZombie.UMS.Side == SideType.LeftSide ? SideType.RightSide : SideType.LeftSide;
        playerZombie.UMS.WalkingSide = playerZombie.UMS.WalkingSide == WalkingSideType.LeftSide ? WalkingSideType.RightSide : WalkingSideType.LeftSide;
        playerZombie.UMS.Facing = playerZombie.UMS.Facing == FacingType.Left ? FacingType.Right : FacingType.Left;
       
        //Emptying previous position 
        foreach (Vector2Int item in playerZombie.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Empty);
        }

        //resetting pos on char
        playerZombie.UMS.Pos.Clear();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            playerZombie.UMS.Pos.Add(item);
        }



        if (playerZombie.CharInfo.Health > 0)
        {
            //Wait 1 sec
            zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Skill_Mind_2_Teleporting);
            zombiePs.SetActive(true);
            zombiePs.transform.position = playerZombie.SpineAnim.transform.position;
            bts = GridManagerScript.Instance.GetFreeBattleTile(zombie.UMS.WalkingSide);
            playerZombie.SpineAnim.SetAnim(CharacterAnimationStateType.Idle);
            yield return MoveCharToBoardWithDelay(0.1f, playerZombie, new Vector3(-100, -100, -100));
            //Restore char events + anims
            playerZombie.SpineAnim.SpineAnimationState.ClearTracks();
            playerZombie.SpineAnim.SetAnim(CharacterAnimationStateType.Idle);
            playerZombie.SpineAnim.SpineAnimationState.Event -= playerZombie.SpineAnimationState_Event;
            playerZombie.SpineAnim.SpineAnimationState.Complete -= playerZombie.SpineAnimationState_Complete;
            playerZombie.CharInfo.BaseSpeedChangedEvent -= playerZombie._CharInfo_BaseSpeedChangedEvent;
            playerZombie.CharInfo.DeathEvent -= playerZombie._CharInfo_DeathEvent;
            playerZombie.gameObject.SetActive(false);
        }

        AllPlayersMinionOnField.Remove(playerZombie);

        //Wait until char is disabled
        yield return WaitUntil(() => CurrentBattleState != BattleState.Battle, ()=> !playerZombie.gameObject.activeInHierarchy);

        //Set up zombie char to previous state
        playerZombie.BuffsDebuffsList.ForEach(r =>
        {
            r.Duration = 0;
            r.CurrentBuffDebuff.Stop_Co = true;
        }
       );
        playerZombie.SetAttackReady(false);
        playerZombie.enabled = false;
        zombie.enabled = true;
        zombie.SpineAnim.SpineAnimationState.ClearTracks();
        zombie.SpineAnim.SetAnim(CharacterAnimationStateType.Idle);
        zombie.Attacking = false;
        zombie.shotsLeftInAttack = 0;
        zombie.SetupCharacterSide();
        zombie.CharInfo.BaseSpeedChangedEvent += zombie._CharInfo_BaseSpeedChangedEvent;
        zombie.CharInfo.DeathEvent += zombie._CharInfo_DeathEvent;
        zombie.UMS.Pos.Clear();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            zombie.UMS.Pos.Add(item);
        }
        WaveManagerScript.Instance.WaveCharcters.Add(zombie);

        if(zombie.CharInfo.Health > 0)
        {
            zombie.gameObject.SetActive(true);
            zombiePs = ParticleManagerScript.Instance.GetParticle(ParticlesType.Skill_Mind_2_Teleporting);
            zombiePs.SetActive(true);
            zombiePs.transform.position = bts.transform.position;
            yield return WaveManagerScript.Instance.SetCharInPos(zombie, bts, false);
        }
    }



    //Used when the char is not in the battlefield to move it on the battlefield
    /* public void LoadingNewCharacterToGrid(CharacterNameType cName,SideType side, ControllerType playerController)
     {
         if (CurrentBattleState != BattleState.Battle && CurrentBattleState != BattleState.Intro) return;

         if (CurrentSelectedCharacters[playerController].LoadCharCo != null)
         {
             return;
         }
         if (!AllCharactersOnField.Where(r => r.UMS.Side == side && r.CharInfo.CharacterID == cName).First().IsOnField && 
             (CurrentSelectedCharacters.Where(r=> r.Value.Character != null && r.Value.Character.CharInfo.CharacterID == cName).ToList().Count == 0))
         {
             if (CurrentSelectedCharacters[playerController].Character == null || !singleUnitControls)
             {
                 //Spawn first character for player to a random position on the grid
                 CurrentSelectedCharacters[playerController].LoadCharCo = CharacterLoadingIn(cName, playerController);
             }
             else
             {
                 //Swap the characters if the player already has one under their control
                 if(!CurrentSelectedCharacters[playerController].Character.IsOnField)
                 {
                     return;
                 }

                 CurrentSelectedCharacters[playerController].LoadCharCo = SwapCharacters(cName, playerController);
             }
             CurrentCharactersLoadingInfo.Add(new CharacterLoadingInfoClass(cName, playerController, CurrentSelectedCharacters[playerController].LoadCharCo));
             StartCoroutine(CurrentSelectedCharacters[playerController].LoadCharCo);
         }
         else
         {
             //SelectCharacter(playerController, (CharacterType_Script)AllCharactersOnField.Where(r => r.UMS.Side == side && r.CharInfo.CharacterID == cName).First());
         }
     }*/



    public IEnumerator MoveCharOnPos(CharacterNameType characterID, Vector2Int destination, bool holdForCompletedMove = true)
    {
        BaseCharacter character = AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null)
        {
            character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        }

        if (character == null)
        {
            character = CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        }

        if (character == null)
        {
            yield break;
        }

        List<MoveDetailsClass> moveDetails = new List<MoveDetailsClass>();
        Vector2Int[] path = GridManagerScript.Pathfinding.GetPathTo(destination, character.UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(character.UMS.WalkingSide));
        Vector2Int curPos = character.UMS.CurrentTilePos;
        foreach (Vector2Int movePos in path)
        {
            InputDirection direction = InputDirection.Down;
            Vector2Int move = movePos - curPos;
            if (move == new Vector2Int(1, 0)) direction = InputDirection.Down;
            else if (move == new Vector2Int(-1, 0)) direction = InputDirection.Up;
            else if (move == new Vector2Int(0, 1)) direction = InputDirection.Right;
            else if (move == new Vector2Int(0, -1)) direction = InputDirection.Left;
            moveDetails.Add(new MoveDetailsClass(direction));
            curPos = movePos;
        }

        foreach (MoveDetailsClass moveDetail in moveDetails)
        {
            for (int i = 0; i < moveDetail.amount; i++)
            {
                yield return character.MoveCharOnDir_Co(moveDetail.nextDir);
            }
        }
    }

    public void LoadingNewCharacterToGrid(CharacterNameType cName, SideType side, ControllerType playerController, bool worksOnFungusPappets = false)
    {
        if (CurrentBattleState != BattleState.Battle && !worksOnFungusPappets)
        {
            return;
        }

        CharacterType_Script currentCharacter = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterID == cName && r.UMS.Side == side).First();
        currentCharacter.SetCharSelected(true, playerController);
        if (CurrentSelectedCharacters[playerController].Character == null || !singleUnitControls)
        {
            //Spawn first character for player to a random position on the grid
            CurrentSelectedCharacters[playerController].LoadCharCo = CharacterLoadingIn(cName, playerController);
        }
        else
        {
            CurrentSelectedCharacters[playerController].LoadCharCo = SwapCharacters(cName, playerController);
        }
        CurrentCharactersLoadingInfo.Add(new CharacterLoadingInfoClass(currentCharacter.CharInfo.CharacterID, playerController, CurrentSelectedCharacters[playerController].LoadCharCo));
        StartCoroutine(CurrentSelectedCharacters[playerController].LoadCharCo);
    }

    private int it = 0;

    IEnumerator SwapCharacters(CharacterNameType cName, ControllerType playerController)
    {
        it++;
        int val = it;

        if (CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar == CurrentSelectedCharacters[playerController].Character.CharInfo.CharacterSelection)
        {
            //CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = false;
            yield break;
        }

        // yield return HoldPressTimer(playerController);
        CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = true;
        BaseCharacter cb = AllCharactersOnField.Where(r => r.UMS.PlayerController.Contains(playerController) && r.CharInfo.CharacterID == cName).First();
        while (CurrentSelectedCharacters[playerController].OffsetSwap > Time.time || !CurrentSelectedCharacters[playerController].Character.IsOnField || cb.IsOnField ||
            CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle.ToString() ||
            CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString() ||
            CurrentSelectedCharacters[playerController].Character.SkillActivation != null ||
            CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Arriving.ToString())
        {
            if (CurrentSelectedCharacters[playerController].Character.CharInfo.Health <= 0f)
            {
                DeselectCharacter(cName, CurrentSelectedCharacters[playerController].Character.UMS.Side, playerController);
                yield break;
            }
            yield return null;
        }

        if (CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar == AllCharactersOnField.Where(r => r.CharInfo.CharacterID == cName &&
        CurrentSelectedCharacters[playerController].NextSelectionChar.Side == r.UMS.Side).First().CharInfo.CharacterSelection)
        {
            if (!CurrentSelectedCharacters[playerController].Character.IsSwapping && CurrentSelectedCharacters[playerController].Character.CharInfo.CharacterID != cName)
            {
                CurrentSelectedCharacters[playerController].isSwapping = true;
                Vector2Int spawnPos = CurrentSelectedCharacters[playerController].Character.UMS._CurrentTilePos;
                CharacterType_Script currentCharacter = SetCharOnBoardOnFixedPos(playerController, cName, spawnPos);
                //Debug.Log("Exit  " + CurrentSelectedCharacters[playerController].OffsetSwap + "    " + Time.time + CurrentSelectedCharacters[playerController].NextSelectionChar + AllCharactersOnField.Where(r => r.CharInfo.CharacterID == cName).First().CharInfo.CharacterSelection);
                if (currentCharacter != null)
                {
                    //Debug.Log(val + "   swapping char");
                    CurrentSelectedCharacters[playerController].Character.IsSwapping = true;

                    // currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", false);
                    //currentCharacter.SpineAnim.SetAnimationSpeed(2);
                    yield return RemoveCharacterFromBaord(CurrentSelectedCharacters[playerController].Character, false);

                    SelectCharacter(playerController, currentCharacter);
                    CurrentSelectedCharacters[playerController].Character.IsSwapping = true;
                    // currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", true);
                }
            }
        }

        CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = false;
        //And drop the new character in
        CurrentSelectedCharacters[playerController].LoadCharCo = null;
        CurrentSelectedCharacters[playerController].isSwapping = false;
    }


    public void RemoveAllNonUsedCharFromBoard(List<CharacterNameType> KeepCharOnBattlefield)
    {
        List<BaseCharacter> cbs = AllCharactersOnField.Where(r => r.IsOnField).ToList();
        for (int i = 0; i < cbs.Count; i++)
        {
            bool isIn = false;
            for (int a = 0; a < CurrentSelectedCharacters.Count; a++)
            {
                if (CurrentSelectedCharacters[(ControllerType)a].Character != null && CurrentSelectedCharacters[(ControllerType)a].Character == cbs[i])
                {
                    isIn = true;
                }
            }

            if (!isIn && !KeepCharOnBattlefield.Contains(cbs[i].CharInfo.CharacterID))
            {
                RemoveNamedCharacterFromBoard(cbs[i].CharInfo.CharacterID);
            }
        }
       
        for (int i = 0; i < CharsForTalkingPart.Count; i++)
        {
            if(CharsForTalkingPart[i].IsOnField && !KeepCharOnBattlefield.Contains(CharsForTalkingPart[i].CharInfo.CharacterID))
            {
                RemoveNamedCharacterFromBoard(CharsForTalkingPart[i].CharInfo.CharacterID);
            }
        }
    }

    public void RemoveNamedCharacterFromBoard(CharacterNameType charToRemoveName)
    {
        BaseCharacter charToRemove = AllCharactersOnField.Where(r => r.CharInfo.CharacterID == charToRemoveName).FirstOrDefault();
        if (charToRemove != null)
        {
            foreach (KeyValuePair<ControllerType, CurrentSelectedCharacterClass> item in CurrentSelectedCharacters)
            {
                if (item.Value.Character != null && item.Value.Character == charToRemove)
                {
                    DeselectCharacter(charToRemove.CharInfo.CharacterID, charToRemove.UMS.Side, item.Key);
                    item.Value.Character = null;
                }
            }
        }
        else
        {
            charToRemove = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == charToRemoveName && r.IsOnField).FirstOrDefault();
        }
        if (charToRemove == null)
        {
            charToRemove = CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == charToRemoveName && r.IsOnField).FirstOrDefault();
        }
        Debug.Log("Character removed: " + charToRemove.CharInfo.CharacterID.ToString());
        ControllerType controller = ControllerType.None;
        if (CurrentSelectedCharacters.Values != null && CurrentSelectedCharacters.Where(r => r.Value.Character == charToRemove).FirstOrDefault().Key != ControllerType.None)
        {
            controller = CurrentSelectedCharacters.Where(r => r.Value.Character == charToRemove).FirstOrDefault().Key;
        }
        charToRemove.SpineAnim.SetAnimationSpeed(2);
        StartCoroutine(RemoveCharacterFromBaord(charToRemove, true));
    }


    public void ResetAllActiveChars()
    {
        foreach (CharacterType_Script item in AllCharactersOnField.Where(r => r.IsOnField).ToList())
        {
            item.isMoving = false;
            item.isDefending = false;
            item.isDefendingStop = false;
            item.isSpecialLoading = false;
            item.isSpecialStop = false;
            item.chargingAttackTimer = 0f;
            item.ResetAudioManager();
            item.SpineAnim.transform.localPosition = item.LocalSpinePosoffset;
        }
    }

    //Stop the loading of a char
    public void StopLoadingNewCharacter(CharacterNameType cName, ControllerType playerController)
    {
        if (CurrentCharactersLoadingInfo.Where(r => r.CName == cName && r.PlayerController == playerController).ToList().Count > 0)
        {
            CurrentCharactersLoadingInfo.Remove(CurrentCharactersLoadingInfo.Where(r => r.PlayerController == playerController && r.CName == cName).First());
        }
    }

    //Used to select a char under a determinated player
    public void SetCharacterSelection(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SelectCharacter(playerController, (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == characterSelection && r.UMS.PlayerController.Contains(playerController)).FirstOrDefault());
    }

    public void DeselectCharacter(CharacterNameType charToDeselectName, SideType side, ControllerType playerController)
    {
        if (CurrentBattleState != BattleState.Battle && CurrentBattleState != BattleState.Intro && CurrentBattleState != BattleState.FungusPuppets)
        {
            return;
        }

        CharacterType_Script charToDeselect = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterID == charToDeselectName && r.UMS.Side == side).FirstOrDefault();

        if (charToDeselect != null)
        {
            charToDeselect.SetCharSelected(false, playerController);
        }
    }

    //Used to select a char 
    public void SelectCharacter(ControllerType playerController, CharacterType_Script currentCharacter)
    {
        if (currentCharacter != null && currentCharacter.CharInfo.HealthPerc > 0)
        {
            //If the character is not already selected by another player
            if (CurrentSelectedCharacters.Where(r => r.Value.Character == currentCharacter).ToList().Count == 0)
            {
                //If the player already has a character selected
                if (CurrentSelectedCharacters[playerController].Character != null)
                {
                    //Deselect it
                    //DeselectCharacter(CurrentSelectedCharacters[playerController].Character.CharInfo.CharacterID);
                }
                else
                {
                    //Otherwise, Set the player's UI elements to active
                    switch (matchType)
                    {
                        case MatchType.PvE:
                            if (playerController == ControllerType.Player1)
                            {
                                UIBattleManager.Instance.PlayerA.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerA.Anim.SetBool("FadeInOut", true);
                            }
                            break;
                        case MatchType.PvP:
                            if (playerController == ControllerType.Player1)
                            {
                                UIBattleManager.Instance.PlayerA.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerA.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player2)
                            {
                                UIBattleManager.Instance.PlayerB.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerB.Anim.SetBool("FadeInOut", true);
                            }
                            break;
                        case MatchType.PPvE:

                            if (playerController == ControllerType.Player1)
                            {
                                UIBattleManager.Instance.PlayerA.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerA.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player2)
                            {
                                UIBattleManager.Instance.PlayerC.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerC.Anim.SetBool("FadeInOut", true);
                            }
                            break;
                        case MatchType.PPvPP:
                            if (playerController == ControllerType.Player1)
                            {
                                UIBattleManager.Instance.PlayerA.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerA.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player2)
                            {
                                UIBattleManager.Instance.PlayerB.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerB.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player3)
                            {
                                UIBattleManager.Instance.PlayerC.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerC.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player4)
                            {
                                UIBattleManager.Instance.PlayerD.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerD.Anim.SetBool("FadeInOut", true);
                            }
                            break;
                        case MatchType.PPPPvE:
                            if (playerController == ControllerType.Player1)
                            {
                                UIBattleManager.Instance.PlayerA.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerA.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player2)
                            {
                                UIBattleManager.Instance.PlayerB.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerB.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player3)
                            {
                                UIBattleManager.Instance.PlayerC.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerC.Anim.SetBool("FadeInOut", true);
                            }

                            if (playerController == ControllerType.Player4)
                            {
                                UIBattleManager.Instance.PlayerD.gameObject.SetActive(true);
                                UIBattleManager.Instance.PlayerD.Anim.SetBool("FadeInOut", true);
                            }
                            break;
                    }
                }
                //Change this player's character to the new character
                CurrentSelectedCharacters[playerController].Character = currentCharacter;
                currentCharacter.CurrentPlayerController = playerController;
                CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar = currentCharacter.CharInfo.CharacterSelection;
                CurrentSelectedCharacters[playerController].NextSelectionChar.Side = currentCharacter.UMS.Side;
                currentCharacter.UMS.SetBattleUISelection(playerController);
                currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", true);

                int newPlayerCount = CurrentSelectedCharacters.Values.Where(r => r._Character != null).ToList().Count;
                maxPlayersUsed = maxPlayersUsed < newPlayerCount ? newPlayerCount : maxPlayersUsed;
                //Change the player's UI to the new character
                //UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);


                //Set the new character to selected by this player
                //currentCharacter.SetCharSelected(true, playerController);
            }
        }
    }

    //Load char in a random pos
    private IEnumerator CharacterLoadingIn(CharacterNameType cName, ControllerType playerController)
    {
        // yield return HoldPressTimer(playerController);
        if (CurrentCharactersLoadingInfo.Where(r => r.CName == cName && r.PlayerController == playerController).ToList().Count > 0)
        {
            CharacterType_Script cb = SetCharOnBoardOnRandomPos(playerController, cName);
            if (cb != null)
            {
                SelectCharacter(playerController, cb);
            }
        }
        yield return null;

        CurrentSelectedCharacters[playerController].LoadCharCo = null;
    }

    //Load char in a fixed pos
    private IEnumerator CharacterLoadingIn(CharacterNameType cName, ControllerType playerController, Vector2Int pos)
    {
        //yield return HoldPressTimer(playerController);
        if (CurrentCharactersLoadingInfo.Where(r => r.CName == cName && r.PlayerController == playerController).ToList().Count > 0)
        {
            CharacterType_Script cb = SetCharOnBoardOnRandomPos(playerController, cName);
            if (cb != null)
            {
                SelectCharacter(playerController, cb);
            }
        }

        yield return null;
    }

    public void UpdateCurrentSelectedCharacters(CharacterType_Script oldChar, CharacterType_Script newChar, SideType side)
    {
        CurrentSelectedCharacterClass cscc = CurrentSelectedCharacters.Where(r => r.Value.Character != null && r.Value.Character.CharInfo.CharacterID == oldChar.CharInfo.CharacterID && r.Value.Character.UMS.Side == side).First().Value;
        cscc.Character = newChar;
    }

    #endregion

    #region Move Character

    //Move selected char under determinated player
    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirection dir)
    {
        if (CurrentBattleState != BattleState.Battle) return;

        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            if (CurrentSelectedCharacters[playerController].Character.UMS.UnitBehaviour == UnitBehaviourType.ControlledByPlayer)
            {
                CurrentSelectedCharacters[playerController].Character.MoveCharOnDirection(dir);
            }
        }
    }
    #endregion

    #region Switch Input
    public void Switch_StopLoadingNewCharacter(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        if (CurrentBattleState == BattleState.Battle || CurrentBattleState == BattleState.FungusPuppets || CurrentBattleState == BattleState.Intro)
        {
            SideType side = GetSideFromPlayer(new List<ControllerType> { playerController });
            BaseCharacter cb = AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == characterSelection && r.UMS.Side == side).FirstOrDefault();
            if (cb != null)
            {
                StopLoadingNewCharacter(cb.CharInfo.CharacterID, playerController);
            }
        }
    }

    public void Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType characterSelection, ControllerType playerController, bool isRandom = false, bool worksOnFungusPappets = false)
    {
        if(CurrentSelectedCharacters[playerController].Character != null && !CurrentSelectedCharacters[playerController].Character.CharActionlist.Contains(CharacterActionType.SwitchCharacter))
        {
            return;
        }

        SideType side = GetSideFromPlayer(new List<ControllerType> { playerController });
        BaseCharacter cb = new BaseCharacter();
        CharacterSelectionType cs = CharacterSelectionType.Up;
        bool deselction = true;
        if (InputControllerT == InputControllerType.SelectionOnABXY)
        {
            if (CurrentBattleState == BattleState.Battle)
            {

                cb = AllCharactersOnField.FirstOrDefault();
                if (cb != null)
                {
                    LoadingNewCharacterToGrid(cb.CharInfo.CharacterID, side, playerController);
                }
            }
        }
        else if (InputControllerT == InputControllerType.SelectionOnLR)
        {
            if ((CurrentBattleState == BattleState.Battle && !CurrentSelectedCharacters[playerController].isSwapping) || worksOnFungusPappets)
            {
                cb = null;
                if (CurrentSelectedCharacters[playerController].Character == null)
                {
                    
                    deselction = false;
                }
                else
                {
                    cs = CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar;
                }

                if(isRandom)
                {
                    bool found = false;
                    List<BaseCharacter> res = AllCharactersOnField.Where(r => r.gameObject.activeInHierarchy && r.UMS.Side == side && r.CharInfo.HealthPerc > 0 && !r.IsOnField &&
                    r.BuffsDebuffsList.Where(a=> a.Stat == BuffDebuffStatsType.Zombification).ToList().Count == 0 && r.CharActionlist.Contains(CharacterActionType.SwitchCharacter)).ToList();
                    if(res.Count > 0)
                    {
                        while (!found)
                        {
                            cs = (CharacterSelectionType)Random.Range(0, 4);
                            cb = res.Where(r => r.CharInfo.CharacterSelection == cs).FirstOrDefault();

                        if (cb != null && CurrentSelectedCharacters.Where(r => r.Value.Character != null && ((r.Value.Character == cb)
                        || (r.Value.NextSelectionChar.NextSelectionChar == cs && r.Value.NextSelectionChar.Side == cb.UMS.Side && r.Value.Character != null)) && r.Key != playerController).ToList().Count == 0)
                            {

                                found = true;
                            }
                        }
                    }
                    else
                    {

                    }
                    
                }
                else
                {
                    for (int i = 0; i < AllCharactersOnField.Count; i++)
                    {
                        cs = cs + (characterSelection == CharacterSelectionType.Left ? -1 : 1);
                        int maxChars = AllCharactersOnField.Count > 4 ? 4 : AllCharactersOnField.Count;
                        cs = (int)cs >= maxChars ? 0 : cs < 0 ? ((CharacterSelectionType)maxChars - 1) : cs;
                        string t = cs.ToString();
                        //Debug.Log(t);
                        cb = AllCharactersOnField.Where(r => r.gameObject.activeInHierarchy && r.CharInfo.CharacterSelection == cs && r.UMS.Side == side && r.CharInfo.HealthPerc > 0 &&
                        r.BuffsDebuffsList.Where(a => a.Stat == BuffDebuffStatsType.Zombification).ToList().Count == 0 && r.CharActionlist.Contains(CharacterActionType.SwitchCharacter)).FirstOrDefault();
                        if (cb != null)
                        {
                            t = cb.CharInfo.CharacterID.ToString() + "    " + cb.UMS.Side.ToString();
                        }
                        //Debug.Log(t);
                        if (cb != null && CurrentSelectedCharacters.Where(r => r.Value.Character != null && ((r.Value.Character == cb)
                        || (r.Value.NextSelectionChar.NextSelectionChar == cs && r.Value.NextSelectionChar.Side == cb.UMS.Side && r.Value.Character != null)) && r.Key != playerController).ToList().Count == 0)
                        {
                            CharacterType_Script PrevCharacter = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar && r.UMS.Side == side).First();

                            if (deselction)
                            {
                                DeselectCharacter(PrevCharacter.CharInfo.CharacterID, side, playerController);
                            }
                            //Debug.Log("Prev " + CurrentSelectedCharacters[playerController].NextSelectionChar.ToString());
                            CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar = cs;
                            CurrentSelectedCharacters[playerController].NextSelectionChar.Side = cb.UMS.Side;
                            //Debug.Log(cs.ToString());
                            break;
                        }
                    }                    
                }
                if (cb != null)
                {

                    //Debug.LogError(cb.CharInfo.CharacterID);
                    SetNextChar(deselction, cb, side, playerController, cs, worksOnFungusPappets);
                }
                else
                {
                    CurrentSelectedCharacters[playerController].Character = null;
                }
            }
        }
    }


    public void SetNextChar(bool deselction, BaseCharacter cb, SideType side, ControllerType playerController, CharacterSelectionType cs,  bool worksOnFungusPappets = false)
    {
        CharacterType_Script PrevCharacter = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar && r.UMS.Side == side).First();

        if (deselction)
        {
            DeselectCharacter(PrevCharacter.CharInfo.CharacterID, side, playerController);
        }
        //Debug.Log("Prev " + CurrentSelectedCharacters[playerController].NextSelectionChar.ToString());
        CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar = cs;
        CurrentSelectedCharacters[playerController].NextSelectionChar.Side = cb.UMS.Side;
        Debug.Log(cs.ToString());
       
        LoadingNewCharacterToGrid(cb.CharInfo.CharacterID, side, playerController, worksOnFungusPappets);
        
    }

    public void StopChargingAttack(ControllerType controllerType)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                CurrentSelectedCharacters[controllerType].Character.isSpecialStop = true;
                Debug.Log("<b>FINISHED CANCELLING CHARGE ATTACK</b>");
            }
        }
    }
    

    public void StartChargingAttack(ControllerType controllerType, AttackInputType atk)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                CurrentSelectedCharacters[controllerType].Character.CharacterInputHandler((InputActionType)System.Enum.Parse(typeof(InputActionType), atk.ToString()));
            }
        }
    }
    public void StartQuickAttack(ControllerType controllerType)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                CurrentSelectedCharacters[controllerType].Character.CharacterInputHandler(InputActionType.Weak);
            }
        }
    }

    public void CurrentCharacterStartDefending(ControllerType playerController)
    {
        if (CurrentBattleState != BattleState.Battle) return;

        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            CurrentSelectedCharacters[playerController].Character.CharacterInputHandler(InputActionType.Defend);
        }
    }

    public void CurrentCharacterStopDefending(ControllerType playerController)
    {
        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            CurrentSelectedCharacters[playerController].Character.CharacterInputHandler(InputActionType.Defend_Stop);
        }
    }

    #endregion

    #region Mobile Input
    public void Mobile_StopLoadingNewCharacter(CharacterNameType cName, ControllerType playerController)
    {
        StopLoadingNewCharacter(cName, playerController);
    }

    public void Mobile_LoadingNewCharacterInRandomPosition(CharacterNameType cName, ControllerType playerController)
    {
        LoadingNewCharacterToGrid(cName, GetSideFromPlayer(new List<ControllerType> { playerController }), playerController);
    }

    #endregion

    public void RecruitCharFromWave(CharacterNameType characterID)
    {
        GameObject rC = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault().gameObject;
        WaveManagerScript.Instance.WaveCharcters.Remove(rC.GetComponent<BaseCharacter>());
        rC.transform.parent = transform;
        rC.SetActive(true);
        MinionType_Script m = rC.GetComponent<MinionType_Script>();
        m.SpineAnim.SpineAnimationState.Event -= m.SpineAnimationState_Event;
        m.SpineAnim.SpineAnimationState.Complete -= m.SpineAnimationState_Complete;
        m.CharInfo.BaseSpeedChangedEvent -= m._CharInfo_BaseSpeedChangedEvent;
        m.CharInfo.DeathEvent -= m._CharInfo_DeathEvent;
        rC.SetActive(false);
        Destroy(rC.GetComponent<MinionType_Script>());
        CharacterType_Script recruitableChar = rC.AddComponent<CharacterType_Script>();
        AllCharactersOnField.Add(recruitableChar);
        
        recruitableChar.UMS = recruitableChar.GetComponent<UnitManagementScript>();
        recruitableChar.UMS.CharOwner = recruitableChar;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == recruitableChar.CharInfo.CharacterID).First();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            recruitableChar.UMS.Pos.Add(item);
        }
        recruitableChar.UMS.Facing = FacingType.Right;
        recruitableChar.UMS.PlayerController = AllCharactersOnField[0].UMS.PlayerController;
        recruitableChar.UMS.Side = SideType.LeftSide;
        recruitableChar.UMS.UnitBehaviour = UnitBehaviourType.ControlledByPlayer;
        recruitableChar.UMS.WalkingSide = WalkingSideType.LeftSide;
        recruitableChar.CharInfo.CharacterSelection = (CharacterSelectionType)AllCharactersOnField.Count - 1;
        recruitableChar.CharInfo.BaseCharacterType = BaseCharType.CharacterType_Script;
        recruitableChar.CharInfo.SetupChar();
        NewIManager.Instance.SetUICharacterToButton(recruitableChar, recruitableChar.CharInfo.CharacterSelection);
        recruitableChar.CharInfo.HealthStats.Health = recruitableChar.CharInfo.HealthStats.Base;
        recruitableChar.CharActionlist.Add(CharacterActionType.Move);
        recruitableChar.CharActionlist.Add(CharacterActionType.Defence);
        recruitableChar.CharActionlist.Add(CharacterActionType.Skill1);
        recruitableChar.CharActionlist.Add(CharacterActionType.Skill2);
        recruitableChar.CharActionlist.Add(CharacterActionType.Skill3);
        recruitableChar.CharActionlist.Add(CharacterActionType.StrongAttack);
        recruitableChar.CharActionlist.Add(CharacterActionType.SwitchCharacter);
        recruitableChar.CharActionlist.Add(CharacterActionType.WeakAttack);
        recruitableChar.gameObject.SetActive(true);
        recruitableChar.SetupCharacterSide();
        /*foreach (BaseCharacter playableCharOnScene in AllCharactersOnField)
        {
            NewIManager.Instance.SetUICharacterToButton((CharacterType_Script)playableCharOnScene, playableCharOnScene.CharInfo.CharacterSelection);
        }*/


        recruitableChar.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        SetUICharacterSelectionIcons();
    }

    public List<BaseCharacter> GetAllPlayerActiveChars()
    {
        List<BaseCharacter> res = new List<BaseCharacter>();
        res.AddRange(AllCharactersOnField.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList());
        res.AddRange(AllPlayersMinionOnField.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList());

        return res;
    }


    public void RestartScene()
    {
        if (CurrentBattleState == BattleState.Pause)
        {
            CurrentBattleState = BattleState.End;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SplashPage_GIO");
            //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            EventManager.Instance.ResetEventsInManager();
        }
    }


    public BaseCharacter GetCharInPos(Vector2Int pos)
    {
        BaseCharacter cb = AllCharactersOnField.Where(r => r.IsOnField && r.UMS.Pos.Contains(pos)).FirstOrDefault();

        if (cb == null)
        {
            cb = AllPlayersMinionOnField.Where(r => r.IsOnField && r.UMS.Pos.Contains(pos)).FirstOrDefault();
        }

        if (cb == null && WaveManagerScript.Instance != null)
        {
            cb = WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.UMS.Pos.Contains(pos)).FirstOrDefault();
        }

        return cb;
    }

    public BaseCharacter GetActiveCharNamed(CharacterNameType _name)
    {
        BaseCharacter chara = AllCharactersOnField.Where(r => r.CharInfo.CharacterID == _name).FirstOrDefault();
        if (chara == null) chara = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == _name).FirstOrDefault();
        if (chara == null) Debug.LogError("Character with ID: " + _name.ToString() + " does not exist in the scene");
        return chara;
    }

    //Used to setup all the current char icons
    public void SetUICharacterSelectionIcons()
    {
        List<UIIconClass> resLeft = new List<UIIconClass>();
        List<UIIconClass> resRight = new List<UIIconClass>();

        foreach (BaseCharacter item in AllCharactersOnField)
        {
            if (item.UMS.Side == SideType.RightSide)
            {
                resRight.Add(new UIIconClass(item, item.CharInfo.CharacterSelection));
            }
            else
            {
                resLeft.Add(new UIIconClass(item, item.CharInfo.CharacterSelection));
            }
        }

        UIBattleManager.Instance.UICharacterSelectionLeft.SetupCharacterIcons(resLeft);
        UIBattleManager.Instance.UICharacterSelectionRight.SetupCharacterIcons(resRight);
    }

    //
    public SideType GetSideFromPlayer(List<ControllerType> ct)
    {
        matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                return ct.Contains(ControllerType.Player1) ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PvP:
                return ct.Contains(ControllerType.Player1) ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PPvE:
                return ct.Contains(ControllerType.Player1) || ct.Contains(ControllerType.Player2) ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PPvPP:
                return ct.Contains(ControllerType.Player1) || ct.Contains(ControllerType.Player2) ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PPPPvE:
                return !ct.Contains(ControllerType.Enemy) ? SideType.LeftSide : SideType.RightSide;

        }
        return SideType.LeftSide;
    }


    public IEnumerator WaitUpdate(System.Func<bool> pauseCondition)
    {
        yield return null;

        while (pauseCondition())
        {
            yield return null;
        }
    }

    public IEnumerator WaitUpdate(System.Action action, System.Func<bool> condition)
    {
        yield return null;

        while (condition())
        {
            action();
            yield return null;
        }
    }

    public IEnumerator WaitFor(float duration, System.Func<bool> pauseCondition)
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return WaitUpdate(pauseCondition);
            timer += DeltaTime;
        }
    }

    public IEnumerator WaitFor(float duration, System.Func<bool> pauseCondition, System.Func<bool> stopCondition)
    {
        float timer = 0;
        while (timer < duration)
        {
            yield return WaitUpdate(pauseCondition);
            timer += DeltaTime;
            if(stopCondition())
            {
                yield break;
            }
        }
    }

    public IEnumerator WaitUntil(System.Func<bool> pauseCondition, System.Func<bool> stopCondition)
    {
        while (!stopCondition())
        {
            yield return WaitUpdate(pauseCondition);
        }
    }

    public IEnumerator WaitFixedUpdate(System.Func<bool> condition)
    {
        yield return new WaitForFixedUpdate();

        while (condition())
        {
            yield return null;
        }
    }


    public IEnumerator WaitFixedUpdate(System.Action action, System.Func<bool> condition)
    {
        yield return new WaitForFixedUpdate();

        while (condition())
        {
            yield return null;
            action();
        }
    }


    public void UpdateCharactersRelationship(bool allTeam, List<CharacterNameType> playerChars, List<TargetRecruitableClass> recruitableChars)
    {
        if(allTeam)
        {
            foreach (TargetRecruitableClass recruitableChar in recruitableChars)
            {
                foreach (RelationshipClass item in TeamRelationship.Where(r => r.CharacterId == recruitableChar.CharTargetRecruitableID).ToList())
                {
                    item.CurrentValue += recruitableChar.Value;
                }
            }
        }
        else
        {
            foreach (TargetRecruitableClass recruitableChar in recruitableChars)
            {
                foreach (RelationshipClass item in TeamRelationship.Where(r => r.CharacterId == recruitableChar.CharTargetRecruitableID && playerChars.Contains(r.CharOwnerId)).ToList())
                {
                    item.CurrentValue += recruitableChar.Value;
                }
            }
        }
    }

}


public class CharacterLoadingInfoClass
{
    public CharacterNameType CName;
    public ControllerType PlayerController;
    public IEnumerator LoadingNewCharacterCo;

    public CharacterLoadingInfoClass()
    {

    }

    public CharacterLoadingInfoClass(CharacterNameType cName, ControllerType playerController, IEnumerator loadingNewCharacterCo)
    {
        CName = cName;
        PlayerController = playerController;
        LoadingNewCharacterCo = loadingNewCharacterCo;
    }
}


public class PlayableCharOnScene
{
    public CharacterNameType CName;
    public List<ControllerType> PlayerController = new List<ControllerType>();
    public bool isUsed;
    public bool isAlive = true;
    public SideType Side;

    public PlayableCharOnScene(CharacterNameType cname, List<ControllerType> playerController, bool isused, SideType side)
    {
        CName = cname;
        PlayerController = playerController;
        isUsed = isused;
        Side = side;
    }
}


public class CurrentSelectedCharacterClass
{
    public CharacterType_Script _Character;

    public CharacterType_Script Character
    {
        get
        {
            return _Character;
        }
        set
        {
            _Character = value;
        }
    }


    public IEnumerator LoadCharCo;
    public NextSelectionCharClass NextSelectionChar;
    public bool isSwapping = false;
    public float OffsetSwap;

    public CurrentSelectedCharacterClass()
    {
        NextSelectionChar = new NextSelectionCharClass();
    }
}



public class DisposableGameObjectClass : System.IDisposable
{
    public GameObject BaseGO;

    public DisposableGameObjectClass()
    {
    }

    public DisposableGameObjectClass(GameObject baseGO)
    {
        BaseGO = baseGO;
    }

    public void Dispose()
    {
    }
}


public class NextSelectionCharClass
{
    public CharacterSelectionType NextSelectionChar;
    public SideType Side;

    public NextSelectionCharClass()
    {

    }

    public NextSelectionCharClass(CharacterSelectionType nextSelectionChar, SideType side)
    {
        NextSelectionChar = nextSelectionChar;
        Side = side;
    }
}