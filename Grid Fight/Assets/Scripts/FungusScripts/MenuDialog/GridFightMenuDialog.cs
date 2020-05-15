using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridFightMenuDialog : MenuDialog
{
    private int SelectionIndex;
    public float TimeOffset = 0;
    public float CoolDown = 0.5f;
    protected List<FungusMenuOptionBoxScript> Boxes = new List<FungusMenuOptionBoxScript>();
    bool isMenuReady = false;
    protected int Options = 0;

    protected override void Awake()
    {
        Boxes = GetComponentsInChildren<FungusMenuOptionBoxScript>().ToList();
      
        Slider timeoutSlider = GetComponentInChildren<Slider>();
        cachedSlider = timeoutSlider;
        if (Application.isPlaying)
        {
            // Don't auto disable buttons in the editor
            Clear();
        }

        
        CheckEventSystem();
    }

    private void Item_AnimationInfoScriptAnimationCompletedEvent()
    {
        isMenuReady = true;
    }

    public override void Clear()
    {
        StopAllCoroutines();

        //if something was shown notify that we are ending
        if (nextOptionIndex != 0)
            MenuSignals.DoMenuEnd(this);
        nextOptionIndex = 0;

        for (int i = 0; i < Boxes.Count; i++)
        {
            Boxes[i].gameObject.SetActive(false);
        }

        Slider timeoutSlider = CachedSlider;
        if (timeoutSlider != null)
        {
            timeoutSlider.gameObject.SetActive(false);
        }

    }

    public bool AddOption(string text, bool interactable, bool hideOption, Block targetBlock, MenuRelationshipInfoClass relationshipInfo)
    {
        var block = targetBlock;
        UnityEngine.Events.UnityAction action = delegate
        {
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            Clear();
            HideSayDialog();
            if (block != null)
            {
                var flowchart = block.GetFlowchart();
#if UNITY_EDITOR
                // Select the new target block in the Flowchart window
                flowchart.SelectedBlock = block;
#endif
                gameObject.SetActive(false);
                // Use a coroutine to call the block on the next frame
                // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                flowchart.StartCoroutine(CallBlock(block));
            }
        };

        return AddOption(text, interactable, hideOption, action, relationshipInfo);
    }

    protected bool AddOption(string text, bool interactable, bool hideOption, UnityEngine.Events.UnityAction action, MenuRelationshipInfoClass relationshipInfo)
    {
        if (nextOptionIndex >= Boxes.Count)
            return false;
       
        BattleManagerScript.Instance.FungusState = FungusDialogType.Menu;
        isMenuReady = false;
        //if first option notify that a menu has started
        if (nextOptionIndex == 0)
            MenuSignals.DoMenuStart(this);

        FungusMenuOptionBoxScript box = Boxes[nextOptionIndex];
      
        //move forward for next call
        nextOptionIndex++;

        //don't need to set anything on it
        if (hideOption)
            return true;
        box.gameObject.SetActive(true);
        box.BoxAnim.SetBool("InOut", true);
        AnimationInfoScript[] anims = box.BoxAnim.GetBehaviours<AnimationInfoScript>();
        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].AnimationInfoScriptAnimationCompletedEvent += Item_AnimationInfoScriptAnimationCompletedEvent;
        }

       
        if (SelectionIndex + 1 == nextOptionIndex)
        {
            Boxes[SelectionIndex].BoxAnim.SetBool("isSelected", true);
        }
        if (!string.IsNullOrEmpty(text))
        {
            box.RelationshipInfo = relationshipInfo;
            box.NextBlock = (Block)action.Target.GetType().GetField("block").GetValue(action.Target);
        }
        
        TextAdapter textAdapter = new TextAdapter();
        textAdapter.InitFromGameObject(box.gameObject, true);
        if (textAdapter.HasTextObject())
        {
            text = TextVariationHandler.SelectVariations(text);

            textAdapter.Text = text;
        }

        return true;
    }


    public override int DisplayedOptionsCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Boxes.Where(r => r.gameObject.activeInHierarchy).ToList().Count; i++)
            {
                var button = Boxes[i];
                if (button.gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }
    }


    private void Start()
    {
        StartCoroutine(LookingForRewired());
    }

    protected override void OnEnable()
    {
        SelectionIndex = 0;
        base.OnEnable();
    }

    IEnumerator LookingForRewired()
    {
        while (InputController.Instance == null)
        {
            yield return null;
        }

        InputController.Instance.ButtonADownEvent += Instance_ButtonADownEvent;
        InputController.Instance.LeftJoystickUsedEvent += Instance_LeftJoystickUsedEvent;
    }

    private void Instance_LeftJoystickUsedEvent(int player, InputDirection dir)
    {
        if (Time.time > TimeOffset + CoolDown && BattleManagerScript.Instance.FungusState == FungusDialogType.Menu && isMenuReady)
        {
            if(SelectionIndex >=0 && SelectionIndex <= Boxes.Where(r => r.gameObject.activeInHierarchy).ToList().Count)
            {
                Boxes[SelectionIndex].BoxAnim.SetBool("isSelected", false);
            }
            Options = Boxes.Where(r => r.gameObject.activeInHierarchy).ToList().Count;
            switch (dir)
            {
                case InputDirection.Up:
                    SelectionIndex--;
                    break;
                case InputDirection.Down:
                    SelectionIndex++;
                    break;
            }

            
            SelectionIndex = SelectionIndex >= Options ? 0 : SelectionIndex < 0 ? Options - 1 : SelectionIndex;
            SelectMenu();
            TimeOffset = Time.time;
        }
    }

    public void SelectMenu()
    {

        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Menus_SelectButton, AudioBus.MidPrio);
        Boxes[SelectionIndex].BoxAnim.SetBool("isSelected", true);
    }


    private void Instance_ButtonADownEvent(int player)
    {
        if(BattleManagerScript.Instance.FungusState == FungusDialogType.Menu && isMenuReady)
        {
            Boxes[SelectionIndex].NextBlock.StartExecution();

            if (Boxes[SelectionIndex].RelationshipInfo.IsRelationshipUpdateForTheWholeTeam)
            {
                BattleManagerScript.Instance.UpdateCharactersRelationship(true, new List<CharacterNameType>(), Boxes[SelectionIndex].RelationshipInfo.CharTargetRecruitableIDs, Boxes[SelectionIndex].RelationshipInfo.Value);
            }
            else if (Boxes[SelectionIndex].RelationshipInfo.CharTarget.Count > 0)
            {
                BattleManagerScript.Instance.UpdateCharactersRelationship(false, Boxes[SelectionIndex].RelationshipInfo.CharTarget, Boxes[SelectionIndex].RelationshipInfo.CharTargetRecruitableIDs, Boxes[SelectionIndex].RelationshipInfo.Value);
            }

            isMenuReady = false;
            BattleManagerScript.Instance.FungusState = FungusDialogType.Dialog;
            
            foreach (FungusMenuOptionBoxScript item in Boxes)
            {
                item.BoxAnim.SetBool("InOut", false);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Menus_PressButton, AudioBus.MidPrio);

                for (int a = 0; a < Boxes.Count; a++)
                {
                    AnimationInfoScript[] anims = Boxes[a].BoxAnim.GetBehaviours<AnimationInfoScript>();
                    for (int i = 0; i < anims.Length; i++)
                    {
                        anims[i].AnimationInfoScriptAnimationCompletedEvent -= Item_AnimationInfoScriptAnimationCompletedEvent;
                    }
                }
            }

            StartCoroutine(ClearMenu(1));
        }
    }

    private IEnumerator ClearMenu(float delay)
    {
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Dialogue_Exiting, AudioBus.MidPrio);
        yield return new WaitForSecondsRealtime(delay);
        SelectionIndex = 0;
        Clear();
    }
}
