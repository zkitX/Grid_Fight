using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Grid_UIButton : MonoBehaviour
{
    [HideInInspector] public TextMeshProUGUI buttonText;
    public UnityEvent PressActions;

    private void Awake()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void PressAction()
    {

    }

    public virtual void SelectAction()
    {

    }

    public virtual void DeselectAction()
    {

    }


    private void OnEnable()
    {
        if (buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText.text == "ButtonText" || buttonText.text == "StandardButton") buttonText.text = gameObject.name;
    }

    private void OnValidate()
    {
        if(buttonText == null) buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text == "ButtonText" || buttonText.text == "StandardButton") buttonText.text = gameObject.name;
    }
}
