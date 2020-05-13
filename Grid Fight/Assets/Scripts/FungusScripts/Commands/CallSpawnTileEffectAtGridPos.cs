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
    [ConditionalField("randomisePosition", inverse: true)] public List<AffectTile> affectedTiles = new List<AffectTile>();
    public bool shareSameParticleEffect = true;
    [ConditionalField("shareSameParticleEffect")] public ParticlesType particleEffectToPlay = ParticlesType.None;
    public bool shareSameEffect = true;
    [ConditionalField("shareSameEffect")] public ScriptableObjectAttackEffect effect = null; 
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

        if (shareSameParticleEffect) foreach (AffectTile affectTile in affectedTiles) affectTile.particleEffectToPlay = particleEffectToPlay;
        if (shareSameEffect) foreach (AffectTile affectTile in affectedTiles) affectTile.effect = effect;

        foreach (AffectTile aT in affectedTiles)
        {
            GridManagerScript.Instance.GetBattleTile(aT.pos).SetupEffect(new List<ScriptableObjectAttackEffect> { aT.effect }, duration, aT.particleEffectToPlay, destroyOnCollection);
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

    private void OnValidate()
    {
        if (randomisePosition)
        {
            shareSameParticleEffect = true;
            shareSameEffect = true;
        }
        else
        {
            if (shareSameParticleEffect) foreach (AffectTile affectTile in affectedTiles) affectTile.particleEffectToPlay = particleEffectToPlay;
            if (shareSameEffect) foreach (AffectTile affectTile in affectedTiles) affectTile.effect = effect;
        }
    }
}

[System.Serializable]
public class AffectTile
{
    public Vector2Int pos = new Vector2Int(0,0);
    public ParticlesType particleEffectToPlay = ParticlesType.None;
    public ScriptableObjectAttackEffect effect = null;

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