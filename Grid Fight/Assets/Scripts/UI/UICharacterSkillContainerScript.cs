using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICharacterSkillContainerScript : MonoBehaviour
{
    public UICharSkillScript ASkill;
    public UICharSkillScript BSkill;
    public UICharSkillScript YSkill;
    public UICharSkillScript XSkill;



    public void SetupCharacterSkills(CharacterBase currentSelectedChar)
    {
        ASkill.Anim.SetBool("Active", false);
        YSkill.Anim.SetBool("Active", false);
        XSkill.Anim.SetBool("Active", false);
        BSkill.Anim.SetBool("Active", false);
        UICharSkillScript nextAnimAvailable = BSkill;
        ASkill.Anim.SetBool("Active", true);
        ASkill.SkillIcon.sprite = currentSelectedChar.CharInfo.CharacterIcon;
        foreach (CharacterBase item in BattleManagerScript.Instance.AllCharactersOnField.Where(r=> r != currentSelectedChar && r.UMS.Side == currentSelectedChar.UMS.Side))
        {
            CharactersRelationshipClass crc = currentSelectedChar.CharacterInfo.CharacterRelationships.Where(r => r.CharacterName == item.CharInfo.CharacterName).FirstOrDefault();
            if (crc != null)
            {
                nextAnimAvailable.Anim.SetBool("Active", true);
                nextAnimAvailable.SkillIcon.sprite = item.CharInfo.CharacterIcon;
                nextAnimAvailable = nextAnimAvailable == BSkill ? YSkill : XSkill;
            }
        }
    }
}





