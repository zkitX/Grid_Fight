using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSectionScript : MonoBehaviour
{
    public CanvasGroup CurrentCanvasGroup;
    private int PlayerID;
    private CharacterType_Script currentSelectedCharacter;
    private Color PlayerColor;
    [SerializeField]
    private List<Image> ComponentToColor = new List<Image>();
    [SerializeField]
    private TextMeshProUGUI ClassText;
    [SerializeField]
    private TextMeshProUGUI CharacterNameText;
    [SerializeField]
    private Image CharacterIcon;
    [SerializeField]
    private List<Image> CharacterLevels = new List<Image>();
    [SerializeField]
    private Image CharacterHealthBar;
    [SerializeField]
    private Image CharacterStaminaBar;
    [SerializeField]
    private UICharacterSkillContainerScript CharSkills;
    public Animator Anim;


   // private Animator CharLevel;
    public void SetSelectedCharacter(CharacterType_Script selectedCharacter)
    {
        currentSelectedCharacter = selectedCharacter;
        SetupCharacter();
    }

    public void SetupCharacter()
    {
        ClassText.text = currentSelectedCharacter.CharInfo.ClassType.ToString();
        CharacterNameText.text = currentSelectedCharacter.CharInfo.Name.ToString();
        CharacterIcon.sprite = currentSelectedCharacter.CharInfo.CharacterIcon;
        for (int i = 0; i < (int)currentSelectedCharacter.CharInfo.CharacterLevel - 1; i++)
        {
            //TODO level 
        }

        CharSkills.SetupCharacterSkills(currentSelectedCharacter);
       // CharLevel.SetInteger("CharLevel", (int)currentSelectedCharacter.CharInfo.CharacterLevel);
    }

    public void SetupPlayer(int idPlayer)
    {
        PlayerID = idPlayer;
        //ColorUtility.TryParseHtmlString("#" + ((PlayerColorType)PlayerID).ToString().Split('_').Last(), out PlayerColor);
        /*foreach (Image item in ComponentToColor)
        {
            item.color = PlayerColor;
        }*/
    }

    private void Update()
    {
        if(currentSelectedCharacter != null)
        {
            if(currentSelectedCharacter.UMS.Side == SideType.LeftSide)
            {
                CharacterHealthBar.rectTransform.anchoredPosition = new Vector2((CharacterHealthBar.rectTransform.rect.width * currentSelectedCharacter.CharInfo.HealthPerc) / 100, 0);
                CharacterStaminaBar.rectTransform.anchoredPosition = new Vector2((CharacterStaminaBar.rectTransform.rect.width * currentSelectedCharacter.CharInfo.StaminaPerc) / 100, 0);
            }
            else if (currentSelectedCharacter.UMS.Side == SideType.RightSide)
            {
                CharacterHealthBar.rectTransform.anchoredPosition = new Vector2(-(CharacterHealthBar.rectTransform.rect.width * currentSelectedCharacter.CharInfo.HealthPerc) / 100, 0);
                CharacterStaminaBar.rectTransform.anchoredPosition = new Vector2(-(CharacterStaminaBar.rectTransform.rect.width * currentSelectedCharacter.CharInfo.StaminaPerc) / 100, 0);
            }


        }
    }
}
