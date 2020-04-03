using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStageInfoScript : MonoBehaviour
{
    public List<CameraInfoClass> CameraInfo = new List<CameraInfoClass>();
}

[System.Serializable]
public class CameraInfoClass
{
    public int StageIndex;
    [HideInInspector] public bool used = false;
    public Vector3 CameraPosition;
    public float OrthographicSize;
}
