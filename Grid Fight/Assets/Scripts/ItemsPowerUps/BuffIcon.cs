using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Threading;

public class BuffIcon : MonoBehaviour
{
    Animation[] anims = new Animation[0];
    [SerializeField] protected SpriteRenderer statusIcon = null;
    protected float statusIconStartingScale = 0.5f;
    [SerializeField] protected GameObject DebuffObj = null;
    protected List<Color> DebuffDefaultColors = new List<Color>();
    [SerializeField] protected GameObject BuffObj = null;
    protected List<Color> BuffDefaultColors = new List<Color>();

    public ScriptableObjectAttackEffect StatusEffect = null;

    [HideInInspector] public Vector3 movingPos = new Vector3();

    protected GameObject curStatusObj = null;
    protected Animation anim = null;

    private void Awake()
    {
        anims = GetComponentsInChildren<Animation>();
        foreach (Animation anim in anims)
        {
            anim.Play();
        }
        foreach (SpriteRenderer sprite in DebuffObj.GetComponentsInChildren<SpriteRenderer>())
        {
            DebuffDefaultColors.Add(sprite.color);
        }
        foreach (SpriteRenderer sprite in BuffObj.GetComponentsInChildren<SpriteRenderer>())
        {
            BuffDefaultColors.Add(sprite.color);
        }
        DebuffObj.SetActive(false);
        BuffObj.SetActive(false);
        statusIconStartingScale = 1f;//statusIcon.transform.localScale.x;
        statusIcon.color = Color.clear;
    }




    public void InitiateStatusIcon(ScriptableObjectAttackEffect statusEffect)
    {
        if (statusEffect == StatusEffect) return;
        StatusEffect = statusEffect;

        //Switch to either buff/debuff icon background
        curStatusObj?.SetActive(false);
        curStatusObj = statusEffect.classification == StatusEffectType.Buff ? BuffObj : DebuffObj;
        curStatusObj.SetActive(true);
        SpriteRenderer[] sprites = curStatusObj.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < (statusEffect.classification == StatusEffectType.Buff ? BuffDefaultColors.Count : DebuffDefaultColors.Count); i++)
        {
            sprites[i].color = statusEffect.classification == StatusEffectType.Buff ? BuffDefaultColors[i] : DebuffDefaultColors[i];
            sprites[i].color += statusEffect.recolorCharUI ? statusEffect.statusIconColor : Color.clear;
        }

        //apply affect imagery
        statusIcon.transform.SetParent(curStatusObj.transform.GetChild(0));
        statusIcon.sprite = statusEffect.icon;
        statusIcon.transform.localScale = new Vector3(statusIconStartingScale, statusIconStartingScale, statusIconStartingScale);
        statusIcon.color = statusEffect.classification == StatusEffectType.Buff ? Color.black : Color.white;

        //Start Anims And Shit
        anim = curStatusObj.GetComponentInChildren<Animation>();
        if (CurrentEnterExitProcess != null) StopCoroutine(CurrentEnterExitProcess);
        CurrentEnterExitProcess = InitiatorProcess();
        StartCoroutine(CurrentEnterExitProcess);
    }

    public void TerminateStatusIcon()
    {
        StatusEffect = null;

        //Start Anims And Shit
        if (CurrentEnterExitProcess != null) StopCoroutine(CurrentEnterExitProcess);
        CurrentEnterExitProcess = TerminatorProcess();
        StartCoroutine(CurrentEnterExitProcess);
    }

    IEnumerator CurrentEnterExitProcess = null;
    
    IEnumerator InitiatorProcess()
    {
        anim.Stop();

        anim.clip = anim.GetClip("StatusEffect-Icon-PopIn");
        anim.Play();
        while (anim.isPlaying)
        {
            yield return null;
        }

        anim.clip = anim.GetClip("StatusEffect-Icon-Idle");
        anim.Play();
    }

    IEnumerator TerminatorProcess()
    {
        anim.Stop();

        anim.clip = anim.GetClip("StatusEffect-Icon-PopOut");
        anim.Play();
        while (anim.isPlaying)
        {
            yield return null;
        }
    }


    public void MoveStatusIcon(Vector3 pos, float duration = 0.4f)
    {
        movingPos = pos;
        if (StatusEffect == null) duration = 0f;
        if (CurrentMove != null) StopCoroutine(CurrentMove);
        CurrentMove = MoveStatusIcon_Co(pos, duration);
        StartCoroutine(CurrentMove);
    }

    IEnumerator CurrentMove = null;

    IEnumerator MoveStatusIcon_Co(Vector3 pos, float duration)
    {
        float timeRemaining = duration;
        while(timeRemaining != 0f)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 99f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos, 1f - (timeRemaining / duration));
            yield return null;
        }
        transform.localPosition = pos;
    }
}
