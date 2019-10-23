using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelectionScript : MonoBehaviour
{
    public Image Up;
    public Image Down;
    public Image Left;
    public Image Right;

    public void SetupCharacterIcons(List<UIIconClass> listOfIcons)
    {
        foreach (UIIconClass item in listOfIcons)
        {
            switch (item.CharacterSelection)
            {
                case CharacterSelectionType.Up:
                    Up.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.Down:
                    Down.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.Left:
                    Left.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.Right:
                    Right.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.A:
                    Right.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.B:
                    Down.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.X:
                    Up.sprite = item.CharIcon;
                    break;
                case CharacterSelectionType.Y:
                    Left.sprite = item.CharIcon;
                    break;
            }
        }
    }

    /// <summary>
    /// Firing animation of Character Loadin or selection
    /// </summary>
    public void LoadingOrSelectionChar(CharacterSelectionType characterSelection, bool status)
    {
        switch (characterSelection)
        {
            case CharacterSelectionType.Up:
                Up.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Down:
                Down.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Left:
                Left.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Right:
                Right.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.A:
                Right.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.B:
                Down.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.X:
                Up.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Y:
                Left.GetComponent<Animator>().SetBool("LoadSelect", status);
                break;
        }
    }

}


public class UIIconClass
{
    public Sprite CharIcon;
    public CharacterSelectionType CharacterSelection;
    public UIIconClass()
    {
    }

    public UIIconClass(Sprite charIcon, CharacterSelectionType characterSelection)
    {
        CharIcon = charIcon;
        CharacterSelection = characterSelection;
    }
}