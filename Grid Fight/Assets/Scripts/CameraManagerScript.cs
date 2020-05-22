using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{

    public static CameraManagerScript Instance;
    public Animator Anim;
    public AnimationCurve ZoomIn;
    public AnimationCurve ZoomOut;
    public float TransitionINZoomValue = 2;
    public float Duration = 1;

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

    public void CameraJumpInOut(int jumpInOut)
    {
        StartCoroutine(CameraJumpInOutCo(jumpInOut));
    }


    public IEnumerator CameraJumpInOutCo(int jumpInOut)
    {
        Anim.SetInteger("JumpInOut", jumpInOut);
        yield return new WaitForSeconds(0.2f);
        Anim.SetInteger("JumpInOut", 0);
    }

    public void SetFalse()
    {
        Anim.SetInteger("Shake", 0);
    }

    public void ChangeFocusToNewGrid(CameraInfoClass newGrid, float duration, bool moveCameraInternally)
    {
        if (GridManagerScript.Instance.currentGridStructureObject == null)
        {
            GetComponent<Camera>().orthographicSize = newGrid.OrthographicSize;

            transform.position = newGrid.CameraPosition;
            return;
        }
        if (moveCameraInternally)
        {
          //  StartCoroutine(CameraFocusSequence(newGrid.CameraPosition, duration, newGrid.OrthographicSize, newGrid));
        }
    }

    public void CameraFocusSequence(float duration, float endOrtho, AnimationCurve animCurve, Vector3 playerPos)
    {
        StartCoroutine(CameraFocusSequence_Co(duration, endOrtho, animCurve, playerPos));
    }

    IEnumerator CameraFocusSequence_Co(float duration, float endOrtho, AnimationCurve animCurve, Vector3 playerPos)
    {
        bool hasStarted = false;
        Camera cam = GetComponent<Camera>();
        float startingOrtho = cam.orthographicSize;
        Vector3 cameraStartingPosition = transform.position;
        Vector3 finalPos = new Vector3(playerPos.x, playerPos.y, cameraStartingPosition.z);
        float progress = 0f;
        while(progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += Time.fixedDeltaTime / duration;
            transform.position = Vector3.Lerp(cameraStartingPosition, finalPos, progress);
            cam.orthographicSize = Mathf.Lerp(startingOrtho, endOrtho, animCurve.Evaluate(progress));
            yield return null;
        }

    }
}
