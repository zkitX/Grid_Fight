using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{
    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    public SpriteRenderer Icon;
    public void SetItemPowerUp(ScriptableObjectItemPowerUps itemPowerUpInfo, Vector3 pos)
    {
        ItemPowerUpInfo = itemPowerUpInfo;
        Icon.sprite = itemPowerUpInfo.Icon;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            other.GetComponentInParent<BaseCharacter>().Buff_DebuffCo(new Buff_DebuffClass(ItemPowerUpInfo.EffectDuration,
                Random.Range(ItemPowerUpInfo.Value.x, ItemPowerUpInfo.Value.y),
                ItemPowerUpInfo.StatsToAffect, new ElementalResistenceClass(),
                ElementalType.Neutral, ItemPowerUpInfo.AnimToFire));

            gameObject.SetActive(false);
        }
    }
}
