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
    public void SetSelectedCharacter(CharacterBase selectedCharacter)
    {
        currentSelectedCharacter = selectedCharacter;
    }

    public void SetupCharacter()
    {
        ClassText.text = currentSelectedCharacter.BulletInfo.ClassType.ToString();
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
}
