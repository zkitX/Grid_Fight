// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using MyBox;

namespace Fungus
{
    /// <summary>
    /// Displays a button in a multiple choice menu.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Gridfight_Menu", 
                 "Displays a button in a multiple choice menu")]
    [AddComponentMenu("")]
    public class GridFight_Menu : Menu
    {
        [Header("Relationship info")]
        public MenuRelationshipInfoClass RelationshipInfo;

        #region Public members

        public override void OnEnter()
        {
            if (setMenuDialog != null)
            {
                // Override the active menu dialog
                MenuDialog.ActiveMenuDialog = setMenuDialog;
            }

            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            GridFightMenuDialog menuDialog = (GridFightMenuDialog)MenuDialog.GetMenuDialog();
                if (menuDialog != null)
                {
                    menuDialog.SetActive(true);

                    var flowchart = GetFlowchart();
                    string displayText = flowchart.SubstituteVariables(text);

                    menuDialog.AddOption(displayText, interactable, hideOption, targetBlock, RelationshipInfo);
                }
            
            Continue();
        }
        #endregion
    }
}

[System.Serializable]
public class MenuRelationshipInfoClass
{
    public bool IsRelationshipUpdateForTheWholeTeam = true;
    public List<CharacterNameType> CharTargetRecruitableIDs = new List<CharacterNameType>();
    public int Value;
    public List<CharacterNameType> CharTarget = new List<CharacterNameType>();

    public MenuRelationshipInfoClass()
    {

    }
}