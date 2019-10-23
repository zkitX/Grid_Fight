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
    private CharacterBase currentSelectedCharacter;
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
    public void SetSelectedCharacter(CharacterBase selectedCharacter)
    {
        currentSelectedCharacter = selectedCharacter;
        SetupCharacter();
    }

    public void SetupCharacter()
    {
        ClassText.text = currentSelectedCharacter.CharInfo.ClassType.ToString();
        CharacterNameText.text = currentSelectedCharacter.CharInfo.CharacterName.ToString();
        CharacterIcon.sprite = currentSelectedCharacter.CharInfo.CharacterIcon;
        for (int i = 0; i < (int)currentSelectedCharacter.CharacterInfo.CharacterLevel - 1; i++)
        {
            //TODO level 
        }

        CharSkills.SetupCharacterSkills(currentSelectedCharacter);
    }

    public void SetupPlayer(int idPlayer)
    {
        PlayerID = idPlayer;
        ColorUtility.TryParseHtmlString("#" + ((PlayerColorType)PlayerID).ToString().Split('_').Last(), out PlayerColor);
        foreach (Image item in ComponentToColor)
        {
            item.color = PlayerColor;
        }
    }

    private void Update()
    {
        if(currentSelectedCharacter != null)
        {
            CharacterHealthBar.rectTransform.anchoredPosition = new Vector2((CharacterHealthBar.rectTransform.rect.width * currentSelectedCharacter.CharacterInfo.HealthPerc)/ 100, 0);
            CharacterStaminaBar.rectTransform.anchoredPosition = new Vector2((CharacterStaminaBar.rectTransform.rect.width * currentSelectedCharacter.CharacterInfo.StaminaPerc) / 100, 0);
        }
    }
}
