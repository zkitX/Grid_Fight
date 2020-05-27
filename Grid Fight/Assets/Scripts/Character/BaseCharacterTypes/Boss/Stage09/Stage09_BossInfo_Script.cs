using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Stage09_BossInfo_Script : MonoBehaviour
{
    public Vector2 maidenFormeAttackCoolDown = new Vector2(3f, 5f);
    public Vector2 moonlightBlessCastCoolDown = new Vector2(0.5f, 2f);
    public Vector2 moonlightBlessDuration = new Vector2(10f, 20f);
    [Range(0f, 100f)] public float moonlightBlessOdds = 20f;
    public float moonlightBlessRegenMultiplier = 5f;
    [Range(0f, 1f)] public float moonlightBlessAttackDampener = 0.3f;

    public List<float> divineEvocationLevels { private set; get; } = new List<float>();
    public List<NoFace_IntensityClass> demonFormeIntensityLevels = new List<NoFace_IntensityClass>
    {
        new NoFace_IntensityClass(80f, -1f, 40f, 2f, 4f, 1f),
        new NoFace_IntensityClass(40f, -2f, 80f, 1.5f, 3f, 3f),
        new NoFace_IntensityClass(20f, -3f, 120f, 1.25f, 2f, 6f),
        new NoFace_IntensityClass(0f, -5f, 200f, 0.75f, 1.5f, 10f)
    };

    public void InitialiseBossInfo()
    {
        List<NoFace_IntensityClass> demonIntensities = new List<NoFace_IntensityClass>();
        foreach(NoFace_IntensityClass demonIntensity in demonFormeIntensityLevels)
        {
            for (int i = 0; i < demonIntensities.Count; i++)
            {
                if(demonIntensities[i].evocationHealthLevel < demonIntensity.evocationHealthLevel)
                {
                    demonIntensities.Insert(i, demonIntensity);
                    break;
                }
                else if(i + 1 == demonIntensities.Count)
                {
                    demonIntensities.Add(demonIntensity);
                    break;
                }
            }
            if (demonIntensities.Count == 0)
            {
                demonIntensities.Add(demonIntensity);
            }
        }
        demonFormeIntensityLevels = demonIntensities;
        foreach(NoFace_IntensityClass intensity in demonFormeIntensityLevels)
        {
            divineEvocationLevels.Add(intensity.evocationHealthLevel);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < demonFormeIntensityLevels.Count; i++)
        {
            demonFormeIntensityLevels[i].Name = "Stage " + (i + 1).ToString() + " @" + demonFormeIntensityLevels[i].evocationHealthLevel.ToString() + "% Health";
        }
    }
}




[System.Serializable]
public class NoFace_IntensityClass
{
    [HideInInspector]public string Name = "new Intensity";
    public float evocationHealthLevel = 80f;
    public float healthDrainRate = -1f;
    public float healthAmount = 50f;
    public Vector2 attackRateRange = new Vector2(1f, 3f);
    public float transformationSpeedMultiplier = 1f;

    public NoFace_IntensityClass(float healthLevel, float _healthDrainRate, float _healthAmount, float attackRateMin, float attackRateMax, float _transformationSpeedMultiplier)
    {
        healthDrainRate = _healthDrainRate;
        healthAmount = _healthAmount;
        attackRateRange.x = attackRateMin;
        attackRateRange.y = attackRateMax;
        transformationSpeedMultiplier = _transformationSpeedMultiplier;
    }
}
