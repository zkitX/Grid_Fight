using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{

    public static CameraManagerScript Instance;
    public Animator Anim;
    [Header("Zoom in")]
    public float TransitionINZoomValue = 2;
    public float DurationIn = 1;
    public AnimationCurve ZoomIn;

    [Header("Zoom out")]
    public float TransitionOutZoomValue = 2;
    public float DurationOut = 1;
    public AnimationCurve ZoomOut;


    bool isCamMoving = false;
    public Camera Cam;

    private void Awake()
    {
        Instance = this;
    }

    public void CameraShake(CameraShakeType shakeType)
    {
        if(!isCamMoving)
        {
            StartCoroutine(CameraShakeCo(shakeType));
        }
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
            Cam.orthographicSize = newGrid.OrthographicSize;

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

        if (playerPos != Vector3.zero)
        {
            StartCoroutine(CameraMoveSequence_Co(duration, playerPos));
        }

        if (endOrtho > 0)
        {
            StartCoroutine(CameraFocusSequence_Co(duration, endOrtho, animCurve));
        }
    }

    public IEnumerator CameraFocusSequence_Co(float duration, float endOrtho, AnimationCurve animCurve)
    {
        bool hasStarted = false;
        float startingOrtho = Cam.orthographicSize;
        float progress = 0f;
        while (progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += Time.deltaTime / duration;
            Cam.orthographicSize = Mathf.Lerp(startingOrtho, endOrtho, animCurve.Evaluate(progress));
            yield return null;
        }
        Cam.orthographicSize = endOrtho;
    }

    public IEnumerator CameraMoveSequence_Co(float duration, Vector3 playerPos)
    {
        bool hasStarted = false;
        Vector3 cameraStartingPosition = transform.position;
        Vector3 finalPos = new Vector3(playerPos.x, playerPos.y, cameraStartingPosition.z);
        float progress = 0f;
        isCamMoving = true;
        while (progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(cameraStartingPosition, finalPos, progress);
            yield return null;
        }
        transform.position = finalPos;
        isCamMoving = false;
    }
}
