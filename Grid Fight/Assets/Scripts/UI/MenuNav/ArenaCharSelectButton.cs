using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ArenaCharSelectButton : CharSelectButton
{
    protected Color[] TeamColors;
    public Image[] SelectorImages;
    public TextMeshProUGUI SelectorCharName;
    public TextMeshProUGUI SelectorPlayers;

    public CharacterLoadInformation loadInfo;

    public Vector2Int pos = new Vector2Int();

    public override bool InSquad
    {
        get
        {
            return false;
        }
    }

    private void Awake()
    {
        TeamColors = SceneLoadManager.Instance.teamsColor;
    }

    public override void DisplayChar(CharacterLoadInformation character, bool applyEffects = true)
    {
        loadInfo = character;

        if (character != null && character.encounterState != CharacterLoadInformation.EncounterState.Hidden)
        {
            SelectorCharName.text = character.displayName;
        }
        else
        {
            SelectorCharName.text = "???";
        }

        base.DisplayChar(character, applyEffects);
    }

    public override void RefreshButton()
    {
        DisplayChar(SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == displayedChar).FirstOrDefault());
    }

    public override void UpdateSelection()
    {
        //Get the info from the nav box on which team is selecting this and update the ui
        PlayerNavGroup[] navGroupsSelecting = ArenaCharSelectBox.Instance.playerNavGroups.Where(r => r.pos == pos).ToArray();

        SelectorPlayers.text = "";
        for (int i = 0; i < navGroupsSelecting.Length; i++)
        {
            if (i > 0)
            {
                SelectorPlayers.text += "/";
            }
            SelectorPlayers.text += navGroupsSelecting[i].Name;
        }

        switch (navGroupsSelecting.Length)
        {
            case (1):
                foreach (Image img in SelectorImages)
                {
                    img.color = navGroupsSelecting[0].Name == "T1" ? TeamColors[0] : TeamColors[1];
                }
                for (int i = 2; i < SelectorImages.Length; i++)
                {
                    SelectorImages[i].color *= new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                break;
            case (2):
                SelectorImages[0].color = navGroupsSelecting[0].Name == "T1" ? TeamColors[0] : TeamColors[1];
                SelectorImages[2].color = navGroupsSelecting[0].Name == "T1" ? TeamColors[0] : TeamColors[1];
                SelectorImages[1].color = navGroupsSelecting[0].Name == "T1" ? TeamColors[1] : TeamColors[0];
                SelectorImages[3].color = navGroupsSelecting[0].Name == "T1" ? TeamColors[1] : TeamColors[0];
                for (int i = 2; i < SelectorImages.Length; i++)
                {
                    SelectorImages[i].color *= new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                break;
            default:
                break;
        }

        //Send info to the arena selection display 
        foreach(PlayerNavGroup png in navGroupsSelecting)
        {
            ArenaSquadBoxes.Instance.DisplaySelection(
                png.Name == "T1" ? 1 : 2,
                loadInfo == null ? null : loadInfo.charImage
                );
        }

        return;
    }

    public void CheckDeselect()
    {
        if (ArenaCharSelectBox.Instance.playerNavGroups.Where(r => r.pos == pos).FirstOrDefault() != null) return;
        GetComponent<Animation>().clip = GetComponent<Animation>().GetClip("ArenaSelectChar_DeselectShrink");
        GetComponent<Animation>().Play();
    }

    public void SelectChar(int squadIndex)
    {
        if (displayedChar == CharacterNameType.None) return;

        if (loadInfo.encounterState != CharacterLoadInformation.EncounterState.Recruited) return; //if the character is available to the player

        if (!SceneLoadManager.Instance.SquadContains(displayedChar, squadIndex))
        {
            SceneLoadManager.Instance.AddSquadMate(displayedChar, squadIndex);
        }
        else
        {
            SceneLoadManager.Instance.RemoveSquadMate(displayedChar, squadIndex);
        }

        RefreshButton();
    }

}
