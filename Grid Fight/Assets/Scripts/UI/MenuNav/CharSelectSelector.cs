using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CharSelectSelector : MonoBehaviour
{
    public delegate void FungusEventTriggerAction(string blockName);
    public static event FungusEventTriggerAction OnFungusEventTrigger;

    public bool visible = false;
    [SerializeField] protected TextMeshProUGUI NameText;
    [SerializeField] protected TextMeshProUGUI SquadBonusText;
    [SerializeField] protected Grid_UIStarRanking levelDisplay = null;
    [SerializeField] protected Image Relations;
    [SerializeField] protected TextMeshProUGUI SelectText;
    [SerializeField] protected Animation SelectDisplay;
    [SerializeField] protected bool SelectDisplayed = false;
    [SerializeField] protected Animation ChatDisplay;
    [SerializeField] protected bool ChatDisplayed = false;

    protected CharacterNameType curSelectedChar = CharacterNameType.None;
    protected CharSelectButton curButton = null;

    public CharSelectBox parentBox = null;

    public AnimationClip popInAnim;
    public AnimationClip popOutAnim;

    public AnimationCurve travelCurve;

    private void Awake()
    {
        SelectDisplay.Play();
        ChatDisplay.Play();
        visible = true;
    }

    public void UpdateSelection(CharSelectButton btn, CharacterNameType charName, Vector3 selectionPosition, float duration = 0.25f)
    {
        curButton = btn;
        UpdateSelection(charName, selectionPosition, duration);
    }

    public void UpdateSelection(CharacterNameType charName, Vector3 selectionPosition, float duration = 0.25f)
    {
        if (SelectionUpdater != null) StopCoroutine(SelectionUpdater);
        SelectionUpdater = UpdateSelection_Co(charName, selectionPosition, duration);
        StartCoroutine(SelectionUpdater);
    }

    bool moving = false;
    IEnumerator SelectionUpdater = null;
    IEnumerator UpdateSelection_Co(CharacterNameType charName, Vector3 selectionPosition, float duration)
    {
        moving = true;
        CharacterLoadInformation loadInfo = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == charName).FirstOrDefault();
        curSelectedChar = charName;
        CharSelectBox.Instance.lastSelectedChar = curSelectedChar;

        if (SceneLoadManager.Instance.squad.Values.Where(r => r.characterID == curSelectedChar).FirstOrDefault() == null)
        {
            SelectText.text = "SELECT";
        }
        else
        {
            SelectText.text = "DESELECT";
        }

        if(parentBox.selectionMode == CharSelectBox.SelectionMode.Units)
        {
            SelectText.text = "MASK";
        }

        if (ChatDisplayed != (loadInfo != null && loadInfo.availableChats.Count != 0 && loadInfo.encounterState == CharacterLoadInformation.EncounterState.Recruited && parentBox.selectionMode == CharSelectBox.SelectionMode.Units))
        {
            ChatDisplayed = !ChatDisplayed;
            DisplayOption(ChatDisplay, ChatDisplayed);
        }

        if (SelectDisplayed != (loadInfo != null && loadInfo.encounterState == CharacterLoadInformation.EncounterState.Recruited && (charName != CharacterNameType.CleasTemple_Character_Valley_Donna || parentBox.selectionMode == CharSelectBox.SelectionMode.Units) /*&& parentBox.selectionMode == CharSelectBox.SelectionMode.Squad*/))
        {
            SelectDisplayed = !SelectDisplayed;
            DisplayOption(SelectDisplay, SelectDisplayed);
        }

        if(levelDisplay != null && loadInfo != null)
        {
            levelDisplay.SetStarRanking((float)((float)loadInfo.Level / (float)4f));
        }

        if (loadInfo == null || loadInfo.encounterState == CharacterLoadInformation.EncounterState.Hidden)
        {
            NameText.text = "???"; //should be fancier in the future
            SquadBonusText.text = "???"; //should be fancier in the future
        }
        else
        {
            NameText.text = loadInfo.displayName; //should be fancier in the future
            SquadBonusText.text = loadInfo.squadBonusDetails; //should be fancier in the future
        }

        if(loadInfo != null && loadInfo.encounterState == CharacterLoadInformation.EncounterState.Recruited)
        {
            CharInfoBox.Instance?.UpdateCharInfo(loadInfo.characterID);
        }
        else
        {
            CharInfoBox.Instance?.UpdateCharInfo(CharacterNameType.None);
        }


        if (SelectDisplayed && !curButton.InSquad)
        {
            SquadBox.Instance?.DisplaySelected(loadInfo.charPortrait);
        }
        else
        {
            SquadBox.Instance?.DisplaySelected(null);
        }

        if (transform.position != selectionPosition)
        {
            GetComponent<Animation>().Play();
            if (duration == 0f) transform.position = selectionPosition;
            else
            {
                Vector3 startPos = transform.position;
                float timeLeft = duration;
                float progress = 1f - (timeLeft / duration);
                while (timeLeft != 0f)
                {
                    timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, 99f);
                    progress = 1f - (timeLeft / duration);
                    transform.position = startPos + (selectionPosition - startPos) * travelCurve.Evaluate(progress);
                    yield return null;
                }
            }
        }

        moving = false;
    }

    void DisplayOption(Animation option, bool state)
    {
        option.clip = (state ? popInAnim : popOutAnim);
        option.Play();
    }

    public void ShowSelector(bool state)
    {
        if (state == visible) return;

        SelectDisplay.clip = state ? popInAnim : popOutAnim;
        ChatDisplay.clip = state ? popInAnim : popOutAnim;

        SelectDisplay.Play();
        ChatDisplay.Play();

        visible = state;
    }

    public void SelectCurrent()
    {

        if (!visible || moving) return;
        if (curSelectedChar == CharacterNameType.None) return;

        CharacterLoadInformation loadInfo = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == curSelectedChar).FirstOrDefault();

        if (!SelectDisplayed) return; //if the character is available to the player

        if (SceneLoadManager.Instance.squad.Values.Where(r => r.characterID == curSelectedChar).FirstOrDefault() == null)
        {
            SceneLoadManager.Instance.AddSquadMate(curSelectedChar, 0);
        }
        else
        {
            SceneLoadManager.Instance.RemoveSquadMate(curSelectedChar, 0);
        }

        UpdateSelection(curSelectedChar, transform.position, 0f);
        curButton.RefreshButton();
    }

    public void TalkToCurrent()
    {
        if (!visible || moving) return;
        if (curSelectedChar == CharacterNameType.None) return;

        CharacterLoadInformation loadInfo = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == curSelectedChar).FirstOrDefault();

        if (!ChatDisplayed) return;
        if (loadInfo.availableChats.Count <= 0) return;

        StartCoroutine(TalkToCurrentCo(loadInfo.availableChats[0]));
        loadInfo.availableChats.RemoveAt(0);
        curButton.UpdateSelection();
    }

    public IEnumerator TalkToCurrentCo(string blockName)
    {
        yield return Grid_UINavigator.Instance.TriggerUIActivatorCo("MenuChatTransition");

        OnFungusEventTrigger?.Invoke(blockName);
    }

}
