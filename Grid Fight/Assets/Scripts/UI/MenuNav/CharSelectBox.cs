using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharSelectBox : MonoBehaviour
{
    public static CharSelectBox Instance;

    bool enabled = false;
    public bool enableFromStart = true;

    protected Image selectionBox = null;
    protected Vector2 boxDimens
    {
        get
        {
            return new Vector2(selectionBox.rectTransform.sizeDelta.x * (Screen.width / 1920f), selectionBox.rectTransform.sizeDelta.y * (Screen.height / 1080f));
        }
    }
    protected Vector2 btnDimens
    {
        get
        {
            RectTransform rt = selectableCharButton_Prefab.GetComponent<Image>().rectTransform;
            return new Vector2(rt.sizeDelta.x * (Screen.width / 1920f), rt.sizeDelta.y * (Screen.height / 1080f));
        }
    }
    protected CharSelectSelector selector = null;

    public Animation nextPageDisplay = null;
    public Animation prevPageDisplay = null;

    public GameObject selectableCharButton_Prefab = null;
    public GameObject selector_Prefab = null;
    public int rows = 3;
    public int columns = 5;

    public enum SelectionMode { Squad, Units }
    public SelectionMode selectionMode = SelectionMode.Squad;
    
    protected int pages
    {
        get
        {
            if(Mathf.FloorToInt(SceneLoadManager.Instance.loadedCharacters.Length / (rows * columns)) != (float)SceneLoadManager.Instance.loadedCharacters.Length / ((float)rows * (float)columns))
            {
                return Mathf.FloorToInt(SceneLoadManager.Instance.loadedCharacters.Length / (rows * columns)) + 1;
            }
            else
            {
                return Mathf.FloorToInt(SceneLoadManager.Instance.loadedCharacters.Length / (rows * columns));
            }
        }
    }
    protected int currentPageIndex = 1;
    protected int GetCharPage(CharacterNameType charName)
    {
        for (int i = 0; i < SceneLoadManager.Instance.loadedCharacters.Length; i++)
        {
            if(SceneLoadManager.Instance.loadedCharacters[i].characterID == charName)
            {
                return Mathf.FloorToInt(((float)(i)) / (rows * columns)) + 1;
            }
        }
        return 1;
    }

    protected List<CharSelectButton> activeButtons = new List<CharSelectButton>();

    [HideInInspector] public CharacterNameType lastSelectedChar = CharacterNameType.None;

    private void Awake()
    {
        Instance = this;

        selectionBox = GetComponent<Image>();

        DisplayPage(1);

        EnableSelect(enableFromStart);
    }

    public void UpdateListSizing(int _columns)
    {
        columns = _columns;
        SetupButtons();
        DisplayPage(currentPageIndex);
        selector?.transform.SetAsLastSibling();
        //selector?.UpdateSelection(activeButtons[0], activeButtons[0].displayedChar, activeButtons[0].transform.position, 0f);
    }

    public void ChangeBoxXSize(float val)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta.Set(val, rectTransform.sizeDelta.y);
        rectTransform.anchoredPosition = Vector3.zero;
    }

    public void SetBoxSelectionMode(int mode)
    {
        selectionMode = (SelectionMode)mode;
    }

    public void SelectFirstButton()
    {
        Grid_UINavigator.Instance.SelectButton(activeButtons[0].GetComponent<Grid_UIButton>());
    }

    public void SelectLastSelectedButton()
    {
        SelectCharacterButton(lastSelectedChar);
    }

    public void SelectCharacterButton(CharacterNameType charName)
    {
        if (charName == CharacterNameType.None)
        {
            SelectFirstButton();
            return;
        }

        if (currentPageIndex != GetCharPage(charName))
        {
            DisplayPage(GetCharPage(charName));
        }

        Grid_UINavigator.Instance.SelectButton(activeButtons.Where(r => r.displayedChar == charName).First().GetComponent<Grid_UIButton>());
    }

    void SetupButtons()
    {
        foreach(CharSelectButton btn in activeButtons)
        {
            Destroy(btn.gameObject);
        }
        activeButtons = new List<CharSelectButton>();

        //Get the space between each button, presuming the buttons will be displayed touching the outer walls and equally spaced apart inside
        Vector2 spacing = new Vector2(
            ((boxDimens.x - (btnDimens.x * columns)) / (columns - 1)) + btnDimens.x,
            ((boxDimens.y - (btnDimens.y * rows)) / (rows - 1)) + btnDimens.y
            );

        Vector2 currentPos = (transform.position - (new Vector3(boxDimens.x, -boxDimens.y) / 2f)) + new Vector3((btnDimens.x / 2f), -(btnDimens.y / 2f));

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                CharSelectButton curBtn = Instantiate(selectableCharButton_Prefab, new Vector3(currentPos.x, currentPos.y), Quaternion.identity, transform).GetComponent<CharSelectButton>();
                curBtn.selectionBoxRef = this;
                curBtn.DisplayChar(null);
                curBtn.GetComponent<Grid_UIButton>().parentPanel = GetComponentInParent<Grid_UIPanel>();
                activeButtons.Add(curBtn);

                if (x != columns - 1)
                {
                    currentPos += new Vector2(spacing.x, 0f);
                }
                else
                {
                    currentPos = new Vector2((transform.position.x - (boxDimens.x / 2f)) + (btnDimens.x / 2f), currentPos.y);
                }
            }

            currentPos -= new Vector2(0f, spacing.y);
        }
    }

    void DisplayPage(int page)
    {
        if (activeButtons == null || activeButtons.Count == 0) SetupButtons();

        //Start adding new ones
        currentPageIndex = Mathf.Clamp(page, 1, pages);

        int curCharIndex = (currentPageIndex - 1) * (rows * columns);

        for (int i = 0; i < rows * columns; i++)
        {
            activeButtons[i].DisplayChar(null);
            if (curCharIndex < SceneLoadManager.Instance.loadedCharacters.Length)
            {
                activeButtons[i].DisplayChar(SceneLoadManager.Instance.loadedCharacters[curCharIndex], selectionMode == SelectionMode.Squad);
            }
            if (activeButtons[i].GetComponent<Grid_UIButton>().selected) activeButtons[i].UpdateSelection();
            curCharIndex++;
        }
    }

    public void EnableSelect(bool state)
    {
        enabled = state;

        if(selector != null) selector.ShowSelector(state);

        foreach(Grid_UIButton btn in GetComponentsInChildren<Grid_UIButton>())
        {
            btn.enabled = enabled;
            if (!enabled && btn.selected)
            {
                Grid_UINavigator.Instance.DeselectButton(btn);
            }
        }
    }

    public void TurnPage(int value)
    {
        if (!enabled) return;

        value = value / Mathf.Abs(value);
        DisplayPage(currentPageIndex + value);

        if (Mathf.Clamp(currentPageIndex, 1, pages) == currentPageIndex - value) return;

        if(value == 1)
        {
            nextPageDisplay.Play();
        }
        else if(value == -1)
        {
            prevPageDisplay.Play();
        }
    }

    public void UpdateSelection(CharSelectButton btn, CharacterNameType charName, Vector3 selectionPosition)
    {
        if(selector == null)
        {
            selector = Instantiate(selector_Prefab, transform).GetComponent<CharSelectSelector>();
            selector.parentBox = this;
        }
        selector.UpdateSelection(btn, charName, selectionPosition, 0.25f);
    }

    public void SelectorSelectAction()
    {
        if (selectionMode == SelectionMode.Units)
        {
            if (CharInfoBox.Instance.curCharID == CharacterNameType.None) return;

            CharacterLoadInformation charInfo = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == CharInfoBox.Instance.curCharID).First();
            if (charInfo.heldMask == MaskTypes.None)
            {
                Grid_UINavigator.Instance.TriggerUIActivator("UnitsMaskTransition");
            }
            else
            {
                SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == charInfo.heldMask).First().maskHolder = CharacterNameType.None;
                CharInfoBox.Instance.UpdateCurCharInfo();
            }
            return;
        }
        selector?.SelectCurrent();
    }

    public void SelectorChatAction()
    {
        if (selectionMode == SelectionMode.Squad) return;
        selector?.TalkToCurrent();
    }

    
}
