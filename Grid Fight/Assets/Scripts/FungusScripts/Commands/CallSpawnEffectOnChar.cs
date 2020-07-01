using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using MyBox;
using System.Linq;

[CommandInfo("Scripting",
                "Call CallSpawnEffectOnChar",
                "Spawns a tile effect at a specified tile position for a set duration")]
[AddComponentMenu("")]
public class CallSpawnEffectOnChar : Command
{
    public List<CharacterNameType> AffectedChars = new List<CharacterNameType>();
    public ScriptableObjectAttackEffect Effect;
    

    protected virtual void CallTheMethod()
    {
        foreach (CharacterNameType item in AffectedChars)
        {
            BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == item).FirstOrDefault();
            if(cb == null)
            {
                cb = BattleManagerScript.Instance.CharsForTalkingPart.Where(r => r.CharInfo.CharacterID == item).FirstOrDefault();
                if (cb == null)
                {
                    cb = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == item).FirstOrDefault();
                }
            }

            cb.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Dark, new BaseCharacter(), Effect));
        }
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion

}

