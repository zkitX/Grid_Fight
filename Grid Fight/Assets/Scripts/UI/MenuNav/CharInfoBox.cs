using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CharInfoBox : MonoBehaviour
{
    public static CharInfoBox Instance;

    public CharacterNameType curCharID = CharacterNameType.None;

    [SerializeField] protected Image CharImage;
    [SerializeField] protected Image MaskImage;
    [SerializeField] protected TextMeshProUGUI CharName;
    [SerializeField] protected TextMeshProUGUI Class;
    [SerializeField] protected TextMeshProUGUI Level;
    [SerializeField] protected TextMeshProUGUI NextLevel;
    [SerializeField] protected TextMeshProUGUI HP;
    [SerializeField] protected TextMeshProUGUI Stamina;
    [SerializeField] protected TextMeshProUGUI Power;
    [SerializeField] protected TextMeshProUGUI Shield;
    [SerializeField] protected TextMeshProUGUI Speed;
    [SerializeField] protected TextMeshProUGUI Defence;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateCurCharInfo()
    {
        UpdateCharInfo(curCharID);
    }



    public void UpdateCharInfo(CharacterNameType charName)
    {
        curCharID = charName;

        CharacterLoadInformation loadInfo = charName == CharacterNameType.None ? new CharacterLoadInformation() :
            SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == charName).FirstOrDefault();

        CharName.text = charName == CharacterNameType.None ? "???" : loadInfo.displayName;
        if (charName != CharacterNameType.None &&
            SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == loadInfo.heldMask).FirstOrDefault() != null &&
            SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == loadInfo.heldMask).First().maskType != MaskTypes.None)
        {
            MaskImage.color = Color.white;
            MaskImage.sprite = SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == loadInfo.heldMask).First().maskImage;
        }
        else
        {
            MaskImage.color = new Color(1f,1f,1f,0f);
        }
        CharImage.sprite = loadInfo.charImage;
        Class.text = charName == CharacterNameType.None ? "CLASS: UNKNOWN" : "CLASS: " + loadInfo.charClass.ToString();
        Level.text = "LEVEL : " + loadInfo.Level;
        NextLevel.text = "XP: " + loadInfo.xp.ToString();
        HP.text = "HP: " + loadInfo.health;
        Stamina.text = "STAMINA: " + loadInfo.stamina;
        Power.text = "POWER: " + loadInfo.attackDamage;
        Shield.text = "SHIELD: " + loadInfo.shield;
        Speed.text = "SPEED: " + loadInfo.moveSpeed;
        Defence.text = "DEFENCE: " + loadInfo.defence;

        CharImage.color = charName == CharacterNameType.None ? new Color(1f, 1f, 1f, 0f) : Color.white;
    }
}
