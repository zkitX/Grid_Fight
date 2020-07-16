using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class BuffIconHandler : MonoBehaviour
{
    [SerializeField] protected GameObject statusIconPrefab = null;

    protected List<BuffIcon> buffIcons = new List<BuffIcon>();

    [SerializeField] protected List<Transform> iconSlots = new List<Transform>();


    private void Awake()
    {
        foreach (Transform iconSlot in iconSlots)
        {
            buffIcons.Add(Instantiate(statusIconPrefab, iconSlot.position, Quaternion.identity, transform).GetComponent<BuffIcon>());
        }
        buffIcons = GetComponentsInChildren<BuffIcon>().ToList();
    }

    public void RefreshIcons(List<BuffDebuffClass> statusList)
    {
        foreach (BuffIcon ico in buffIcons)
        {
            for (int i = 0; i < iconSlots.Count; i++)
            {
                if (ico.StatusEffect == null) break;
                //statusList[i].Stat == ico.StatusEffect.StatsToAffect;
            }
        }

        
    }
}

