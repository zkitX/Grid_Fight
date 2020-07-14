using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHelperScript : MonoBehaviour
{
    private List<ParticleChildSimulationSpeed> Children = new List<ParticleChildSimulationSpeed>();
    [Tooltip("Insert particles that consist in only one long particle")]
    public List<ParticleSystem> LongParticles = new List<ParticleSystem>();
    public float PSTime = 10f;
    [Tooltip("All trails inside the group")]
    List<TrailRenderer> Trails = new List<TrailRenderer>();
    [Tooltip("Cache of trail initial information")]
    List<float> TrailInitialTime = new List<float>();
    public float timet = 0;

    ParticleSystem PS;
    ParticleSystem[] PSChildren;
    public int iter = 0;


    public float MyProperty
    {
        get
        {
            var m = PS.main;
            return m.duration;
        }
    }

    private void Awake()
    {
        foreach (ParticleSystem item in GetComponentsInChildren<ParticleSystem>(true))
        {
            Children.Add(new ParticleChildSimulationSpeed(item.main.simulationSpeed, item));
        }

        foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
        {
            Trails.Add(trail);
            if (trail.GetComponent<VFXBulletSpeedCalibration>())
            {
                VFXBulletSpeedCalibration vfx = trail.GetComponent<VFXBulletSpeedCalibration>();
                TrailInitialTime.Add(vfx.trailTimeCache * (vfx.BulletDuration / vfx.BulletOriginalDuration));
            }
            else
            {
                TrailInitialTime.Add(trail.time);
            }
        }

        if (GetComponent<ParticleSystem>())
        {
            PS = GetComponent<ParticleSystem>();
        }
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
       // Debug.Log("System has stopped!");
        ResetParticle();
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

        foreach (ParticleChildSimulationSpeed item in Children)
        {
            item.Child.Clear();
        }

        transform.parent = null;
        gameObject.SetActive(false);
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

    public void UpdatePSTime()
    {
        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleSystem PS in GetComponentsInChildren<ParticleSystem>())
        {
            var m = PS.main;
            m.loop = false;
            //To change the duration the particle needs to pe paused
            PS.Pause();
            m.duration = PSTime / m.simulationSpeed;
            //check if the particle is finished
            PS.Play();
            if (PS.time >= m.duration)
            {
                for (int i = 0; i < Trails.Count; i++)
                {
                    TrailRenderer trail = Trails[i];
                    trail.time = trail.time / 1.1f;
                    if (trail.time < 0.005f)
                    {
                        trail.time = 0;
                        trail.emitting = false;
                    }
                }
            }
        }
        foreach (ParticleSystem p in LongParticles)
        {
            p.Pause();
            //p.gameObject.SetActive(false);
            var m = p.main;
            m.startLifetime = m.duration;
            // p.gameObject.SetActive(true);
            p.Play();
        }
    }

    /// <summary>
    /// Set the Particle Systems Time with the inputted variable, if put to 0 it will stop immediately
    /// </summary>
    public void UpdatePSTime(float time, int it)
    {
        iter = it;
        PSTime = time;
        UpdatePSTime();
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