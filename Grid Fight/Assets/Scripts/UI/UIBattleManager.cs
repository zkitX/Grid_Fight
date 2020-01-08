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

    public GameObject TimeToPlayLeftSide;
    public TextMeshProUGUI SecondsToPlayLeftSide;
    public bool isLeftSidePlaying = false;

    public GameObject TimeToPlayRightSide;
    public TextMeshProUGUI SecondsToPlayRightSide;
    public bool isRightSidePlaying = false;
    public CanvasGroup Win;
    public CanvasGroup Lose;

   // public TextMeshProUGUI LeftSide;
 //   public TextMeshProUGUI RightSide;

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
                PlayerB.CurrentCanvasGroup.alpha = 0;
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
                PlayerB.CurrentCanvasGroup.alpha = 0;
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

    public void CharacterSelected(ControllerType playerController, CharacterType_Script currentCharacter)
    {
        currentPlayers[(int)playerController].SetSelectedCharacter(currentCharacter);
        if (GridManagerScript.Instance.GetSideTypeFromControllerType(playerController) == SideType.LeftSide)
        {
            UICharacterSelectionLeft.SetCharSelected(playerController, currentCharacter.CharInfo.CharacterSelection);
        }
        else
        {
            UICharacterSelectionRight.SetCharSelected(playerController, currentCharacter.CharInfo.CharacterSelection);
        }
    }


    public void StartTimeUp(float duration, SideType side)
    {
        if (side == SideType.LeftSide)
        {
            StartCoroutine(TimeUpCoP1(duration, side));
        }
        else if (side == SideType.RightSide)
        {
            StartCoroutine(TimeUpCoP2(duration, side));
        }
    }

    private IEnumerator TimeUpCoP1(float duration, SideType side)
    {
        TimeToPlayLeftSide.SetActive(true);
        isLeftSidePlaying = false;
        while (duration > 0 && !isLeftSidePlaying)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            SecondsToPlayLeftSide.text = ((int)duration).ToString();
            duration -= Time.fixedDeltaTime;

        }

        if(duration > 0 )
        {
            TimeToPlayLeftSide.SetActive(false);

            BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        }
        else
        {
            if (side == SideType.LeftSide)
            {
                Lose.gameObject.SetActive(true);
            }
            else
            {
                Win.gameObject.SetActive(true);
            }
            // Winner(side == SideType.LeftSide ? "Lost" : "Win", side == SideType.RightSide ? "Lost" : "Win");
            BattleManagerScript.Instance.CurrentBattleState = BattleState.End;
        }
    }

    private IEnumerator TimeUpCoP2(float duration, SideType side)
    {
        TimeToPlayRightSide.SetActive(true);
        isRightSidePlaying = false;
        while (duration > 0 && !isRightSidePlaying)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            SecondsToPlayRightSide.text = ((int)duration).ToString();
            duration -= Time.fixedDeltaTime;

        }

        if (duration > 0)
        {
            TimeToPlayRightSide.SetActive(false);

            BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
        }
        else
        {
            BattleManagerScript.Instance.CurrentBattleState = BattleState.End;
            if(side == SideType.LeftSide)
            {

            }
            else
            {

            }
           // Winner(side == SideType.LeftSide ? "Lost" : "Win", side == SideType.RightSide ? "Lost" : "Win");
        }
    }
 /*   public void Winner(string p1, string p2)
    {
        Win.alpha = 1;
       // LeftSide.text = p1;
      //  RightSide.text = p2;
    }*/
}
