using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game sequence event calls it
    /// </summary>
    [EventHandlerInfo("",
                      "Called by Grid UI Activator",
                      "A UI Element is triggering a fungus block")]
    [AddComponentMenu("")]
    public class CalledByGridUIActivator : EventHandler
    {
        private void Awake()
        {
            CharSelectSelector.OnFungusEventTrigger += BlockTriggered;
            // EventEffect.eventBlocksToTrigger.Add(ParentBlock);
        }




    }
}