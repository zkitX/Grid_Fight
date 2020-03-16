using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;

namespace Fungus
{
    [System.Serializable]
    public class WeightedBlockCallClass
    {
        [SerializeField] public Block targetBlock;
        [SerializeField] public int bias = 10;
    }

    [CommandInfo("Scripting",
                 "Call (Random)",
                 "Randomly execute another block in the same Flowchart as the command, or in a different Flowchart.")]
    [AddComponentMenu("")]
    public class CallRandom : Command
    {
        [Tooltip("Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.")]
        [SerializeField] protected Flowchart targetFlowchart;

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to start executing")]
        [SerializeField] protected WeightedBlockCallClass[] targetBlocks;
        protected Block targetBlock;

        [Tooltip("Label to start execution at. Takes priority over startIndex.")]
        [SerializeField] protected StringData startLabel = new StringData();

        [Tooltip("Command index to start executing")]
        [FormerlySerializedAs("commandIndex")]
        [SerializeField] protected int startIndex;

        [Tooltip("Select if the calling block should stop or continue executing commands, or wait until the called block finishes.")]
        [SerializeField] protected CallMode callMode;

        #region Public members

        public override void OnEnter()
        {
            //Randomiser
            int randomWeightTotal = 0;
            foreach (WeightedBlockCallClass block in targetBlocks) randomWeightTotal += block.bias;
            int randomWeight = UnityEngine.Random.Range(0, randomWeightTotal);

            int curBlockIndex = 0;
            foreach(WeightedBlockCallClass block in targetBlocks)
            {
                if(randomWeight - block.bias <= 0)
                {
                    targetBlock = block.targetBlock;
                }
                else
                {
                    randomWeightTotal -= block.bias;
                }
            }
            if (targetBlock == null) Debug.LogError("Issue with the randomising system");
            //

            var flowchart = GetFlowchart();

            if (targetBlock != null)
            {
                // Check if calling your own parent block
                if (ParentBlock != null && ParentBlock.Equals(targetBlock))
                {
                    // Just ignore the callmode in this case, and jump to first command in list
                    Continue(0);
                    return;
                }

                if (targetBlock.IsExecuting())
                {
                    Debug.LogWarning(targetBlock.BlockName + " cannot be called/executed, it is already running.");
                    Continue();
                    return;
                }

                // Callback action for Wait Until Finished mode
                Action onComplete = null;
                if (callMode == CallMode.WaitUntilFinished)
                {
                    onComplete = delegate {
                        flowchart.SelectedBlock = ParentBlock;
                        Continue();
                    };
                }

                // Find the command index to start execution at
                int index = startIndex;
                if (startLabel.Value != "")
                {
                    int labelIndex = targetBlock.GetLabelIndex(startLabel.Value);
                    if (labelIndex != -1)
                    {
                        index = labelIndex;
                    }
                }

                if (targetFlowchart == null ||
                    targetFlowchart.Equals(GetFlowchart()))
                {
                    // If the executing block is currently selected then follow the execution 
                    // onto the next block in the inspector.
                    if (flowchart.SelectedBlock == ParentBlock)
                    {
                        flowchart.SelectedBlock = targetBlock;
                    }

                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    StartCoroutine(targetBlock.Execute(index, onComplete));
                }
                else
                {
                    if (callMode == CallMode.StopThenCall)
                    {
                        StopParentBlock();
                    }
                    // Execute block in another Flowchart
                    targetFlowchart.ExecuteBlock(targetBlock, index, onComplete);
                }
            }

            if (callMode == CallMode.Stop)
            {
                StopParentBlock();
            }
            else if (callMode == CallMode.Continue)
            {
                Continue();
            }
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }
        }

        public override string GetSummary()
        {
            string summary = "";

            if (targetBlock == null)
            {
                summary = "<None>";
            }
            else
            {
                summary = targetBlock.BlockName;
            }

            summary += " : " + callMode.ToString();

            return summary;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return startLabel.stringRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}

