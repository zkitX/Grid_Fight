using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
                "Call SetRelationshipValues",
                "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
[AddComponentMenu("")]
public class SetRelationshipValues : Command
{
    [Header("Relationship info")]
    public MenuRelationshipInfoClass RelationshipInfo;
    protected virtual void CallTheMethod()
    {
        if (RelationshipInfo.IsRelationshipUpdateForTheWholeTeam)
        {
            BattleManagerScript.Instance.UpdateCharactersRelationship(true, new List<CharacterNameType>(), RelationshipInfo.CharTargetRecruitable);
        }
        else if (RelationshipInfo.CharTarget.Count > 0)
        {
            BattleManagerScript.Instance.UpdateCharactersRelationship(false, RelationshipInfo.CharTarget, RelationshipInfo.CharTargetRecruitable);
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
