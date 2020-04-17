using MyBox;
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
    public CharacterInfoScript CharInfo;
    public Vector2Int DestinationTile;
    public Vector3 DestinationWorld;
    public SideType Side;
    public FacingType Facing;
    public ElementalType Elemental;
    public List<ControllerType> PlayerController = new List<ControllerType>();
    public bool Dead = false;
    public GameObject PS;
    public GameObject TargetIndicator;
    public CharacterLevelType attackLevel;
    public bool VFXTestMode = false;
    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
    public Vector2Int BulletGapStartingTile;
    public Vector2Int StartingTile;
    public float ChildrenExplosionDelay;
    public float EffectChances;

    bool isMoving = false;
    public CastLoopImpactAudioClipInfoClass attackAudioType;
    ManagedAudioSource bulletSoundSource = null;
    public List<ScriptableObjectAttackEffect> BulletEffects = new List<ScriptableObjectAttackEffect>();

    //Private 
    private VFXBulletSpeedController vfx;
    private BattleTileScript bts;

    private void OnEnable()
    {
        //On enabled setup the collision avoidance for determinated layers 
        Physics.IgnoreLayerCollision(Side == SideType.LeftSide ? 9 : 10, Side == SideType.LeftSide ? 11 : 12);
        Dead = false;
    }

    public void StartMoveToTile()
    {
        StartCoroutine(MoveToTile());
    }

    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
	{
        if (attackAudioType.Loop.clip != null)
        {
            bulletSoundSource = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackAudioType.Loop, AudioBus.LowPriority, transform, true);
        }
        vfx = GetComponentInChildren<VFXBulletSpeedController>();
        if (vfx != null)
        {
            vfx.BulletTargetTime = CharInfo.SpeedStats.BulletSpeed;
            vfx.ApplyTargetTime();
        }

        BulletTarget();

        //setup the base offset for the movement
        Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        //Destination position
        Vector3 destination = bts.transform.position;
        //Duration of the particles 
        PS.GetComponent<PSTimeGroup>().UpdatePSTime(CharInfo.SpeedStats.BulletSpeed);
        //float Duration = Vector3.Distance(transform.position, destination) / CharInfo.BulletSpeed;
        Vector3 res;
        isMoving = true;
        float ti = 0;
        while (isMoving)
        {
            yield return new WaitForFixedUpdate();
            if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Intro) isMoving = false;

            //In case the game ended or in pause I will block the movement
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle &&
                BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets 
                && BattleManagerScript.Instance.CurrentBattleState != BattleState.End))
            {
                yield return new WaitForFixedUpdate();
            }
            //Calutation for next world position of the bullet
            res = Vector3.Lerp(offset, destination, timer);

            res.y = Trajectory_Y.Evaluate(timer) + res.y;
            res.z = Trajectory_Z.Evaluate(timer) + res.z;

            transform.position = res;
            timer += Time.fixedDeltaTime / CharInfo.SpeedStats.BulletSpeed;
            ti += Time.fixedDeltaTime;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
                isMoving = false;
                //StartCoroutine(ChildExplosion(BulletEffectTiles.Where(r=> r != Vector2Int.zero).ToList()));
                FireEffectParticles(bts.transform.position, true);//BulletEffectTiles.Count == 1 ? true : false
            }
        }
    }


    public void BulletTarget()
    {

        GetComponent<BoxCollider>().enabled = true;
        int startingYTile = Facing == FacingType.Left ? StartingTile.y - BulletGapStartingTile.y : StartingTile.y + BulletGapStartingTile.y;
        GameObject go = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Particles);
        go.transform.position = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position;
        go.GetComponent<BattleTileTargetScript>().StartTarget(
            (Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position) * CharInfo.SpeedStats.BulletSpeed) /
            Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing).transform.position));
        bts = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(DestinationTile, Facing);
        float duration = CharInfo.SpeedStats.BulletSpeed;
        foreach (Vector2Int item in BulletEffectTiles)
        {
            if (GridManagerScript.Instance.isPosOnField(DestinationTile + item))
            {
                BattleTileScript btsT = GridManagerScript.Instance.GetBattleTile(DestinationTile + item, Facing == FacingType.Left ? WalkingSideType.LeftSide : WalkingSideType.RightSide);
                if (btsT != null)
                {

                    go = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Particles);
                    go.transform.position = btsT.transform.position;
                    go.GetComponent<BattleTileTargetScript>().StartTarget(duration);
                }
            }
        }
    }

    public IEnumerator ChildExplosion(List<Vector2Int> bet, Vector2Int basePos)
    {
        float timer = 0;
        BaseCharacter target;

        bulletSoundSource = null;

        while (timer < ChildrenExplosionDelay)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End))
            {
                yield return new WaitForFixedUpdate();
            }
        }
       
        for (int i = 0; i < bet.Count; i++)
        {
            if (GridManagerScript.Instance.isPosOnField(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]))
            {
                if (!VFXTestMode)
                {
                    target = BattleManagerScript.Instance.GetCharInPos(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]);
                    MakeDamage(target);
                }
                FireEffectParticles(GridManagerScript.Instance.GetBattleTile(Side == SideType.LeftSide ? basePos + bet[i] : basePos - bet[i]).transform.position, i == bet.Count - 1 ? true : false);
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


    public void MakeDamage(BaseCharacter target)
    {
        if (target != null)
        {
            if (target.tag.Contains("Side") && target.tag != Side.ToString())
            {
                bool iscritical = CharInfo.IsCritical(attackLevel == CharacterLevelType.Novice ? true : false);
                //Set damage to the hitting character
                if(attackLevel == CharacterLevelType.Godness)
                {
                    CameraManagerScript.Instance.CameraShake(CameraShakeType.PowerfulAttackHit);
                }
                
                target.SetDamage((CharInfo.DamageStats.BaseDamage * (attackLevel == CharacterLevelType.Novice ? CharInfo.RapidAttack.DamageMultiplier.x : CharInfo.PowerfulAttac.DamageMultiplier.x)) * (iscritical ? 2 : 1),
                    Elemental, iscritical, CharInfo.ClassType == CharacterClassType.Desert && attackLevel == CharacterLevelType.Godness ? true : false);

                int chances = Random.Range(0, 100);
                if (chances < 100) //TODO: Setup actual chances calculations
                {
                    foreach (ScriptableObjectAttackEffect item in BulletEffects)
                    {
                        target.Buff_DebuffCo(new Buff_DebuffClass(item.Name, item.Duration.x, item.Value.x, item.StatsToAffect, item.StatsChecker, new ElementalResistenceClass(), ElementalType.Dark, item.AnimToFire, item.Particles));
                    }
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //If the bullet collide with a character 
        if(other.tag.Contains("Side") && other.tag != Side.ToString() && CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script && isMoving) 
        {
            isMoving = false;
            BaseCharacter target = other.GetComponentInParent<BaseCharacter>();
            MakeDamage(target);
            //fire the Effect
            StartCoroutine(ChildExplosion(BulletEffectTiles.Where(r => r != Vector2Int.zero).ToList(), target.UMS.CurrentTilePos));
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackAudioType.Impact, AudioBus.HighPriority,
                GridManagerScript.Instance.GetBattleTile(target.UMS.CurrentTilePos).transform);
            if (bulletSoundSource != null) bulletSoundSource.ResetSource();
            FireEffectParticles(transform.position, BulletEffectTiles.Count > 1 || (CharInfo.ClassType == CharacterClassType.Valley && attackLevel == CharacterLevelType.Godness)  ? false : true);
        
        }

    }

    public void FireEffectParticles(Vector3 pos, bool destroyBullet)
    {
        if(!Dead)
        {
            //fire the Effect
            GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.EffectLeft, pos, Side);
            LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
            if (lps != null)
            {
                lps.Shot = attackLevel;
                lps.SelectShotLevel();

            }
            if(destroyBullet)
            {
                Dead = true;
                StopAllCoroutines();
                Invoke("test", 2);
                PS.GetComponent<PSTimeGroup>().UpdatePSTime(0.1f);
                if (bulletSoundSource != null) bulletSoundSource.ResetSource();
                bulletSoundSource = null;
            }
        }
    }


    void test()
    {
        
        PS.GetComponent<DisableParticleScript>().ResetParticle();
        gameObject.SetActive(false);
    }

}

