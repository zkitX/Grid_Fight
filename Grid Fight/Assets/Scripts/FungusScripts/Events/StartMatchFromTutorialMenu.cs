// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game starts playing.
    /// </summary>
    [EventHandlerInfo("",
                      "Start Match from Tutorial",
                      "The block will execute when the game starts playing.")]
    [AddComponentMenu("")]
    public class StartMatchFromTutorialMenu : EventHandler
    {
        private void Start()
        {
            BattleManagerScript.Instance.CurrentBattleStateChangedEvent += Instance_CurrentBattleStateChangedEvent; 
        }

        private void Instance_CurrentBattleStateChangedEvent(BattleState currentBattleState)
        {
            if(currentBattleState != BattleState.Tutorial)
            {
                ExecuteBlock();
            }
        }
    }
}
