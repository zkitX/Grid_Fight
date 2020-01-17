using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticles : MonoBehaviour
{
    public GameObject MeshParticles;
    public GameObject BillboardParticles;

    public List<GameObject> totalObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
        InputController.Instance.ButtonBUpEvent += Instance_ButtonBUpEvent;
        InputController.Instance.ButtonXUpEvent += Instance_ButtonXUpEvent;
    }

    private void Instance_ButtonXUpEvent(int player)
    {
        foreach (GameObject item in totalObj)
        {
            Destroy(item);
        }
    }

    private void Instance_ButtonBUpEvent(int player)
    {
        GameObject go = Instantiate(BillboardParticles, new Vector3(Random.Range(-10, 10), Random.Range(-3, 3), 0), Quaternion.identity);
        totalObj.Add(go);
    }

    private void Instance_ButtonAUpEvent(int player)
    {
        GameObject go = Instantiate(MeshParticles, new Vector3(Random.Range(-10, 10), Random.Range(-3, 3), 0), Quaternion.identity);
        totalObj.Add(go);
    }
}
