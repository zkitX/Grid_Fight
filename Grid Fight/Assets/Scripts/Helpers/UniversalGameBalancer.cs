using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGameBalancer : MonoBehaviour
{
    public static UniversalGameBalancer Instance;

    [Header("Defending")]
    [Tooltip("The set cost incurred in the character's defence bar, each time they start defending")] public float defenceCost = 20f;
    [Tooltip("The set cost incurred in the character's defence bar, each time they block an incoming attack imprefectly")] public float partialDefenceCost = 10f;
    [Tooltip("The set amount of stamina regained by the character upon timing a defence perfectly")] public float staminaRegenOnPerfectBlock = 10f;

    private void Awake()
    {
        Instance = this;
    }
}
