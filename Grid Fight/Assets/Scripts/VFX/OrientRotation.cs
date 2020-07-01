using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientRotation : MonoBehaviour
{

    public Vector3 adjustment = new Vector3();
    private Vector2 previousPos;
    private Vector2 currentPos;


    // Update is called once per frame
    void LateUpdate()
    {
        //Check if the object is still for no reasons
        if((Vector2)transform.position!=previousPos)
            //bake previous position to take a new one
            previousPos = currentPos;
        currentPos = transform.position;
        //transform.RotateAround(currentPos,new Vector3(0,0,1),)
        transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 1),(Vector3) previousPos - (Vector3)currentPos + adjustment);
        var main = GetComponent<ParticleSystem>().main;
        int Offset = transform.localScale.x < 0 ? 1 : 0;
        main.startRotationZMultiplier = Quaternion.ToEulerAngles( Quaternion.FromToRotation(new Vector3(0, 0, 1), previousPos - currentPos)).z+180*(Offset) ;
    }
}
