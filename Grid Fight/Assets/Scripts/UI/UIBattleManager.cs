using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBattleManager : MonoBehaviour
{

    public static UIBattleManager Instance;

    public UIPlayerSectionScript PlayerA;
    public UIPlayerSectionScript PlayerB;
    public UIPlayerSectionScript PlayerC;
    public UIPlayerSectionScript PlayerD;

    public UICharacterSelectionScript UICharacterSelectionLeft;
    public UICharacterSelectionScript UICharacterSelectionRight;

    public TextMeshProUGUI RestartMatch;
    public TextMeshProUGUI StartMatch;

    public GameObject TimeToPlayP1;
    public TextMeshProUGUI SecondsToPlayP1;
    public bool isPlayerPlayingP1 = false;

    public GameObject TimeToPlayP2;
    public TextMeshProUGUI SecondsToPlayP2;
    public bool isPlayerPlayingP2 = false;
    public CanvasGroup Win;

    public TextMeshProUGUI Player1;
    public TextMeshProUGUI Player2;

    private Dictionary<int, UIPlayerSectionScript> currentPlayers = new Dictionary<int, UIPlayerSectionScript>();
   
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;


        switch (matchType)
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
        if (GridManagerScript.Instance.GetSideTypeFromControllerType(playerController) == SideType.LeftSide)
        {
            UICharacterSelectionLeft.SetCharSelected(currentCharacter.CharInfo.CharacterSelection);
        }
        else
        {
            UICharacterSelectionRight.SetCharSelected(currentCharacter.CharInfo.CharacterSelection);
        }
    }


    public void StartTimeUp(float duration, ControllerType timeupPlayer)
    {
        if (timeupPlayer == ControllerType.Player1)
        {
            StartCoroutine(TimeUpCoP1(duration, timeupPlayer));
        }
        else if (timeupPlayer == ControllerType.Player2)
        {
            StartCoroutine(TimeUpCoP2(duration, timeupPlayer));
        }
      
    }

    private IEnumerator TimeUpCoP1(float duration, ControllerType timeupPlayer)
    {
        TimeToPlayP1.SetActive(true);
        isPlayerPlayingP1 = false;
        while (duration > 0 && !isPlayerPlayingP1)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            SecondsToPlayP1.text = ((int)duration).ToString();
            duration -= Time.fixedDeltaTime;

        }

        if(duration > 0 )
        {
            TimeToPlayP1.SetActive(false);

            BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        }
        else
        {
            Winner(timeupPlayer == ControllerType.Player1 ? "Lost" : "Win", timeupPlayer == ControllerType.Player2 ? "Lost" : "Win");
            BattleManagerScript.Instance.CurrentBattleState = BattleState.End;
        }
    }

    private IEnumerator TimeUpCoP2(float duration, ControllerType timeupPlayer)
    {
        TimeToPlayP2.SetActive(true);
        isPlayerPlayingP2 = false;
        while (duration > 0 && !isPlayerPlayingP2)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            SecondsToPlayP2.text = ((int)duration).ToString();
            duration -= Time.fixedDeltaTime;

        }

        if (duration > 0)
        {
            TimeToPlayP2.SetActive(false);

            BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        }
        else
        {
            Winner(timeupPlayer == ControllerType.Player1 ? "Lost" : "Win", timeupPlayer == ControllerType.Player2 ? "Lost" : "Win");
            BattleManagerScript.Instance.CurrentBattleState = BattleState.End;
        }
    }
    public void Winner(string p1, string p2)
    {
        Win.alpha = 1;

        Player1.text = p1;
        Player2.text = p2;
    }
}
