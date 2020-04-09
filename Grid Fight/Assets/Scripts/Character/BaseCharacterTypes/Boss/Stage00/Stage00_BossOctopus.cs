using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus : MinionType_Script
{
    public bool CanGetDamage = false;
    private List<MinionType_Script> Pieces = new List<MinionType_Script>();
    public bool IsCharArrived = false;
    public bool DialogueComplete = true;


    private List<CharacterNameType> piecesType = new List<CharacterNameType>()
    {
        CharacterNameType.Stage00_BossOctopus_Tentacles,
        CharacterNameType.Stage00_BossOctopus_Head,
        CharacterNameType.Stage00_BossOctopus_Girl
    };

    public override void Start()
    {
        GenerateBoss();
    }

    void GenerateBoss()
    {
        foreach (CharacterNameType piece in piecesType)
        {
            Pieces.Add(CreatePiece(piece));
        }
        ((Stage00_BossOctopus_Girl)GetPiece(CharacterNameType.Stage00_BossOctopus_Girl)).CenteringPoint = GetComponentsInChildren<Transform>().Where(r => r.CompareTag("CenteringPoint")).FirstOrDefault();
    }

    private MinionType_Script CreatePiece(CharacterNameType pieceType)
    {
        MinionType_Script piece = (MinionType_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(pieceType.ToString(), CharacterSelectionType.Up,
        CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, pieceType, WalkingSideType.RightSide, AttackType.Tile, BaseCharType.None), transform);
        piece.UMS.Pos = UMS.Pos;
        piece.UMS.EnableBattleBars(false);
        piece.UMS.CurrentTilePos = UMS.CurrentTilePos;
        piece.SetValueFromVariableName("BaseBoss", this);
        if (pieceType == CharacterNameType.Stage00_BossOctopus_Head)
        {
            ((Stage00_BossOctopus_Head)piece).bossParent = this;
        }
        else if (pieceType == CharacterNameType.Stage00_BossOctopus_Tentacles)
        {
            ((Stage00_BossOctopus_Tentacles)piece).bossParent = this;
        }
        else if (pieceType == CharacterNameType.Stage00_BossOctopus_Girl)
        {
            ((Stage00_BossOctopus_Girl)piece).bossParent = this;
        }
        return piece;
    }

    public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    public override void SetUpLeavingBattle()
    {
        ((Stage00_BossOctopus_Girl)GetPiece(CharacterNameType.Stage00_BossOctopus_Girl)).SetUpLeavingBattle();
    }

    public override IEnumerator Move()
    {
        yield return null;
    }

    protected override void Update()
    {
        if (UIBattleManager.Instance != null && IsOnField)
        {
            if(!UIBattleManager.Instance.UIBoss.gameObject.activeInHierarchy)
            {
                UIBattleManager.Instance.UIBoss.gameObject.SetActive(true);
            }

            float totalHP = 0;
            float currentHP = 0;

            foreach (MinionType_Script item in Pieces.Where(r=> r.CharInfo.CharacterID != CharacterNameType.Stage00_BossOctopus_Girl))
            {
                totalHP += item.CharInfo.HealthStats.Base;
                currentHP += item.CharInfo.HealthStats.Health;
            }
            UIBattleManager.Instance.UIBoss.UpdateHp((100f * currentHP) / totalHP);
        }
        
    }



    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        //BattleManagerScript.Instance.CurrentBattleState = BattleState.Event;

        UMS.EnableBattleBars(false); 

        ((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).bossLady = ((Stage00_BossOctopus_Girl)GetPiece(CharacterNameType.Stage00_BossOctopus_Girl));

        SetAnimation(CharacterAnimationStateType.Arriving);

        while(!IsCharArrived)
        {
            yield return null;
        }

        WaveManagerScript.Instance.BossArrived(this);

        while (!DialogueComplete)
        {
            yield return null;
        }


        foreach (MinionType_Script piece in Pieces)
        {
            piece.UMS.Pos = UMS.Pos;
            piece.UMS.CurrentTilePos = UMS.CurrentTilePos;
            //piece.StartAttakCo();
        }

        SetAttackReady(true);
        float timer = 0;
        while (timer <= 3)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }


        timer = 0;
        while (timer <= 3)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
    }

    MinionType_Script GetPiece(CharacterNameType pieceName)
    {
        foreach (MinionType_Script piece in Pieces)
        {
            //Debug.Log(piece.CharInfo.CharacterID.ToString());
            if (piece.CharInfo.CharacterID == pieceName) return piece;
        }
        Debug.Log("Piece requested '" + pieceName.ToString() + "' does not exist in boss");
        return null;
    }

    public override void SetCharDead(bool hasToDisappear = true)
    {
        if(!((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).disabled ||
            !((Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles)).disabled)
        {
            return;
        }
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        StopCoroutine(attackCoroutine);
        StartCoroutine(PhaseOneEnd());
        Debug.Log("Octobos fully disabled");
    }

    private void TriggerRandomDeathExplosion()
    {
        GameObject boom = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage00BossDeathSmoke);
        boom.transform.parent = transform;
        boom.transform.localPosition = Vector3.zero + new Vector3(Random.Range(0f, DeathExplosionRange.x), Random.Range(0f, DeathExplosionRange.y), 0f);
        boom.SetActive(true);
    }

    IEnumerator DeathExplosionPacer(Vector2 paceRange, int number)
    {
        float timer;

        for (int i = 0; i < number; i++)
        {
            timer = Random.Range(paceRange.x, paceRange.y);
            TriggerRandomDeathExplosion();
            while (timer != 0f)
            {
                timer = Mathf.Clamp(timer - Time.deltaTime, 0f, paceRange.y);
                yield return null;
            }
        }
    }

    public Vector2 DeathExplosionRange = new Vector2(-2f, 4f);
    private IEnumerator PhaseOneEnd()
    {
        //float timer = 0;
        Stage00_BossOctopus_Tentacles tentacles = (Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles);

        StartCoroutine(DeathExplosionPacer(new Vector2(0.1f/5f, 0.3f/5f), 20*5));

        foreach(MinionType_Script character in Pieces)
        {
            currentDeathProcessPhase = DeathProcessStage.Start;
        }
        SetAnimation(CharacterAnimationStateType.Death_Prep);
        while(tentacles.currentDeathProcessPhase == DeathProcessStage.Start)
        {
            yield return null;
        }
        GetPiece(CharacterNameType.Stage00_BossOctopus_Head).SetAnimation(CharacterAnimationStateType.Death_Loop, true);
        GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).SetAnimation(CharacterAnimationStateType.Death_Loop, true);
        yield return BirthOctopusGirl();
        yield return new WaitForSeconds(1f);
        foreach (MinionType_Script character in Pieces)
        {
            currentDeathProcessPhase = DeathProcessStage.End;
        }
        GetPiece(CharacterNameType.Stage00_BossOctopus_Head).SetAnimation(CharacterAnimationStateType.Death_Exit);
        GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).SetAnimation(CharacterAnimationStateType.Death_Exit);
        yield return null;
        /*Stage04_BossMonster_Script mask = (Stage04_BossMonster_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass((CharacterNameType.Stage04_BossMonster).ToString(), CharacterSelectionType.Up,
        CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage04_BossMonster, WalkingSideType.RightSide, AttackType.Tile, BaseCharType.None), WaveManagerScript.Instance.transform);
        BattleManagerScript.Instance.AllCharactersOnField.Add(mask);
        mask.UMS.Pos = UMS.Pos;
        mask.UMS.CurrentTilePos = UMS.CurrentTilePos;
        mask.transform.position = transform.position;
        mask.SetUpEnteringOnBattle();*/

        /*timer = 0;
        while (timer < 3)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event)
            {
                yield return new WaitForFixedUpdate();
            }
            timer += Time.fixedDeltaTime;
        }
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        base.SetCharDead();*/
    }

    public void OctopusGirlLeaves()
    {
        if(!GetPiece(CharacterNameType.Stage00_BossOctopus_Girl).gameObject.activeInHierarchy &&
            !GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).gameObject.activeInHierarchy &&
            !GetPiece(CharacterNameType.Stage00_BossOctopus_Head).gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    void OctoBossDies()
    {
        gameObject.SetActive(false);
    }

    IEnumerator BirthOctopusGirl()
    {
        //The code for the moving into position of and the creation of the octopus girl
        ((Stage00_BossOctopus_Girl)GetPiece(CharacterNameType.Stage00_BossOctopus_Girl)).SetAnimation(CharacterAnimationStateType.Death_Born);
        yield return ((Stage00_BossOctopus_Girl)GetPiece(CharacterNameType.Stage00_BossOctopus_Girl)).CenterCharacterToTile(5f);
    }

    public override IEnumerator AttackAction(bool yieldBefore)
    {
        //Handles the attack for all of the components of the boss
        while (GetPiece(CharacterNameType.Stage00_BossOctopus_Head) == null)
        {
            yield return null;
        }
        while(!GetPiece(CharacterNameType.Stage00_BossOctopus_Head).IsOnField)
        {
            yield return null;
        }

        while(BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return null;
        }

        Vector2 blanketAttackTimings = new Vector2(15f, 25f);
        Vector2 headAttackTimings = new Vector2(4f, 7f);
        Vector2 tentacleAttackTimings = new Vector2(3f, 6f);
        float blanketAttackCooldown = blanketAttackTimings.x;
        float headAttackCooldown = headAttackTimings.x;
        float tentacleAttackCooldown = tentacleAttackTimings.x;

        while (true)
        {
            while(BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && 
                BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets)
            {
                yield return null;
            }

            blanketAttackCooldown = Mathf.Clamp(blanketAttackCooldown - Time.deltaTime, 0f, 10000f);
            if(blanketAttackCooldown == 0f &&
                !((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).disabled &&
                !((Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles)).disabled)
            {
                //If it gets here, launch an attack from the whole body
                while (GetPiece(CharacterNameType.Stage00_BossOctopus_Head).Attacking && GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).Attacking)
                {
                    yield return null;
                }
                StartCoroutine(((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).AttackSequence());
                StartCoroutine(((Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles)).AttackSequence());

                while (GetPiece(CharacterNameType.Stage00_BossOctopus_Head).Attacking && GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).Attacking)
                {
                    yield return null;
                }
                blanketAttackCooldown = Random.Range(blanketAttackTimings.x, blanketAttackTimings.y);
                headAttackCooldown = Random.Range(headAttackTimings.x, headAttackTimings.y);
                tentacleAttackCooldown = Random.Range(tentacleAttackTimings.x, tentacleAttackTimings.y);
            }
            else
            {
                headAttackCooldown = Mathf.Clamp(headAttackCooldown - Time.deltaTime, 0f, 10000f);
                tentacleAttackCooldown = Mathf.Clamp(tentacleAttackCooldown - Time.deltaTime, 0f, 10000f);

                if(headAttackCooldown == 0f && !((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).disabled)
                {
                    //If it gets here, launch an attack from the head
                    if (!GetPiece(CharacterNameType.Stage00_BossOctopus_Head).Attacking)
                    {
                        yield return ((Stage00_BossOctopus_Head)GetPiece(CharacterNameType.Stage00_BossOctopus_Head)).AttackSequence();
                        headAttackCooldown = Random.Range(headAttackTimings.x, headAttackTimings.y);
                    }
                }
                if(tentacleAttackCooldown == 0f && !((Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles)).disabled)
                {
                    //If it gets here, launch an attack from the tentacles
                    if (!GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles).Attacking)
                    {
                        yield return ((Stage00_BossOctopus_Tentacles)GetPiece(CharacterNameType.Stage00_BossOctopus_Tentacles)).AttackSequence();
                        tentacleAttackCooldown = Random.Range(tentacleAttackTimings.x, tentacleAttackTimings.y);
                    }
                }
            }
            yield return null;
        }
    }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        if (CanGetDamage)
        {
            return base.SetDamage(damage, elemental, isCritical);
        }
        return false;

    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        foreach (MinionType_Script piece in Pieces)
        {
            piece.SetAnimation(animState, loop, transition);
        }
    }

    private void SetAnimForSinglePiece()
    {

    }

    public override bool GeneralTestAI()
    {
        return true;
    }
}
