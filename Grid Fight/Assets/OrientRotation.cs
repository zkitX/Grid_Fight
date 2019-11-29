using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientRotation : MonoBehaviour
{


    private Vector2 previousPos;
    private Vector2 currentPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        previousPos = currentPos;
        currentPos = transform.position;
        //transform.RotateAround(currentPos,new Vector3(0,0,1),)
             transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), previousPos - currentPos);
        var main = GetComponent<ParticleSystem>().main;
        main.startRotationZMultiplier = Quaternion.ToEulerAngles( Quaternion.FromToRotation(new Vector3(0, 0, 1), previousPos - currentPos)).z ;
    }
}
