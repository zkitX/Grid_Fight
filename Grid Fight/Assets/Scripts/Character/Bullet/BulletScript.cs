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
    public ElementalType Elemental;
	public ControllerType ControllerT;
    public bool Dead = false;
    public GameObject PS;
    public GameObject TargetIndicator;
    public List<GameObject> UsedTargets = new List<GameObject>();


    //Private 
    private BattleTileScript bts;


    private void Update()
    {
        //Stop the bullet when the match ended
        if(BattleManagerScript.Instance.CurrentBattleState == BattleState.End)
        {
            StartCoroutine(SelfDeactivate(0));
        }
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

    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
	{
        if (CharInfo.ClassType == CharacterClassType.Valley)
        {
            foreach (BattleTileScript item in GridManagerScript.Instance.GetBattleTileInARowToDestination(DestinationTile, Side))
            {
                GameObject go = UsedTargets.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
                if(go == null)
                {
                    go = Instantiate(TargetIndicator, item.transform.position, Quaternion.identity);
                    go.GetComponent<BattleTileTargetScript>().StartTarget(Vector3.Distance(transform.position, item.transform.position) / CharInfo.BulletSpeed);
                }
                else
                {
                    go.transform.position = GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position;
                    go.SetActive(true);
                }
                UsedTargets.Add(go);
            }
        }
        else
        {
            GameObject go = UsedTargets.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
            if (go == null)
            {
                go = Instantiate(TargetIndicator, GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position, Quaternion.identity);
                go.GetComponent<BattleTileTargetScript>().StartTarget(Vector3.Distance(transform.position, GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position) / CharInfo.BulletSpeed);
            }
            else
            {
                go.transform.position = GridManagerScript.Instance.GetBattleTile(DestinationTile).transform.position;
                go.SetActive(true);
            }
            UsedTargets.Add(go);
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
        float Duration = Vector3.Distance(transform.position, destination) / CharInfo.BulletSpeed;
        Vector3 res;
        while (!Dead)
        {
            yield return new WaitForFixedUpdate();
            //In case the game ended or in pause I will block the movement
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End)
            {
                yield return new WaitForFixedUpdate();
            }
            //Calutation for next world position of the bullet
            res = Vector3.Lerp(offset, destination, timer);

            switch (CharInfo.ClassType)
            {
                case CharacterClassType.Valley:
                    break;
                case CharacterClassType.Mountain:
                    res.y = CharInfo.Trajectory_Y.Evaluate(timer) + res.y;
                    res.z = CharInfo.Trajectory_Z.Evaluate(timer) + res.z;
                    break;
                case CharacterClassType.Forest:
                    res.z = CharInfo.Trajectory_Z.Evaluate(timer) + res.z;
                    break;
                case CharacterClassType.Desert:
                    break;
                default:
                    break;
            }
            
            transform.position = res;
            timer += Time.fixedDeltaTime / Duration;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
                FireEffectParticles(destination);
            }
        }
    }


    //Move the bullet on a straight movement 
    public IEnumerator MoveStraight()
    {
        //setup the base offset for the movement
        Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        float Duration = Vector3.Distance(transform.position, DestinationWorld) / CharInfo.BulletSpeed;
        while (!Dead)
        {
           
            yield return new WaitForFixedUpdate();
            //In case the game ended or in pause I will block the movement
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End)
            {
                yield return new WaitForFixedUpdate();
            }

            //Calutation for next world position of the bullet
            transform.position = Vector3.Lerp(offset, DestinationWorld, timer);
            timer += Time.fixedDeltaTime / Duration;
            //if timer ended the bullet fire the Effect
            if (timer > 1)
            {
                FireEffectParticles(transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the bullet collide with a character 
        if (other.tag.Contains("Side") && other.tag != Side.ToString())
        {

            foreach (GameObject item in UsedTargets)
            {
                Destroy(item);
            }
            CharacterBase target = other.GetComponentInParent<CharacterBase>();
            //Set damage to the hitting character
            target.SetDamage(CharInfo.Damage, Elemental);
            //fire the Effect
            FireEffectParticles(transform.position);
        }
    }

    public void FireEffectParticles(Vector3 pos)
    {
        if(!Dead)
        {
            Dead = true;
            StopAllCoroutines();
            //fire the Effect
            ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleType, ParticleTypes.Effect, pos, Side);
            PS.GetComponent<DisableParticleScript>().ResetParticle();
            PS.SetActive(false);
            PS.transform.parent = null;
            Destroy(this.gameObject);
        }
    }

}

