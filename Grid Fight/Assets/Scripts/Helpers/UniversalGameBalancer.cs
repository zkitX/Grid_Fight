using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalGameBalancer : MonoBehaviour
{
    public static UniversalGameBalancer Instance;

    [Header("Defending")]
    [Tooltip("The set cost incurred in the character's defence bar, each time they start defending")] public float defenceCost = 20f;
    [Tooltip("The set cost incurred in the character's defence bar, each time they block an incoming attack imprefectly")] public float partialDefenceCost = 10f;
    [Tooltip("The set cost incurred in the character's defence bar, each time they block an incoming attack perfectly")] public float fullDefenceCost = 10f;
    [Tooltip("The set amount of stamina regained by the character upon timing a defence perfectly")] public float staminaRegenOnPerfectBlock = 10f;


    [Header("Stage Jumping")]
    [Tooltip("A curve that determines the movement speed of the camera between the first and second stage positions")] public AnimationCurve cameraTravelCurve;
    [Tooltip("A curve that determines the jump height of the characters throughout the stage movement")] public AnimationCurve characterJumpCurve;
    [Tooltip("A curve that determines the animation playback speed of the jump animation across the duration of the jump")] public AnimationCurve jumpAnimationCurve;

    private void Awake()
    {
        Instance = this;
    }
}
