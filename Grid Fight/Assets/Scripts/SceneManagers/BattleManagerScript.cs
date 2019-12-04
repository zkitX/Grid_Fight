using System.Collections;
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
            if(CurrentBattleStateChangedEvent != null)
            {
                CurrentBattleStateChangedEvent(value);
            }
            _CurrentBattleState = value;
        }
    }


    public static BattleManagerScript Instance;
    public BattleState _CurrentBattleState;
    public List<BattleTileScript> OccupiedBattleTiles = new List<BattleTileScript>();
    public GameObject CharacterBasePrefab;
    public Dictionary<ControllerType, CharacterBase> CurrentSelectedCharacters = new Dictionary<ControllerType, CharacterBase>();
    public List<ScriptableObjectCharacterPrefab> ListOfScriptableObjectCharacterPrefab = new List<ScriptableObjectCharacterPrefab>();
    public List<CharacterBase> AllCharactersOnField = new List<CharacterBase>();
    public List<CharacterLoadingInfoClass> CurrentCharactersLoadingInfo = new List<CharacterLoadingInfoClass>();
    private IEnumerator CharacterLoadingCo;
    [SerializeField]
    private Transform CharactersContainer;
    private List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
    private List<PlayableCharOnScene> PlayablesCharOnScene = new List<PlayableCharOnScene>();

    public List<CharacterBase> WaveCharcters = new List<CharacterBase>();
    private MatchType matchType;
    public void SetupBattleState()
    {

        CurrentBattleState = BattleState.Battle;
       UIBattleManager.Instance.StartMatch.gameObject.SetActive(false);
    }

    #region Unity Life Cycle
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update

    #endregion

    #region Events

    #endregion

    #region Waves

    public CharacterBase GetWaveCharacter(CharacterNameType characterName, Transform parent)
    {
        CharacterBase res;
        res = WaveCharcters.Where(r => r.CharInfo.CharacterName == characterName && !r.IsOnField).FirstOrDefault();
        if(res == null)
        {
            res = CreateChar(new CharacterBaseInfoClass(characterName.ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, ControllerType.Enemy, characterName), parent);
        }

        return res;
    }
    #endregion

    #region SetCharacterOnBoard
    //Used to set the already created char on a random Position in the battlefield
    public void SetCharOnBoardOnRandomPos(ControllerType playerController, CharacterNameType cName)
    {
        CharacterBase currentCharacter = AllCharactersOnField.Where(r=> r.UMS.PlayerController == playerController && r.CharInfo.CharacterName == cName).First();
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(GridManagerScript.Instance.GetSideTypeFromControllerType(playerController), currentCharacter.UMS.Pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
        if(!CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters.Add(playerController, currentCharacter);
        }
       
        SelectCharacter(playerController, currentCharacter);

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);
        currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
        StartCoroutine(MoveCharToBoardWithDelay(0.1f, currentCharacter, bts.transform.position));
        if (playerController == ControllerType.Player1)
        {
            UIBattleManager.Instance.isPlayerPlayingP1 = true;

        }
        else if (playerController == ControllerType.Player2)
        {
            UIBattleManager.Instance.isPlayerPlayingP2 = true;

        }
        PlayablesCharOnScene.Where(r => r.PlayerController == playerController && r.CName == cName).First().isUsed = true; ;
    }
    
   

    //Used to set the already created char on a fixed Position in the battlefield
    public void SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterNameType cName, Vector2Int pos)
    {
        CharacterBase currentCharacter = AllCharactersOnField.Where(r => r.UMS.PlayerController == playerController && r.CharInfo.CharacterName == cName).First();
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
        if (!CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters.Add(playerController, currentCharacter);
        }

        SelectCharacter(playerController, currentCharacter);

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);
        currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
        StartCoroutine(MoveCharToBoardWithDelay(0.2f, currentCharacter, bts.transform.position));
    }

    public IEnumerator MoveCharToBoardWithDelay(float delay, CharacterBase cb, Vector3 nextPos)
    {
        float timer = 0;
        while (timer <= delay)
        {
            yield return new WaitForFixedUpdate();
            while (CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            timer += Time.fixedDeltaTime;
        }

        cb.transform.position = nextPos;
    }

    #endregion

    #region Create Character

    public IEnumerator InstanciateAllChar(float delay)
    {
        PlayerBattleInfo = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.PlayerBattleInfo : BattleInfoManagerScript.Instance.PlayerBattleInfo;
        foreach (CharacterBaseInfoClass item in PlayerBattleInfo)
        {
            PlayablesCharOnScene.Add(new PlayableCharOnScene(item.CharacterName, item.PlayerController, false));
            AllCharactersOnField.Add(CreateChar(item, CharactersContainer));
            yield return new WaitForSeconds(delay);
        }

        SetUICharacterSelectionIcons();
    }
//Creation of the character with the basic info
    public CharacterBase CreateChar(CharacterBaseInfoClass charInfo, Transform parent)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CharacterName == charInfo.CharacterName).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100, 100, 100), Quaternion.identity, parent);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.UMS.PlayerController = charInfo.PlayerController;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.UMS.Pos.Add(item);
        }
        currentCharacter.SetupCharacterSide();
        currentCharacter.CharInfo.CharacterSelection = charInfo.CharacterSelection;
        currentCharacter.SelectionIndicator.eulerAngles = new Vector3(0,0, currentCharacter.CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
            currentCharacter.CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
            currentCharacter.CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
        currentCharacter.CharInfo.CharacterSelection = charInfo.CharacterSelection;
        currentCharacter.CurrentCharIsDeadEvent += CurrentCharacter_CurrentCharIsDeadEvent;
        return currentCharacter;
    }

    private void CurrentCharacter_CurrentCharIsDeadEvent(CharacterNameType cName, ControllerType playerController)
    {
        PlayablesCharOnScene.Where(r=> r.PlayerController == playerController && r.CName == cName).First().isAlive = false;

        List<PlayableCharOnScene> res = new List<PlayableCharOnScene>();

        res = PlayablesCharOnScene.Where(r => r.PlayerController == playerController).ToList();

        if(res.Where(r=> !r.isAlive && r.isUsed).ToList().Count == res.Count)
        {
            UIBattleManager.Instance.Winner(playerController == ControllerType.Player1 ? "Lost" : "Win", playerController == ControllerType.Player2 ? "Lost" : "Win");
            CurrentBattleState = BattleState.End;
            return;
        }

        if(res.Where(r=> r.isUsed).ToList().Count == res.Where(r=> !r.isAlive).ToList().Count)
        {
            UIBattleManager.Instance.StartTimeUp(15, playerController);
        }

    }

    #endregion

    #region Loading_Selection Character

    //Used when the char is not in the battlefield to move it on the battlefield
    public void LoadingNewCharacterInRandomPosition(CharacterNameType cName,SideType side, ControllerType playerController)
    {
        if(!AllCharactersOnField.Where(r=> r.UMS.Side == side && r.CharInfo.CharacterName == cName).First().IsOnField)
        {
            CharacterLoadingCo = CharacterLoadingInRandomPosition(cName, playerController);
            CurrentCharactersLoadingInfo.Add(new CharacterLoadingInfoClass(cName, playerController, CharacterLoadingCo));
            StartCoroutine(CharacterLoadingCo);
        }
        else
        {
            SelectCharacter(playerController, AllCharactersOnField.Where(r => r.UMS.Side == side && r.CharInfo.CharacterName == cName).First());
        }
    }

    //Stop the loading of a char
    public void StopLoadingNewCharacter(CharacterNameType cName, ControllerType playerController)
    {
        if (CurrentCharactersLoadingInfo.Where(r=> r.CName == cName && r.PlayerController == playerController).ToList().Count > 0)
        {
            StopCoroutine(CharacterLoadingCo);
        }
    }

    //Used to select a char under a determinated player
    public void SetCharacterSelection(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SelectCharacter(playerController, AllCharactersOnField.Where(r=> r.CharInfo.CharacterSelection == characterSelection && r.UMS.PlayerController == playerController).FirstOrDefault());
    }

    //Used to select a char 
    public void SelectCharacter(ControllerType playerController, CharacterBase currentCharacter)
    {
        if(currentCharacter != null)
        {
            CurrentSelectedCharacters[playerController].SetCharSelected(false);
            CurrentSelectedCharacters[playerController] = currentCharacter;
            UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);
            currentCharacter.SetCharSelected(true);
        }
    }

    //Load char in a random pos
    private IEnumerator CharacterLoadingInRandomPosition(CharacterNameType cName, ControllerType playerController)
    {
        //TODO Setup animation for the UI
        float timer = 0;
        while (timer <= 1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            timer += Time.fixedDeltaTime;
        }
        SetCharOnBoardOnRandomPos(playerController, cName);
    }
    //Load char in a fixed pos
    private IEnumerator CharacterLoadingInFixedPosition(CharacterNameType cName, ControllerType playerController, Vector2Int pos)
    {
        //TODO Setup animation for the UI
        float timer = 0;
        while (timer <= 1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            timer += Time.fixedDeltaTime;
        }
        SetCharOnBoardOnFixedPos(playerController, cName, pos);
    }

    #endregion

    #region Move Character

    //Move selected char under determinated player
    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirection dir)
    {
        if (CurrentSelectedCharacters.ContainsKey(playerController))
        {
            if(CurrentSelectedCharacters[playerController].UMS.UnitBehaviour == UnitBehaviourType.ControlledByPlayer)
            {
                CurrentSelectedCharacters[playerController].MoveCharOnDirection(dir);

            }
        }
    }

    #endregion

    #region Switch Input
    public void Switch_StopLoadingNewCharacter(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        SideType side = GetSideFromPlayer(controllerType);
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == characterSelection && r.UMS.Side == side).FirstOrDefault();
        if (cb != null)
        {
            StopLoadingNewCharacter(cb.CharInfo.CharacterName, controllerType);
        }

    }

    public void Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        SideType side = GetSideFromPlayer(controllerType);
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharInfo.CharacterSelection == characterSelection && r.UMS.Side == side).FirstOrDefault();
        if (cb != null)
        {
            LoadingNewCharacterInRandomPosition(cb.CharInfo.CharacterName, side, controllerType);
        }

    }

    public void Switch_StopLoadingSpecial(ControllerType controllerType)
    {
        CurrentSelectedCharacters[controllerType].isSpecialLoading = false;

    }

    public void Switch_LoadingSpecial(ControllerType controllerType)
    {
        StartCoroutine(CurrentSelectedCharacters[controllerType].LoadSpecialAttack());
    }

    #endregion

    #region Mobile Input
    public void Mobile_StopLoadingNewCharacter(CharacterNameType cName, ControllerType playerController)
    {
        StopLoadingNewCharacter(cName, playerController);
    }

    public void Mobile_LoadingNewCharacterInRandomPosition(CharacterNameType cName, ControllerType playerController)
    {
        LoadingNewCharacterInRandomPosition(cName, GetSideFromPlayer(playerController), playerController);
    }

    #endregion




   //Used to setup all the current char icons
    public void SetUICharacterSelectionIcons()
    {
        List<UIIconClass> resLeft = new List<UIIconClass>();
        List<UIIconClass> resRight = new List<UIIconClass>();

        foreach (CharacterBase item in AllCharactersOnField)
        {
            if(item.UMS.Side == SideType.RightSide)
            {
                resRight.Add(new UIIconClass(item.CharInfo.CharacterIcon, item.CharInfo.CharacterSelection));
            }
            else 
            {
                resLeft.Add(new UIIconClass(item.CharInfo.CharacterIcon, item.CharInfo.CharacterSelection));
            }
        }

        UIBattleManager.Instance.UICharacterSelectionLeft.SetupCharacterIcons(resLeft);
        UIBattleManager.Instance.UICharacterSelectionRight.SetupCharacterIcons(resRight);
    }

//
    public SideType GetSideFromPlayer(ControllerType ct)
    {
        matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                return ct == ControllerType.Player1 ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PvP:
                return ct == ControllerType.Player1 ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PPvE:
                return ct == ControllerType.Player1 || ct == ControllerType.Player1 ? SideType.LeftSide : SideType.RightSide;
            case MatchType.PPvPP:
                return ct == ControllerType.Player1 || ct == ControllerType.Player1 ? SideType.LeftSide : SideType.RightSide;
        }

        return SideType.LeftSide;
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
    public ControllerType PlayerController;
    public bool isUsed;
    public bool isAlive = true;

    public PlayableCharOnScene(CharacterNameType cname, ControllerType pc, bool isused)
    {
        CName = cname;
        PlayerController = pc;
        isUsed = isused;

    }
}




