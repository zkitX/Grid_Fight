using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnerManagerScript : MonoBehaviour
{
    public static ItemSpawnerManagerScript Instance;
    public GameObject ItemGO;
    public List<ScriptableObjectItemPowerUps> SOItemsPowerUps = new List<ScriptableObjectItemPowerUps>();
    public Vector2 SpawningTimeRange;
    public List<ItemsPowerUPsInfoScript> SpawnedItems = new List<ItemsPowerUPsInfoScript>();
    public bool CoStopper = false;
    private IEnumerator SpawningCo;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartSpawningCo(SpawningTimeRange);
    }

    public void StartSpawningCo(Vector2 spawningTimeRange)
    {
        SpawningTimeRange = spawningTimeRange;
        if(SpawningCo != null)
        {
            StopCoroutine(SpawningCo);
        }
        SpawningCo = Spawning_Co();
        StartCoroutine(SpawningCo);
    }

    private IEnumerator Spawning_Co()
    {

        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForEndOfFrame();
        }

        while (!CoStopper)
        {
            float timer = 0;

            float spawningTime = Random.Range(SpawningTimeRange.x, SpawningTimeRange.y);

            while (timer <= spawningTime)
            {
                yield return BattleManagerScript.Instance.PauseUntil();
                timer += Time.fixedDeltaTime;
            }

            ScriptableObjectItemPowerUps nextItemPowerUp = SOItemsPowerUps[Random.Range(0,SOItemsPowerUps.Count)];
            ItemsPowerUPsInfoScript item = SpawnedItems.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
            if(item == null)
            {
                item = Instantiate(ItemGO, transform).GetComponent<ItemsPowerUPsInfoScript>();
                SpawnedItems.Add(item);
            }
            item.gameObject.SetActive(true);
            item.SetItemPowerUp(nextItemPowerUp, GridManagerScript.Instance.GetFreeBattleTile(WalkingSideType.LeftSide).transform.position);
        }
    }


    public void SpawnItem(ItemType item)
    {

    }
}