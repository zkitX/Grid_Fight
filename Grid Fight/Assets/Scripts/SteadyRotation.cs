using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyRotation : MonoBehaviour
{

    public Vector3 Rotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(Rotation);
    }
}
