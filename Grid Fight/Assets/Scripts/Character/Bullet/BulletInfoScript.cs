using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletInfoScript : MonoBehaviour
{
    public AttackType AttackT;
    public AttackParticleTypes ParticleType;
    public List<ElementalType> Elemental = new List<ElementalType>();
    public AnimationCurve TrajectoryHeightUp;
    public int MultiBulletAttackAngle;
    public int MultiBulletAttackNumberOfBullets;
}
