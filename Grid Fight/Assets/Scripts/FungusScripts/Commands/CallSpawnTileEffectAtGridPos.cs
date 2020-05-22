using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Spawn Tile Effect At Grid Pos",
                "Spawns a tile effect at a specified tile position for a set duration")]
[AddComponentMenu("")]
public class CallSpawnTileEffectAtGridPos : Command
{
    public bool randomisePosition = true;
    [ConditionalField("randomisePosition")] public int tilesEffected = 1;
    [ConditionalField("randomisePosition")] public WalkingSideType gridSide = WalkingSideType.Both;
    public List<AffectTile> affectedTiles = new List<AffectTile>();
    public float duration = 5f;
    public bool destroyOnCollection = false;
    
    bool TileAlreadyUsed(Vector2Int pos)
    {
        foreach(AffectTile aT in affectedTiles)
        {
            if (aT.pos == pos) return true;
        }
        return false;
    }

    protected virtual void CallTheMethod()
    {
        if (randomisePosition)
        {
            affectedTiles = new List<AffectTile>();
            for (int i = 0; i < tilesEffected; i++)
            {
                int loops = 0;
                Vector2Int nextPos = new Vector2Int(-1, -1);
                while(nextPos == new Vector2Int(-1,-1) || TileAlreadyUsed(nextPos))
                {
                    nextPos = GridManagerScript.Instance.GetFreeBattleTile(gridSide).Pos;
                    loops++;
                    if (loops > 99)
                    {
                        Debug.LogError("TOO MANY LOOPS, cant find an empty tile for the required effect");
                        break;
                    }
                }
                affectedTiles.Add(new AffectTile(nextPos));
            }
        }

        foreach (AffectTile aT in affectedTiles)
        {
            if (!aT.playFromPrefab)
                GridManagerScript.Instance.GetBattleTile(aT.pos).SetupEffect(
                    new List<ScriptableObjectAttackEffect> { aT.overrideEffect }, duration, aT.particleEffectToPlay, destroyOnCollection
                    );
            else ParticleManagerScript.Instance.FireParticlesInPosition(aT.particlePrefabToFire, CharacterNameType.None, 
                AttackParticlePhaseTypes.Cast, GridManagerScript.Instance.GetBattleTile(aT.pos, WalkingSideType.Both).transform.position, 
                SideType.LeftSide, AttackInputType.Skill1);

        }
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion

}

[System.Serializable]
public class AffectTile
{
    public Vector2Int pos = new Vector2Int(0,0);
    public bool playFromPrefab = false;
    [ConditionalField("playFromPrefab", true)] public ParticlesType particleEffectToPlay = ParticlesType.None;
    [ConditionalField("playFromPrefab")] public GameObject particlePrefabToFire = null;
    public ScriptableObjectAttackEffect overrideEffect = null;

    public AffectTile()
    {
        pos = new Vector2Int(0, 0);
        particleEffectToPlay = ParticlesType.None;
    }

    public AffectTile(Vector2Int _pos, ParticlesType particles = ParticlesType.None)
    {
        pos = _pos;
        particleEffectToPlay = particles;
    }
}