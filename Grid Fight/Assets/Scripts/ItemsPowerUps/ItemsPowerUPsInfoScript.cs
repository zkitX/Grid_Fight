using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{
    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    public SpriteRenderer Icon;
    private IEnumerator DurationOnBattleFieldCo;
    public Animator Anim;


    public void SetItemPowerUp(ScriptableObjectItemPowerUps itemPowerUpInfo, Vector3 pos)
    {
        ItemPowerUpInfo = itemPowerUpInfo;
        Icon.sprite = itemPowerUpInfo.Icon;
        transform.position = pos;
        if(DurationOnBattleFieldCo != null)
        {
            StopCoroutine(DurationOnBattleFieldCo);
        }

        DurationOnBattleFieldCo = DurationOnBattleField_Co();
        StartCoroutine(DurationOnBattleFieldCo);
        Anim.SetBool("FadeInOut", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            other.GetComponentInParent<BaseCharacter>().Buff_DebuffCo(new Buff_DebuffClass(ItemPowerUpInfo.Name, ItemPowerUpInfo.EffectDuration,
                Random.Range(ItemPowerUpInfo.Value.x, ItemPowerUpInfo.Value.y),
                ItemPowerUpInfo.StatsToAffect, ItemPowerUpInfo.StatsChecker, new ElementalResistenceClass(),
                ElementalType.Neutral, ItemPowerUpInfo.AnimToFire, ItemPowerUpInfo.Particles));

            ItemType itemType = ItemType.PowerUP_FullRecovery;
            switch (ItemPowerUpInfo.StatsToAffect)
            {
                case (BuffDebuffStatsType.HealthStats_Health_Overtime):
                    itemType = ItemType.PowerUP_Health;
                    break;
                case (BuffDebuffStatsType.StaminaStats_Stamina_Overtime):
                    itemType = ItemType.PowerUp_Speed;
                    break;
                case (BuffDebuffStatsType.StaminaStats_Regeneration):
                    itemType = ItemType.PowerUP_Stamina;
                    break;
                case (BuffDebuffStatsType.RapidAttack_DamageMultiplier):
                    itemType = ItemType.PowerUp_Damage;
                    break;
                default:
                    Debug.LogError("Error with potion type in event collection");
                    break;
            }
            EventManager.Instance?.AddPotionCollected(itemType);
            gameObject.SetActive(false);
        }
    }


    private IEnumerator DurationOnBattleField_Co()
    {
        float timer = 0;
        while (timer <= ItemPowerUpInfo.DurationOnField)
        {
            yield return BattleManagerScript.Instance.PauseUntil();
            timer += Time.fixedDeltaTime;
        }
        DurationOnBattleFieldCo = null;
        Anim.SetBool("FadeInOut", false);
    }

    public void DisablePowerUp()
    {
        gameObject.SetActive(false);
    }
}
