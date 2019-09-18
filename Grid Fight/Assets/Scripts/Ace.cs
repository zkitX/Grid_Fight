using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ace : MonoBehaviour
{

    private Vector3 PrevPosition = new Vector3(-1000,-1000, -1000);
    private Vector3 Offset = new Vector3(-1000, -1000, -1000);
    // Start is called before the first frame update
    void Start()
    {
        Offset = transform.position;
        PrevPosition = Offset;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Offset != new Vector3(-1000, -1000, -1000))
        {
            float res = 0;
            float sinA, a, c;

            Debug.Log(PrevPosition + "      " + transform.position);

            c = Vector3.Distance(PrevPosition, transform.position);
            if (PrevPosition.y > transform.position.y)
            {
                a = PrevPosition.y - transform.position.y;
                sinA = a / c;

                res  = (Mathf.Sin(sinA) * 180) / Mathf.PI;
                Debug.Log(a + "  111 " + c + "   " + sinA + "   " + res);

            }
            else if(PrevPosition.y < transform.position.y)
            {
                 a = transform.position.y - PrevPosition.y;
                 sinA = a / c;
                 res = 90 - (Mathf.Asin(sinA) * 180) / Mathf.PI;
                 Debug.Log(a + " 2222  " + c + "   " + sinA + "   " + res);

            }

            if (!float.IsNaN(res) && res != 0)
            {
                //Debug.Log(res);
                transform.eulerAngles = new Vector3(0, 0, res);

                PrevPosition = transform.position;
            }
           
            
        }
    }
}
