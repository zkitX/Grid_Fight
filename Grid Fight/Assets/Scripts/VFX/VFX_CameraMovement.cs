using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_CameraMovement : MonoBehaviour
{
    public float minSize = 4;
    public float maxSize = 20;
    // Update is called once per frame
    void LateUpdate()
    {
        //if(Mathf.Abs((Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).x) > Screen.width / 5 || Mathf.Abs((Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).y) > Screen.height / 5)
        //{
            if (Input.GetMouseButton(0))
                {
                    transform.position += (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)) / 6000 * GetComponent<Camera>().orthographicSize;
                
                }
            GetComponent<Camera>().orthographicSize -= (Input.mouseScrollDelta.y);
            if (GetComponent<Camera>().orthographicSize < minSize)
            {
                GetComponent<Camera>().orthographicSize = minSize;
            }
            else
            if (GetComponent<Camera>().orthographicSize > maxSize)
            {
                GetComponent<Camera>().orthographicSize = maxSize;
            }
        //}
            
        
    }
}
