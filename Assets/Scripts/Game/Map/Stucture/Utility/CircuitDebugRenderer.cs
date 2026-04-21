using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CircuitDebugRenderer : MonoBehaviour
{
    [SerializeField] private Tilemap debugTilemap;
    [SerializeField] private TileBase debugTile;
    [SerializeField] private UtilityMap utilityMap;
    [SerializeField] private bool showDebug = true;

    private void Awake()
    {
        debugTilemap = UnityResolver.Resolve(debugTilemap, this, "Debug Tilemap");
        utilityMap = UnityResolver.Resolve(utilityMap, this, "UtilityMap");
    }

    public void RefreshDebug()
    {
        if (debugTilemap == null || utilityMap == null)
            return;

        debugTilemap.ClearAllTiles();

        if (!showDebug || debugTile == null)
            return;

        Dictionary<Vector3Int, Circuit> ownerAt = utilityMap.OwnerAt;

        foreach (var kv in ownerAt)
        {
            Vector3Int pos = kv.Key;
            Circuit circuit = kv.Value;

            debugTilemap.SetTile(pos, debugTile);
            debugTilemap.SetColor(pos, circuit.DebugColor);
        }
    }

    public void SetVisible(bool visible)
    {
        showDebug = visible;
        RefreshDebug();
    }

    private void OnEnable()
    {
        if (utilityMap != null)
            utilityMap.OnCircuitsChanged += RefreshDebug;
    }

    private void OnDisable()
    {
        if (utilityMap != null)
            utilityMap.OnCircuitsChanged -= RefreshDebug;
    }
}