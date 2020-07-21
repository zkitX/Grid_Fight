using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

[System.Serializable]
[CreateAssetMenu(fileName = "New_Stage_Profile", menuName = "ScriptableObjects/Stage/Stage Profile")]
public class StageProfile : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public string ID = "S0_XMPL";
    [SerializeField] public StageType type;
    [ConditionalField("type", false, StageType.Pvp)] public Sprite Thumbnail;
    [TextArea(15, 20)] [SerializeField] public string Description;
    [SerializeField] public RewardsRating bestAccuracyRating = new RewardsRating();
    [SerializeField] public RewardsRating bestReflexRating = new RewardsRating();
    [SerializeField] public RewardsRating bestDamageRating = new RewardsRating();

    [Space(5)]
    [Header("StageObjects")]
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

[System.Serializable]
public class RewardsRating
{
    public float ValueToAchieve = 1f;
    public bool UseMaximumRewardSystem = true;
    [ConditionalField("UseMaximumRewardSystem")] public float MaximumReward = 500f;

    public RewardsRating()
    {
        ValueToAchieve = 1f;
        UseMaximumRewardSystem = true;
        MaximumReward = 500f;
    }
}