/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

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

            float ratio = circuit.PowerRatio;
            Color color = Color.Lerp(Color.red, Color.green, ratio);

            debugTilemap.SetTile(pos, debugTile);
            debugTilemap.SetTileFlags(pos, TileFlags.None);
            debugTilemap.SetColor(pos, color);
        }
    }

    public void Update()
    {
        RefreshDebug();
    }

    public void SetVisible(bool visible)
    {
        showDebug = visible;
        RefreshDebug();
    }
}