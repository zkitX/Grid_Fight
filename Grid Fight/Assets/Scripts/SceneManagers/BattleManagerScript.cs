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
    public void SetupBattleState()
    {
        CurrentBattleState = BattleState.Battle;
    }

    #region Unity Life Cycle
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    #endregion

    #region Events

    #endregion 

    #region SetCharacterOnBoard_WorldPosition

    public void SetCharOnBoardOnRandomPos(ControllerType playerController, CharacterType ct)
    {
        CharacterBase currentCharacter = AllCharactersOnField.Where(r=> r.UMS.PlayerController == playerController && r.CharacterInfo.CT == ct).First();
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

    public void SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterType ct, Vector2Int pos)
    {
        CharacterBase currentCharacter = AllCharactersOnField.Where(r => r.UMS.PlayerController == playerController && r.CharacterInfo.CT == ct).First();
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos, GridManagerScript.Instance.GetSideTypeFromControllerType(playerController));
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        currentCharacter.transform.position = bts.transform.position;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
    }

    #endregion

    #region Create Character

    public IEnumerator InstanciateAllChar(float delay)
    {
        PlayerBattleInfo = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.PlayerBattleInfo : BattleInfoManagerScript.Instance.PlayerBattleInfo;
        foreach (CharacterBaseInfoClass item in PlayerBattleInfo)
        {
            AllCharactersOnField.Add(CreateChar(item.CT, item.playerController));
            yield return new WaitForSeconds(delay);
        }

        SetUICharacterSelectionIcons();
    }

    public CharacterBase CreateChar(CharacterType ct, ControllerType playerController)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100,100,100), Quaternion.identity, CharactersContainer);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.CharacterInfo = PlayerBattleInfo.Where(r => r.CT == ct).First();
        currentCharacter.UMS.PlayerController = playerController;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.UMS.Pos.Add(item);
        }
        currentCharacter.SetupCharacterSide();
        currentCharacter.SelectionIndicator.eulerAngles = new Vector3(0,0, currentCharacter.CharacterInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
            currentCharacter.CharacterInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
            currentCharacter.CharacterInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
        return currentCharacter;
    }

    #endregion

    #region Loading_Selection Character
    public void LoadingNewCharacterInRandomPosition(CharacterType ct,SideType side, ControllerType playerController)
    {
        if(!AllCharactersOnField.Where(r=> r.UMS.Side == side && r.CharacterInfo.CT == ct).First().IsOnField)
        {
            CharacterLoadingCo = CharacterLoadingInRandomPosition(ct, playerController);
            CurrentCharactersLoadingInfo.Add(new CharacterLoadingInfoClass(ct, playerController, CharacterLoadingCo));
            StartCoroutine(CharacterLoadingCo);
        }
        else
        {
            SelectCharacter(playerController, AllCharactersOnField.Where(r => r.UMS.Side == side && r.CharacterInfo.CT == ct).First());
        }
    }

    public void StopLoadingNewCharacter(CharacterType ct, ControllerType playerController)
    {
        if (CurrentCharactersLoadingInfo.Where(r=> r.Ct == ct && r.PlayerController == playerController).ToList().Count > 0)
        {
            StopCoroutine(CharacterLoadingCo);
        }
    }
    public void SetCharacterSelection(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SelectCharacter(playerController, AllCharactersOnField.Where(r=> r.CharacterInfo.CharacterSelection == characterSelection && r.UMS.PlayerController == playerController).FirstOrDefault());
    }

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

    private IEnumerator CharacterLoadingInRandomPosition(CharacterType ct, ControllerType playerController)
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
        SetCharOnBoardOnRandomPos(playerController, ct);
    }

    #endregion

    #region Move Character
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
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharacterInfo.CharacterSelection == characterSelection && r.UMS.Side == side).FirstOrDefault();
        if (cb != null)
        {
            StopLoadingNewCharacter(cb.CharacterInfo.CT, controllerType);
        }

    }

    public void Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        SideType side = GetSideFromPlayer(controllerType);
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharacterInfo.CharacterSelection == characterSelection && r.UMS.Side == side).FirstOrDefault();
        if (cb != null)
        {
            LoadingNewCharacterInRandomPosition(cb.CharacterInfo.CT, side, controllerType);
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
    public void Mobile_StopLoadingNewCharacter(CharacterType ct, ControllerType playerController)
    {
        StopLoadingNewCharacter(ct, playerController);
    }

    public void Mobile_LoadingNewCharacterInRandomPosition(CharacterType ct, ControllerType playerController)
    {
        LoadingNewCharacterInRandomPosition(ct,GetSideFromPlayer(playerController), playerController);
    }

    #endregion

    public void SetUICharacterSelectionIcons()
    {
        List<UIIconClass> resLeft = new List<UIIconClass>();
        List<UIIconClass> resRight = new List<UIIconClass>();

        foreach (CharacterBase item in AllCharactersOnField)
        {
            if(item.UMS.Side == SideType.RightSide)
            {
                resRight.Add(new UIIconClass(item.CharInfo.CharacterIcon, item.CharacterInfo.CharacterSelection));
            }
            else 
            {
                resLeft.Add(new UIIconClass(item.CharInfo.CharacterIcon, item.CharacterInfo.CharacterSelection));
            }
        }

        UIBattleManager.Instance.UICharacterSelectionLeft.SetupCharacterIcons(resLeft);
        UIBattleManager.Instance.UICharacterSelectionRight.SetupCharacterIcons(resRight);
    }


    public SideType GetSideFromPlayer(ControllerType ct)
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
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
    public CharacterType Ct;
    public ControllerType PlayerController;
    public IEnumerator LoadingNewCharacterCo;

    public CharacterLoadingInfoClass()
    {

    }

    public CharacterLoadingInfoClass(CharacterType ct, ControllerType playerController, IEnumerator loadingNewCharacterCo)
    {
        Ct = ct;
        PlayerController = playerController;
        LoadingNewCharacterCo = loadingNewCharacterCo;
    }
}







