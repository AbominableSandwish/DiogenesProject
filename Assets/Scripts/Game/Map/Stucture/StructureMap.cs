using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureMap<T> : MonoBehaviour
{
    #region Private Data
    [SerializeField] protected Tilemap _tilemap;

    public int Width, Height;
    protected static T _instance;
    protected GameManager _game;
    protected MapManager _map;
    #endregion

    #region Public Method
    public virtual bool AddStructure<TStructure>(Vector3Int pos)
    {
        return false;
    }

    public virtual bool RemoveStructure<TStructure>(Vector3Int pos)
    {
        return false;
    }

    public virtual Structure GetStructure(Vector3Int pos)
    {
        return null;
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

}

