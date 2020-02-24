using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridFightMenuDialog : MenuDialog
{
    private int SelectionIndex;
    public float TimeOffset = 0;
    public float CoolDown = 0.5f;
    protected List<FungusMenuOptionBoxScript> Boxes = new List<FungusMenuOptionBoxScript>();

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

    protected override bool AddOption(string text, bool interactable, bool hideOption, UnityEngine.Events.UnityAction action)
    {
        if (nextOptionIndex >= Boxes.Count)
            return false;
        BattleManagerScript.Instance.FungusState = FungusDialogType.Menu;
        //if first option notify that a menu has started
        if (nextOptionIndex == 0)
            MenuSignals.DoMenuStart(this);

        var box = Boxes[nextOptionIndex];

        //move forward for next call
        nextOptionIndex++;

        //don't need to set anything on it
        if (hideOption)
            return true;

        box.gameObject.SetActive(true);


        var a = action.Target;

        if (!string.IsNullOrEmpty(text))
        {
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
        SelectionIndex = -1;
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
        if (Time.time > TimeOffset + CoolDown)
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

    public override void HideSayDialog()
    {
        
    }

    public void SelectMenu()
    {
        Boxes[SelectionIndex].BoxAnim.SetBool("isSelected", true);
    }


    private void Instance_ButtonADownEvent(int player)
    {
        if(BattleManagerScript.Instance.FungusState == FungusDialogType.Menu)
        {
            StartCoroutine(CallBlock(Boxes[SelectionIndex].NextBlock));
        }
    }
}
