using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public float Speed;
	public float Damage;
    public AttackParticleTypes AttackParticle;
    public Vector2Int Destination;
    public SideType Side;
    public ElementalType Elemental;
    public AttackType AType;
	public ControllerType ControllerT;
    public AnimationCurve Height;
    public bool Dead = false;
    public float Duration;
    BattleTileScript bts;
    public GameObject PS;

    private void Update()
    {
        if(BattleManagerScript.Instance.CurrentBattleState == BattleState.End)
        {
            StartCoroutine(SelfDeactivate(0));
        }
    }

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
        Physics.IgnoreLayerCollision(Side == SideType.PlayerCharacter ? 9 : 10, Side == SideType.PlayerCharacter ? 11 : 12);
    }
    public IEnumerator Move()
	{
    
		Vector3 offset = transform.position;
        float timer = 0;
        bts = GridManagerScript.Instance.GetBattleTile(Destination);
        Vector3 destination = bts.transform.position;
        Duration = Vector3.Distance(transform.position, destination) / Speed;
        while (!Dead)
        {
            yield return new WaitForFixedUpdate();
			/*while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForEndOfFrame();
            }*/

            /*	if(Hit)
                {
                    break;
                }*/


            Vector3 res;
            res = Vector3.Lerp(offset, destination, timer);
            res.y = AType == AttackType.PowerAct ? Height.Evaluate(timer) + res.y : res.y;
            transform.position = res;
            timer += Time.fixedDeltaTime / Duration;
            if (timer > 1)
            {
                FireEffectParticles(destination);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log(other.tag);
        if (other.tag.Contains("Character") && other.tag != Side.ToString())
        {
            CharacterBase target = other.GetComponentInParent<CharacterBase>();
            target.SetDamage(Damage, Elemental);
            FireEffectParticles(transform.position);
        }
    }

    public void FireEffectParticles(Vector3 pos)
    {
        if(!Dead)
        {
            Dead = true;
            StopAllCoroutines();
            ParticleManagerScript.Instance.FireParticlesInPosition(AttackParticle, ParticleTypes.Effect, pos);
            PS.GetComponent<DisableParticleScript>().ResetParticle();
            PS.SetActive(false);
            PS.transform.parent = null;
            Destroy(this.gameObject);
        }
    }

}

