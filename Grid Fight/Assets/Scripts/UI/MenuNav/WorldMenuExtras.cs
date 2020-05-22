using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMenuExtras : MonoBehaviour
{
    protected Vector3 startingPos;
    protected Vector3 startingScale;
    protected Vector2 screenPos = new Vector2(0.5f, 0.5f);
    protected Transform focus = null;
    protected float zoom = 1f;

    private void Awake()
    {
        startingPos = transform.position;
        startingScale = transform.localScale;
    }

    public void SetFocusToObject(Vector2 _screenPos, float duration, Transform _focus = null, float _zoom = 1f)
    {
        if (FocusLerper != null) StopCoroutine(FocusLerper);
        FocusLerper = FocusLerp(_screenPos, duration, _focus, _zoom);
        StartCoroutine(FocusLerper);
    }

    IEnumerator FocusLerper = null;
    public IEnumerator FocusLerp(Vector2 _screenPos, float duration, Transform _focus, float _zoom)
    {
        focus = _focus;
        zoom = _zoom;
        screenPos = _screenPos;

        Vector3 endScale = startingScale * zoom;
        Vector3 endMove;

        float timeLeft = duration;
        float progress = 0f;
        while (timeLeft != 0)
        {
            endMove = focus == null ? startingPos : transform.position + VectorToCentre(focus);
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, 1000f);
            progress = 1f - timeLeft / duration;
            transform.localScale = Vector3.Lerp(transform.localScale, endScale, progress);
            transform.position = Vector3.Lerp(transform.position, endMove, progress);
            yield return null;
        }
    }

    public Vector3 VectorToCentre(Transform tran)
    {
        return new Vector3(Screen.width * screenPos.x, Screen.height * screenPos.y) - tran.position;
    }
}
