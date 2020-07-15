// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using MyBox;
using System.Linq;

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

        [Header("Variable Info")]
        public string ThisBlockVariableName;

        public bool Unlockable = false;
        public List<string> EnablingBlocksName = new List<string>();

        public GameObject Parent;
        #region Public members

        public override void OnEnter()
        {
            if (setMenuDialog != null)
            {
                // Override the active menu dialog
                MenuDialog.ActiveMenuDialog = setMenuDialog;
            }
            bool unlock = true;
            bool hiddenOn = false;
            foreach (string item in EnablingBlocksName)
            {
                if(FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == item).First().Value == "OFF")
                {
                    hiddenOn = false;
                    unlock = false;
                    break;
                }
                hiddenOn = true;
            }

            if(unlock)
            {
                GridFightMenuDialog menuDialog = (GridFightMenuDialog)MenuDialog.GetMenuDialog();
                if (menuDialog != null)
                {
                    menuDialog.SetActive(true);

                    var flowchart = GetFlowchart();
                    string displayText = flowchart.SubstituteVariables(text);
                    string varVal = FlowChartVariablesManagerScript.instance.Variables.Where(r => r.Name == ThisBlockVariableName).First().Value;
                    menuDialog.AddOption(displayText, interactable, false, targetBlock, RelationshipInfo, varVal == "ON" ? OptionBoxAnimType.AlreadySelected :
                        hiddenOn ? OptionBoxAnimType.Hidden : OptionBoxAnimType.Active, ThisBlockVariableName);
                }
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
    public List<TargetRecruitableClass> CharTargetRecruitable = new List<TargetRecruitableClass>();
    public List<CharacterNameType> CharTarget = new List<CharacterNameType>();

    public MenuRelationshipInfoClass()
    {

    }
    public MenuRelationshipInfoClass(List<TargetRecruitableClass> charTargetRecruitable, List<CharacterNameType> charTarget)
    {
        CharTargetRecruitable = charTargetRecruitable;
        CharTarget = charTarget;
    }
}

[System.Serializable]
public class TargetRecruitableClass
{
    public CharacterNameType CharTargetRecruitableID;
    public int Value;

    public TargetRecruitableClass()
    {

    }

    public TargetRecruitableClass(CharacterNameType charTargetRecruitableID, int value)
    {
        CharTargetRecruitableID = charTargetRecruitableID;
        Value = value;
    }
}