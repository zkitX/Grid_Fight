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

	public void FireParticlesInPosition(AttackParticleTypes pType, ParticleTypes ParticleType, Transform parent)
	{
        FiredParticle psToFire = ParticlesFired.Where(r => r.ParticleType == ParticleType && r.AttackParticle == pType && !r.PS.gameObject.activeInHierarchy).FirstOrDefault();
		if(psToFire != null)
		{
            psToFire.PS.transform.rotation = Quaternion.Euler(parent.eulerAngles);
            psToFire.PS.transform.position = parent.position;
            psToFire.PS.SetActive(true);
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
            GameObject go = Instantiate(ps, parent.position, Quaternion.Euler(parent.eulerAngles));
			go.SetActive(true);
			ParticlesFired.Add(new FiredParticle(go, pType, ParticleType));
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




