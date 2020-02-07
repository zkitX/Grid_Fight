using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleManagerScript : MonoBehaviour
{
    public static ParticleManagerScript Instance;
    public List<ScriptableObjectAttackParticle> ListOfAttckParticles = new List<ScriptableObjectAttackParticle>();
    public List<ScriptableObjectParticle> ListOfParticles = new List<ScriptableObjectParticle>();
    public List<FiredAttackParticle> AttackParticlesFired = new List<FiredAttackParticle>();
    public List<FiredParticle> ParticlesFired = new List<FiredParticle>();
    bool isTheGamePaused = false;
    public Transform Container;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        BattleManagerScript.Instance.CurrentBattleStateChangedEvent += Instance_CurrentBattleStateChangedEvent;
    }

    private void Instance_CurrentBattleStateChangedEvent(BattleState currentBattleState)
    {
        if (currentBattleState == BattleState.Pause && !isTheGamePaused)
        {
            isTheGamePaused = true;
            foreach (var item in AttackParticlesFired.Where(r => r.PS.activeInHierarchy).ToList())
            {
                foreach (ParticleSystem ps in item.PS.GetComponentsInChildren<ParticleSystem>())
                {
                    var main = ps.main;
                    main.simulationSpeed = 0;
                }
            }
        }
        else if (isTheGamePaused && currentBattleState == BattleState.Battle)
        {
            isTheGamePaused = false;
            foreach (var item in AttackParticlesFired.Where(r => r.PS.activeInHierarchy).ToList())
            {
                foreach (ParticleSystem ps in item.PS.GetComponentsInChildren<ParticleSystem>())
                {
                    var main = ps.main;
                    main.simulationSpeed = 1;
                }
            }
        }
    }

    public GameObject FireParticlesInPosition(AttackParticleTypes pType, AttackParticlePhaseTypes ParticleType, Vector3 pos, SideType side)
    {
        using (FiredAttackParticle psToFire = AttackParticlesFired.Where(r => r.ParticleType == ParticleType && r.AttackParticle == pType && !r.PS.gameObject.activeInHierarchy).FirstOrDefault())
        {
            if (psToFire != null)
            {
                psToFire.PS.transform.position = pos;
                psToFire.PS.SetActive(true);
                psToFire.PS.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
                return psToFire.PS;
            }
            else
            {
                using (DisposableGameObjectClass ps = new DisposableGameObjectClass(null))
                {
                    switch (ParticleType)
                    {
                        case AttackParticlePhaseTypes.Cast:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastPS;
                            break;
                        case AttackParticlePhaseTypes.Attack:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().AttackPS;
                            break;
                        case AttackParticlePhaseTypes.Effect:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().EffectPS;
                            break;
                        case AttackParticlePhaseTypes.Charging:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastLoopPS;
                            break;
                        case AttackParticlePhaseTypes.CastActivation:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastActivationPS;
                            break;
                    }
                    using (DisposableGameObjectClass go = new DisposableGameObjectClass(null))
                    {
                        go.BaseGO = Instantiate(ps.BaseGO, pos, Quaternion.identity, Container);
                        go.BaseGO.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
                        go.BaseGO.SetActive(true);
                        AttackParticlesFired.Add(new FiredAttackParticle(go.BaseGO, pType, ParticleType));
                        return go.BaseGO;
                    }

                }

            }
        }


    }


    public GameObject FireParticlesInTransform(AttackParticleTypes pType, AttackParticlePhaseTypes ParticleType, Transform parent, SideType side, bool particlesVisible)
    {
        //pType = AttackParticleTypes.Test_Mesh;
        using (FiredAttackParticle psToFire = AttackParticlesFired.Where(r => r.ParticleType == ParticleType && r.AttackParticle == pType && !r.PS.gameObject.activeInHierarchy).FirstOrDefault())
        {
            if (psToFire != null)
            {
                psToFire.PS.transform.parent = parent;
                psToFire.PS.transform.localPosition = Vector3.zero;
                psToFire.PS.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
                psToFire.PS.SetActive(particlesVisible);//particlesVisible
                return psToFire.PS;
            }
            else
            {
                using (DisposableGameObjectClass ps = new DisposableGameObjectClass(null))
                {
                    switch (ParticleType)
                    {
                        case AttackParticlePhaseTypes.Cast:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().CastPS;
                            break;
                        case AttackParticlePhaseTypes.Attack:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().AttackPS;
                            break;
                        case AttackParticlePhaseTypes.Effect:
                            ps.BaseGO = ListOfAttckParticles.Where(r => r.PSType == pType).First().EffectPS;
                            break;
                    }
                    using (DisposableGameObjectClass go = new DisposableGameObjectClass(null))
                    {

                        go.BaseGO = Instantiate(ps.BaseGO, parent.position, parent.rotation, parent);
                        go.BaseGO.transform.localPosition = Vector3.zero;
                        go.BaseGO.transform.localScale = side == SideType.RightSide ? new Vector3Int(1, 1, 1) : new Vector3Int(-1, 1, 1);
                        go.BaseGO.SetActive(particlesVisible);//particlesVisible
                        AttackParticlesFired.Add(new FiredAttackParticle(go.BaseGO, pType, ParticleType));
                        return go.BaseGO;
                    }
                }
            }
        }



            
       
    }


    public GameObject GetParticle(ParticlesType particle)
    {
        FiredParticle ps = ParticlesFired.Where(r => r.Particle == particle).FirstOrDefault();
        if (ps == null)
        {
            ps = new FiredParticle(Instantiate(ListOfParticles.Where(r => r.PSType == particle).First().PS), particle);
        }
        return ps.PS;
    }
}


public class FiredParticle
{
    public GameObject PS;
    public ParticlesType Particle;

    public FiredParticle()
    {
    }

    public FiredParticle(GameObject ps, ParticlesType particle)
    {
        PS = ps;
        Particle = particle;
    }

}



public class FiredAttackParticle : IDisposable
{
    public GameObject PS;
    public AttackParticleTypes AttackParticle;
    public AttackParticlePhaseTypes ParticleType;

    public FiredAttackParticle()
    {

    }

    public FiredAttackParticle(GameObject ps, AttackParticleTypes attackParticle, AttackParticlePhaseTypes particleType)
    {
        PS = ps;
        AttackParticle = attackParticle;
        ParticleType = particleType;
    }

    public void Dispose()
    {
    }
}



