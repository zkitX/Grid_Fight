using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawnerManagerScript : MonoBehaviour
{
    public delegate void ItemPickedUp();
    public event ItemPickedUp ItemPickedUpEvent;



    public static ItemSpawnerManagerScript Instance;
    public GameObject ItemGO;
    public List<ScriptableObjectItemPowerUps> SOItemsPowerUps = new List<ScriptableObjectItemPowerUps>();
    public Vector2 SpawningTimeRange;
    public List<ItemsPowerUPsInfoScript> SpawnedItems = new List<ItemsPowerUPsInfoScript>();
    public bool CoStopper = false;
    private bool spawningCoPaused = false;
    private IEnumerator SpawningCo;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartSpawningCo(SpawningTimeRange);
    }

    public void PauseSpawning()
    {
        spawningCoPaused = true;
    }

    public void PlaySpawning()
    {
        spawningCoPaused = false;
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

        yield return BattleManagerScript.Instance.WaitUpdate(()=> BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

        while (!CoStopper)
        {
            float timer = 0;

            float spawningTime = Random.Range(SpawningTimeRange.x, SpawningTimeRange.y);

            while (timer <= spawningTime)
            {
                while (spawningCoPaused)
                {
                    yield return null;
                }
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
                timer += BattleManagerScript.Instance.DeltaTime;
            }

            SpawnItemRandomPos(SOItemsPowerUps[Random.Range(0, SOItemsPowerUps.Count)], WalkingSideType.LeftSide);
        }
    }

    public void SpawnItemRandomPos(ScriptableObjectItemPowerUps nextItemPowerUp, WalkingSideType walkingSide)
    {
        BattleTileScript bts = null;
        while (true)
        {
            bts = GridManagerScript.Instance.GetFreeBattleTile(walkingSide);
            if (bts.BattleTileState == BattleTileStateType.Empty && SpawnedItems.Where(r => r.isActiveAndEnabled && r.Pos == bts.Pos).ToList().Count == 0)
            {
                break;
            }
        }
        SpawnItem(nextItemPowerUp, bts);
    }
    public void SpawnItem(ScriptableObjectItemPowerUps nextItemPowerUp, BattleTileScript bts)
    {
        ItemsPowerUPsInfoScript item = SpawnedItems.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (item == null)
        {
            item = Instantiate(ItemGO, transform).GetComponent<ItemsPowerUPsInfoScript>();
            SpawnedItems.Add(item);
        }
        item.ItemPickedUpEvent += Item_ItemPickedUpEvent;
        item.gameObject.SetActive(true);
        item.SetItemPowerUp(nextItemPowerUp, bts.transform.position, bts.Pos);
    }

    private void Item_ItemPickedUpEvent()
    {
        ItemPickedUpEvent?.Invoke();
    }

    public void SpawnPowerUpAtGridPos(ScriptableObjectItemPowerUps powerUp, Vector2Int pos, float duration = 0f)
    {
        ItemsPowerUPsInfoScript item = SpawnedItems.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if (item == null)
        {
            item = Instantiate(ItemGO, transform).GetComponent<ItemsPowerUPsInfoScript>();
            SpawnedItems.Add(item);
        }
        item.ItemPickedUpEvent += Item_ItemPickedUpEvent;
        item.gameObject.SetActive(true);
        item.SetItemPowerUp(powerUp, GridManagerScript.Instance.GetBattleTile(pos).transform.position, pos, duration);
    }

    public void SpawnPowerUpAtRandomPointOnSide(ScriptableObjectItemPowerUps powerUp, WalkingSideType side, float duration = 0f)
    {
        SpawnPowerUpAtGridPos(powerUp, GridManagerScript.Instance.GetFreeBattleTile(side).Pos, duration);
    }
}