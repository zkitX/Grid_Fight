using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutorialScript : MonoBehaviour
{
    private BattleState previousBattleState = BattleState.Battle;    
    private bool isSetup = false;
    // Update is called once per frame

    [SerializeField] UnityEngine.Audio.AudioMixerSnapshot snapshot_Default;
    [SerializeField] [Range(0f, 3f)] float snapshot_DefaultTransition = 0.2f;
    [SerializeField] UnityEngine.Audio.AudioMixerSnapshot snapshot_Pause;
    [SerializeField] [Range(0f, 3f)] float snapshot_PauseTransition = 0.2f;

    void Update()
    {
        if (!isSetup && InputController.Instance != null && UserInputManager.Instance != null && UserInputManager.Instance.IsReadyToBeSetUp)
        {
            isSetup = true;
            InputController.Instance.ButtonMinusUpEvent += Instance_ButtonMinusUpEvent;
            InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;
        }
    }

    private void Instance_ButtonMinusUpEvent(int player)
    {
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.Menus_PressButton, AudioBus.MidPrio);

        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.CurrentBattleState = previousBattleState;
            if (snapshot_Default != null) snapshot_Default.TransitionTo(snapshot_DefaultTransition);
        }
        else
        {
            previousBattleState = BattleManagerScript.Instance.CurrentBattleState;
            BattleManagerScript.Instance.CurrentBattleState = BattleState.Pause;
            if (snapshot_Pause != null) snapshot_Pause.TransitionTo(snapshot_PauseTransition);
        }
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    private void Instance_ButtonPlusUpEvent(int player)
    {
        if (gameObject.activeInHierarchy)
        {
            BattleManagerScript.Instance.RestartScene();
        }
    }
}
