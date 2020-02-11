using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewIManager : MonoBehaviour
{
    public static NewIManager Instance;

    [SerializeField] protected string buttonTypeB;
    [SerializeField] protected string buttonTypeA;
    [SerializeField] protected string buttonTypeX;
    [SerializeField] protected string buttonTypeY;

    protected NewICharacterVitality[] vitalityBoxes;

    private void Awake()
    {
        Instance = this;
        vitalityBoxes = GetComponentsInChildren<NewICharacterVitality>();
        foreach(NewICharacterVitality vitalityBox in vitalityBoxes)
        {
            vitalityBox.SetCharacter(null);
        }
    }

    public void SetSelected(bool state, ControllerType controller, CharacterNameType charName)
    {
        if (state) GetvitalityBoxOfCharacter(charName).SelectCharacter(controller);
        else GetvitalityBoxOfCharacter(charName).DeselectCharacter();
    }

    public void SetUICharacterToButton(CharacterType_Script character, CharacterSelectionType buttonToAssignTo)
    {
        GetvitalityBoxOfAssignedButton(buttonToAssignTo).SetCharacter(character.CharInfo);
    }

    public void UpdateVitalitiesOfCharacter(CharacterInfoScript character)
    {
        GetvitalityBoxOfCharacter(character.CharacterID).UpdateVitalities();
    }

    public NewICharacterVitality GetvitalityBoxOfCharacter(CharacterNameType charName)
    {
        return vitalityBoxes.Where(r => r.assignedCharDetails.CharacterID == charName).FirstOrDefault();
    }

    public NewICharacterVitality GetvitalityBoxOfAssignedButton(CharacterSelectionType inButton)
    {
        return vitalityBoxes.Where(r => r.assignedButton == inButton).FirstOrDefault();
    }

    public string GetButtonTypeString(CharacterSelectionType input)
    {
        switch (input)
        {
            case (CharacterSelectionType.Up):
                return buttonTypeB;
            case (CharacterSelectionType.Down):
                return buttonTypeA;
            case (CharacterSelectionType.Left):
                return buttonTypeX;
            case (CharacterSelectionType.Right):
                return buttonTypeY;
            default:
                return null;
        }
    }

}
