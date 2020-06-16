// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game sequence event calls it
    /// </summary>
    [EventHandlerInfo("",
                      "Call MatchLost",
                      "")]
    [AddComponentMenu("")]
    public class MatchLost : EventHandler
    {
        private void Awake()
        {
            BattleManagerScript.Instance.MatchLostEvent += Instance_MatchLostEvent;
        }

        private void Instance_MatchLostEvent()
        {
            ExecuteBlock();
        }
    }
}