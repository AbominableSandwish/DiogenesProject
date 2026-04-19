using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class StructureMap<TMap> : MonoBehaviour
{
    #region Private Data
    [SerializeField] protected Tilemap _tilemap;
    public Dictionary<Vector3Int, Structure> structures;

    public int Width, Height;
    protected GameManager _game;
    protected MapManager _map;
    #endregion

    #region Public Method
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
            structures[position] = null;
            _tilemap.SetTile(position, null);
        }

        return canRemove;
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
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
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
        _tilemap.ClearAllTiles();
    }

}

