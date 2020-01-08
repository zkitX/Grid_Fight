using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelectionScript : MonoBehaviour
{
    public Image Up;
    public Image Down;
    public Image Left;
    public Image Right;
    public List<Image> UpImageToColor = new List<Image>();
    public List<Image> DownImageToColor = new List<Image>();
    public List<Image> LeftImageToColor = new List<Image>();
    public List<Image> RightImageToColor = new List<Image>();
    public Animator UpAnim;
    public Animator DownAnim;
    public Animator LeftAnim;
    public Animator RightAnim;



    private Dictionary<ControllerType, CharacterSelectionType> LastSelectedD = new Dictionary<ControllerType, CharacterSelectionType>()
    {
        { ControllerType.Player1, CharacterSelectionType.A},
        { ControllerType.Player2, CharacterSelectionType.B},
        { ControllerType.Player3, CharacterSelectionType.X},
        { ControllerType.Player4, CharacterSelectionType.Y}
    };


    public class Charsinfo
    {
        public Image Icon;
        public CharacterNameType CharName;

        public Charsinfo(Image icon, CharacterNameType charName)
        {
            Icon = icon;
            CharName = charName;
        }
    }
    private List<Charsinfo> Chars = new List<Charsinfo>();


    private void Awake()
    {
        Chars.Add(new Charsinfo(Up, CharacterNameType.Stage00_Character_Desert));
        Chars.Add(new Charsinfo(Down, CharacterNameType.Stage00_Character_Desert));
        Chars.Add(new Charsinfo(Left, CharacterNameType.Stage00_Character_Desert));
        Chars.Add(new Charsinfo(Right, CharacterNameType.Stage00_Character_Desert));
    }




    public void SetupCharacterIcons(List<UIIconClass> listOfIcons)
    {
        foreach (UIIconClass item in listOfIcons)
        {
            item.CharIcon.CurrentCharIsDeadEvent += CharIcon_CurrentCharIsDeadEvent;
            switch (item.CharacterSelection)
            {
                case CharacterSelectionType.Up:
                    Up.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    Chars[0].CharName = item.CharIcon.CharInfo.CharacterID;

                    break;
                case CharacterSelectionType.Down:
                    Down.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    Chars[1].CharName = item.CharIcon.CharInfo.CharacterID;
                    break;
                case CharacterSelectionType.Left:
                    Left.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    Chars[2].CharName = item.CharIcon.CharInfo.CharacterID;
                    break;
                case CharacterSelectionType.Right:
                    Right.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    Chars[3].CharName = item.CharIcon.CharInfo.CharacterID;
                    break;
                case CharacterSelectionType.A:
                    Right.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    break;
                case CharacterSelectionType.B:
                    Down.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    break;
                case CharacterSelectionType.X:
                    Up.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    break;
                case CharacterSelectionType.Y:
                    Left.sprite = item.CharIcon.CharInfo.CharacterIcon;
                    break;
            }
        }
    }

    private void CharIcon_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        Charsinfo icon = Chars.Where(r => r.CharName == cName).FirstOrDefault();
        if(icon != null)
        {
            icon.Icon.color = Color.gray;
        }

    }

    /// <summary>
    /// Firing animation of Character Loading or selection
    /// </summary>
    public void LoadingOrSelectionChar(ControllerType playerController, CharacterSelectionType characterSelection, bool status)
    {
        switch (characterSelection)
        {
            case CharacterSelectionType.Up:
                UpAnim.SetBool("LoadSelect", status);
                ChangeColorForSelection(UpImageToColor, status ? BattleManagerScript.Instance.playersColor[(int)playerController] : Color.white);
                break;
            case CharacterSelectionType.Down:
                DownAnim.SetBool("LoadSelect", status);
                ChangeColorForSelection(DownImageToColor, status ? BattleManagerScript.Instance.playersColor[(int)playerController] : Color.white);
                break;
            case CharacterSelectionType.Left:
                LeftAnim.SetBool("LoadSelect", status);
                ChangeColorForSelection(LeftImageToColor, status ? BattleManagerScript.Instance.playersColor[(int)playerController] : Color.white);
                break;
            case CharacterSelectionType.Right:
                RightAnim.SetBool("LoadSelect", status);
                ChangeColorForSelection(RightImageToColor, status ? BattleManagerScript.Instance.playersColor[(int)playerController] : Color.white);
                break;
            case CharacterSelectionType.A:
                RightAnim.SetBool("LoadSelect", status);
              //  ChangeColorForSelection(UpImageToColor, status ? BattleManagerScript.Instance.playersColor[idColor] : Color.white);
                break;
            case CharacterSelectionType.B:
                DownAnim.SetBool("LoadSelect", status);
             //   ChangeColorForSelection(UpImageToColor, status ? BattleManagerScript.Instance.playersColor[idColor] : Color.white);
                break;
            case CharacterSelectionType.X:
                UpAnim.SetBool("LoadSelect", status);
             //   ChangeColorForSelection(UpImageToColor, status ? BattleManagerScript.Instance.playersColor[idColor] : Color.white);
                break;
            case CharacterSelectionType.Y:
                LeftAnim.SetBool("LoadSelect", status);
             //   ChangeColorForSelection(UpImageToColor, status ? BattleManagerScript.Instance.playersColor[idColor] : Color.white);
                break;
        }
    }

    public void ChangeColorForSelection(List<Image> imgs, Color color)
    {
        foreach (Image item in imgs)
        {
            item.color = color;
        }
    }

    public void SetCharSelected(ControllerType playerController, CharacterSelectionType selection)
    {
        LoadingOrSelectionChar(playerController,LastSelectedD[playerController], false);
        LastSelectedD[playerController] = selection;
        LoadingOrSelectionChar(playerController, selection, true);
    }

}


public class UIIconClass
{
    public BaseCharacter CharIcon;
    public CharacterSelectionType CharacterSelection;
    public UIIconClass()
    {
    }

    public UIIconClass(BaseCharacter charIcon, CharacterSelectionType characterSelection)
    {
        CharIcon = charIcon;
        CharacterSelection = characterSelection;
    }
}
