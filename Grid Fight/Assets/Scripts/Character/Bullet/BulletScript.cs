using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	public float Speed;
	public float Damage;
	public AttackType AType;
	public ControllerType ControllerT;
	public float Minx, Maxx, Miny, Maxy;
    public AnimationCurve Height;
	public bool Hit = false;
    public bool Dead = false;
	//public ParticleSystem PS;


	// Start is called before the first frame update
	private void OnEnable()
	{
		Hit = false;
        Dead = false;
		if (AType == AttackType.Static)
		{
			StartCoroutine(SelfDeactivate(3));
		}
		else if (AType == AttackType.PowerAct)
		{
			StartCoroutine(SelfDeactivate(7));
            float x = (ControllerT == ControllerType.Player1 ? Random.Range(Minx, Maxx) : -Random.Range(Minx, Maxx));
            float y = (float)Random.Range(Miny, Maxy);

            StartCoroutine(MoveParabola(x,y, 3.14f));
		}
		else
		{
			StartCoroutine(SelfDeactivate(3));
			StartCoroutine(MoveLinear(transform.right * (ControllerT == ControllerType.Player1 ? 15 : -15)));
		}
    }

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

    private IEnumerator MoveLinear(Vector3 dest)
	{
		Vector3 offset = transform.position;
        float timer = 0;

        while (!Dead)
        {
            yield return new WaitForFixedUpdate();
			while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForEndOfFrame();
            }

		/*	if(Hit)
			{
				break;
			}*/
			transform.position = Vector3.Lerp(offset, dest + offset, timer);
            timer += Time.fixedDeltaTime;
        }
	}

	private IEnumerator MoveParabola(float x, float y, float time)
    {
		//Debug.Log(time);
		Vector3 offset = transform.position;
		float timer = 0;
		float pp = 0;
		bool inside = true;
		while (inside)
		{
			yield return new WaitForFixedUpdate();
			while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForEndOfFrame();
            }

            /*	if (Hit)
                {
                    break;
                }*/

            Vector3 res = new Vector3(Mathf.Lerp(0, x, timer) + offset.x,
                                          (((float)(Height.Evaluate(timer)) * y)) + offset.y,
                                          offset.z);

            

			timer += Time.fixedDeltaTime / time;
			//Debug.Log(timer);
			pp += Time.fixedDeltaTime;// * (time / 3.14f);
			//Debug.Log(pp);
			transform.position = res;
			if(timer > 1)
			{
				inside = false;
			}
		}
	}
}

