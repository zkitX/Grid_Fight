using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{
    protected bool MoveCoOn = true;
    protected IEnumerator MoveActionCo;
    protected float LastAttackTime;
    public int AttackWillPerc = 13;
    public int UpDownMovementPerc = 13;
    public int TowardMovementPerc = 13;
    public int AwayMovementPerc = 13;
    List<HitInfoClass> HittedByList = new List<HitInfoClass>();
    List<AggroInfoClass> AggroInfoList = new List<AggroInfoClass>();
    float totDamage = 0;
    bool strongAnimDone = false;
    public ScriptableObjectAI CurrentAIState;

    protected bool UnderAttack
    {
        get
        {
            if (Time.time > LastAttackTime + 10f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    protected bool AIMove = false;

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
        CurrentPlayerController = ControllerType.Enemy;
        shotsLeftInAttack = 0;
    }

    public override void SetUpLeavingBattle()
    {
        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        EventManager.Instance.AddCharacterSwitched((BaseCharacter)this);
    }

    public override void SetAttackReady(bool value)
    {
        if (value)
        {
            /* StartAttakCo();
             StartMoveCo();*/

            StartCoroutine(AI());
            HittedByList.Clear();
        }
        CharInfo.DefenceStats.BaseDefence = Random.Range(0.7f, 1);
        base.SetAttackReady(value);
    }


    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
     
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        Attacking = false;
        BuffsDebuffsList.ForEach(r =>
        {
            r.Duration = 0;
            r.CurrentBuffDebuff.Effect._Duration = Vector2.zero;
            r.CurrentBuffDebuff.Stop_Co = true;
        }
        );

     
 		for (int i = 0; i < HittedByList.Count; i++)
        {
            StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == HittedByList[i].CharacterId).First();
            sic.BaseExp += (HittedByList[i].Damage / totDamage) * CharInfo.ExperienceValue;
        }
        totDamage = 0;

        for (int i = 0; i < UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
            UMS.Pos[i] = Vector2Int.zero;
        }
        base.SetCharDead();
        transform.position = new Vector3(100, 100, 100);
        SpineAnim.SpineAnimationState.ClearTracks();
        Invoke("DisableChar", 0.5f);
    }
    private void DisableChar()
    {

        gameObject.SetActive(false);

    }

    float lastAttackTime = 0;
    public virtual IEnumerator AI_Old()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }


                List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                if (enemys.Count > 0)
                {
                    BaseCharacter targetChar = enemys.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
                    /*BaseCharacter targetChar = null;
                    List<BaseCharacter> possibleTargets = enemys.Where(r => Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x) <= 1).ToList();
                    if (possibleTargets.Count > 0)
                    {
                        targetChar = possibleTargets[Random.Range(0, possibleTargets.Count)];
                    }*/
                    if (targetChar != null && (nextAttack == null || (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler)))
                    {
                        lastAttackTime = Time.time;
                        nextAttackPos = targetChar.UMS.CurrentTilePos;
                        yield return AttackSequence();
                    }
                    else
                    {

                        int randomizer = Random.Range(0, 100);
                        if (randomizer < UpDownMovementPerc)
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Left);
                        }
                        else if (randomizer > (100 - UpDownMovementPerc))
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Right);
                        }
                        else
                        {
                            targetChar = GetTargetChar(enemys);
                            if (targetChar.UMS.CurrentTilePos.x < UMS.CurrentTilePos.x)
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Up);
                            }
                            else
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Down);
                            }
                        }
                    }
                }
                yield return null;
            }
        }
    }

    GameObject psAI = null;
    public virtual IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }
                ScriptableObjectAI prev = CurrentAIState;
                CurrentAIState = CharInfo.GetCurrentAI(AggroInfoList, UMS.CurrentTilePos);
                if(prev == null || prev.AI_Type != CurrentAIState.AI_Type)
                {
                    SetCurrentAIValues();
                    CurrentAIState.ModifyStats(CharInfo);
                    if(psAI != null)
                    {
                        psAI.SetActive(false);
                    }
                    psAI = ParticleManagerScript.Instance.GetParticle(CurrentAIState.AIPs.PSType);
                    psAI.transform.parent = SpineAnim.transform;
                    psAI.transform.localPosition = Vector3.zero;
                    psAI.SetActive(true);
                }

                int atkChances = Random.Range(0 ,100);
                if (CurrentAIState.t != null && (nextAttack == null || (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler)) && atkChances < AttackWillPerc && CurrentAIState.t.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x)
                {
                    lastAttackTime = Time.time;
                    nextAttackPos = CurrentAIState.t.UMS.CurrentTilePos;
                    yield return AttackSequence();
                }
                else
                {
                    BattleTileScript bts;
                    int randomizer = Random.Range(0, 100);

                    if (TowardMovementPerc > randomizer)
                    {
                        if (CurrentAIState.t.UMS.CurrentTilePos.x != UMS.CurrentTilePos.x)
                        {
                            InputDirection dir = CurrentAIState.t.UMS.CurrentTilePos.x < UMS.CurrentTilePos.x ? InputDirection.Up : InputDirection.Down;
                            bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(dir == InputDirection.Up ? UMS.CurrentTilePos.x - 1 : UMS.CurrentTilePos.x + 1, UMS.CurrentTilePos.y));
                            if (bts != null && bts.BattleTileState != BattleTileStateType.Empty)
                            {
                                bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(UMS.CurrentTilePos.x, UMS.Side == SideType.RightSide ? UMS.CurrentTilePos.y - 1 : UMS.CurrentTilePos.y + 1));
                                if (bts.BattleTileState == BattleTileStateType.Empty)
                                {
                                    yield return MoveCharOnDir_Co(UMS.Side == SideType.RightSide ? InputDirection.Left : InputDirection.Right);
                                }
                            }
                            else
                            {
                                yield return MoveCharOnDir_Co(dir);
                            }
                        }
                        else
                        {
                            bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(UMS.CurrentTilePos.x, UMS.Side == SideType.RightSide ? UMS.CurrentTilePos.y - 1 : UMS.CurrentTilePos.y + 1));
                            if (bts.BattleTileState == BattleTileStateType.Empty)
                            {
                                yield return MoveCharOnDir_Co(UMS.Side == SideType.RightSide ? InputDirection.Left : InputDirection.Right);
                            }
                        }
                    }
                    else if (AwayMovementPerc > randomizer - TowardMovementPerc)
                    {
                        if (CurrentAIState.t.UMS.CurrentTilePos.x != UMS.CurrentTilePos.x)
                        {
                            bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(UMS.CurrentTilePos.x, UMS.Side == SideType.RightSide ? UMS.CurrentTilePos.y - 1 : UMS.CurrentTilePos.y + 1));
                            if (bts.BattleTileState == BattleTileStateType.Empty)
                            {
                                yield return MoveCharOnDir_Co(UMS.Side == SideType.LeftSide ? InputDirection.Left : InputDirection.Right);
                            }
                        }
                        else
                        {
                            int updown = Random.Range(0, 100);
                            InputDirection dir = updown < 50 ? InputDirection.Up : InputDirection.Down;
                            bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(dir == InputDirection.Up ? UMS.CurrentTilePos.x - 1 : UMS.CurrentTilePos.x + 1, UMS.CurrentTilePos.y));
                            if (bts != null && bts.BattleTileState != BattleTileStateType.Empty)
                            {
                                bts = GridManagerScript.Instance.GetBattleTile(new Vector2Int(UMS.CurrentTilePos.x, UMS.Side == SideType.RightSide ? UMS.CurrentTilePos.y - 1 : UMS.CurrentTilePos.y + 1));
                                if (bts.BattleTileState == BattleTileStateType.Empty)
                                {
                                    yield return MoveCharOnDir_Co(UMS.Side == SideType.LeftSide ? InputDirection.Left : InputDirection.Right);
                                }
                            }
                            else
                            {
                                yield return MoveCharOnDir_Co(dir);
                            }
                        }
                    }
                }
            yield return null;
            }
        }
    }

    protected BaseCharacter GetTargetChar(List<BaseCharacter> enemys)
    {
        return enemys.OrderBy(r => (r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)).First();
    }

    protected void SetCurrentAIValues()
    {
        if (CurrentAIState.UpdateAttckWill)
        {
            AttackWillPerc = CurrentAIState.AttackWill;
        }
        if (CurrentAIState.UpdateMoveForward)
        {
            TowardMovementPerc = CurrentAIState.MoveForward;
        }
        if (CurrentAIState.UpdateMoveBackward)
        {
            AwayMovementPerc = CurrentAIState.MoveBackward;
        }
        if (CurrentAIState.UpdateMoveUpDown)
        {
            UpDownMovementPerc = CurrentAIState.MoveUpDown;
        }

        
    }

    public virtual IEnumerator Move()
    {
        while (true)
        {
            if (MoveCoOn && currentAttackPhase == AttackPhasesType.End && !Attacking)
            {
                float timer = 0;
                float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y) / 3;
                while (timer < MoveTime && !AIMove)
                {
                    yield return null;
                    while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || Attacking)
                    {
                        yield return null;
                    }
                    // Debug.Log(timer + "    " + MoveTime);
                    timer += BattleManagerScript.Instance.DeltaTime;
                }
                AIMove = false;
                if (CharInfo.Health > 0)
                {
                    while (currentAttackPhase != AttackPhasesType.End)
                    {
                        yield return null;
                    }
                    InputDirection dir = InputDirection.Up;

                    foreach (var item in BattleManagerScript.Instance.AllCharactersOnField.Where(a => a.IsOnField).OrderBy(r => Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)))
                    {
                        dir = item.UMS.CurrentTilePos.x > UMS.CurrentTilePos.x ? InputDirection.Down : InputDirection.Up;
                        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + GridManagerScript.Instance.GetVectorFromDirection(dir));
                        if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                        {
                            break;
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + GridManagerScript.Instance.GetVectorFromDirection((InputDirection)1 + i));
                                if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                                {
                                    break;
                                }
                            }
                            if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                            {
                                break;
                            }
                        }
                    }

                    MoveCharOnDirection(dir);
                }
                else
                {
                    timer = 0;
                }
            }
            yield return null;
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    //Basic attack sequence
    public override IEnumerator AttackSequence(ScriptableObjectAttackBase atk = null)
    {
        Attacking = true;
        bulletFired = false;
        string animToFire;
        bool isLooped = false;
        if (atk != null)
        {
            nextAttack = atk;
        }
        else
        {
            GetAttack();
        }
        if(nextAttack != null)
        {
            isLooped = false;
            animToFire = nextAttack.PrefixAnim + "_IdleToAtk";
            strongAnimDone = false;
            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(animToFire, isLooped, 0f);

            while (Attacking)
            {
                yield return null;
            }
        }
    }


    public override void CreateBullet(BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo)
    {
        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.SOAttack = nextAttack;
        bs.BulletBehaviourInfo = null; 
        bs.BulletBehaviourInfoTile = bulletBehaviourInfo;
        bs.Facing = UMS.Facing;
        bs.Elemental = CharInfo.DamageStats.CurrentElemental;
        bs.Side = UMS.Side;
        bs.isColliding = false;
        bs.CharOwner = this;
        bs.attackAudioType = GetAttackAudio();
        bs.BulletEffects.Clear();
        bs.DestinationTile = bulletBehaviourInfo.BulletEffectTiles[0].Pos + nextAttackPos;
        float duration = bulletBehaviourInfo.BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y));
        bs.BulletDuration = duration > bulletBehaviourInfo.Delay ? bulletBehaviourInfo.Delay - SpineAnim.SpineAnimationState.GetCurrent(0).TrackTime : duration;
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(nextAttack.Particles.Right.Bullet, CharInfo.CharacterID, AttackParticlePhaseTypes.Bullet, bullet.transform, UMS.Side,
            nextAttack.AttackInput, true);
        bs.gameObject.SetActive(true);
        bs.StartMoveToTile();
    }

    public override void fireAttackAnimation(Vector3 pos)
    {
      /*  if (!SpineAnim.CurrentAnim.Contains("Loop"))
        {
            if(nextAttack.PrefixAnim != AttackAnimPrefixType.Atk1)
            {
                if(!strongAnimDone)
                {
                    strongAnimDone = true;
                    SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                }
            }
            else
            {
                SetAnimation(nextAttack.PrefixAnim + "_Loop");
            }
        }

        if (chargeParticles != null && shotsLeftInAttack <= 0)
        {
            chargeParticles.SetActive(false);

            chargeParticles = null;
        }*/
    }


    public override void FireAttackAnimAndBullet(Vector3 pos)
    {
        if (nextAttack.AttackInput != AttackInputType.Weak)
        {
            if (!strongAnimDone)
            {
                strongAnimDone = true;
                SetAnimation(nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? nextAttack.PrefixAnim + "_Loop" : nextAttack.PrefixAnim + "_AtkToIdle");
            }
        }
        else
        {
            SetAnimation(nextAttack.PrefixAnim + "_Loop");
        }
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (isAttackBlocking)
        {
            int rand = UnityEngine.Random.Range(0, 100);

            if (rand <= 200)
            {
                Attacking = false;
                shotsLeftInAttack = 0;
            }
        }

        LastAttackTime = Time.time;
        return SetDamage(attacker, damage, elemental, isCritical);
    }


    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        float defenceChances = Random.Range(0, 100);
        if(defenceChances < CharInfo.DefenceStats.MinionPerfectDefenceChances)
        {
            isDefending = true;
            DefendingHoldingTimer = 0;
            SetAnimation(CharacterAnimationStateType.Defending);
            damage = 0;
        }
        else if (defenceChances < (CharInfo.DefenceStats.MinionPerfectDefenceChances + CharInfo.DefenceStats.MinionDefenceChances))
        {
            isDefending = true;
            DefendingHoldingTimer = 10;
            SetAnimation(CharacterAnimationStateType.Defending);
            damage = damage - CharInfo.DefenceStats.BaseDefence;
        }
        else
        {
            isDefending = false;
            DefendingHoldingTimer = 0;
        }
        return base.SetDamage(attacker, damage, elemental, isCritical);
    }


    public override void SetFinalDamage(BaseCharacter attacker, float damage)
    {
        HitInfoClass hic = HittedByList.Where(r => r.CharacterId == attacker.CharInfo.CharacterID).FirstOrDefault();
        if (hic == null)
        {
            HittedByList.Add(new HitInfoClass(attacker.CharInfo.CharacterID, damage));
        }
        else
        {
            hic.Damage += damage;
        }

        AggroInfoClass aggro = AggroInfoList.Where(r => r.PlayerController == attacker.CurrentPlayerController).FirstOrDefault();
        if (aggro == null)
        {
            AggroInfoList.Add(new AggroInfoClass(attacker.CurrentPlayerController, 1));
        }
        else
        {
            aggro.Hit ++;
            AggroInfoList.ForEach(r =>
            {
                if(r.PlayerController != attacker.CurrentPlayerController)
                {
                    r.Hit = r.Hit == 0 ? 0 : r.Hit - 1;
                }
            });
        }
        attacker.Sic.DamageMade += damage;
        totDamage += damage;
        base.SetFinalDamage(attacker, damage);
    }


    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
        UMS.SetUnit(UnitBehaviourType.NPC);
    }

    public override void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        CastLoopImpactAudioClipInfoClass attackTypeAudioInfo = GetAttackAudio();
        if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            ArrivingEvent();
        }
        else if (e.Data.Name.Contains("FireCastParticle"))
        {
            if (attackTypeAudioInfo != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackTypeAudioInfo.Cast, AudioBus.LowPrio, transform);
            }

            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;

            }
            FireCastParticles();
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;
                    
            }
            BulletAttack();
        }
        else if (e.Data.Name.Contains("FireTileAttack") && !trackEntry.Animation.Name.Contains("Loop"))
        {
            currentAttackPhase = SpineAnim.CurrentAnim.Contains("Atk1") ? AttackPhasesType.Bullet_Weak : AttackPhasesType.Bullet_Strong;
            CreateTileAttack();
        }
    }


    public void BulletAttack()
    {
        bulletFired = true;
        if (nextAttack.AttackInput == AttackInputType.Strong)
        {
            CreateBullet(nextAttack.TilesAtk.BulletTrajectories[0]);
        }
        else if(nextAttack.AttackInput == AttackInputType.Weak)
        {
            Debug.Log(nextAttack.TilesAtk.BulletTrajectories.Count - shotsLeftInAttack - 1);
            CreateBullet(nextAttack.TilesAtk.BulletTrajectories[0]);
        }
    }

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" 
         || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString() )
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim == CharacterAnimationStateType.Defeat.ToString())
        {
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            transform.position = new Vector3(100, 100, 100);
            gameObject.SetActive(false);
            return;
        }

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
            currentAttackPhase = AttackPhasesType.End;
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
        }

        base.SpineAnimationState_Complete(trackEntry);
    }

}


public class HitInfoClass
{
    public CharacterNameType CharacterId;
    public float Damage;

    public HitInfoClass()
    {

    }

    public HitInfoClass(CharacterNameType characterId, float damage)
    {
        CharacterId = characterId;
        Damage = damage;
    }
}


public class AggroInfoClass
{
    public ControllerType PlayerController;
    public int Hit;

    public AggroInfoClass()
    {

    }

    public AggroInfoClass(ControllerType playerController, int hit)
    {
        PlayerController = playerController;
        Hit = hit;
    }
}
