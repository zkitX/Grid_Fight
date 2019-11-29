using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableParticleScript : MonoBehaviour
{
    private ParticleSystem ps;
    private List<ParticleChildSimulationSpeed> Children = new List<ParticleChildSimulationSpeed>();
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>(true))
        {
            Children.Add(new ParticleChildSimulationSpeed(item.main.simulationSpeed, item));
        }
    }

    void Update()
    {
        if(!ps.IsAlive(true))
        {
            ResetParticle();
        }
    }

    private void OnEnable()
    {
        foreach (TrailRenderer item in GetComponentsInChildren<TrailRenderer>())
        {
            item.Clear();
        }
    }

    private void OnDisable()
    {
        SetSimulationSpeedToBase();
    }

    public void ResetParticle()
    {
        ps.time = 0;
        gameObject.SetActive(false);
        transform.parent = null;
    }

    public void SetSimulationSpeedToBase()
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            main.simulationSpeed = item.BaseValue;
        }
    }

    public void SetSimulationSpeed(float speed)
    {
        foreach (ParticleChildSimulationSpeed item in Children)
        {
            var main = item.Child.main;
            main.simulationSpeed *= speed;
        }
    }
}


public class ParticleChildSimulationSpeed
{
    public float BaseValue;
    public ParticleSystem Child;

    public ParticleChildSimulationSpeed(float baseValue, ParticleSystem child)
    {
        BaseValue = baseValue;
        Child = child;
    }
}