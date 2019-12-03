using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXOffsetToTargetVOL : MonoBehaviour
{
    public Transform Target;
    public bool IncludeChildren = true;


    ParticleSystem PS;
    ParticleSystem[] PSChildren;
    bool IsPSAttached = false;

    private void Awake()
    {
        if(GetComponent<ParticleSystem>())
        {
            PS = GetComponent<ParticleSystem>();
            IsPSAttached = true;
        }
        if (!Target)
        {
            Target = transform;
        }
        if (IncludeChildren)
        {
            PSChildren = GetComponentsInChildren<ParticleSystem>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var VOL = PS.velocityOverLifetime;
        VOL.orbitalOffsetXMultiplier = Target.position.x - transform.position.x;
        VOL.orbitalOffsetYMultiplier = Target.position.y - transform.position.y;
        if (IncludeChildren)
        {
            foreach (ParticleSystem pS in PSChildren)
            {
                var VOLChild = pS.velocityOverLifetime;
                VOLChild.orbitalOffsetXMultiplier = Target.position.x - transform.position.x;
                VOLChild.orbitalOffsetYMultiplier = Target.position.y - transform.position.y;
            }
        }
    }
}
