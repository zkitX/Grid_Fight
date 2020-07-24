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
                      "Match Started",
                      "The block will execute when the game starts playing.")]
    [AddComponentMenu("")]
    public class MatchStarted : EventHandler
    {
        private void Start()
        {
            BattleManagerBaseObjectGeneratorScript.Instance.StartMatchEvent += Instance_StartMatchEvent;
        }

        private void Instance_StartMatchEvent()
        {
            ExecuteBlock();
        }
    }
}
