using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{
    protected bool MoveCoOn = true;
    protected IEnumerator MoveActionCo;
    protected float LastAttackTime;
    public float UpDownPerc = 18;
    public AIType CurrentAI;
    List<HitInfoClass> HittedByList = new List<HitInfoClass>();
    float totDamage = 0;



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

    public override void StartMoveCo()
    {
        MoveCoOn = true;
        if (MoveActionCo != null)
        {
            StopCoroutine(MoveActionCo);

        }
        MoveActionCo = Move();
        StartCoroutine(MoveActionCo);
    }

    public override void SetCharDead(bool hasToDisappear = true)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
     
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        Attacking = false;
        StopAllCoroutines();
        for (int i = 0; i < UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
            UMS.Pos[i] = Vector2Int.zero;
        }
 		for (int i = 0; i < HittedByList.Count; i++)
        {
            StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == HittedByList[i].CharacterId).First();
            sic.DamageExp += (HittedByList[i].Damage / totDamage) * CharInfo.ExperienceValue;
        }
        totDamage = 0;

        if (SpineAnim.skeleton.Data.Animations.Where(r => r.Name == CharacterAnimationStateType.Defeat.ToString()).ToList().Count == 1)
        {
            SetAnimation(CharacterAnimationStateType.Defeat);
            CharBoxCollider.enabled = false;
            base.SetCharDead(false);
        }
        else
        {
            base.SetCharDead();
        }
      
    }


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


                List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                if (enemys.Count > 0)
                {
                    BaseCharacter targetChar = enemys.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
                    if (targetChar != null)
                    {
                        nextAttackPos = targetChar.UMS.CurrentTilePos;
                        yield return AttackSequence();
                    }
                    else
                    {

                        int randomizer = Random.Range(0, 100);
                        if (randomizer < UpDownPerc)
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Left);
                        }
                        else if (randomizer > (100 - UpDownPerc))
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


    protected BaseCharacter GetTargetChar(List<BaseCharacter> enemys)
    {
        return enemys.OrderBy(r => (r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)).First();
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
                    timer += Time.deltaTime;
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

    public override void StopMoveCo()
    {
        MoveCoOn = false;
        if (MoveActionCo != null)
        {
            StopCoroutine(MoveActionCo);
        }
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
            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(animToFire, isLooped, 0f);

            while (Attacking)
            {
                yield return null;
            }
        }
    }

    public virtual bool GeneralTestAI()
    {
        List<BattleFieldAttackTileClass> tilesToCheck = new List<BattleFieldAttackTileClass>();

        foreach (BulletBehaviourInfoClassOnBattleFieldClass item in nextAttack.TilesAtk.BulletTrajectories)
        {
            tilesToCheck.AddRange(item.BulletEffectTiles);
        }
        tilesToCheck = tilesToCheck.Distinct().ToList();
        int chances = Random.Range(0, 100);
        if (GridManagerScript.Instance.IsEnemyOnTileAttackRange(tilesToCheck, UMS.CurrentTilePos))
        {
            if (chances < 10)
            {
                return false;
            }

            return true;
        }
        else
        {
            if (chances < 50)
            {
                return true;
            }
            AIMove = true;
            return false;
        }
    }

    public bool AggressiveTestAI()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
        if (cb != null)
        {
            if (CharInfo.HealthPerc > 20)
            {
                return true;
            }
            else
            {
                AIMove = false;
                return false;
            }
        }
        else
        {
            AIMove = true;
            return false;
        }

    }


   /* public override void CreateBullet(BattleFieldAttackTileClass bulletBehaviourInfo, Vector2Int pos, float delay)
    {
        // Debug.Log(isSpecialLoading);

        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.SOAttack = nextAttack;
        bs.BulletBehaviourInfo = new BulletBehaviourInfoClass(bulletBehaviourInfo.Pos, bulletBehaviourInfo.Trajectory_Y, bulletBehaviourInfo.Trajectory_Z, )
        bs.Facing = UMS.Facing;
        bs.PlayerController = UMS.PlayerController;
        bs.Elemental = CharInfo.DamageStats.CurrentElemental;
        bs.Side = UMS.Side;
        bs.VFXTestMode = VFXTestMode;
        bs.CharOwner = this;
        bs.attackAudioType = GetAttackAudio();





        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.Trajectory_Y = bulletBehaviourInfo.Trajectory_Y;
        bs.Trajectory_Z = bulletBehaviourInfo.Trajectory_Z;
        bs.isColliding = false;
        bs.attackAudioType = GetAttackAudio();
        bs.CharOwner = this;
        bs.DestinationTile = pos;
        bs.BulletEffectTiles.Clear();
        bs.BulletDuration = delay;
        bs.PS = ParticleManagerScript.Instance.FireParticlesInTransform(nextAttack.Particles.Right.Bullet, CharInfo.CharacterID, AttackParticlePhaseTypes.Bullet, bullet.transform, UMS.Side,
            nextAttack.AttackInput, CharInfo.BaseCharacterType == BaseCharType.CharacterType_Script ? true : false);
        bs.PS.SetActive(true);
        bs.gameObject.SetActive(true);
        bs.StartMoveToTile();
    }*/

    public override void fireAttackAnimation(Vector3 pos)
    {
        if (!SpineAnim.CurrentAnim.Contains("Loop"))
        {
            SetAnimation(nextAttack.PrefixAnim + (nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? "_Loop" : "_AtkToIdle"));
        }

        if (chargeParticles != null && shotsLeftInAttack <= 0)
        {
            chargeParticles.SetActive(false);

            chargeParticles = null;
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
        return base.SetDamage(attacker, damage, elemental, isCritical);
    }


    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        damage = damage * CharInfo.DefenceStats.BaseDefence;
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
        attacker.Sic.DamageMade += damage;
        totDamage += damage;
        base.SetFinalDamage(attacker, damage);
    }
    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
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


        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {

            //If they can still attack, keep them in the charging loop
            if (shotsLeftInAttack > 0)
            {
                SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            }
            //otherwise revert them to the idle postion
            else
            {
                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                currentAttackPhase = AttackPhasesType.End;
            }
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            if (shotsLeftInAttack == 0)
            {
                Attacking = false;
            }
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
