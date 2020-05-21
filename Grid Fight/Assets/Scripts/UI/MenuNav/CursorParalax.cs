using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorParalax : MonoBehaviour
{
    public bool startEnabled = false;
    public float snapbackTime = 0.3f;
    [HideInInspector] public bool paraEnabled = false; 
    public ParalaxEffectClass[] paralaxObjects;

    private void Start()
    {
        EnableParalax(startEnabled);
    }

    public void EnableParalax(bool state)
    {
        if (state && (Grid_UINavigator.Instance == null || Grid_UINavigator.Instance.navType != MenuNavigationType.Cursor))
        {
            return;
        }

        paraEnabled = state;

        if (ParalaxApplier != null) StopCoroutine(ParalaxApplier);
        if (ParalaxResetter != null) StopCoroutine(ParalaxResetter);

        if (state)
        {
            foreach (ParalaxEffectClass paraObj in paralaxObjects) paraObj.Setup();
            ParalaxApplier = ApplyParalax();
            StartCoroutine(ParalaxApplier);
        }
        else
        {
            ParalaxResetter = ReturnToCentre();
            StartCoroutine(ParalaxResetter);
        }
    }

    IEnumerator ParalaxApplier = null;
    IEnumerator ApplyParalax()
    {
        while (true)
        {
            foreach(ParalaxEffectClass paraObj in paralaxObjects)
            {
                paraObj.targetPosition = paraObj.startingPos + 
                    new Vector3(
                        Screen.width/2f * -Grid_UINavigator.Instance.cursor.CursorScreenNormalised.x,
                        Screen.height/2f * -Grid_UINavigator.Instance.cursor.CursorScreenNormalised.y
                    ) * (1f - paraObj.maxOffset);
                paraObj.objectToOffset.localPosition = Vector3.Lerp(paraObj.objectToOffset.localPosition, paraObj.targetPosition, Time.deltaTime * 10f);
            }
            yield return null;
        }
    }

    IEnumerator ParalaxResetter = null;
    IEnumerator ReturnToCentre()
    {
        if (!paralaxObjects[0].setup) yield break;

        float timeRemaining = snapbackTime;
        while (true)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 1000f);
            foreach (ParalaxEffectClass paraObj in paralaxObjects)
            {
                paraObj.objectToOffset.localPosition = Vector3.Lerp(paraObj.objectToOffset.localPosition, paraObj.startingPos, 1f - timeRemaining / snapbackTime);
            }
            yield return null;
        }
    }

}

[System.Serializable]
public class ParalaxEffectClass
{
    public Transform objectToOffset;
    [HideInInspector] public Vector3 targetPosition;
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public bool setup = false;
    [Range(1f, 2f)] public float maxOffset = 1.2f;

    public void Setup()
    {
        setup = true;
        startingPos = objectToOffset.localPosition;
    }
}
