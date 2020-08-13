﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemsPowerUPsInfoScript : MonoBehaviour
{

    public delegate void ItemPickedUp();
    public event ItemPickedUp ItemPickedUpEvent;

    public ScriptableObjectItemPowerUps ItemPowerUpInfo;
    //public SpriteRenderer Icon;
    public PowerUpColorTypes color = PowerUpColorTypes.White;
    public TextMeshPro puText = null;
    public Animator Anim;
    public Vector2Int Pos;
    protected Vector3 position;
    protected GameObject activeParticles = null;
    private IEnumerator OnField_Co;
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
        if(ItemPowerUpInfo.activeParticles != null)
        {
            activeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.activeParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, transform.position, SideType.LeftSide, AttackInputType.Skill1);
            activeParticles.transform.position -= new Vector3(0f, 0.3f, 0f);
        }
        Anim.SetInteger("Color", (int)color);
        Anim.SetBool("FadeInOut", true);
        StartCoroutine(spawn_Co());
    }

    private IEnumerator spawn_Co()
    {
        OnField_Co = DurationOnBattleField_Co();
        yield return OnField_Co;
        yield return StopItem_Co();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Side"))
        {
            ItemPickedUpEvent?.Invoke();
            CharHitted = other.GetComponentInParent<BaseCharacter>();
            CharHitted.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(),
                ElementalType.Neutral, other.GetComponentInParent<BaseCharacter>(), ItemPowerUpInfo));
            CharHitted.Sic.PotionPicked++;

            ItemType itemType = ItemType.PowerUP_FullRecovery;
            switch (ItemPowerUpInfo.StatsToAffect)
            {
                case (BuffDebuffStatsType.BaseSpeed):
                    itemType = ItemType.PowerUp_Speed;
                    break;
                case (BuffDebuffStatsType.Ether):
                    itemType = ItemType.PowerUP_Stamina;
                    break;
                case (BuffDebuffStatsType.Damage):
                    itemType = ItemType.PowerUp_Damage;
                    break;
                case (BuffDebuffStatsType.ShieldRegeneration):
                    itemType = ItemType.PowerUp_Shield;
                    break;
                case (BuffDebuffStatsType.Regen):
                    itemType = ItemType.PowerUP_Health;
                    break;
                default:
                    Debug.LogError("Error with potion type in event collection... Collected powerup effect is: " + ItemPowerUpInfo.StatsToAffect.ToString());
                    break;
            }
            EventManager.Instance?.AddPotionCollected(itemType);


            if (ItemPowerUpInfo.collectionAudio != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, ItemPowerUpInfo.collectionAudio, AudioBus.MidPrio, other.gameObject.transform);
            }
            if(ItemPowerUpInfo.terminationParticles != null)
            {
                ParticleManagerScript.Instance.FireParticlesInPosition(ItemPowerUpInfo.terminationParticles, CharacterNameType.None, AttackParticlePhaseTypes.Bullet, position, SideType.LeftSide, AttackInputType.Skill1);
            }


            StopCoroutine(OnField_Co);
            StartCoroutine(StopItem_Co());
        }
    }


    private IEnumerator StopItem_Co()
    {
        activeParticles?.SetActive(false);
        Anim.SetBool("FadeInOut", false);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }


    private IEnumerator DurationOnBattleField_Co()
    {
        yield return BattleManagerScript.Instance.WaitFor(ItemPowerUpInfo.DurationOnField, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
    }
}
