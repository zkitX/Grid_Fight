using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBattleFieldManager : MonoBehaviour
{
    public static UIBattleFieldManager Instance;
    public GameObject BaseBattleFieldIndicator;


    TextMeshProUGUI currentTMP;
    GameObject b;
    BattleFieldIndicatorMaterialClass currentBFIM;
    public GameObject ComboIndicator;
    private List<GameObject> BattleFieldIndicators = new List<GameObject>();
    private List<GameObject> ComboIndicators = new List<GameObject>();

    public List<BattleFieldIndicatorMaterialClass> Materials = new List<BattleFieldIndicatorMaterialClass>();


    private Camera mCamera;
    bool setupIsComplete = false;

    private void Awake()
    {
        Instance = this;
        mCamera = Camera.main;
    }

    public void SetupCharListener(BaseCharacter charOwner)
    {
        charOwner.HealthStatsChangedEvent += CharOwner_HealthStatsChangedEvent;
    }


    #region Combo
    public void DisplayComboStyleSplasher(string text, Vector3 pos, float scaler, Color color, bool animateLong, out float animLength)
    {
        GameObject cI = ComboIndicators.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (cI == null)
        {
            cI = Instantiate(ComboIndicator, transform);
        }

        List<Color> colors = new List<Color>();
        //colors.Add(color * 0.3f);
        colors.Add(color);

        TextMeshProUGUI[] thaScaredtexts = cI.GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < thaScaredtexts.Length; i++)
        {
            thaScaredtexts[i].color = colors[i];
            thaScaredtexts[i].text = text;
        }

        cI.transform.localScale = Vector3.one * scaler;
        cI.transform.position = Camera.main.WorldToScreenPoint(pos);

        cI.GetComponent<Animation>().clip = cI.GetComponent<Animation>().GetClip(animateLong ? "ComboSplash_Long":"ComboSplash");
        animLength = cI.GetComponent<Animation>().clip.length;

        StartCoroutine(DisplayComboSplashAnim(cI));
    }

    IEnumerator DisplayComboSplashAnim(GameObject obj)
    {
        obj.SetActive(true);
        Animation anim = obj.GetComponent<Animation>();
        anim.Play();

        while (anim.isPlaying)
        {
            yield return null;
        }
        obj.SetActive(false);
    }

    #endregion

    #region BattleFieldIndicator

    private void CharOwner_HealthStatsChangedEvent(float value, BattleFieldIndicatorType changeType, Transform charOwner)
    {
        StartCoroutine(BackfireCo(value, changeType, charOwner));
    }

    private IEnumerator BackfireCo(float damage, BattleFieldIndicatorType changeType, Transform charOwner)
    {
        if (damage == 0) yield break;

        float timer = 0;
        GameObject d = GetBattleFieldIndicator();
        d.SetActive(true);
        switch (changeType)
        {
            case BattleFieldIndicatorType.Damage:
                SetupIndicator(changeType, ((int)(damage * 100)).ToString(), d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 1);
                break;
            case BattleFieldIndicatorType.Defend:
                SetupIndicator(changeType, ((int)(damage * 100)).ToString(), d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 4);
                break;
            case BattleFieldIndicatorType.CompleteDefend:
                SetupIndicator(changeType, "DEFEND", d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 3);
                break;
            case BattleFieldIndicatorType.Heal:
                SetupIndicator(changeType, ((int)(damage * 100)).ToString(), d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 5);
                break;
            case BattleFieldIndicatorType.CriticalHit:
                SetupIndicator(changeType, "CRITICAL  " + ((int)(damage * 100)).ToString(), d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 2);
                break;
            case BattleFieldIndicatorType.Invulnerable:
                SetupIndicator(changeType, "INVULNERABLE", d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 5);
                break;
            case BattleFieldIndicatorType.Rebirth:
                SetupIndicator(changeType, "RIBIRTH", d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 5);
                break;
            case BattleFieldIndicatorType.Backfire:
                SetupIndicator(changeType, "BACKFIRE", d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 5);
                break;
            case BattleFieldIndicatorType.Miss:
                SetupIndicator(changeType, "MISS", d);
                timer = 0.8f;
                SetAnim(d.GetComponentInChildren<Animator>(), 5);
                break;
        }

        while (timer >= 0f)
        {
            if (charOwner.gameObject.activeInHierarchy)
            {
                d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

            timer -= BattleManagerScript.Instance.FixedDeltaTime;
        }
        d.SetActive(false);
    }


    private void CharOwner_HealthStatsChangedEvent(string text, BattleFieldIndicatorType changeType, Transform charOwner)
    {
        StartCoroutine(BackfireCo(text, changeType, charOwner));
    }

    private IEnumerator BackfireCo(string text, BattleFieldIndicatorType changeType, Transform charOwner)
    {
        float timer = 0;
        GameObject d = GetBattleFieldIndicator();
        d.SetActive(true);
        SetupIndicator(changeType, text, d);
        timer = 2f;
        SetAnim(d.GetComponentInChildren<Animator>(), changeType == BattleFieldIndicatorType.Buff ? 6 : 7);

        while (timer >= 0f)
        {
            if (charOwner.gameObject.activeInHierarchy)
            {
                d.transform.position = mCamera.WorldToScreenPoint(charOwner.transform.position);
            }
            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);

            timer -= BattleManagerScript.Instance.FixedDeltaTime;
        }
        d.SetActive(false);
    }


    private void SetupIndicator(BattleFieldIndicatorType changeType, string txt, GameObject d)
    {
        currentBFIM = Materials.Where(r => r.BattleFieldIndicatorT == changeType).First();
        currentTMP = d.GetComponentInChildren<TextMeshProUGUI>();
        currentTMP.text = txt;
        currentTMP.material = currentBFIM.Mat;
        SetAnim(d.GetComponentInChildren<Animator>(), 1);
    }

    private void SetAnim(Animator anim, int value)
    {
        anim.SetInteger("State", value);
    }
    private GameObject GetBattleFieldIndicator()
    {
        b = BattleFieldIndicators.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (b == null)
        {
            b = Instantiate(BaseBattleFieldIndicator, transform);
            BattleFieldIndicators.Add(b);
        }

        return b;
    }
    #endregion
}


[System.Serializable]
public class BattleFieldIndicatorMaterialClass
{
    [HideInInspector]public string name;
    public BattleFieldIndicatorType BattleFieldIndicatorT;
    public Material Mat;

    public BattleFieldIndicatorMaterialClass()
    {

    }

    public BattleFieldIndicatorMaterialClass(BattleFieldIndicatorType battleFieldIndicatorT, Material mat)
    {
        BattleFieldIndicatorT = battleFieldIndicatorT;
        Mat = mat;
    }
}
