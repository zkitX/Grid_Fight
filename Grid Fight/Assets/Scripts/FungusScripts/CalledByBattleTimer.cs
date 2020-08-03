using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game sequence event calls it
    /// </summary>
    [EventHandlerInfo("",
                      "Called by Battle Timer",
                      "The Battle Timer completing triggered this block")]
    [AddComponentMenu("")]
    public class CalledByBattleTimer : EventHandler
    {
        private void Awake()
        {
            StartCoroutine(WaitForWaveManager());
        }

        IEnumerator WaitForWaveManager()
        {
            while(WaveManagerScript.Instance == null)
            {
                yield return null;
            }

            WaveManagerScript.Instance.OnBattleTimerComplete += BlockTriggered;
        }
    }
}