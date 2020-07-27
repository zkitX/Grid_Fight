// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Move execution to a specific Label command in the same block.
    /// </summary>
    [CommandInfo("Flow",
                 "CallSkipDialog", 
                 "Move execution to a specific Label command in the same block")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class CallSkipDialog : Command
    {
        [Tooltip("Name of a label in this block to jump to")]
        [SerializeField] protected StringData _targetLabel = new StringData("");

        #region Public members



        public override void OnEnter()
        {
            InputController.Instance.ButtonPlusDownEvent += Instance_ButtonPlusDownEvent;
            Continue();
        }

        private void Instance_ButtonPlusDownEvent(int player)
        {
            if (_targetLabel.Value == "" || BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets)
            {
                return;
            }
            InputController.Instance.ButtonPlusDownEvent -= Instance_ButtonPlusDownEvent;
            var commandList = ParentBlock.CommandList;
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                Label label = command as Label;
                if (label != null && label.Key == _targetLabel.Value)
                {
                    Continue(label.CommandIndex + 1);
                    return;
                }
            }

            // Label not found
            Debug.LogWarning("Label not found: " + _targetLabel.Value);
            InputController.Instance.ButtonPlusDownEvent -= Instance_ButtonPlusDownEvent;
            Continue();
        }

        public override string GetSummary()
        {
            if (_targetLabel.Value == "")
            {
                return "Error: No label selected";
            }

            return _targetLabel.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetLabel.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("targetLabel")] public Label targetLabelOLD;

        protected virtual void OnEnable()
        {
            if (targetLabelOLD != null)
            {
                _targetLabel.Value = targetLabelOLD.Key;
                targetLabelOLD = null;
            }
        }

        #endregion
    }
}