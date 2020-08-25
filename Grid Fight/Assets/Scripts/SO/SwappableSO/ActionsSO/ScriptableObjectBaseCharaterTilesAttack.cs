using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/TilesAttack")]
public class ScriptableObjectBaseCharaterTilesAttack : ScriptableObjectBaseCharacterBaseAttack
{

    protected float tempFloat_1;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    private Spine.Animation tempAnimation;
    private List<Spine.Timeline> tempTimeLine;
    private Spine.EventTimeline tempEventTimeLine;
    private float ChargingTime;

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString() || completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString())
        {
            CharOwner.transform.position = new Vector3(100, 100, 100);
            CharOwner.SetAttackReady(false);
            CharOwner.gameObject.SetActive(false);
        }

        if (completedAnim.Contains("IdleToAtk") && CharOwner.SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            currentAttackPhase = AttackPhasesType.Charging;
        }

        if (completedAnim.Contains("_Loop") && CharOwner.SpineAnim.CurrentAnim.Contains("_Loop"))
        {
            currentAttackPhase = AttackPhasesType.End;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            InteruptAttack();
        }

        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override void CreateAttack(Vector2Int nextAttackPos)
    {
        if (CharOwner.nextAttack == null) return;

        if (CharOwner.nextAttack.CurrentAttackType == AttackType.Totem)
        {
            base.CreateAttack(nextAttackPos);
            return;
        }

        CreateTileAttack(nextAttackPos);
    }

    public override void CreateTileBullet(BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo, Vector2Int nextAttackPos)
    {
        bullet = BulletManagerScript.Instance.GetBullet().GetComponent<BulletScript>();
        bullet.isColliding = false;
        bullet.BulletBehaviourInfo = null;
        bullet.BulletBehaviourInfoTile = bulletBehaviourInfo;
        float duration = bulletBehaviourInfo.BulletTravelDurationPerTile * (float)(Mathf.Abs(CharOwner.UMS.CurrentTilePos.y - nextAttackPos.y));
        bullet.BulletDuration = duration > bulletBehaviourInfo.Delay ? bulletBehaviourInfo.Delay - CharOwner.SpineAnim.SpineAnimationState.GetCurrent(0).TrackTime : duration;
        bullet.BulletEffects.Clear();
        bullet.bts = GridManagerScript.Instance.GetBattleTile(nextAttackPos);
        bullet.DestinationTile = nextAttackPos;

        CompleteBulletSetup();
    }

    public void CreateTileAttack(Vector2Int nextAttackPos)
    {
        if (CharOwner.nextAttack != null && CharOwner.nextAttack.CurrentAttackType == AttackType.Tile && CharOwner.CharInfo.Health > 0 && CharOwner.IsOnField)
        {
            CharOwner.CharInfo.WeakAttack.DamageMultiplier = CharOwner.CharInfo.WeakAttack.B_DamageMultiplier * CharOwner.nextAttack.DamageMultiplier;
            CharOwner.CharInfo.StrongAttack.DamageMultiplier = CharOwner.CharInfo.StrongAttack.B_DamageMultiplier * CharOwner.nextAttack.DamageMultiplier;

            if (CharOwner.nextAttack.AttackInput > AttackInputType.Strong && CharOwner.CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script)
            {
                StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == CharOwner.CharInfo.CharacterID).First().DamageExp += CharOwner.nextAttack.ExperiencePoints;
            }

            for (int i = 0; i < CharOwner.nextAttack.TilesAtk.BulletTrajectories.Count; i++)
            {
                foreach (BattleFieldAttackTileClass target in CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].BulletEffectTiles)
                {
                    tempInt_1 = UnityEngine.Random.Range(0, 100);
                    if (tempInt_1 <= CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].ExplosionChances)
                    {
                        tempVector2Int = CharOwner.nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnTarget ? target.Pos + nextAttackPos :
                        CharOwner.nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf ? target.Pos + CharOwner.UMS.CurrentTilePos : target.Pos;
                        if (GridManagerScript.Instance.isPosOnField(tempVector2Int))
                        {
                            BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(tempVector2Int);
                            if (bts._BattleTileState != BattleTileStateType.NonUsable)
                            {
                                if (CharOwner.nextAttack.TilesAtk.AtkType == BattleFieldAttackType.OnItSelf && bts.WalkingSide == CharOwner.UMS.WalkingSide)
                                {
                                    shotsLeftInAttack = 1;

                                    bts.BattleTargetScript.SetAttack(CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharOwner.CharInfo.Elemental, this,
                                    target, target.EffectChances);
                                }
                                else if (CharOwner.nextAttack.TilesAtk.AtkType != BattleFieldAttackType.OnItSelf && bts.WalkingSide != CharOwner.UMS.WalkingSide)
                                {
                                    //new way

                                    tempString = CharOwner.GetAttackAnimName();
                                    tempAnimation = CharOwner.SpineAnim.skeleton.Data.FindAnimation(tempString);

                                    tempTimeLine = tempAnimation?.Timelines?.Items?.Where(r => r is Spine.EventTimeline).ToList();
                                    tempEventTimeLine = tempTimeLine.Where(r => ((Spine.EventTimeline)r).Events.Where(p => p.Data.Name == "FireBulletParticle").ToList().Count > 0).First() as Spine.EventTimeline;

                                    ChargingTime = tempEventTimeLine.Events.Where(r => r.Data.Name == "FireBulletParticle").First().Time;

                                    shotsLeftInAttack = 1;
                                    if (CharOwner.nextAttack.AttackInput > AttackInputType.Weak && i == 0)
                                    {
                                        bts.BattleTargetScript.SetAttack(CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharOwner.CharInfo.Elemental, CharOwner,
                                    target, target.EffectChances, (CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(CharOwner.UMS.CurrentTilePos.y - CharOwner.nextAttackPos.y))) + tempFloat_1);//(nextAttack.TilesAtk.BulletTrajectories[i].Delay * 0.1f)
                                    }
                                    else if (CharOwner.nextAttack.AttackInput == AttackInputType.Weak)
                                    {
                                        bts.BattleTargetScript.SetAttack(CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].Delay, tempVector2Int,
                                    CharOwner.CharInfo.Elemental, CharOwner,
                                    target, target.EffectChances, (CharOwner.nextAttack.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile * (float)(Mathf.Abs(CharOwner.UMS.CurrentTilePos.y - CharOwner.nextAttackPos.y))) + tempFloat_1); // 
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (shotsLeftInAttack == 0)
            {
               InteruptAttack();
            }
        }
    }

    public override IEnumerator Attack()
    {
        yield return base.Attack();
        CharOwner.ResetAudioManager();
    }


    public override IEnumerator StartIdleToAtk()
    {
        yield break;
    }
    public override IEnumerator IdleToAtk()
    {
        yield break;
    }

    public override IEnumerator StartCharging()
    {
        yield break;
    }
    public override IEnumerator Charging()
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        strongAttackTimer += BattleManagerScript.Instance.DeltaTime;
        if (strongAttackTimer > ChargingTime && CharOwner.CharInfo.Health > 0f)
        {
            currentAttackPhase = AttackPhasesType.Firing;
        }
    }
  
    public override IEnumerator StartLoop()
    {
        yield break;
    }
    public override IEnumerator Loop()
    {
        yield break;
    }
    public override IEnumerator StartAtkToIdle()
    {
        yield break;
    }
    public override IEnumerator AtkToIdle()
    {
        yield break;
    }


}


