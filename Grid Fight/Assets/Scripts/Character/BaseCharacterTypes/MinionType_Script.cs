using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{


    protected bool MoveCoOn = true;
    protected IEnumerator MoveActionCo;
    protected float LastAttackTime;
    public float UpDownPerc = 18;
    public AIType CurrentAI;

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
        StopAllCoroutines();
        base.SetCharDead();
    }


    protected IEnumerator AI()
    {
        while(BattleManagerScript.Instance.PlayerControlledCharacters.Length == 0)
        {
            yield return null;
        }

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
                BaseCharacter targetChar = enemys.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
                if (targetChar != null)
                {
                    GetAttack(CharacterAnimationStateType.Atk);
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
        }
    }


    private BaseCharacter GetTargetChar(List<BaseCharacter> enemys)
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

    public override void SetAnimation(string animState, bool loop = false, float transition = 0)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        base.SetAnimation(animState, loop, transition);
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    //Basic attack sequence
    public override IEnumerator AttackSequence()
    {
        /*shotsLeftInAttack = 0;
        if (currentAttackPhase != AttackPhasesType.End) yield break;

        yield return null;*/
        Attacking = true;

        CharacterAnimationStateType animToFire = CharacterAnimationStateType.Atk;
        bool isLooped = false;
        switch (nextAttack.AttackAnim)
        {
            case AttackAnimType.Atk:
                sequencedAttacker = false;
                chargeParticles = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, AttackParticlePhaseTypes.Charging, transform.position, UMS.Side);
                animToFire = CharacterAnimationStateType.Idle;
                isLooped = true;
                currentAttackPhase = AttackPhasesType.Cast_Powerful;
                CreateTileAttack();
                break;
            case AttackAnimType.RapidAtk:
                animToFire = CharacterAnimationStateType.Atk1_IdleToAtk;
                isLooped = false;
                break;
            case AttackAnimType.PowerfulAtk:
                break;
            case AttackAnimType.Special1:
                break;
            case AttackAnimType.Special2:
                break;
            case AttackAnimType.Special3:
                break;
        }




      /*  switch (CurrentAI)
        {
            case AIType.GeneralAI:
                res = GeneralTestAI();
                break;
            case AIType.AggressiveAI:
                res = AggressiveTestAI();
                break;
        }*/
        //shotsLeftInAttack = GetHowManyAttackAreOnBattleField(((ScriptableObjectAttackTypeOnBattlefield)nextAttack).BulletTrajectories);

        currentAttackPhase = AttackPhasesType.Start;
        sequencedAttacker = true; //Temporary until anims are added
        SetAnimation(animToFire,isLooped, 0f);
        CreateTileAttack();


        while (shotsLeftInAttack != 0)
        {
            yield return null;
        }

        currentAttackPhase = AttackPhasesType.End;
        Attacking = false; //Temporary until anims are added
        yield break;
    }


    /*public bool GeneralTestAI()
    {
        BaseCharacter cb = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x && r.IsOnField).FirstOrDefault();
        if (cb != null)
        {
            if (UnderAttack)
            {
                if (CharInfo.HealthPerc > 40)
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
                return true;
            }
        }
        else
        {
            AIMove = true;
            return false;
        }

    }*/


    public virtual bool GeneralTestAI()
    {
        List<BattleFieldAttackTileClass> tilesToCheck = new List<BattleFieldAttackTileClass>();

        foreach (BulletBehaviourInfoClassOnBattleFieldClass item in ((ScriptableObjectAttackTypeOnBattlefield)nextAttack).BulletTrajectories)
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


    public override void fireAttackAnimation(Vector3 pos)
    {
        //Debug.Log("<b>Shots left in this charge of attacks: </b>" + shotsLeftInAttack);
        if (sequencedAttacker) SetAnimation(CharacterAnimationStateType.Atk1_Loop);
        else SetAnimation(CharacterAnimationStateType.Atk); //Temporary until anims are added
        if (chargeParticles != null && shotsLeftInAttack == 0)
        {
            chargeParticles.SetActive(false);

            chargeParticles = null;
        }
    }


    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
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
        return base.SetDamage(damage, elemental, isCritical);
    }


    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        damage = damage * CharInfo.DefenceStats.BaseDefence;
        return base.SetDamage(damage, elemental, isCritical);
    }
}
