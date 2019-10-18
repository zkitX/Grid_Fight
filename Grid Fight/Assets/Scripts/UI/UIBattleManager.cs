using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleManager : MonoBehaviour
{

    public static UIBattleManager Instance;

    public UIPlayerSectionScript PlayerA;
    public UIPlayerSectionScript PlayerB;
    public UIPlayerSectionScript PlayerC;
    public UIPlayerSectionScript PlayerD;

    public UICharacterSelectionScript UICharacterSelectionLeft;
    public UICharacterSelectionScript UICharacterSelectionRight;

    private Dictionary<int, UIPlayerSectionScript> currentPlayers = new Dictionary<int, UIPlayerSectionScript>();
   
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        switch (BattleInfoManagerScript.Instance.MatchInfoType)
        {
            case MatchType.PvE:
                PlayerA.SetupPlayer(0);
                PlayerC.CurrentCanvasGroup.alpha = 0;
                PlayerD.CurrentCanvasGroup.alpha = 0;

                currentPlayers.Add(0, PlayerA);
                currentPlayers.Add(4, PlayerB);
                break;
            case MatchType.PvP:
                PlayerA.SetupPlayer(0);
                PlayerB.SetupPlayer(1);
                PlayerC.CurrentCanvasGroup.alpha = 0;
                PlayerD.CurrentCanvasGroup.alpha = 0;

                currentPlayers.Add(0, PlayerA);
                currentPlayers.Add(1, PlayerB);
                break;
            case MatchType.PPvE:
                PlayerA.SetupPlayer(0);
                PlayerC.SetupPlayer(1);
                PlayerD.CurrentCanvasGroup.alpha = 0;

                currentPlayers.Add(0, PlayerA);
                currentPlayers.Add(1, PlayerC);
                currentPlayers.Add(4, PlayerB);
                break;
            case MatchType.PPvPP:
                PlayerA.SetupPlayer(0);
                PlayerB.SetupPlayer(1);
                PlayerC.SetupPlayer(2);
                PlayerD.SetupPlayer(3);

                currentPlayers.Add(0, PlayerA);
                currentPlayers.Add(1, PlayerB);
                currentPlayers.Add(2, PlayerC);
                currentPlayers.Add(3, PlayerD);
                break;
        }
    }

    public void CharacterSelected(ControllerType playerController, CharacterBase currentCharacter)
    {
        currentPlayers[(int)playerController].SetSelectedCharacter(currentCharacter);
    }
}
