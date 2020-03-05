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

    StageProfile()
    {
       /* if (Rewired == null) Rewired = null;
        if (Rewired == null) BattleInfoManager = null;
        if (Rewired == null) AudioManager = null;
        if (Rewired == null) BattleManager = null;
        if (Rewired == null) BaseEnvironment = null;
        if (Rewired == null) UI_Battle = null;
        if (Rewired == null) Wave = null;
        if (Rewired == null) EventManager = Resources.Load("Default_Stage_File").;*/
    }
}
