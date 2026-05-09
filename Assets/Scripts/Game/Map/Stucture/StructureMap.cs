/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IStructureMap
{
    Tilemap Tilemap { get; }
}
public class StructureMap<TMap> : MonoBehaviour, IStructureMap
{
    #region Private Data
    [SerializeField] protected Tilemap _tilemap;
    public Tilemap Tilemap => _tilemap;
    public Dictionary<Vector3Int, Structure> structures;

    public int Width, Height;
    protected GameManager _game;
    protected MapManager _map;
    #endregion

    #region Public Method
    public virtual void Init(int width, int height)
    {
        structures = new Dictionary<Vector3Int, Structure>();
    }

    public virtual bool AddStructure(Structure structure, Vector3Int position)
    {
        if (structure == null)
            return false;

        structure.Position = position;
        this.structures[position] = structure;
        return true;
    }

    public virtual bool RemoveStructure(Vector3Int position)
    {
        bool canRemove = structures.ContainsKey(position);
        if (canRemove)
        {
            structures.Remove(position);
            Tilemap.SetTile(position, null);
        }

        return canRemove;
    }

    public void Init()
    {
        _game = GameManager.Instance;
        _map = MapManager.Instance;

        structures = new Dictionary<Vector3Int, Structure>();
    }

    public virtual Structure GetStructure(Vector3Int position)
    {
        if (structures.TryGetValue(position, out Structure structure))
            return structure;

        return null;
    }

    public void SetStructure(Vector3Int pos, Structure structure)
    {
        structures[pos] = structure;
    }

    public bool HasStructure(Vector3Int pos)
    {
        return structures.ContainsKey(pos);
    }

    public virtual TileBase GetTile(Vector3Int position)
    {
        return Tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
    #endregion  

    /// <summary> Construit un MapData à partir de la tilemap / structures de cette couche </summary>
    public virtual MapData Capture()
    {
        // Par défaut: taille depuis GridManager (si dispo)
        var md = new MapData
        {
            width = _map != null ? _map.Width : 0,
            height = _map != null ? _map.Height : 0,
            cells = new List<MapCellData>()
        };
        return md;
    }

    /// <summary>Reconstruit la map depuis un MapData</summary>
    public virtual void Restore(MapData data)
    {
        // TODO: Clear la tilemap & reconstruire depuis data.cells
        // _tilemap.ClearAllTiles();
        // foreach (var c in data.cells) { ... SetTile / AddStructure ... }
    }

    public virtual void Refresh() { 
    
    }

    public void ClearMap()
    {
        structures.Clear();

        if (Tilemap != null)
        {
            structures.Clear();
            Tilemap.ClearAllTiles();
        }
        else
        {
            Debug.LogWarning("Tilemap is null in ClearMap");
        }
    }

    [SerializeField] private ConstructionSiteView constructionSiteViewPrefab;

    public void SpawnConstructionView(Vector3Int cell, ConstructionSite site)
    {
        Tilemap tilemap = _map.GetMapByLayer(site.Layer).Tilemap; // ou ta tilemap principale visuelle

        Vector3 worldPos = tilemap.GetCellCenterWorld(cell);

        ConstructionSiteView view = Instantiate(
            constructionSiteViewPrefab,
            worldPos + new Vector3(0f, 0.8f, 0f),
            Quaternion.identity
        );

        view.Bind(site, cell, tilemap, _map);
    }

}

