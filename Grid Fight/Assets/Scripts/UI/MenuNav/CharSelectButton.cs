using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharSelectButton : MonoBehaviour
{
    [HideInInspector] public CharSelectBox selectionBoxRef = null;

    public CharacterNameType displayedChar = CharacterNameType.None;
    public Image displayImageRef = null;
    public Image lockedCharIcon = null;

    public Color HiddenColor = Color.black;
    public Color EcounteredColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color RecruitedColor = Color.white;
    public Color InSquadColor = new Color(1f, 1f, 1f, 0.3f);

    public virtual bool InSquad
    {
        get
        {
            return SceneLoadManager.Instance.SquadContains(displayedChar, 0);
        }
    }

    public virtual void DisplayChar(CharacterLoadInformation character, bool applyEffects = true)
    {
        lockedCharIcon?.gameObject.SetActive(false);

        if (character == null)
        {
            displayImageRef.color = new Color(1f, 1f, 1f, 0f);
            return;
        }

        displayedChar = character.characterID;
        if (displayImageRef == null) Debug.LogError("NO image ref assigned");
        displayImageRef.sprite = character.charPortrait;

        switch (character.encounterState)
        {
            case CharacterLoadInformation.EncounterState.Hidden:
                displayImageRef.color = HiddenColor;
                lockedCharIcon?.gameObject.SetActive(true);
                break;
            case CharacterLoadInformation.EncounterState.Encountered:
                displayImageRef.color = EcounteredColor;
                lockedCharIcon?.gameObject.SetActive(true);
                break;
            case CharacterLoadInformation.EncounterState.Recruited:
                if (InSquad && applyEffects)
                {
                    displayImageRef.color = InSquadColor;
                    break;
                }
                displayImageRef.color = RecruitedColor;
                break;
            default:
                break;
        }
    }

    public virtual void RefreshButton()
    {
        DisplayChar(SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == displayedChar).FirstOrDefault());
    }

    public virtual void UpdateSelection()
    {
        selectionBoxRef.UpdateSelection(this, displayedChar, transform.position);
    }
}
