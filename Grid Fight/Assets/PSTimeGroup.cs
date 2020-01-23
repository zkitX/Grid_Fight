using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSTimeGroup : MonoBehaviour
{
    [Tooltip("Set this to override the time of the particles")]
    public float PSTime = 10f;
    [Tooltip("If enabled it will constantly update the duration of every particles")]
    public bool AutoUpdate = true;
    [Tooltip("This will disable once the particles finish")]
    bool DisableTrail = false;
    [Tooltip("All trails inside the group")]
    List<TrailRenderer> Trails = new List<TrailRenderer>();
    [Tooltip("Cache of trail initial information")]
    List<float> TrailInitialTime = new List<float>();

    void Awake()
    {
        //search and register the trails in the group
        foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
        {
            Trails.Add(trail);
            TrailInitialTime.Add(trail.time);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Activate only if the variable AutoUpdate is enable
        if (AutoUpdate)
        {
            UpdatePSTime();
        }
        TrailStateUpdate();
    }
    /// <summary>
    /// Set the Particle Systems Time according to PSTime variable, if put to 0 it will stop immediately
    /// </summary>
    public void UpdatePSTime()
    {
        //set the simulation time in every particle, minding the the simulation speed
        foreach (ParticleSystem PS in GetComponentsInChildren<ParticleSystem>())
        {
            var m = PS.main;
            //To change the duration the particle needs to pe paused
            PS.Pause();
            m.duration = PSTime * m.simulationSpeed;
            //check if the particle is finished
            DisableTrail = !PS.isEmitting;
            PS.Play();
        }
    }

    /// <summary>
    /// Set the Particle Systems Time with the inputted variable, if put to 0 it will stop immediately
    /// </summary>
    public void UpdatePSTime(float time)
    {
        UpdatePSTime();
    }

    //if the particles have finished emitting particles, disable the trail in time
    private void TrailStateUpdate()
    {
        if (DisableTrail)
        {
            for (int i = 0; i < Trails.Count; i++)
            {
                TrailRenderer trail = Trails[i];
                trail.time -= trail.time / 10;
                if (trail.time < 0.005f)
                {
                    trail.time = 0;
                }
            }
        }
        else
        {
            for (int i = 0; i < Trails.Count; i++)
            {
                TrailRenderer trail = Trails[i];
                trail.time = TrailInitialTime[i];
            }
        }
    }
}
