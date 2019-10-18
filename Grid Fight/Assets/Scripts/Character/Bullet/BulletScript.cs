using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the component that take care of all the Bullet behaviours in the game
/// </summary>
public class BulletScript : MonoBehaviour
{
    //Public
    public BulletInfoScript BulletInfo;
    public Vector2Int DestinationTile;
    public Vector3 DestinationWorld;
    public SideType Side;
    public ElementalType Elemental;
	public ControllerType ControllerT;
    public bool Dead = false;
    public GameObject PS;


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
        Physics.IgnoreLayerCollision(Side == SideType.PlayerCharacter ? 9 : 10, Side == SideType.PlayerCharacter ? 11 : 12);
    }

    //Move the bullet on a determinated tile using the BulletInfo.Trajectory
    public IEnumerator MoveToTile()
	{
        //setup the base offset for the movement
		Vector3 offset = transform.position;
        //Timer used to set up the coroutine
        float timer = 0;
        //Destination battle tile
        bts = GridManagerScript.Instance.GetBattleTile(DestinationTile);
        //Destination position
        Vector3 destination = bts.transform.position;
        //Duration of the particles 
        float Duration = Vector3.Distance(transform.position, destination) / BulletInfo.BulletSpeed;
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
            res.y = BulletInfo.ClassType == CharacterClassType.Mountain ? BulletInfo.Trajectory.Evaluate(timer) + res.y : res.y;
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
       
        while (!Dead)
        {
           
            yield return new WaitForFixedUpdate();
            //In case the game ended or in pause I will block the movement
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.End)
            {
                yield return new WaitForFixedUpdate();
            }

            //Calutation for next world position of the bullet
            transform.position = Vector3.Lerp(offset, DestinationWorld + offset, timer);
            timer += Time.fixedDeltaTime;
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
        if (other.tag.Contains("Character") && other.tag != Side.ToString())
        {
            CharacterBase target = other.GetComponentInParent<CharacterBase>();
            //Set damage to the hitting character
            target.SetDamage(BulletInfo.Damage, Elemental);
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
            ParticleManagerScript.Instance.FireParticlesInPosition(BulletInfo.ParticleType, ParticleTypes.Effect, pos, Side);
            PS.GetComponent<DisableParticleScript>().ResetParticle();
            PS.SetActive(false);
            PS.transform.parent = null;
            Destroy(this.gameObject);
        }
    }

}

