using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{
    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    //public SpriteRenderer Icon;
    public PowerUpColorTypes color = PowerUpColorTypes.White;
    public TextMeshPro puText = null;
    private IEnumerator DurationOnBattleFieldCo;
    public Animator Anim;

    protected Vector3 position;
    protected GameObject activeParticles = null;

    private BaseCharacter CharHitted;
    public void SetItemPowerUp(ScriptableObjectItemPowerUps itemPowerUpInfo, Vector3 pos, float duration = 0f)
    {
        position = pos;
        ItemPowerUpInfo = itemPowerUpInfo;
        //Icon.sprite = itemPowerUpInfo.Icon;
        puText.text = itemPowerUpInfo.powerUpText;
        color = itemPowerUpInfo.color;
        transform.position = pos;

        activeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.activeParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, pos, SideType.LeftSide, AttackInputType.Skill1);
        activeParticles.transform.position -= new Vector3(0f, 0.6f, 0f);

        Anim.SetInteger("Color", (int)color);
        Anim.SetBool("FadeInOut", true);
        if (DurationOnBattleFieldCo != null)
        {
            StopCoroutine(DurationOnBattleFieldCo);
        }

        DurationOnBattleFieldCo = DurationOnBattleField_Co(duration);
        StartCoroutine(DurationOnBattleFieldCo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            CharHitted = other.GetComponentInParent<BaseCharacter>();
            CharHitted.Buff_DebuffCo(new Buff_DebuffClass(ItemPowerUpInfo.Name, ItemPowerUpInfo.EffectDuration,
                ItemPowerUpInfo.Value,
                ItemPowerUpInfo.StatsToAffect, ItemPowerUpInfo.StatsChecker, new ElementalResistenceClass(),
                ElementalType.Neutral, ItemPowerUpInfo.AnimToFire, ItemPowerUpInfo.Particles, other.GetComponentInParent<BaseCharacter>()));
            CharHitted.Sic.PotionPicked++;
            AudioClipInfoClass powerUpAudio = null;
            ItemType itemType = ItemType.PowerUP_FullRecovery;
            switch (ItemPowerUpInfo.StatsToAffect)
            {
                case (BuffDebuffStatsType.HealthStats_BaseHealthRegeneration):
                    itemType = ItemType.PowerUP_Health;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Health;
                    break;
                case (BuffDebuffStatsType.SpeedStats_BaseSpeed):
                    itemType = ItemType.PowerUp_Speed;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Speed;
                    break;
                case (BuffDebuffStatsType.StaminaStats_Stamina):
                    itemType = ItemType.PowerUP_Stamina;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Stamina;
                    break;
                case (BuffDebuffStatsType.RapidAttack_CriticalChance):
                    itemType = ItemType.PowerUp_Damage;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Damage;
                    break;
                case (BuffDebuffStatsType.DamageStats_BaseDamage):
                    itemType = ItemType.PowerUp_Damage;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Damage;
                    break;
                case (BuffDebuffStatsType.ShieldStats_BaseShieldRegeneration):
                    itemType = ItemType.PowerUp_Shield;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Shield;
                    break;
                default:
                    Debug.LogError("Error with potion type in event collection... Collected powerup effect is: " + ItemPowerUpInfo.StatsToAffect.ToString());
                    break;
            }
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, powerUpAudio, AudioBus.MidPrio, other.gameObject.transform);
            EventManager.Instance?.AddPotionCollected(itemType);
            StartCoroutine(CollectedCo());
        }
    }

    IEnumerator CollectedCo()
    {
        activeParticles.SetActive(false);
        Anim.SetBool("FadeInOut", false);
        ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.terminationParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, position, SideType.LeftSide, AttackInputType.Skill1);
        yield return null;

        yield return new WaitForSeconds(Anim.GetCurrentAnimatorStateInfo(0).length + Anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.2f);

        gameObject.SetActive(false);
    }


    private IEnumerator DurationOnBattleField_Co(float duration = 0f)
    {
        if (duration == 0f) duration = ItemPowerUpInfo.DurationOnField;
        float timer = 0;
        while (timer <= duration)
        {
            yield return BattleManagerScript.Instance.WaitUpdate();
            timer += Time.fixedDeltaTime;
        }
        DurationOnBattleFieldCo = null;


        activeParticles.SetActive(false);
        Anim.SetBool("FadeInOut", false);
        yield return null;
        yield return new WaitForSeconds(Anim.GetCurrentAnimatorStateInfo(0).length + Anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.2f);
        gameObject.SetActive(false);
    }

    public void DisablePowerUp()
    {
        gameObject.SetActive(false);
    }
}
