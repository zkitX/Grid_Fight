using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoaderManagerScript : MonoBehaviour
{

    public static LoaderManagerScript Instance;
    public Image LoadingBar;
    public CanvasGroup MainCanvasGroup;
    public List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
    public MatchType MatchInfoType;
    private void Awake()
    {
        Instance = this;
    }

    public void LoadNewSceneWithLoading(string nextScene, string prevScene)
    {
        StartCoroutine(LoadNewSceneWithLoading_co(nextScene, prevScene));
    }
    // Start is called before the first frame update
    public IEnumerator LoadNewSceneWithLoading_co(string nextScene, string prevScene)
    {
        MainCanvasGroup.alpha = 1;
        bool isSceneActive = false;
        if(nextScene == "BattleScene")
        {

        }

        SceneManager.UnloadSceneAsync(prevScene);
        //Begin to load the Scene you specify
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        //Don't let the Scene activate until you allow it to
        asyncLoad.allowSceneActivation = false;
        //When the load is still in progress, output the Text and progress bar
        while (!asyncLoad.isDone)
        {
            //Output the current progress
            LoadingBar.fillAmount = asyncLoad.progress;

            // Check if the load has finished
            if (!isSceneActive && asyncLoad.progress >= 0.9f)
            {
                isSceneActive = true;
                asyncLoad.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
    }


}

[System.Serializable]
public class CharacterBaseInfoClass
{
    public string Name;
    public CharacterSelectionType CharacterSelection;
    public CharacterLevelType CharacterLevel;
    public ControllerType PlayerController;
    public CharacterNameType CharacterName;
    /* public AttackParticleTypes AttackParticle;
     public ArmorClass Armor;
     public WeaponClass Weapon;
     public List<ElementalResistenceClass> ElementalsResistence = new List<ElementalResistenceClass>();
     public List<ElementalType> ElementalsPower = new List<ElementalType>();
     public List<CharactersRelationshipClass> CharacterRelationships = new List<CharactersRelationshipClass>();
     public float AttackTimeRatio;
     public float Special2LoadingDuration;
     public float Special3LoadingDuration;
     public float Health;
     public float HealthBase;
     public float Regeneration;
     public float BaseSpeed = 1;
     public float Stamina;
     public float StaminaBase;
     public float StaminaRegeneration;
     public float StaminaCostSpecial1;
     public float StaminaCostSpecial2;
     public float StaminaCostSpecial3;


     public float HealthPerc
     {
         get
         {
             return (Health * 100) / HealthBase;
         }
     }

     public float StaminaPerc
     {
         get
         {
             return (Stamina * 100) / StaminaBase;
         }
     }*/


    public CharacterBaseInfoClass(string name, CharacterSelectionType characterSelection, CharacterLevelType characterLevel, ControllerType playerController, CharacterNameType characterName)
    {
        Name = name;
        CharacterSelection = characterSelection;
        CharacterLevel = characterLevel;
        PlayerController = playerController;
        CharacterName = characterName;
    }                             
}                                
