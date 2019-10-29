using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManagerScript : MonoBehaviour
{
	public static ParticleManagerScript Instance;
    public List<ScriptableObjectAttackParticle> ListOfAttckParticles = new List<ScriptableObjectAttackParticle>();
    public List<FiredParticle> ParticlesFired = new List<FiredParticle>();
    bool isTheGamePaused = false;


    private void Awake()
	{
		Instance = this;
	}

	public GameObject FireParticlesInPosition(AttackParticleTypes pType, ParticleTypes ParticleType, Vector3 pos, SideType side)
	{
        FiredParticle psToFire = ParticlesFired.Where(r => r.ParticleType == ParticleType && r.AttackParticle == pType && !r.PS.gameObject.activeInHierarchy).FirstOrDefault();
        if (psToFire != null)
		{
            psToFire.PS.transform.position = pos;
            psToFire.PS.SetActive(true);
            psToFire.PS.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
            return psToFire.PS;
		}
		else
		{
            GameObject ps = null;
            switch (ParticleType)
            {
                case ParticleTypes.Cast:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastPS;
                    break;
                case ParticleTypes.Attack:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().AttackPS;
                    break;
                case ParticleTypes.Effect:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().EffectPS;
                    break;
            }
            GameObject go = Instantiate(ps, pos, Quaternion.identity);
            go.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
            go.SetActive(true);
			ParticlesFired.Add(new FiredParticle(go, pType, ParticleType));
            return go;
        }
	}


    public GameObject FireParticlesInTransform(AttackParticleTypes pType, ParticleTypes ParticleType, Transform parent, SideType side)
    {
        FiredParticle psToFire = ParticlesFired.Where(r => r.ParticleType == ParticleType && r.AttackParticle == pType && !r.PS.gameObject.activeInHierarchy).FirstOrDefault();
        if (psToFire != null)
        {
            psToFire.PS.transform.parent = parent;
            psToFire.PS.transform.localPosition = Vector3.zero;
            psToFire.PS.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
            psToFire.PS.SetActive(true);
            return psToFire.PS;
        }
        else
        {
            GameObject ps = null;
            switch (ParticleType)
            {
                case ParticleTypes.Cast:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastPS;
                    break;
                case ParticleTypes.Attack:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().AttackPS;
                    break;
                case ParticleTypes.Effect:
                    ps = ListOfAttckParticles.Where(r => r.PSType == pType).First().EffectPS;
                    break;
            }
            GameObject go = Instantiate(ps, parent.position, parent.rotation, parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
            go.SetActive(true);
            ParticlesFired.Add(new FiredParticle(go, pType, ParticleType));
            return go;
        }
    }


    private void Update()
	{
		if(BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause && !isTheGamePaused)
		{
			isTheGamePaused = true;
			foreach (var item in ParticlesFired.Where(r=> r.PS.activeInHierarchy).ToList())
			{
				foreach (ParticleSystem ps in item.PS.GetComponentsInChildren<ParticleSystem>())
				{
					var main = ps.main;
					main.simulationSpeed = 0;
				}
			}
		}
		else if(isTheGamePaused && BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle)
		{
			isTheGamePaused = false;
            foreach (var item in ParticlesFired.Where(r => r.PS.activeInHierarchy).ToList())
            {
                foreach (ParticleSystem ps in item.PS.GetComponentsInChildren<ParticleSystem>())
                {
                    var main = ps.main;
                    main.simulationSpeed = 1;
                }
            }
		}
	}
}


public class FiredParticle
{
    public GameObject PS;
    public AttackParticleTypes AttackParticle;
    public ParticleTypes ParticleType;

    public FiredParticle()
    {

    }

    public FiredParticle(GameObject ps, AttackParticleTypes attackParticle, ParticleTypes particleType)
    {
        PS = ps;
        AttackParticle = attackParticle;
        ParticleType = particleType;
    }

}




