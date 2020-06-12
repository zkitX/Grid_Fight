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
    public Animator Anim;
    public Vector2Int Pos;
    protected Vector3 position;
    protected GameObject activeParticles = null;

    private BaseCharacter CharHitted;
    float Duration;


    public void SetItemPowerUp(ScriptableObjectItemPowerUps itemPowerUpInfo, Vector3 worldPos, Vector2Int gridPos, float duration = 0f)
    {
        position = worldPos;
        Pos = gridPos;
        ItemPowerUpInfo = itemPowerUpInfo;
        //Icon.sprite = itemPowerUpInfo.Icon;
        puText.text = itemPowerUpInfo.powerUpText;
        color = itemPowerUpInfo.color;
        transform.position = worldPos;
        Duration = duration;
        activeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.activeParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, worldPos, SideType.LeftSide, AttackInputType.Skill1);
        activeParticles.transform.position -= new Vector3(0f, 0.3f, 0f);
        Anim.SetInteger("Color", (int)color);
        Anim.SetBool("FadeInOut", true);
        StartCoroutine(spawn_Co());
    }

    private IEnumerator spawn_Co()
    {
        yield return DurationOnBattleField_Co();
        activeParticles.SetActive(false);
        Anim.SetBool("FadeInOut", false);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
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
                case (BuffDebuffStatsType.Health):
                    itemType = ItemType.PowerUP_Health;
                    powerUpAudio = BattleManagerScript.Instance.AudioProfile.PowerUp_Health;
                    break;
                default:
                    Debug.LogError("Error with potion type in event collection... Collected powerup effect is: " + ItemPowerUpInfo.StatsToAffect.ToString());
                    break;
            }
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, powerUpAudio, AudioBus.MidPrio, other.gameObject.transform);
            EventManager.Instance?.AddPotionCollected(itemType);
            ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.terminationParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, position, SideType.LeftSide, AttackInputType.Skill1);
            Duration = 0;
        }
    }

    private IEnumerator DurationOnBattleField_Co()
    {
        yield return BattleManagerScript.Instance.WaitFor(ItemPowerUpInfo.DurationOnField, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
    }
}
