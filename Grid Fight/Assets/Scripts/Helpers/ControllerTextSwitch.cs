using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ControllerTextSwitch : MonoBehaviour
{
    TextMeshProUGUI textMesh = null;
    protected string originalText = "";
    public string textQueueToReplace = "[Control]";

    public List<ControllerTextSwitchInfo> textSwitchInfo = new List<ControllerTextSwitchInfo>()
    {
        new ControllerTextSwitchInfo(Rewired.ControllerType.Joystick, "controller button"),
        new ControllerTextSwitchInfo(Rewired.ControllerType.Keyboard, "keyboard button"),
    };

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        originalText = textMesh.text;
    }

    private void OnEnable()
    {
        InputController.Instance.OnLastControllerTypeChange += UpdateText;
        UpdateText(InputController.Instance.LastControllerType);
    }

    private void OnDisable()
    {
        InputController.Instance.OnLastControllerTypeChange -= UpdateText;
    }

    void UpdateText(Rewired.ControllerType controller)
    {
        ControllerTextSwitchInfo tSInfo = textSwitchInfo.Where(r => r.controllerType == controller).FirstOrDefault();
        if (tSInfo == null) return;

        string inputText = tSInfo.textInCase;

        if (textQueueToReplace == "" || !originalText.Contains(textQueueToReplace))
        {
            textMesh.text = inputText;
            return;
        }

        string outputText = originalText;

        textMesh.text = outputText.Replace(textQueueToReplace, inputText);
    }


}

[System.Serializable]
public class ControllerTextSwitchInfo
{
    public Rewired.ControllerType controllerType;
    public string textInCase;

    public ControllerTextSwitchInfo()
    {
        controllerType = Rewired.ControllerType.Joystick;
        textInCase = "";
    }

    public ControllerTextSwitchInfo(Rewired.ControllerType ctrlType, string text)
    {
        controllerType = ctrlType;
        textInCase = text;
    }
}