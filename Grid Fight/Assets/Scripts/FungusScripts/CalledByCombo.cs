using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game sequence event calls it
    /// </summary>
    [EventHandlerInfo("",
                      "Called by Combo",
                      "A combo threshold being reached triggered this block")]
    [AddComponentMenu("")]
    public class CalledByCombo : EventHandler
    {
        private void Awake()
        {
            ComboManager.OnFungusEventTrigger += BlockTriggered;
            // EventEffect.eventBlocksToTrigger.Add(ParentBlock);
        }




    }
}