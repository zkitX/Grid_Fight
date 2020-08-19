using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Teleport")]
public class ScriptableObjectBaseCharaterTeleport : ScriptableObjectBaseCharaterBaseMove
{
    public ParticlesType overrideTeleportParticleIn = ParticlesType.None;
    public ParticlesType overrideTeleportParticleOut = ParticlesType.None;

    [HideInInspector] public GameObject MovementPsIn;
    [HideInInspector] public GameObject MovementPsOut;

    public override Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return new Vector2Int[] { poses.First() };
    }

    public override IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        if (MovementPsOut == null)
        {
            MovementPsOut = ParticleManagerScript.Instance.GetParticle(overrideTeleportParticleOut == ParticlesType.None ? ParticlesType.Chapter01_TohoraSea_Boss_TeleportationOut : overrideTeleportParticleOut);
        }
        MovementPsOut.transform.position = CharOwner.transform.position;
        MovementPsOut.SetActive(true);

        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharOwner.CharInfo.AudioProfile.Footsteps, AudioBus.LowPrio, CharOwner.SpineAnim.transform);
        float timer = 0;
        bool inOut = false;
        while (MovementPsOut.activeInHierarchy)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
            if (timer > 0.2f && !inOut)
            {
                inOut = true;
                CharOwner.transform.position = new Vector3(100, 100, 100);
            }
        }
        timer = 0;

        if (MovementPsIn == null)
        {
            MovementPsIn = ParticleManagerScript.Instance.GetParticle(overrideTeleportParticleIn == ParticlesType.None ? ParticlesType.Chapter01_TohoraSea_Boss_TeleportationIn : overrideTeleportParticleIn);
        }
        MovementPsIn.transform.position = nextPos;
        MovementPsIn.SetActive(true);

        while (MovementPsIn.activeInHierarchy)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
            if (timer > 0.2f && inOut)
            {
                inOut = false;
                CharOwner.transform.position = nextPos;
            }
        }
        CharOwner.isMoving = false;
    }


}


