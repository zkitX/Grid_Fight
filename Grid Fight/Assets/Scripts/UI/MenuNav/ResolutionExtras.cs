using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionExtras : MonoBehaviour
{
    public Transform tallyStartPoint;
    public float tallySpacing = 150f;
    public GameObject tallyPrefab = null;
    List<Grid_UITally> Tallies = new List<Grid_UITally>();

    public void SetupResolution()
    {
        resolved = false;

        foreach (Grid_UITally tally in Tallies) Destroy(tally);
        Tallies = new List<Grid_UITally>();

        Vector3 spawnPoint = tallyStartPoint.position;
        bool indent = true;
        for (int i = 0; i < BattleInfoManagerScript.Instance.PlayerBattleInfo.Count; i++)
        {
            Tallies.Add(Instantiate(tallyPrefab, spawnPoint, Quaternion.identity, tallyStartPoint).GetComponent<Grid_UITally>());
            Tallies[i].transform.SetAsFirstSibling();
            Tallies[i].SetupTally(BattleInfoManagerScript.Instance.PlayerBattleInfo[i].CharacterName, indent);
            Tallies[i].GetComponent<CanvasGroup>().alpha = 0;

            indent = !indent;
            spawnPoint += new Vector3(0, -tallySpacing, 0);
        }
    }

    public IEnumerator TallyRevealer()
    {
        foreach (Grid_UITally tally in Tallies)
        {
            tally.RevealTally();
            yield return new WaitForSeconds(0.2f);
        }
    }

    bool resolved = true;
    public IEnumerator TallyResolver()
    {
        if (resolved) yield break;

        foreach (Grid_UITally tally in Tallies)
        {
            StartCoroutine(tally.ResolveTally());
            yield return new WaitForSeconds(0.4f);
        }
    }
}
