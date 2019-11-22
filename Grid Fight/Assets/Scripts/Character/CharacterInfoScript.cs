using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contains the basic info of the character
/// </summary>
public class CharacterInfoScript : MonoBehaviour
{
    public CharacterNameType CharacterName;
    public Sprite CharacterIcon;
    public CharacterClassType ClassType;
    public AttackParticleTypes ParticleType;
    public List<ElementalType> Elemental = new List<ElementalType>();
  //  public AnimationCurve Trajectory_Y;
  //  public AnimationCurve Trajectory_Z;
    public float BulletSpeed = 1;
   // public List<Vector2Int> BulletDistanceInTile = new List<Vector2Int>();
    public float Damage = 10;
    public int MultiBulletAttackNumberOfBullets = 3;

}
