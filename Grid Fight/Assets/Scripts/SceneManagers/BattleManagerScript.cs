using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManagerScript : MonoBehaviour
{
    public static BattleManagerScript Instance;
    public BattleState CurrentBattleState;
    public List<BattleTileScript> OccupiedBattleTiles = new List<BattleTileScript>();
    public GameObject CharacterBasePrefab;
    public Dictionary<ControllerType, CharacterBase> CurrentSelectedCharacters = new Dictionary<ControllerType, CharacterBase>();
    public List<ScriptableObjectCharacterPrefab> ListOfScriptableObjectCharacterPrefab = new List<ScriptableObjectCharacterPrefab>();
    [SerializeField]
    private Transform CharactersContainer;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GridManagerScript.Instance.InitializationCompleteEvent += Instance_InitializationCompleteEvent;
    }

    private void Instance_InitializationCompleteEvent()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player1, CharacterType.c1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player1, CharacterType.c2);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player2, CharacterType.c3);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player2, CharacterType.c4);
        }
        if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player3, CharacterType.c5);
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player3, CharacterType.c6);
        }
        if (Input.GetKeyUp(KeyCode.Alpha7))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player4, CharacterType.c7);
        }
        if (Input.GetKeyUp(KeyCode.Alpha8))
        {
            SetCharOnBoardOnRandomPos(ControllerType.Player4, CharacterType.c8);
        }

    }


    public void SetCharOnBoardOnRandomPos(ControllerType playerController, CharacterType ct)
    {
        BattleTileScript freeBattleTile = GridManagerScript.Instance.GetFreeBattleTile(true);
        CharacterBase Pchar = CreatePlayerChar(ct, playerController, freeBattleTile);
    }

    public void SetCharOnBoardOnFixedPos(ControllerType playerController, CharacterType ct, Vector2Int pos)
    {
        BattleTileScript battleTile = GridManagerScript.Instance.GetBattleTile(pos);
        CharacterBase Pchar = CreatePlayerChar(ct, playerController, battleTile);
    }

    public CharacterBase CreatePlayerChar(CharacterType ct, ControllerType playerController, BattleTileScript bts)
    {
        GameObject characterBasePrefab = null;
        ScriptableObjectCharacterPrefab soCharacterPrefab = ListOfScriptableObjectCharacterPrefab.Where(r => r.CT == ct).First();
        characterBasePrefab = Instantiate(CharacterBasePrefab,bts.transform.position, Quaternion.identity, CharactersContainer);
        Instantiate(soCharacterPrefab.CharacterPrefab, characterBasePrefab.transform.position, Quaternion.identity, characterBasePrefab.transform);
        CharacterBase currentCharacter = characterBasePrefab.GetComponent<CharacterBase>();
        currentCharacter.CharacterInfo = BattleInfoManagerScript.Instance.PlayerBattleInfo.Where(r=> r.CT == ct).First();
        currentCharacter.Pos = bts.Pos;
        currentCharacter.CurrentBattleTile = bts;
        currentCharacter.PlayerController = playerController;
        if(!CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters.Add(playerController, currentCharacter);
        }
        else
        {
            CurrentSelectedCharacters[playerController] = currentCharacter;
        }
        GridManagerScript.Instance.SetBattleTileState(bts.Pos, BattleTileStateType.Occupied);
        return currentCharacter;
    }

    public void MoveSelectedCharacterInDirection(ControllerType playerController, InputDirection dir)
    {
        if(CurrentSelectedCharacters.ContainsKey(playerController))
        {
            CurrentSelectedCharacters[playerController].MoveChar(dir);
        }
    }
}







