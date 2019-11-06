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
    public GameObject BaseBullet;
    public List<CharacterBase> AllCharactersOnField = new List<CharacterBase>();

    public CharacterLoadingInfoClass CurrentCharacterLoadingInfo;
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
        CharacterBase currentCharacter = AllCharactersOnField.Where(r=> r.PlayerController == playerController && r.CharacterInfo.CT == ct).First();
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(GridManagerScript.Instance.GetSideTypeFromControllerType(playerController), currentCharacter.Pos);
        currentCharacter.PhysicalPosOnTile = bts.Pos;
        currentCharacter.transform.position = bts.transform.position;
        for (int i = 0; i < currentCharacter.Pos.Count; i++)
        {
            currentCharacter.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
        currentCharacter.IsOnField = true;
        currentCharacter.transform.GetChild(0).gameObject.SetActive(true);

        if(!CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters.Add(playerController, currentCharacter);
        }
        else
        {
            CurrentSelectedCharacters[playerController] = currentCharacter;
        }
        
        UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);
        
    }

    public void SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterType ct, Vector2Int pos)
    {
        CharacterBase currentCharacter = AllCharactersOnField.Where(r => r.PlayerController == playerController && r.CharacterInfo.CT == ct).First();
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos, GridManagerScript.Instance.GetSideTypeFromControllerType(playerController));
        currentCharacter.PhysicalPosOnTile = bts.Pos;
        currentCharacter.transform.position = bts.transform.position;
        for (int i = 0; i < currentCharacter.Pos.Count; i++)
        {
            currentCharacter.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
    }

    public void SetCharOnWorldPositionMovingToTile(ControllerType playerController, CharacterType ct, Vector3 worldpos, Vector2Int tilepos, float duration)
    {
        BattleTileScript battleTile = GridManagerScript.Instance.GetBattleTile(tilepos);
        CharacterBase Pchar = CreateCharOnWorldPos(ct, playerController, worldpos, battleTile);
        Pchar.MoveCharToTargetDestination(battleTile.transform.position, CharacterAnimationStateType.DashRight, duration);
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

    public CharacterBase CreateCharOnTile(CharacterType ct, ControllerType playerController, BattleTileScript bts)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab,bts.transform.position, Quaternion.identity, CharactersContainer);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.CharacterInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r=> r.CT == ct).First();
        currentCharacter.PhysicalPosOnTile = bts.Pos;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.Pos.Add(bts.Pos + item);
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(bts.Pos + item);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }
        
        currentCharacter.PlayerController = playerController;
        currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
        SelectCharacter(playerController, currentCharacter);
        currentCharacter.SetupCharacterSide();
        foreach (Vector2Int item in currentCharacter.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        
        return currentCharacter;
    }

    public CharacterBase CreateChar(CharacterType ct, ControllerType playerController)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, new Vector3(100,100,100), Quaternion.identity, CharactersContainer);
        GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.CharacterInfo = PlayerBattleInfo.Where(r => r.CT == ct).First();
        currentCharacter.PlayerController = playerController;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.Pos.Add(item);
        }
        currentCharacter.SetupCharacterSide();
        return currentCharacter;
    }

    public CharacterBase CreateCharOnRandomTile(CharacterType ct, ControllerType playerController)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(GridManagerScript.Instance.GetSideTypeFromControllerType(playerController), soCharacterPrefab.OccupiedTiles);
        if(bts != null)
        {
            characterBasePrefab = Instantiate(CharacterBasePrefab, bts.transform.position, Quaternion.identity, CharactersContainer);
            GameObject child = Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
            CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
            currentCharacter.CharacterInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r => r.CT == ct).First();
            currentCharacter.PhysicalPosOnTile = bts.Pos;
            foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
            {
                currentCharacter.Pos.Add(bts.Pos + item);
                BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(bts.Pos + item);
                currentCharacter.CurrentBattleTiles.Add(cbts);
            }

            currentCharacter.PlayerController = playerController;
            currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
            SelectCharacter(playerController, currentCharacter);
            currentCharacter.SetupCharacterSide();
            foreach (Vector2Int item in currentCharacter.Pos)
            {
                GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
            }

            return currentCharacter;
        }

        return null;
    }

    public CharacterBase CreateCharOnWorldPos(CharacterType ct, ControllerType playerController, Vector3 worldpos, BattleTileScript bts)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab, worldpos, Quaternion.identity, CharactersContainer);
        Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.CharacterInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r => r.CT == ct).First();
        currentCharacter.PhysicalPosOnTile = bts.Pos;
        foreach (Vector2Int item in soCharacterPrefab.OccupiedTiles)
        {
            currentCharacter.Pos.Add(bts.Pos + item);
            currentCharacter.CurrentBattleTiles.Add(GridManagerScript.Instance.GetBattleTile(bts.Pos + item));
        }
        currentCharacter.PlayerController = playerController;
        SelectCharacter(playerController, currentCharacter);
        currentCharacter.SetupCharacterSide();
        GridManagerScript.Instance.SetBattleTileState(bts.Pos, BattleTileStateType.Occupied);
        return currentCharacter;
    }

    #endregion

    #region Loading_Selection Character
    public void LoadingNewCharacterInRandomPosition(CharacterType ct,SideType side, ControllerType playerController)
    {
        if(CurrentCharacterLoadingInfo == null)
        {
            if(!AllCharactersOnField.Where(r=> r.Side == side && r.CharacterInfo.CT == ct).First().IsOnField)
            {
                CharacterLoadingCo = CharacterLoadingInRandomPosition(ct, playerController);
                CurrentCharacterLoadingInfo = new CharacterLoadingInfoClass(ct, playerController, CharacterLoadingCo);
                StartCoroutine(CharacterLoadingCo);
            }
            else
            {
                SelectCharacter(playerController, AllCharactersOnField.Where(r => r.Side == side && r.CharacterInfo.CT == ct).First());
            }
        }
    }

    public void StopLoadingNewCharacter(CharacterType ct, ControllerType playerController)
    {
        if (CurrentCharacterLoadingInfo != null && CurrentCharacterLoadingInfo.Ct == ct && CurrentCharacterLoadingInfo.PlayerController == playerController)
        {
            StopCoroutine(CharacterLoadingCo);
            CurrentCharacterLoadingInfo = null;
        }
    }
    public void SetCharacterSelection(CharacterSelectionType characterSelection, ControllerType playerController)
    {
        SelectCharacter(playerController, AllCharactersOnField.Where(r=> r.CharacterInfo.CharacterSelection == characterSelection && r.CharacterInfo.playerController == playerController).FirstOrDefault());
    }

    public void SelectCharacter(ControllerType playerController, CharacterBase currentCharacter)
    {
        if(currentCharacter != null)
        {
            CurrentSelectedCharacters[playerController] = currentCharacter;
            UIBattleManager.Instance.CharacterSelected(playerController, currentCharacter);
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
        CurrentCharacterLoadingInfo = null;
    }

    #endregion

    #region Move Character
    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirection dir)
    {
        if (CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters[playerController].MoveCharOnDirection(dir);
        }
    }

    #endregion

    #region Switch Input
    public void Switch_StopLoadingNewCharacter(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        SideType side = GetSideFromPlayer(controllerType);
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharacterInfo.CharacterSelection == characterSelection && r.Side == side).FirstOrDefault();
        if (cb != null)
        {
            StopLoadingNewCharacter(cb.CharacterInfo.CT, controllerType);
        }

    }

    public void Switch_LoadingNewCharacterInRandomPosition(CharacterSelectionType characterSelection, ControllerType controllerType)
    {
        SideType side = GetSideFromPlayer(controllerType);
        CharacterBase cb = AllCharactersOnField.Where(r => r.CharacterInfo.CharacterSelection == characterSelection && r.Side == side).FirstOrDefault();
        if (cb != null)
        {
            LoadingNewCharacterInRandomPosition(cb.CharacterInfo.CT, side, controllerType);
        }

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
            if(item.Side == SideType.RightSide)
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







