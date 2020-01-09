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

    public void CameraShake()
    {
        StartCoroutine(CameraShakeCo());
    }

    public IEnumerator CameraShakeCo()
    {
        Anim.SetBool("Shake", false);
        yield return null;
        Anim.SetBool("Shake", true);
    }

    public void SetFalse()
    {
        Anim.SetBool("Shake", false);
    }
}
