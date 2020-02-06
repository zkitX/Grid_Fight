using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBattleFieldManager : MonoBehaviour
{
    public static UIBattleFieldManager Instance;
    public GameObject UIBattleField;

    private List<GameObject> ListOfUIBattleField = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }
    public void SetUIBattleField(BaseCharacter cb)
    {
       /* GameObject uibattleField = ListOfUIBattleField.Where(r => !r.activeInHierarchy).FirstOrDefault();
        if (uibattleField == null)
        {
            uibattleField = Instantiate(UIBattleField, transform);
            /ListOfUIBattleField.Add(uibattleField);
            uibattleField.GetComponent<UIBattleFieldScript>().SetupCharOwner(cb);
        }
        else
        {
            uibattleField.GetComponent<UIBattleFieldScript>().SetupCharOwner(cb);
            uibattleField.SetActive(true);
        }*/
    }
}
