using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using MyBox;

public class CharSelectButton : MonoBehaviour
{
    [HideInInspector] public int positionInParent = 0;
    [HideInInspector] public CharSelectBox selectionBoxRef = null;

    public CharacterNameType displayedChar = CharacterNameType.None;
    public Image displayImageRef = null;
    public Image lockedCharIcon = null;

    public Color HiddenColor = Color.black;
    public Color EcounteredColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color RecruitedColor = Color.white;
    public Color InSquadColor = new Color(1f, 1f, 1f, 0.3f);

    public bool useAnims = false;
    protected Animation portraitAnim;
    [ConditionalField("useAnims")] public AnimationClip inSquadClip;
    [ConditionalField("useAnims")] public AnimationClip outOfSquadClip;

    protected Grid_UIButton btnRef;

    private void Awake()
    {
        btnRef = GetComponent<Grid_UIButton>();
        portraitAnim = GetComponentsInChildren<Animation>()[1];
        if (portraitAnim.GetClip(inSquadClip.name) == null) portraitAnim.AddClip(inSquadClip, inSquadClip.name);
        if (portraitAnim.GetClip(outOfSquadClip.name) == null) portraitAnim.AddClip(outOfSquadClip, outOfSquadClip.name);
    }

    public virtual bool InSquad
    {
        get
        {
            return SceneLoadManager.Instance.SquadContains(displayedChar, 0);
        }
    }

    public virtual void DisplayChar(CharacterLoadInformation character, bool applyEffects = true, bool instantChange = false)
    {
        lockedCharIcon?.gameObject.SetActive(false);

        if (character == null)
        {
            displayImageRef.color = new Color(1f, 1f, 1f, 0f);
            displayedChar = CharacterNameType.None;
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
                    if(useAnims && portraitAnim.clip != inSquadClip)
                    {
                        portraitAnim.Stop();
                        portraitAnim.clip = inSquadClip;
                        if (instantChange)
                        {
                            AnimationState state = portraitAnim[portraitAnim.clip.name];
                            state.enabled = true;
                            state.weight = 1;
                            state.normalizedTime = 1;
                            portraitAnim.Sample();
                            state.enabled = false;
                        }
                        else
                        {
                            portraitAnim.Play();
                        }
                    }
                    displayImageRef.color = InSquadColor;
                    break;
                }

                if (useAnims && portraitAnim.clip != outOfSquadClip)
                {
                    portraitAnim.Stop();
                    portraitAnim.clip = outOfSquadClip;

                    if (instantChange)
                    {
                        AnimationState state = portraitAnim[portraitAnim.clip.name];
                        state.enabled = true;
                        state.weight = 1;
                        state.normalizedTime = 1;
                        portraitAnim.Sample();
                        state.enabled = false;
                    }
                    else
                    {
                        portraitAnim.Play();
                    }
                }
                displayImageRef.color = RecruitedColor;
                break;
            default:
                break;
        }
    }

    public virtual void RefreshButton()
    {
        DisplayChar(SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == displayedChar).FirstOrDefault(), instantChange: true);
    }

    public virtual void UpdateSelection()
    {
        selectionBoxRef.UpdateSelection(this, displayedChar, transform.position);
    }

    public void UpdateLayer()
    {
        if (btnRef.selected)
        {
            transform.SetAsLastSibling();
        }
        else
        {
            transform.SetSiblingIndex(positionInParent);
        }
    }
}
