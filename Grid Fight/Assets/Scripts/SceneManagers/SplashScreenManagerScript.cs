using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenManagerScript : MonoBehaviour
{

    public static SplashScreenManagerScript Instance;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        PlayerPrefsSwitch.PlayerPrefsSwitch.Init();
#endif
        Invoke("GoToMainMenu", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
