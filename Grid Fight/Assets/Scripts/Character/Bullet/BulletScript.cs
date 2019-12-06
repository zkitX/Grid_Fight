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
	public ControllerType ControllerT;
    public bool Dead = false;
    public GameObject PS;
    public GameObject TargetIndicator;
    public List<GameObject> UsedTargets = new List<GameObject>();
    public CharacterLevelType attackLevel;
    public bool VFXTestMode = false;
    public AnimationCurve Trajectory_Y;
    public AnimationCurve Trajectory_Z;
    public List<Vector2Int> BulletEffectTiles = new List<Vector2Int>();
    public Vector2Int BulletGapStartingTile;
    public Vector2Int StartingTile;
    public float ChildrenExplosionDelay;
    private VFXBulletSpeedController vfx;
    //Private 
    private BattleTileScript bts;

    private void Update()
    {

        //Stop the bullet when the match ended
        /* if(BattleManagerScript.Instance.CurrentBattleState == BattleState.End)
         {
             StartCoroutine(SelfDeactivate(0));
         }*/
    }

    //Self deactivation method with a delay parameter
    public IEnumerator SelfDeactivate(float delay)
	{
		float timer = 0;
		while (timer < delay && !Dead)
        {
			timer += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End)
            {
				yield return new WaitForFixedUpdate();
            }
        }
        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>())
        {
            item.time = 0;
        }
		
        Dead = true;
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //On enabled setup the collision avoidance for determinated layers 
        Physics.IgnoreLayerCollision(Side == SideType.LeftSide ? 9 : 10, Side == SideType.LeftSide ? 11 : 12);
        
    }

    public void StartMoveToTile()
    {
        StartCoroutine(MoveToTile());
    }

    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
	{
        vfx = GetComponentInChildren<VFXBulletSpeedController>();
        if (vfx != null)
        {
            vfx.BulletTargetTime = CharInfo.BulletSpeed;
            vfx.ApplyTargetTime();
        }
        if (CharInfo.ClassType != CharacterClassType.Mountain)
        {
            int startingYTile = Facing == FacingType.Left ? StartingTile.y - BulletGapStartingTile.y : StartingTile.y + BulletGapStartingTile.y;
            foreach (BattleTileScript item in GridManagerScript.Instance.GetBattleTileInARowToDestination(DestinationTile, Facing, startingYTile))
            {
                GameObject go = UsedTargets.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
                if(go == null)
                {
                    go = Instantiate(TargetIndicator, item.transform.position, Quaternion.identity);
                    go.GetComponent<BattleTileTargetScript>().StartTarget(
                        (Vector3.Distance(transform.position, item.transform.position) * CharInfo.BulletSpeed) /
                        Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position));
                }
                else
                {
                    go.transform.position = GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position;
                    go.SetActive(true);
                }
                UsedTargets.Add(go);
            }
        }
        else if (CharInfo.ClassType == CharacterClassType.Mountain)
        {
            GetComponent<BoxCollider>().enabled = false;
            int ran = Random.Range(0, 101);
            DestinationTile.y = ran < 25 ? DestinationTile.y - 1 : ran < 75 ? DestinationTile.y : DestinationTile.y + 1;
            bts = GridManagerScript.Instance.GetBattleTile(DestinationTile);
            float duration = CharInfo.BulletSpeed;
            foreach (Vector2Int item in BulletEffectTiles)
            {
                if(GridManagerScript.Instance.isPosOnField(DestinationTile + item))
                {
                    bts = GridManagerScript.Instance.GetBattleTile(DestinationTile + item, Facing == FacingType.Left ? SideType.LeftSide : SideType.RightSide);
                    if(bts != null)
                    {
                        GameObject go = Instantiate(TargetIndicator, bts.transform.position, Quaternion.identity);
                        go.GetComponent<BattleTileTargetScript>().StartTarget(duration);
                        UsedTargets.Add(go);
                    }
                }
            }
        }

        //setup the base offset for the movement
		Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        //Destination battle tile
        bts = GridManagerScript.Instance.GetBattleTile(DestinationTile);
        //Destination position
        Vector3 destination = bts.transform.position;
        //Duration of the particles 
       // float Duration = Vector3.Distance(transform.position, destination) / CharInfo.BulletSpeed;
        Vector3 res;
        bool isMoving = true;
        while (isMoving)
        {
            yield return new WaitForFixedUpdate();
            //In case the game ended or in pause I will block the movement
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End))
            {
                yield return new WaitForFixedUpdate();
            }
            //Calutation for next world position of the bullet
            res = Vector3.Lerp(offset, destination, timer);

            res.y = Trajectory_Y.Evaluate(timer) + res.y;
            res.z = Trajectory_Z.Evaluate(timer) + res.z;

            transform.position = res;
            timer += Time.fixedDeltaTime / CharInfo.BulletSpeed;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
                isMoving = false;
                FireEffectParticles(GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position, BulletEffectTiles.Count == 1 ? true : false);
                StartCoroutine(ChildExplosion(BulletEffectTiles.Where(r=> r != Vector2Int.zero).ToList()));
            }
        }
    }

    public IEnumerator ChildExplosion(List<Vector2Int> bet)
    {
        float timer = 0;
        CharacterBase target = target = BattleManagerScript.Instance.GetCharInPos(DestinationTile);
        MakeDamage(target);
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
            if (GridManagerScript.Instance.isPosOnField(DestinationTile + bet[i]))
            {
                target = BattleManagerScript.Instance.GetCharInPos(DestinationTile + bet[i]);
                MakeDamage(target);
                FireEffectParticles(GridManagerScript.Instance.GetBattleTile(DestinationTile + bet[i]).transform.position, i == bet.Count - 1 ? true : false);
            }
            else
            {
                Vector3 dest = new Vector3(bet[i].y * GridManagerScript.Instance.GetWorldDistanceBetweenTiles() * (-1),
                    bet[i].x * GridManagerScript.Instance.GetWorldDistanceBetweenTiles() * (-1), 0);
                FireEffectParticles(GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position
                    + dest, i == bet.Count - 1 ? true : false);
            }

            
        }

    }


    public void MakeDamage(CharacterBase target)
    {
        if (target != null)
        {
            //Set damage to the hitting character
            target.SetDamage(CharInfo.DamageStats.CurrentDamage, Elemental);
        }
    }


    //Move the bullet on a straight movement 
    public IEnumerator MoveToWorldPos()
    {
        vfx = GetComponentInChildren<VFXBulletSpeedController>();
        if (vfx != null)
        {
            vfx.BulletTargetTime = CharInfo.BulletSpeed;
            vfx.ApplyTargetTime();
        }
        //setup the base offset for the movement
        Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        Vector3 res;
       // float Duration = Vector3.Distance(transform.position, DestinationWorld) / CharInfo.BulletSpeed;
        while (!Dead)
        {

            yield return new WaitForFixedUpdate();
            //In case the game ended or in pause I will block the movement
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End))
            {
                yield return new WaitForFixedUpdate();
            }
            //Calutation for next world position of the bullet
            res = Vector3.Lerp(offset, DestinationWorld, timer);

            res.y = Trajectory_Y.Evaluate(timer) + res.y;
            res.z = Trajectory_Z.Evaluate(timer) + res.z;

            transform.position = res;
            timer += Time.fixedDeltaTime / CharInfo.BulletSpeed;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
               
                FireEffectParticles(transform.position, true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the bullet collide with a character 
        if(other.tag.Contains("Side") && other.tag != Side.ToString()) 
        {
            CharacterBase target = other.GetComponentInParent<CharacterBase>();
            MakeDamage(target);
            //fire the Effect
            FireEffectParticles(transform.position, CharInfo.ClassType == CharacterClassType.Desert ? false : true);
        }
    }

    public void FireEffectParticles(Vector3 pos, bool destroyBullet)
    {
        if(!Dead)
        {
            //fire the Effect
            GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, ParticleTypes.Effect, pos, Side);
            LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
            if (lps != null)
            {
                lps.Shot = attackLevel;
                lps.SelectShotLevel();

            }
            if(destroyBullet)
            {
                foreach (GameObject item in UsedTargets)
                {
                    Destroy(item);
                }
                PS.GetComponent<DisableParticleScript>().ResetParticle();
                PS.SetActive(false);
                PS.transform.parent = null;
                Dead = true;
                StopAllCoroutines();
                Destroy(this.gameObject);
            }
                
        }
    }

}

