using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is the component that take care of all the Bullet behaviours in the game
/// </summary>
public class BulletScript : MonoBehaviour
{
    //Public
    public BaseCharacter CharOwner;
    public Vector2Int DestinationTile;
    public Vector3 DestinationWorld;
    public SideType Side;
    public FacingType Facing;
    public ElementalType Elemental;
    public GameObject PS;
    public GameObject TargetIndicator;
    public CastLoopImpactAudioClipInfoClass attackAudioType;
    ManagedAudioSource bulletSoundSource = null;
    public List<ScriptableObjectAttackEffect> BulletEffects = new List<ScriptableObjectAttackEffect>();
    bool isMoving = false;
    public bool isColliding = true;
    public float BulletDuration;
	IEnumerator movingCo;
	public ScriptableObjectAttackBase SOAttack;
    public BulletBehaviourInfoClass BulletBehaviourInfo;
    public BulletBehaviourInfoClassOnBattleFieldClass BulletBehaviourInfoTile;
    private bool SkillHit = false;
    //Private 
    private VFXBulletSpeedController vfx;
    private BattleTileScript bts;
    
    private Vector2Int StartingTile;
    private void OnEnable()
    {
        //On enabled setup the collision avoidance for determinated layers 
        Physics.IgnoreLayerCollision(Side == SideType.LeftSide ? 9 : 10, Side == SideType.LeftSide ? 11 : 12);
    }

    public void StartMoveToTile()
    {
        hitTarget = false;
        movingCo = MoveToTile();
        StartCoroutine(movingCo);
    }
    float timet = 0;
    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
    {
        if (attackAudioType.Loop.clip != null)
        {
            bulletSoundSource = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackAudioType.Loop, AudioBus.LowPrio, transform, true);
        }
        SkillHit = false;
        StartingTile = CharOwner.UMS.CurrentTilePos;
        CharOwner.Sic.BulletFired++;
       // Debug.Log(CharOwner.Sic.BulletFired);
        vfx = GetComponentInChildren<VFXBulletSpeedController>();
        if (vfx != null)
        {
            vfx.BulletTargetTime = SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletSpeed : CharOwner.CharInfo.SpeedStats.StrongBulletSpeed;
            vfx.ApplyTargetTime();
        }

        BulletTarget();

        //setup the base offset for the movement
        Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        //Destination position
        Vector3 destination = bts.transform.position + new Vector3(Side == SideType.LeftSide ? 0.2f : -0.2f, 0, 0);
        if (isColliding)
        {
            BulletDuration = ((SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletSpeed : CharOwner.CharInfo.SpeedStats.StrongBulletSpeed) / 12) * Mathf.Abs(bts.Pos.y - (StartingTile.y));
        }
        //Duration of the particles 
        PS = ParticleManagerScript.Instance.FireParticlesInTransform(CharOwner.UMS.Side == SideType.LeftSide ? SOAttack.Particles.Left.Bullet : SOAttack.Particles.Right.Bullet, CharOwner.CharInfo.CharacterID, AttackParticlePhaseTypes.Bullet, transform, CharOwner.UMS.Side,
          SOAttack.AttackInput, BulletDuration + 0.5f, iter);
        ParticleHelperScript pstg = PS.GetComponent<ParticleHelperScript>();
      //  PS.SetActive(true);

        //float Duration = Vector3.Distance(transform.position, destination) / CharOwner.CharInfo.BulletSpeed;
        Vector3 res;
        isMoving = true;
        timet = +Time.time;
        pstg.timet = BulletDuration + 0.5f;

        //Debug.Log("Bullet_1   " + timet + "   " + BulletDuration + 0.5f);
        while (isMoving)
        {
            yield return new WaitForFixedUpdate();
            if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Intro) isMoving = false;

            //In case the game ended or in pause I will block the movement
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle &&
                BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets
                && BattleManagerScript.Instance.CurrentBattleState != BattleState.End)
            {
                yield return null;
            }
            //Calutation for next world position of the bullet
            res = Vector3.Lerp(offset, destination, timer);

            res.y = BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Y.Evaluate(timer) + res.y : BulletBehaviourInfoTile.Trajectory_Y.Evaluate(timer) + res.y;
            res.z = BulletBehaviourInfo != null ? BulletBehaviourInfo.Trajectory_Z.Evaluate(timer) + res.z : BulletBehaviourInfoTile.Trajectory_Z.Evaluate(timer) + res.z;

            transform.position = res;
            timer += BattleManagerScript.Instance.FixedDeltaTime / BulletDuration;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
                isMoving = false;

                if(isColliding)
                {
                    //StartCoroutine(ChildExplosion(BulletEffectTiles.Where(r=> r != Vector2Int.zero).ToList()));
                    FireEffectParticles(bts.transform.position);//BulletEffectTiles.Count == 1 ? true : false
                }
                
            }
        }
        //Debug.Log("Bullet_2   " + timet + "    " + Time.time);
        EndBullet(1f);
    }

    GameObject go;
    public void BulletTarget()
    {
        GetComponent<BoxCollider>().enabled = isColliding;
        int startingYTile = 0;
        if (BulletBehaviourInfo != null)
        {
            startingYTile = Facing == FacingType.Left ? StartingTile.y - BulletBehaviourInfo.BulletGapStartingTile.y : StartingTile.y + BulletBehaviourInfo.BulletGapStartingTile.y;
        }
        else
        {
            startingYTile = StartingTile.y;
        }

        if (isColliding)
        {
            go = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Particles);
            go.transform.position = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position;
            go.GetComponent<BattleTileTargetScript>().StartTarget(
                (Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position) * (SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletSpeed : CharOwner.CharInfo.SpeedStats.StrongBulletSpeed)) /
                Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position));
            
            foreach (Vector2Int item in BulletBehaviourInfo.BulletEffectTiles)
            {
                if (GridManagerScript.Instance.isPosOnField(DestinationTile + item))
                {
                    BattleTileScript btsT = GridManagerScript.Instance.GetBattleTile(DestinationTile + item, Facing == FacingType.Left ? WalkingSideType.LeftSide : WalkingSideType.RightSide);
                    if (btsT != null)
                    {

                        go = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Particles);
                        go.transform.position = btsT.transform.position;
                        go.GetComponent<BattleTileTargetScript>().StartTarget(SOAttack.AttackInput == AttackInputType.Weak ? CharOwner.CharInfo.SpeedStats.WeakBulletSpeed : CharOwner.CharInfo.SpeedStats.StrongBulletSpeed);
                    }
                }
            }
        }
       
        bts = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing);
        
    }

    public IEnumerator ChildExplosion(List<Vector2Int> bet, Vector2Int basePos)
    {
        BaseCharacter target;
        bulletSoundSource = null;

        yield return BattleManagerScript.Instance.WaitFor(BulletBehaviourInfo.ChildrenBulletDelay, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

        for (int i = 0; i < bet.Count; i++)
        {
            if (GridManagerScript.Instance.isPosOnField(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]))
            {
                target = BattleManagerScript.Instance.GetCharInPos(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]);
                MakeDamage(target, CharOwner.NextAttackDamage * 0.3f);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackAudioType.Impact, AudioBus.HighPrio, GridManagerScript.Instance.GetBattleTile(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]).transform);
                FireEffectParticles(GridManagerScript.Instance.GetBattleTile(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]).transform.position);
            }
            else
            {
                /* Vector3 dest = new Vector3(bet[i].y * GridManagerScript.Instance.GetWorldDistanceBetweenTiles() * (-1),
                     bet[i].x * GridManagerScript.Instance.GetWorldDistanceBetweenTiles() * (-1), 0);
                 FireEffectParticles(GridManagerScript.Instance.GetBattleTile(basePos).transform.position
                     + dest, i == bet.Count - 1 ? true : false);*/
            }
        }
    }

    public int iter = 0;
    bool hitTarget = false;
    public void MakeDamage(BaseCharacter target, float baseDamage)
    {
        if (target != null)
        {
            if (target.tag.Contains("Side") && target.tag != Side.ToString())
            {
                hitTarget = true;
                CharOwner.Sic.AccuracyExp += 1f;
                CharOwner.Sic.BulletHits++;
                ComboManager.Instance.TriggerComboForCharacter(CharOwner.CharInfo.CharacterID, ComboType.Attack, true, target.transform.position);
                bool iscritical = CharOwner.CharInfo.IsCritical(SOAttack.AttackInput == AttackInputType.Weak ? true : false);
                //Set damage to the hitting character
                if (SOAttack.AttackInput != AttackInputType.Weak)
                {
                    CameraManagerScript.Instance.CameraShake(CameraShakeType.PowerfulAttackHit);
                }
               // if((SOAttack.AttackInput >= AttackInputType.Strong ? Random.Range(CharOwner.CharInfo.StrongAttack.Chances.x, CharOwner.CharInfo.StrongAttack.Chances.y) :
                   // Random.Range(CharOwner.CharInfo.WeakAttack.Chances.x, CharOwner.CharInfo.WeakAttack.Chances.y)) >= Random.Range(0f, 1f))
                {
                    target.SetDamage(CharOwner, CharOwner.NextAttackDamage * (iscritical ? 2 : 1),
                   Elemental, iscritical, CharOwner.CharInfo.ClassType == CharacterClassType.Desert && SOAttack.AttackInput != AttackInputType.Weak ? true : false);
                    CharOwner.Sic.DamageExp += CharOwner.NextAttackDamage;
                    if (!SkillHit && SOAttack.AttackInput > AttackInputType.Strong)
                    {
                        SkillHit = true;
                        StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == CharOwner.CharInfo.CharacterID).First();
                    }

                    if (target.CharInfo.Health > 0)
                    {
                        int chances = Random.Range(0, 100);
                        if (chances < 100)
                        {
                            foreach (ScriptableObjectAttackEffect item in BulletEffects)
                            {
                                if (item != null)
                                {
                                    target.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Dark, CharOwner, item));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator BlockTileForTime(float duration, Vector2Int pos)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        float timer = 0;
        bts.BattleTileState = BattleTileStateType.NonUsable;
        while (timer < duration)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
        }
        bts.BattleTileState = BattleTileStateType.Empty;
    }


    private void OnTriggerEnter(Collider other)
      {
        //If the bullet collide with a character 
        if (other.tag.Contains("Side") && other.tag != Side.ToString() && CharOwner.CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && isMoving)
        {
            isMoving = false;
            BaseCharacter target = other.GetComponentInParent<BaseCharacter>();
            MakeDamage(target, CharOwner.NextAttackDamage);
            //fire the Effect
            StartCoroutine(ChildExplosion(BulletBehaviourInfo.BulletEffectTiles.Where(r => r != Vector2Int.zero).ToList(), new Vector2Int(DestinationTile.x, target.UMS.CurrentTilePos.y)));
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackAudioType.Impact, AudioBus.MidPrio,
            GridManagerScript.Instance.GetBattleTile(target.UMS.CurrentTilePos).transform);
            FireEffectParticles(transform.position);
        }
    }



    public void FireEffectParticles(Vector3 pos)
    {
        //fire the Effect
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(Side == SideType.LeftSide ? SOAttack.Particles.Left.Hit : SOAttack.Particles.Right.Hit, CharOwner.CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, pos, Side, SOAttack.AttackInput);
    }

    private void EndBullet(float timer)
    {
        //Debug.Log("Bullet_3   " + timet);
        Invoke("RestoreBullet", timer);
        PS.transform.parent = null;
        PS.GetComponent<ParticleHelperScript>().UpdatePSTime(0.1f, 0);
        if (!hitTarget && SOAttack.AttackInput != AttackInputType.Strong)
        {
            ComboManager.Instance.TriggerComboForCharacter(CharOwner.CharInfo.CharacterID, ComboType.Attack, false);
        }
    }


    private void GiveExperience()
    {

    }

    void RestoreBullet()
    {
        //Debug.Log("Bullet_4   " + timet);
        if (bulletSoundSource != null)
        {
            bulletSoundSource.ResetSource();
        }
        bulletSoundSource = null;
        //PS.GetComponent<ParticleHelperScript>().ResetParticle();
        CancelInvoke("RestoreBullet");
        gameObject.SetActive(false);
    }
}

