using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{

    public float MinMovementTimer = 5;
    public float MaxMovementTimer = 8;
    private IEnumerator MoveCo;
    private bool MoveCoOn = true;
    public BaseCharacter CharOwner;
    // Start is called before the first frame update
    private void Start()
    {
            
    }

    public void StartMoveCo(BaseCharacter charOwner)
    {
        CharOwner = charOwner;
        MoveCoOn = true;
        MoveCo = Move();
        StartCoroutine(MoveCo);
    }

    public IEnumerator Move()
    {
        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForFixedUpdate();
        }
        while (MoveCoOn)
        {
            float timer = 0;
            float MoveTime = Random.Range(MinMovementTimer, MaxMovementTimer);
            while (timer < 1)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForFixedUpdate();
                }

                timer += Time.fixedDeltaTime / MoveTime;
            }
            if (CharOwner.CharInfo.Health > 0)
            {
                CharOwner.MoveCharOnDirection((InputDirection)Random.Range(0,4));
            }
        }
    }

    public void StopMoveCo()
    {
        MoveCoOn = false;
        if(MoveCo != null)
        {
            StopCoroutine(MoveCo);
        }
    }

}
