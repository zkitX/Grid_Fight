using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetReadyAudioTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.GetReadyFight, AudioBus.HighPrio);
    }
}
