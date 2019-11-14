using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{

    public float MinMovementTimer = 5;
    public float MaxMovementTimer = 8;

    public CharacterBase CharOwner;
    // Start is called before the first frame update
    void Start()
    {
        CharOwner = GetComponent<CharacterBase>();
    }

    public IEnumerator MoveCo()
    {
        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForFixedUpdate();
        }
        bool MoveCoOn = true;
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
            if (CharOwner.CharacterInfo.Health > 0)
            {
                CharOwner.MoveCharOnDirection((InputDirection)Random.Range(0,4));
            }
        }
    }
}
