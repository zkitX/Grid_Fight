using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class MaskInfoBox : MonoBehaviour
{
    public static MaskInfoBox Instance;

    [SerializeField] protected TextMeshProUGUI NameTxt;
    [SerializeField] protected TextMeshProUGUI LevelTxt;
    [SerializeField] protected TextMeshProUGUI NextLevelTxt;
    [SerializeField] protected Image MaskImage;
    [SerializeField] protected Image CharImage;
    [SerializeField] protected Image[] abilityImages = new Image[3];
    [SerializeField] protected TextMeshProUGUI[] abilityNames = new TextMeshProUGUI[3];
    [SerializeField] protected TextMeshProUGUI[] abilityDescriptions = new TextMeshProUGUI[3];

    protected Color abilityBGDeselected;
    [SerializeField] protected Color abilityBGColorSelected = Color.blue;

    private void Awake()
    {
        Instance = this;
        abilityBGDeselected = abilityImages[0].GetComponentsInParent<Image>()[1].color;
    }

    public void UpdateMaskInfo(MaskTypes maskName)
    {
        MaskLoadInformation maskInfo = new MaskLoadInformation();
        bool noMask = false;
        string noMaskText = "???";

        if (maskName == MaskTypes.None)
        {
            noMask = true;
        }
        else
        {
            maskInfo = SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == maskName).FirstOrDefault();
        }

        NameTxt.text = noMask ? noMaskText : maskInfo.Name.ToUpper();
        LevelTxt.text = "LEVEL: " + (noMask ? noMaskText : maskInfo.Level.ToString());
        NextLevelTxt.text = "XP: " + (noMask ? noMaskText : maskInfo.xp.ToString());

        MaskImage.sprite = noMask ? null : maskInfo.maskImage;
        MaskImage.color = noMask ? new Color(1f, 1f, 1f, 0f) : Color.white;

        CharImage.sprite = noMask ? null : maskInfo.maskHolder == CharacterNameType.None ? null : 
            SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == maskInfo.maskHolder).FirstOrDefault().charPortrait;
        CharImage.color = noMask ? new Color(1f, 1f, 1f, 0f) : maskInfo.maskHolder == CharacterNameType.None ? new Color(1f, 1f, 1f, 0f) : Color.white;

        for (int i = 0; i < 3; i++)
        {
            abilityImages[i].GetComponentsInParent<Image>()[1].color = abilityBGDeselected;
            if (!noMask)
            {
                abilityImages[i].sprite = maskInfo.abilities[i].abilityImage;
                abilityImages[i].color = Color.white;
                if (maskInfo.Level > i) abilityImages[i].GetComponentsInParent<Image>()[1].color = abilityBGColorSelected;
                abilityNames[i].text = maskInfo.abilities[i].Name.ToUpper();
                abilityDescriptions[i].text = maskInfo.abilities[i].description;
            }
            else
            {
                abilityImages[i].sprite = null;
                abilityImages[i].color = new Color(1f, 1f, 1f, 0f);
                abilityNames[i].text = noMaskText;
                abilityDescriptions[i].text = noMaskText;
            }
        }


    }
}
