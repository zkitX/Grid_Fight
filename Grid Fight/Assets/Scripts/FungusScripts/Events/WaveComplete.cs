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
                      "wave Started",
                      "The block will execute when the game starts playing.")]
    [AddComponentMenu("")]
    public class WaveComplete : EventHandler
    {
        private void Start()
        {
            WaveManagerScript.Instance.WaveCompleteEvent += Instance_WaveCompleteEvent; 
        }

        private void Instance_WaveCompleteEvent(string startBlockName)
        {
            StartCoroutine(StartNextBlock(startBlockName));
        }

        private IEnumerator StartNextBlock(string startBlockName)
        {
            yield return BlockTriggeredWithCallBack(startBlockName);
        }
    }
}
