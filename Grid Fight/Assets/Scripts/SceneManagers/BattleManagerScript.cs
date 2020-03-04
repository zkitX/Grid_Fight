﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{

    public delegate void CurrentBattleStateChanged(BattleState currentBattleState);
    public event CurrentBattleStateChanged CurrentBattleStateChangedEvent;


    public BattleState CurrentBattleState
    {
        get
        {
            return _CurrentBattleState;
        }
        set
        {
            if (CurrentBattleStateChangedEvent != null)
            {
                CurrentBattleStateChangedEvent(value);
            }
            _CurrentBattleState = value;
        }
    }

   
    



    public static BattleManagerScript Instance;
    public BattleState _CurrentBattleState;
    public FungusDialogType FungusState;
    public List<BattleTileScript> OccupiedBattleTiles = new List<BattleTileScript>();
    public GameObject CharacterBasePrefab;
    public Dictionary<ControllerType, CurrentSelectedCharacterClass> CurrentSelectedCharacters = new Dictionary<ControllerType, CurrentSelectedCharacterClass>()
    {
        { ControllerType.Player1, new CurrentSelectedCharacterClass() },
        { ControllerType.Player2, new CurrentSelectedCharacterClass() },
        { ControllerType.Player3, new CurrentSelectedCharacterClass() },
        { ControllerType.Player4, new CurrentSelectedCharacterClass() }
    };
    public List<ScriptableObjectCharacterPrefab> ListOfScriptableObjectCharacterPrefab = new List<ScriptableObjectCharacterPrefab>();
    public List<BaseCharacter> AllCharactersOnField = new List<BaseCharacter>();
    public List<CharacterLoadingInfoClass> CurrentCharactersLoadingInfo = new List<CharacterLoadingInfoClass>();
    [SerializeField]
    private Transform CharactersContainer;
    private List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
    public List<PlayableCharOnScene> PlayablesCharOnScene = new List<PlayableCharOnScene>();
    public List<Color> playersColor = new List<Color>();
    public List<Sprite> playersNumberBig = new List<Sprite>();
    public List<Sprite> playersNumberSmall = new List<Sprite>();
    private MatchType matchType;
    public Camera MCam;
    public bool VFXScene = false;
    [SerializeField]  private bool singleUnitControls = true;
    bool matchStarted = false;
    public InputControllerType InputControllerT;
    [SerializeField]
    [HideInInspector] public bool usingFungus = false;

    

    public void SetupBattleState()
    {
        if (matchStarted) return;
        matchStarted = true;
        ConfigureUsingFungus();
        UIBattleManager.Instance.StartMatch.gameObject.SetActive(false);
    }

    void ConfigureUsingFungus()
    {
        EventManager.Instance.StartEventManager();
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
    public void SetCharOnBoardOnRandomPos(ControllerType playerController, CharacterNameType cName)
    {
        BaseCharacter currentCharacter = AllCharactersOnField.Where(r=> r.UMS.PlayerController.Contains(playerController) && r.CharInfo.CharacterID == cName && !r.IsOnField).FirstOrDefault();
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(currentCharacter.UMS.WalkingSide, currentCharacter.UMS.Pos);
        if (currentCharacter != null && bts != null)
        {
            SelectCharacter(playerController, SetCharOnBoardOnFixedPos(playerController, cName, bts.Pos));
        }
    }

    public CharacterType_Script SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterNameType cName, Vector2Int pos)
    {
        if (CurrentSelectedCharacters[playerController].Character != null && (PlayablesCharOnScene.Where(r => r.PlayerController.Contains(playerController) && r.CName == cName).First().isUsed ||
           (CurrentSelectedCharacters[playerController].Character.isMoving)
           || (!CurrentSelectedCharacters[playerController].Character.IsOnField)
          || (CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle)))
        {
            return null;
        }

        using (BaseCharacter currentCharacter = AllCharactersOnField.Where(r => r.UMS.PlayerController.Contains(playerController) && r.CharInfo.CharacterID == cName).First())
        {
            if (currentCharacter.CharInfo.Health <= 0)
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
                Debug.Log(currentCharacter.UMS.Pos[i].ToString());
                GridManagerScript.Instance.SetBattleTileState(currentCharacter.UMS.Pos[i], BattleTileStateType.Occupied);
                BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
                currentCharacter.CurrentBattleTiles.Add(cbts);
            }
            currentCharacter.SetUpEnteringOnBattle();
            StartCoroutine(MoveCharToBoardWithDelay(0.1f, currentCharacter, bts.transform.position));
            
            UIBattleManager.Instance.isLeftSidePlaying = true;
            PlayablesCharOnScene.Where(r => r.PlayerController.Contains(playerController) && r.CName == cName).First().isUsed = true;
            return (CharacterType_Script)currentCharacter;
        }
    }

    public IEnumerator RemoveCharacterFromBaord(ControllerType playerController, BaseCharacter currentCharacter, bool leaveEmpty)
    {
        if (leaveEmpty)
        {
            for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
            {
                GridManagerScript.Instance.SetBattleTileState(currentCharacter.UMS.Pos[i], BattleTileStateType.Empty);
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
        yield return MoveCharToBoardWithDelay(0.3f, currentCharacter, new Vector3(100f, 100f, 100f));

        if (playerController == ControllerType.Player1)
        {
            UIBattleManager.Instance.isLeftSidePlaying = false;
        }
        else if (playerController == ControllerType.Player2)
        {
            UIBattleManager.Instance.isRightSidePlaying = false;
        }

        if (PlayablesCharOnScene.Where(r => r.PlayerController.Contains(playerController) && r.CName == currentCharacter.CharInfo.CharacterID).FirstOrDefault() != null &&
             PlayablesCharOnScene.Where(r => r.PlayerController.Contains(playerController) && r.CName == currentCharacter.CharInfo.CharacterID).First().isUsed)
        {
            PlayablesCharOnScene.Where(r => r.PlayerController.Contains(playerController) && r.CName == currentCharacter.CharInfo.CharacterID).First().isUsed = false;
        }


        if (playerController == ControllerType.None && PlayablesCharOnScene.Where(r => r.CName == currentCharacter.CharInfo.CharacterID).First().isUsed)
        {
            PlayablesCharOnScene.Where(r => r.CName == currentCharacter.CharInfo.CharacterID).First().isUsed = false;
        }

        if (WaveManagerScript.Instance != null)
        {
            WaveManagerScript.Instance.RemoveWaveCharacterFromBoard(currentCharacter);

        }

    }

    //Used to set the already created char on a fixed Position in the battlefield
  

    public IEnumerator MoveCharToBoardWithDelay(float delay, BaseCharacter cb, Vector3 nextPos)
    {
        float timer = 0;
        while (timer <= delay)
        {
            yield return PauseUntil();

            timer += Time.fixedDeltaTime;
        }

        cb.transform.position = nextPos;
    }

    #endregion

    #region Create Character

    public IEnumerator InstanciateAllChar()
    {
        PlayerBattleInfo = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.PlayerBattleInfo : BattleInfoManagerScript.Instance.PlayerBattleInfo;
        foreach (CharacterBaseInfoClass item in PlayerBattleInfo)
        {
            PlayablesCharOnScene.Add(new PlayableCharOnScene(item.CharacterName, item.PlayerController, false, GetSideFromPlayer(item.PlayerController)));
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
        }
        foreach (BaseCharacter playableCharOnScene in AllCharactersOnField)
        {
            NewIManager.Instance.SetUICharacterToButton((CharacterType_Script)playableCharOnScene, BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r => r.CharacterName == playableCharOnScene.CharInfo.CharacterID).FirstOrDefault().CharacterSelection);
        }
        SetUICharacterSelectionIcons();
        yield return null;
    }
//Creation of the character with the basic info
    public BaseCharacter CreateChar(CharacterBaseInfoClass charInfo, Transform parent)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == charInfo.CharacterName).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100, 100, 100), Quaternion.identity, parent);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        BaseCharacter currentCharacter = (BaseCharacter)characterBasePrefab.AddComponent(System.Type.GetType(charInfo.BCharType == BaseCharType.None ? child.GetComponentInChildren<CharacterInfoScript>().BaseCharacterType.ToString() : charInfo.BCharType.ToString()));
        currentCharacter.UMS = currentCharacter.GetComponent<UnitManagementScript>();
        currentCharacter.UMS.CurrentAttackType = charInfo.CharAttackType;
        currentCharacter.UMS.CharOwner = currentCharacter;
        currentCharacter.UMS.PlayerController = charInfo.PlayerController;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.UMS.Pos.Add(item);
        }
        currentCharacter.SetupCharacterSide();
        currentCharacter.UMS.WalkingSide = charInfo.WalkingSide;
        currentCharacter.CharInfo.CharacterSelection = charInfo.CharacterSelection;
        currentCharacter.CharInfo.CharacterLevel = charInfo.CharacterLevel;
        currentCharacter.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        currentCharacter.CharBoxCollider = currentCharacter.GetComponentInChildren<BoxCollider>(true);
        if(currentCharacter.CharBoxCollider != null)currentCharacter.CharBoxCollider.enabled = false;
        UIBattleFieldManager.Instance.SetupCharListener(currentCharacter);
        return currentCharacter;
    }

    private void CurrentCharacter_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController,  SideType side)
    {
        if(!playerController.Contains(ControllerType.Enemy))
        {
            PlayablesCharOnScene.Where(r => r.Side == side && r.CName == cName).First().isAlive = false;

            List<PlayableCharOnScene> res = new List<PlayableCharOnScene>();

            PlayablesCharOnScene.ForEach(r =>
            {
                if(r.PlayerController.Intersect(playerController).Count() == playerController.Count)
                {
                    res.Add(r);
                }
            });

            if (res.Where(r => !r.isAlive && r.isUsed).ToList().Count == res.Count)
            {
                UIBattleManager.Instance.Lose.gameObject.SetActive(true);
                CurrentBattleState = BattleState.End;
                return;
            }

            if (res.Where(r => r.isUsed).ToList().Count == res.Where(r => !r.isAlive).ToList().Count)
            {
                UIBattleManager.Instance.StartTimeUp(15, side);
            }
        }
    }

    #endregion

    #region Loading_Selection Character

    public void ArrivingComplete()
    {

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

    public void LoadingNewCharacterToGrid(CharacterNameType cName, SideType side, ControllerType playerController)
    {
        if (CurrentBattleState != BattleState.Battle)
        {
            return;
        }

        CharacterType_Script currentCharacter = (CharacterType_Script)AllCharactersOnField.Where(r=> r.CharInfo.CharacterID == cName && r.UMS.Side == side).First();
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
        // yield return HoldPressTimer(playerController);
        //Debug.Log(CurrentSelectedCharacters[playerController].OffsetSwap + "    " + Time.time);
        CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = true;
        while (CurrentSelectedCharacters[playerController].OffsetSwap > Time.time || !CurrentSelectedCharacters[playerController].Character.IsOnField ||
            CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk2_AtkToIdle || CurrentSelectedCharacters[playerController].Character.SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving)
        {
            //Debug.Log(val);
            yield return null;
        }
       
        if (CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar == AllCharactersOnField.Where(r=> r.CharInfo.CharacterID == cName &&
        CurrentSelectedCharacters[playerController].NextSelectionChar.Side == r.UMS.Side).First().CharInfo.CharacterSelection)
        {
            while (CurrentSelectedCharacters[playerController].Character.isMoving)
            {
                //Debug.Log(val + "   Moving");
                yield return null;
            }

            if(CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar == CurrentSelectedCharacters[playerController].Character.CharInfo.CharacterSelection)
            {
                //Debug.Log(val + "   same char");
                CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = false;
                yield break;
            }
            if (!CurrentSelectedCharacters[playerController].Character.IsSwapping)
            {
                Vector2Int spawnPos = CurrentSelectedCharacters[playerController].Character.UMS._CurrentTilePos;
                CharacterType_Script currentCharacter = SetCharOnBoardOnFixedPos(playerController, cName, spawnPos);
                //Debug.Log("Exit  " + CurrentSelectedCharacters[playerController].OffsetSwap + "    " + Time.time + CurrentSelectedCharacters[playerController].NextSelectionChar + AllCharactersOnField.Where(r => r.CharInfo.CharacterID == cName).First().CharInfo.CharacterSelection);
                if (currentCharacter != null)
                {
                    Debug.Log(val + "   swapping char");
                    CurrentSelectedCharacters[playerController].Character.IsSwapping = true;

                    // currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", false);
                    currentCharacter.SpineAnim.SetAnimationSpeed(2);
                    yield return RemoveCharacterFromBaord(playerController, CurrentSelectedCharacters[playerController].Character, false);

                    SelectCharacter(playerController, currentCharacter);
                    CurrentSelectedCharacters[playerController].Character.IsSwapping = true;
                    // currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", true);
                }
            }
        }

        CurrentSelectedCharacters[playerController].Character.SwapWhenPossible = false;
        //And drop the new character in
        CurrentSelectedCharacters[playerController].LoadCharCo = null;
    }

    public void RemoveNamedCharacterFromBoard(CharacterNameType charToRemoveName)
    {
        /*if (CurrentSelectedCharacters.Values.Where(r => r.Character != null).FirstOrDefault() != null && CurrentSelectedCharacters.Values.Where(r => r.Character.CharInfo.CharacterID == charToRemoveName).FirstOrDefault() != null)
        {
            charToRemove = CurrentSelectedCharacters.Values.Where(r => r.Character.CharInfo.CharacterID == charToRemoveName).FirstOrDefault().Character;
        }*/
        BaseCharacter charToRemove = AllCharactersOnField.Where(r => r.CharInfo.CharacterID == charToRemoveName).FirstOrDefault();
        if (charToRemove == null) charToRemove = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == charToRemoveName).FirstOrDefault();
        Debug.Log("Character removed: " + charToRemove.CharInfo.CharacterID.ToString());
        ControllerType controller = ControllerType.None;
        if (CurrentSelectedCharacters.Values != null && CurrentSelectedCharacters.Where(r => r.Value.Character == charToRemove).FirstOrDefault().Key != ControllerType.None)
        {
            controller = CurrentSelectedCharacters.Where(r => r.Value.Character == charToRemove).FirstOrDefault().Key;
        }
        charToRemove.SpineAnim.SetAnimationSpeed(2);
        StartCoroutine(RemoveCharacterFromBaord(controller, charToRemove, true));

        /*
        CharacterType_Script charToRemove = null;
        ControllerType controller = ControllerType.None;
        charToRemove = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterID == charToRemoveName).FirstOrDefault();
        if (charToRemove == null) return;
        charToRemove.SpineAnim.SetAnimationSpeed(2);
        if (CurrentSelectedCharacters.Values.Where(r => r.Character != null).FirstOrDefault() != null && CurrentSelectedCharacters.Values.Where(r => r.Character.CharInfo.CharacterID == charToRemoveName).FirstOrDefault() != null)
        {
            controller = CurrentSelectedCharacters.Where(r => r.Value.Character == charToRemove).FirstOrDefault().Key;
        }
        StartCoroutine(RemoveCharacterFromBaord(controller, charToRemove));
         */
    }

    //Stop the loading of a char
    public void StopLoadingNewCharacter(CharacterNameType cName, ControllerType playerController)
    {
        if (CurrentCharactersLoadingInfo.Where(r=> r.CName == cName && r.PlayerController == playerController).ToList().Count > 0)
        {
            CurrentCharactersLoadingInfo.Remove(CurrentCharactersLoadingInfo.Where(r=> r.PlayerController == playerController && r.CName == cName).First());
        }
    }

    //Used to select a char under a determinated player
    public void SetCharacterSelection(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SelectCharacter(playerController, (CharacterType_Script)AllCharactersOnField.Where(r=> r.CharInfo.CharacterSelection == characterSelection && r.UMS.PlayerController.Contains(playerController)).FirstOrDefault());
    }

    public void DeselectCharacter(CharacterNameType charToDeselectName, SideType side, ControllerType playerController)
    {


        /*bool isSelected = false;
        foreach (KeyValuePair<ControllerType, CurrentSelectedCharacterClass> characterSelector in CurrentSelectedCharacters)
        {
            if (characterSelector.Value.Character != null && characterSelector.Value.Character.CharInfo.CharacterID == charToDeselectName)
            {
                isSelected = true;
                break;
            }
        }
        if (!isSelected)
        {
            return;
        }
        
        CharacterType_Script charToDeselect = CurrentSelectedCharacters.Values.Where(r => r.Character != null && r.Character.CharInfo.CharacterID == charToDeselectName).FirstOrDefault().Character;
        if (CurrentSelectedCharacters.Values.Where(r => r.Character == charToDeselect).FirstOrDefault() == null)
        {
            return;

        }
        ControllerType controller = CurrentSelectedCharacters.Where(r => r.Value.Character == charToDeselect).FirstOrDefault().Key;
        if (charToDeselect != null)
        {
            charToDeselect.SetCharSelected(false, ControllerType.None);
            //CurrentSelectedCharacters[controller].Character = null;
        }
        else
        {
            Debug.LogWarning("Character you are trying to deselect does not exist in the currently selected character");
        }*/


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
            if (CurrentSelectedCharacters.Where(r=> r.Value.Character == currentCharacter).ToList().Count == 0)
            {
                //If the player already has a character selected
                if(CurrentSelectedCharacters[playerController].Character != null)
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
                    }
                }
                //Change this player's character to the new character
                CurrentSelectedCharacters[playerController].Character = currentCharacter;
                currentCharacter.UMS.SetBattleUISelection(playerController);
                currentCharacter.UMS.IndicatorAnim.SetBool("indicatorOn", true);

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
            SetCharOnBoardOnRandomPos(playerController, cName);
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
            SetCharOnBoardOnRandomPos(playerController, cName);
        }

        yield return null;
    }

    public void UpdateCurrentSelectedCharacters(CharacterType_Script oldChar, CharacterType_Script newChar)
    {
        CurrentSelectedCharacters.Where(r => r.Value.Character == oldChar).First().Value.Character = newChar;
    }

    //Handle the wait for a button hold (relating to spawning in)
    IEnumerator HoldPressTimer(ControllerType playerController)
    {
        //TODO Setup animation for the UI
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return PauseUntil();
            if (CurrentSelectedCharacters[playerController].Character != null)
            {
                while (CurrentSelectedCharacters[playerController].Character.isMoving)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            timer += Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Move Character

    //Move selected char under determinated player
    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirection dir)
    {
        if (CurrentBattleState != BattleState.Battle && CurrentBattleState != BattleState.Intro) return;

        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            if(CurrentSelectedCharacters[playerController].Character.UMS.UnitBehaviour == UnitBehaviourType.ControlledByPlayer)
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

    public void Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SideType side = GetSideFromPlayer(new List<ControllerType> { playerController });
        BaseCharacter cb = new BaseCharacter();
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
            if (CurrentBattleState == BattleState.Battle)
            {
                cb = null;
                CharacterSelectionType cs;
                bool Deselction = true;
                if (CurrentSelectedCharacters[playerController].Character == null)
                {
                    cs = CharacterSelectionType.Up;
                    Deselction = false;
                }
                else
                {
                    cs = CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar;
                }

                for (int i = 0; i < AllCharactersOnField.Count; i++)
                {
                    cs = cs + (characterSelection == CharacterSelectionType.Left ? -1 : 1);
                    int maxChars = AllCharactersOnField.Count > 4 ? 4 : AllCharactersOnField.Count;
                    cs = (int)cs >= maxChars ? 0 : cs < 0 ? ((CharacterSelectionType)maxChars - 1) : cs;
                    string t = cs.ToString();
                    //Debug.Log(t);
                    cb = AllCharactersOnField.Where(r => r.gameObject.activeInHierarchy && r.CharInfo.CharacterSelection == cs && r.UMS.Side == side && r.CharInfo.HealthPerc > 0).FirstOrDefault();
                    if (cb != null)
                    {
                        t = cb.CharInfo.CharacterID.ToString() + "    " + cb.UMS.Side.ToString();
                    }
                    //Debug.Log(t);
                    if (cb != null && CurrentSelectedCharacters.Where(r => r.Value.Character != null && ((r.Value.Character == cb)
                    || (r.Value.NextSelectionChar.NextSelectionChar == cs && r.Value.NextSelectionChar.Side == cb.UMS.Side && r.Value.Character != null)) && r.Key != playerController).ToList().Count == 0)
                    {
                        CharacterType_Script PrevCharacter = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar && r.UMS.Side == side).First();

                        if (Deselction)
                        {
                            DeselectCharacter(PrevCharacter.CharInfo.CharacterID, side, playerController);
                        }
                        //Debug.Log("Prev " + CurrentSelectedCharacters[playerController].NextSelectionChar.ToString());
                        CurrentSelectedCharacters[playerController].NextSelectionChar = new NextSelectionCharClass(cs, side);
                        //Debug.Log(cs.ToString());
                        break;
                    }
                }
                /*

                if (CurrentSelectedCharacters[playerController].Character == null)
                {
                    cb = AllCharactersOnField.Where(r=>r.gameObject.activeInHierarchy && !r.IsOnField && !r.IsSwapping && r.UMS.Side == side).FirstOrDefault();
                    if(cb != null)
                    {
                        CurrentSelectedCharacters[playerController].NextSelectionChar = new NextSelectionCharClass(cb.CharInfo.CharacterSelection, cb.UMS.Side);
                    }
                }
                else
                {
                  
                    CharacterSelectionType cs = CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar;
                    for (int i = 0; i < AllCharactersOnField.Count; i++)
                    {
                        cs = cs + (characterSelection == CharacterSelectionType.Left ? -1 : 1);
                        int maxChars = AllCharactersOnField.Count > 4 ? 4 : AllCharactersOnField.Count;
                        cs = (int)cs >= maxChars ? 0 : cs < 0 ? ((CharacterSelectionType)maxChars - 1) : cs;
                        string t = cs.ToString();
                        Debug.Log(t);
                        cb = AllCharactersOnField.Where(r =>r.gameObject.activeInHierarchy && r.CharInfo.CharacterSelection == cs && r.UMS.Side == side && r.CharInfo.HealthPerc > 0).FirstOrDefault();
                        if(cb != null)
                        {
                            t = cb.CharInfo.CharacterID.ToString() + "    " + cb.UMS.Side.ToString();
                        }
                        Debug.Log(t);
                        if (cb != null && CurrentSelectedCharacters.Where(r => r.Value.Character != null && ((r.Value.Character == cb) || ( r.Value.NextSelectionChar.NextSelectionChar == cs && r.Value.NextSelectionChar.Side == cb.UMS.Side)) && r.Key != playerController).ToList().Count == 0)
                        {
                            CharacterType_Script PrevCharacter = (CharacterType_Script)AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == CurrentSelectedCharacters[playerController].NextSelectionChar.NextSelectionChar && r.UMS.Side == side).First();
                            DeselectCharacter(PrevCharacter.CharInfo.CharacterID, side, playerController);
                            //Debug.Log("Prev " + CurrentSelectedCharacters[playerController].NextSelectionChar.ToString());
                            CurrentSelectedCharacters[playerController].NextSelectionChar = new NextSelectionCharClass(cs, side);
                            //Debug.Log(cs.ToString());
                            break;
                        }
                    }
                }*/
                if(cb != null)
                {
                    LoadingNewCharacterToGrid(cb.CharInfo.CharacterID, side, playerController);
                }
            }
        }
    }

    public void StopChargingAttack(ControllerType controllerType)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                CurrentSelectedCharacters[controllerType].Character.isSpecialLoading = false;
            }
        }
    }

    public void StartChargingAttack(ControllerType controllerType)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                StartCoroutine(CurrentSelectedCharacters[controllerType].Character.StartChargingAttack());
            }
        }
    }
    public void StartQuickAttack(ControllerType controllerType)
    {
        if (CurrentBattleState == BattleState.Battle)
        {
            if (CurrentSelectedCharacters.ContainsKey(controllerType) && CurrentSelectedCharacters[controllerType] != null && CurrentSelectedCharacters[controllerType].Character != null)
            {
                CurrentSelectedCharacters[controllerType].Character.StartQuickAttack(false);
            }
        }
    }

    public void CurrentCharacterStartDefending(ControllerType playerController)
    {
        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            CurrentSelectedCharacters[playerController].Character.StartDefending();
        }
    }

    public void CurrentCharacterStopDefending(ControllerType playerController)
    {
        if (CurrentSelectedCharacters[playerController].Character != null)
        {
            CurrentSelectedCharacters[playerController].Character.StopDefending();
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
        Destroy(rC.GetComponent<MinionType_Script>());
        CharacterType_Script recruitableChar = rC.AddComponent<CharacterType_Script>();
        AllCharactersOnField.Add(recruitableChar);
        recruitableChar.UMS = recruitableChar.GetComponent<UnitManagementScript>();
        recruitableChar.UMS.CurrentAttackType = AttackType.Particles;
        recruitableChar.UMS.CharOwner = recruitableChar;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == recruitableChar.CharInfo.CharacterID).First();
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            recruitableChar.UMS.Pos.Add(item);
        }
        recruitableChar.CharInfo.CharacterLevel = CharacterLevelType.Godness;
        recruitableChar.UMS.Facing = FacingType.Right;
        recruitableChar.UMS.isAIOn = false;
        recruitableChar.UMS.PlayerController = AllCharactersOnField[0].UMS.PlayerController;
        recruitableChar.UMS.Side = SideType.LeftSide;
        recruitableChar.UMS.UnitBehaviour = UnitBehaviourType.ControlledByPlayer;
        recruitableChar.UMS.WalkingSide = WalkingSideType.LeftSide;
        recruitableChar.UMS.CurrentAttackType = AttackType.Particles;
        recruitableChar.CharInfo.CharacterSelection = (CharacterSelectionType)AllCharactersOnField.Count - 1;
        NewIManager.Instance.SetUICharacterToButton(recruitableChar, recruitableChar.CharInfo.CharacterSelection);
        recruitableChar.CharInfo.HealthStats.Health = recruitableChar.CharInfo.HealthStats.Base;
        recruitableChar.gameObject.SetActive(true);
        recruitableChar.SetupCharacterSide();
        PlayablesCharOnScene.Add(new PlayableCharOnScene(recruitableChar.CharInfo.CharacterID, AllCharactersOnField[0].UMS.PlayerController, false, GetSideFromPlayer(recruitableChar.UMS.PlayerController)));
        /*foreach (BaseCharacter playableCharOnScene in AllCharactersOnField)
        {
            NewIManager.Instance.SetUICharacterToButton((CharacterType_Script)playableCharOnScene, playableCharOnScene.CharInfo.CharacterSelection);
        }*/


        recruitableChar.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        SetUICharacterSelectionIcons();
    }

    public void RestartScene()
    {
        if(CurrentBattleState == BattleState.Pause)
        {
            CurrentBattleState = BattleState.End;
            UnityEngine.SceneManagement.SceneManager.LoadScene("PaxMainMenu");
            //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            EventManager.Instance.ResetEventsInManager();
        }
    }


    public BaseCharacter GetCharInPos(Vector2Int pos)
    {
        BaseCharacter cb = AllCharactersOnField.Where(r => r.IsOnField && r.UMS.Pos.Contains(pos)).FirstOrDefault();

        if(cb == null)
        {
            cb = WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.UMS.Pos.Contains(pos)).FirstOrDefault();
        }

        return cb;
    }

   //Used to setup all the current char icons
    public void SetUICharacterSelectionIcons()
    {
        List<UIIconClass> resLeft = new List<UIIconClass>();
        List<UIIconClass> resRight = new List<UIIconClass>();

        foreach (BaseCharacter item in AllCharactersOnField)
        {
            if(item.UMS.Side == SideType.RightSide)
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
        }
        return SideType.LeftSide;
    }


    public IEnumerator PauseUntil()
    {
        yield return new WaitForFixedUpdate();

        while (CurrentBattleState == BattleState.Pause)
        {
            yield return new WaitForFixedUpdate();
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
    public CharacterType_Script Character;
    public IEnumerator LoadCharCo;
    public NextSelectionCharClass NextSelectionChar;
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