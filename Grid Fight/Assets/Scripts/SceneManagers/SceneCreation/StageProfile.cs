using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Stage_Profile", menuName = "ScriptableObjects/Stage/Stage Profile")]
public class StageProfile : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public string ID = "S0_XMPL";
    [SerializeField] public GameObject Rewired;
    [SerializeField] public GameObject BattleInfoManager;
    [SerializeField] public GameObject AudioManager;
    [SerializeField] public GameObject BattleManager;
    [SerializeField] public GameObject BaseEnvironment;
    [SerializeField] public GameObject UI_Battle;
    [SerializeField] public GameObject EventManager;
    [SerializeField] public GameObject Wave;
}
