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
    public Animator UpAnim;
    public Animator DownAnim;
    public Animator LeftAnim;
    public Animator RightAnim;

    public CharacterSelectionType LastSelected;

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
                UpAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Down:
                DownAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Left:
                LeftAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Right:
                RightAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.A:
                RightAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.B:
                DownAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.X:
                UpAnim.SetBool("LoadSelect", status);
                break;
            case CharacterSelectionType.Y:
                LeftAnim.SetBool("LoadSelect", status);
                break;
        }
    }


    public void SetCharSelected(CharacterSelectionType selection)
    {
        LoadingOrSelectionChar(LastSelected, false);
        LastSelected = selection;
        LoadingOrSelectionChar(LastSelected, true);
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