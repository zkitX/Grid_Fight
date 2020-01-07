using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIBattleFieldScript : MonoBehaviour
{

    public BaseCharacter CharOwner;
    [SerializeField]
    private Image CharacterHealthBar;
    [SerializeField]
    private Image CharacterStaminaBar;
    private Camera mCamera;
    public Canvas CanvasParent;
    public GameObject Damage;
    public GameObject Healing;
    // Start is called before the first frame update
    void Start()
    {
        mCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (CharOwner != null)
        {
            transform.position = mCamera.WorldToScreenPoint(CharOwner.transform.position);
            CharacterHealthBar.rectTransform.anchoredPosition = new Vector2(CharacterHealthBar.rectTransform.rect.width - ((CharacterHealthBar.rectTransform.rect.width / 100) * CharOwner.CharInfo.HealthPerc), 0);
            CharacterStaminaBar.rectTransform.anchoredPosition = new Vector2(CharacterStaminaBar.rectTransform.rect.width - ((CharacterStaminaBar.rectTransform.rect.width / 100) * CharOwner.CharInfo.StaminaPerc), 0);
        }
    }


    public void SetupCharOwner(BaseCharacter charOwner)
    {
        if(CharOwner != null)
        {
            CharOwner.DamageReceivedEvent -= CharOwner_DamageReceivedEvent;
            CharOwner.HealReceivedEvent -= CharOwner_HealReceivedEvent;
        }
        CharOwner = charOwner;
        CharOwner.DamageReceivedEvent += CharOwner_DamageReceivedEvent;
        CharOwner.HealReceivedEvent += CharOwner_HealReceivedEvent;
    }

    private void CharOwner_HealReceivedEvent(float heal)
    {
        StartCoroutine(HealingCo(heal));
    }

    private IEnumerator HealingCo(float heal)
    {
        GameObject h = Instantiate(Healing, transform);
        h.GetComponent<TextMeshProUGUI>().text = ((int)(heal * 100)).ToString();
        yield return new WaitForSecondsRealtime(2);
        Destroy(h);
    }

    private void CharOwner_DamageReceivedEvent(float damage)
    {
        StartCoroutine(DamageCo(damage));
    }

    private IEnumerator DamageCo(float damage)
    {
        GameObject d = Instantiate(Damage, transform);
        d.GetComponent<TextMeshProUGUI>().text = ((int)(damage * 100)).ToString();
        yield return new WaitForSecondsRealtime(2);
        Destroy(d);
    }
}
