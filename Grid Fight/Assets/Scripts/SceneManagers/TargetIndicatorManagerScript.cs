using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetIndicatorManagerScript : MonoBehaviour
{

    public static TargetIndicatorManagerScript Instance;
    private Dictionary<int, GameObject> TargetsEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> TargetsPlayer = new Dictionary<int, GameObject>();
    public GameObject TargetEnemyGameObject;
    public GameObject TargetPlayerGameObject;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetTargetIndicator(AttackType atkType)
    {
        Dictionary<int, GameObject> dToCheck = atkType == AttackType.Particles ? TargetsPlayer : TargetsEnemy;
        GameObject res = null;
        res = dToCheck.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if(res == null)
        {
            res = Instantiate(atkType == AttackType.Particles ? TargetPlayerGameObject : TargetEnemyGameObject, transform);
            dToCheck.Add(dToCheck.Count, res);
        }
        res.SetActive(true);
        return res;
    }
}
