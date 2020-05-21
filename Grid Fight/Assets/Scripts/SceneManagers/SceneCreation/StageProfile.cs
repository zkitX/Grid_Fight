using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Stage_Profile", menuName = "ScriptableObjects/Stage/Stage Profile")]
public class StageProfile : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public string ID = "S0_XMPL";
    [TextArea(15, 20)] [SerializeField] public string Description;
    [SerializeField] public GameObject Rewired;
    [SerializeField] public GameObject BattleInfoManager;
    [SerializeField] public GameObject BattleManager;
    [SerializeField] public GameObject BaseEnvironment;
    [SerializeField] public GameObject UI_Battle;
    [SerializeField] public GameObject EventManager;
    [SerializeField] public GameObject Wave;

    [SerializeField] public GameObject AudioManager;
    [SerializeField] public StageAudioProfileSO StageAudioProfile;

}
