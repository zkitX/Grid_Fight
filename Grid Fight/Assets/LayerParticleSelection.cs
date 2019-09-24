using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerParticleSelection : MonoBehaviour
{
    [Header("Select the level of Shot, Idle is the standard(character is not being pressed)")]
    public ShotLevel Shot = ShotLevel.Idle;

    [Header("The objects the various levels of power")]
    [Tooltip("normal attack particle")]
    public List<Transform> ShotIdle;
    [Tooltip("Level1 attack particle(only when character is pressed)")]
    public List<Transform> ShotLvl01;
    [Tooltip("Level2 attack particle(only when character is pressed)")]
    public List<Transform> ShotLvl02;
    [Tooltip("Level3 attack particle(only when character is pressed)")]
    public List<Transform> ShotLvl03;
    private void Awake()
    {
        //close unnecessary layers
        CloseAllLayers();
    }

    private void OnEnable()
    {
        SelectShotLevel(Shot);
    }

    /// <summary>
    /// Enables the selected particles based on the type of shot(idle for normal shot, Lvl# for the stamina attacks)
    /// </summary>
    public void SelectShotLevel(ShotLevel s)
    {
        Shot = s;
        //close unnecessary layers
        CloseAllLayers();
        //Select all necessary layers for playing the selected shot level
        if (Shot >= ShotLevel.Idle)
        {
            SelectLayer(ShotIdle);
        }
        if (Shot >= ShotLevel.Lvl01)
        {
            SelectLayer(ShotLvl01);
        }
        if (Shot >= ShotLevel.Lvl02)
        {
            SelectLayer(ShotLvl02);
        }
        if (Shot >= ShotLevel.Lvl03)
        {
            SelectLayer(ShotLvl03);
        }
    }

    /// <summary>
    /// Internal method, enable all the objects of a list of transform
    /// </summary>
    void SelectLayer(List<Transform> s)
    {
        foreach (Transform t in s)
        {
            t.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Clear all children particles to start clear
    /// </summary>
    public void CloseAllLayers()
    {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t != transform)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

}

public enum ShotLevel
{
    Idle,
    Lvl01,
    Lvl02,
    Lvl03
}