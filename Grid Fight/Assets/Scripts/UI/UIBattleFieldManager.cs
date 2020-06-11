using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UIBattleFieldManager : MonoBehaviour
{
    public static UIBattleFieldManager Instance;
    public GameObject UIBattleField;

    private List<GameObject> ListOfUIBattleField = new List<GameObject>();

    public GameObject Damage;
    public GameObject Defence;
    public GameObject PartialDefend;
    public GameObject Healing;
    public GameObject CriticalHit;
    private Dictionary<int, GameObject> Damages = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Defends = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> PartialDefends = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> Healings = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> CriticalHits = new Dictionary<int, GameObject>();

    private Camera mCamera;
    bool setupIsComplete = false;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Update()
    {

        if(BattleManagerScript.Instance != null && !setupIsComplete)
        {
            mCamera = Camera.main;
            setupIsComplete = true;
        }
    }

    public void SetupCharListener(BaseCharacter charOwner)
    {
        charOwner.HealthStatsChangedEvent += CharOwner_HealthStatsChangedEvent;
    }

    private void CharOwner_HealthStatsChangedEvent(float value, HealthChangedType changeType, Transform charOwner)
    {
        switch (changeType)
        {
            case HealthChangedType.Damage:
                if (value == 0) return;
                CharOwner_DamageReceivedEvent(value, charOwner);
                break;
            case HealthChangedType.Defend:
                CharOwner_DefendDamageReceivedEvent(value, charOwner);
                break;
            case HealthChangedType.Heal:
                CharOwner_HealReceivedEvent(value, charOwner);
                break;
            case HealthChangedType.CriticalHit:
                CharOwner_CriticalHitReceivedEvent(value, charOwner);
                break;
            default:
                break;
        }
    }

    private void CharOwner_HealReceivedEvent(float heal, Transform charOwner)
    {
        StartCoroutine(HealingCo(heal, charOwner));
    }

    private void CharOwner_DamageReceivedEvent(float damage, Transform charOwner)
    {
        StartCoroutine(DamageCo(damage, charOwner));
    }

    private void CharOwner_DefendDamageReceivedEvent(float damage, Transform charOwner)
    {
        StartCoroutine(DefendCo(damage, charOwner));
    }

    private void CharOwner_CriticalHitReceivedEvent(float damage, Transform charOwner)
    {
        StartCoroutine(CriticalHitCo(damage, charOwner));
    }
    
    private IEnumerator HealingCo(float heal, Transform charOwner)
    {
        float timer = 0;
        bool isAlive = true;
        GameObject h = Healings.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (h == null)
        {
            h = Instantiate(Healing, transform);
        }
        h.SetActive(true);
        h.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(heal * 100)).ToString();
        SetAnim(h.GetComponentInChildren<Animator>(), 5);
        while (timer <= 0.8f)
        {
            if(charOwner.gameObject.activeInHierarchy && isAlive)
            {
                h.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            else if(isAlive)
            {
                isAlive = false;
            }
            yield return BattleManagerScript.Instance.WaitUpdate();

            timer += Time.deltaTime;
        }
        h.SetActive(false);
    }

    private IEnumerator DamageCo(float damage, Transform charOwner)
    {
        float timer = 0;
        bool isAlive = true;
        GameObject d = Damages.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (d == null)
        {
            d = Instantiate(Damage, transform);
            Damages.Add(Damages.Count, d);
        }
        d.SetActive(true);
        d.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(damage * 100)).ToString();
        d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
        SetAnim(d.GetComponentInChildren<Animator>(), 1);
        while (timer <= 0.8f)
        {
            if (charOwner.gameObject.activeInHierarchy && isAlive)
            {
                d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            else if (isAlive)
            {
                isAlive = false;
            }
            yield return BattleManagerScript.Instance.WaitUpdate();

            timer += Time.deltaTime;
        }
        d.SetActive(false);
    }

    private IEnumerator DefendCo(float damage, Transform charOwner)
    {
        float timer = 0;
        bool isAlive = true;
        string res = ((int)(damage * 100)).ToString();
        GameObject d;
        if (damage == 0)
        {
            d = Defends.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
            if (d == null)
            {
                d = Instantiate(Defence, transform);
                Defends.Add(Defends.Count, d);
            }
            res = "DEF";
        }
        else
        {
            d = PartialDefends.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
            if (d == null)
            {
                d = Instantiate(PartialDefend, transform);
                PartialDefends.Add(PartialDefends.Count, d);
            }
        }
        
        d.SetActive(true);
        d.GetComponentInChildren<TextMeshProUGUI>().text = res;
        if (!charOwner.gameObject.activeInHierarchy)
        {
            d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
        }

        SetAnim(d.GetComponentInChildren<Animator>(), res == "DEF" ?  3 : 4);

        while (timer <= 0.8f)
        {
            if (charOwner.gameObject.activeInHierarchy && isAlive)
            {
                d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            else if (isAlive)
            {
                isAlive = false;
            }
            yield return BattleManagerScript.Instance.WaitUpdate();

            timer += Time.deltaTime;
        }
        d.SetActive(false);
    }

    private IEnumerator CriticalHitCo(float damage, Transform charOwner)
    {
        float timer = 0;
        bool isAlive = true;
        GameObject c = CriticalHits.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (c == null)
        {
            c = Instantiate(CriticalHit, transform);
            CriticalHits.Add(CriticalHits.Count, c);
        }
        c.SetActive(true);
        c.GetComponentInChildren<TextMeshProUGUI>().text = "CRITICAL  " + ((int)(damage * 100)).ToString();
        if (!charOwner.gameObject.activeInHierarchy)
        {
            c.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
        }

        SetAnim(c.GetComponentInChildren<Animator>(), 2);

        while (timer <= 0.8f)
        {
            if (charOwner.gameObject.activeInHierarchy && isAlive)
            {
                c.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            else if (isAlive)
            {
                isAlive = false;
            }
            yield return BattleManagerScript.Instance.WaitUpdate();

            timer += Time.deltaTime;
        }
        c.SetActive(false);
    }


    private void SetAnim(Animator anim, int value)
    {
        anim.SetInteger("State", value);
    }

}
