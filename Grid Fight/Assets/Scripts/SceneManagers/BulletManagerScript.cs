using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManagerScript : MonoBehaviour
{

    public static BulletManagerScript Instance;
    private Dictionary<int, GameObject> Bullets = new Dictionary<int, GameObject>();
    public GameObject BulletGameObject;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetBullet()
    {
        GameObject res = null;
        res = Bullets.Values.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (res == null)
        {
            res = Instantiate(BulletGameObject, transform);
            Bullets.Add(Bullets.Count, res);
        }

        res.SetActive(true);
        return res;
    }

}
