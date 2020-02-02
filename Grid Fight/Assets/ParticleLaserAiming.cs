using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLaserAiming : MonoBehaviour
{
    public Transform Target;

    public float Power = -300;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var velocityModule = GetComponent<ParticleSystem>().velocityOverLifetime;
        Vector3 targetDir = Target.position - transform.position;
        float angle = Vector3.Angle(targetDir, new Vector3(-1,0,0));
        float distance = Vector3.Distance(transform.position, Target.position);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        velocityModule.speedModifier =(distance)/Power;
    }
}
