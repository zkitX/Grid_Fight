using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{

    public static CameraManagerScript Instance;
    public Animator Anim;
    public Vector3 CurrentCameraOffset;
    private void Awake()
    {
        CurrentCameraOffset = transform.position;
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

    public void ChangeFocusToNewGrid(CameraInfoClass newGrid, float duration, bool moveCameraInternally)
    {
        if (GridManagerScript.Instance.currentGridStructureObject == null)
        {
            GetComponent<Camera>().orthographicSize = newGrid.OrthographicSize;
            transform.position = newGrid.CameraPosition;
            CurrentCameraOffset = newGrid.CameraPosition;
            return;
        }
        if (moveCameraInternally)
        {
            StartCoroutine(CameraFocusSequence(newGrid.CameraPosition - CurrentCameraOffset, duration, newGrid.OrthographicSize, newGrid));
        }

    }

    IEnumerator CameraFocusSequence(Vector3 translation, float duration, float endOrtho, CameraInfoClass newGrid)
    {
        bool hasStarted = false;
        float startingOrtho = GetComponent<Camera>().orthographicSize;
        Vector3 cameraStartingPosition = transform.position;

        float durationLeft = duration;
        float progress = 0f;
        while(durationLeft != 0 || !hasStarted)
        {
            hasStarted = true;

            durationLeft = Mathf.Clamp(durationLeft - Time.deltaTime, 0f, 999f);
            progress = 1f - (durationLeft / (duration != 0f ? duration : 1f));
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(startingOrtho, endOrtho, progress);
            transform.position = Vector3.Lerp(cameraStartingPosition, cameraStartingPosition + translation, progress);
            yield return null;
        }

        CurrentCameraOffset = newGrid.CameraPosition;
    }
}
