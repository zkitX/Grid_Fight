using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call WaveMinionToSpawn",
                "WaveMinionToSpawn")]
[AddComponentMenu("")]
public class CallWaveMinionToSpawn : Command
{
    public CharacterNameType characterID;

    public string CharIdentifier;

    public string WaveName;
    public bool IsRandomPos = true;
    public bool removeFromWave = false;
    [ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(WaveMinionToSpawn());
    }

    IEnumerator WaveMinionToSpawn()
    {
        yield return WaveManagerScript.Instance.SpawnCharFromGivenWave(WaveName, characterID, CharIdentifier, IsRandomPos, SpawningPos, removeFromWave);
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

