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
                      "Boss Appear",
                      "The block will execute when the Boss appear.")]
    [AddComponentMenu("")]
    public class BossAppear : EventHandler
    {
        private void Start()
        {
            WaveManagerScript.Instance.WaveBossApperEvent += Instance_WaveBossApperEvent1;
        }

        private void Instance_WaveBossApperEvent1(MinionType_Script boss)
        {
            StartCoroutine(StartBossDialog(boss));
        }

        private IEnumerator StartBossDialog(MinionType_Script boss)
        {
            yield return BlockTriggeredWithCallBack("BOSS ARRRIVED");

            boss.SetValueFromVariableName("DialogueComplete", true);
        }

    }
}
