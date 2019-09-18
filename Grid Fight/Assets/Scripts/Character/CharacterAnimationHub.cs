using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationHub : MonoBehaviour
{
    public AnimationCurve UpMovementSpeed;
    public AnimationCurve DownMovementSpeed;
    public AnimationCurve LeftMovementSpeed;
    public AnimationCurve RightMovementSpeed;

    public CharacterBase CharOwner;
}
