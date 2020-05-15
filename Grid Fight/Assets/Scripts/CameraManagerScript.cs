using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{

    public static CameraManagerScript Instance;
    public Animator Anim;
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
            return;
        }
        if (moveCameraInternally)
        {
            StartCoroutine(CameraFocusSequence(newGrid.CameraPosition, duration, newGrid.OrthographicSize, newGrid));
        }

    }

    IEnumerator CameraFocusSequence(Vector3 translation, float duration, float endOrtho, CameraInfoClass newGrid)
    {
        bool hasStarted = false;
        Camera cam = GetComponent<Camera>();
        float startingOrtho = cam.orthographicSize;
        Vector3 cameraStartingPosition = transform.position;
        float progress = 0f;
        while(progress < 1 || !hasStarted)
        {
            hasStarted = true;
            progress += Time.fixedDeltaTime / duration;
            cam.orthographicSize = Mathf.Lerp(startingOrtho, endOrtho, progress);
            transform.position = Vector3.Lerp(cameraStartingPosition, translation, progress);
            yield return null;
        }

        transform.position = newGrid.CameraPosition;
    }
}
