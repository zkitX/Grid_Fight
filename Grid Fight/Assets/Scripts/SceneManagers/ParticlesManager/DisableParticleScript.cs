using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableParticleScript : MonoBehaviour
{
    private ParticleSystem ps;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    void Update()
    {
        if(!ps.IsAlive(true))
        {
            ResetParticle();
        }
    }

    public void ResetParticle()
    {
        ps.time = 0;
        gameObject.SetActive(false);
    }
}
