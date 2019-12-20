using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{
    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    public SpriteRenderer Icon;
    private IEnumerator DurationOnBattleFieldCo;
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


    private IEnumerator DurationOnBattleField_Co()
    {
        float timer = 0;
        while (timer <= ItemPowerUpInfo.DurationOnField)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
        DurationOnBattleFieldCo = null;
        gameObject.SetActive(false);
    }
}
