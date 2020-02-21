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
    }
}
