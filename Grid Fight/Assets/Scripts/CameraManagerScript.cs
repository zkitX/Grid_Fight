using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{

    public static CameraManagerScript Instance;
    public Animator Anim;
    private void Awake()
    {
        Instance = this;
    }

    public void CameraShake(CameraShakeType shakeType)
    {
        StartCoroutine(CameraShakeCo(shakeType));
    }

    public IEnumerator CameraShakeCo(CameraShakeType shakeType)
    {
        Anim.SetInteger("Shake", (int)shakeType);
        yield return null;
        //Anim.SetInteger("Shake", 0);
    }

    public void SetFalse()
    {
        Anim.SetInteger("Shake", 0);
    }
}
