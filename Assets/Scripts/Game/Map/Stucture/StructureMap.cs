using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureMap<T> : MonoBehaviour
{
    #region Private Data
    [SerializeField] protected Tilemap _tilemap;

    protected static T _instance;
    protected GameManager _game;
    protected MapManager _map;
    #endregion

    #region Public Method
    public virtual bool AddStructure<T>(Vector3Int pos)
    {
        return false;
    }

    public virtual bool RemoveStructure<T>(Vector3Int pos)
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

    /// <summary>Construit un MapData à partir de la tilemap / structures de cette couche</summary>
    public virtual MapData Capture()
    {
        // Par défaut: taille depuis GridManager (si dispo)
        var md = new MapData
        {
            width = _map != null ? _map.Width : 0,
            height = _map != null ? _map.Height : 0,
            cells = new List<MapCellData>()
        };

        // TODO: Parcours ta tilemap &/ou structures pour remplir md.cells
        // Exemple indicatif (à adapter à ton projet) :
        // foreach (var pos in _tilemap.cellBounds.allPositionsWithin)
        // {
        //     var tile = _tilemap.GetTile(pos);
        //     if (tile == null) continue;
        //     md.cells.Add(new MapCellData { x = pos.x, y = pos.y, z = pos.z, type = tile.name });
        // }

        return md;
    }

    /*
        string json = JsonUtility.ToJson(data, true);

        string path = Path.Combine(Application.persistentDataPath, "BasicMap.json");
        File.WriteAllText(path, json);

        Debug.Log($"✅ BasicMap sauvegardée: {path}");
    */

    /// <summary>Reconstruit la map depuis un MapData</summary>
    public virtual void Restore(MapData data)
    {
        // TODO: Clear la tilemap & reconstruire depuis data.cells
        // _tilemap.ClearAllTiles();
        // foreach (var c in data.cells) { ... SetTile / AddStructure ... }
    }

    /*
     *  // si tu veux la version persistent
        string path = Path.Combine(Application.persistentDataPath, "BasicMap.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("BasicMap.json introuvable dans persistentDataPath.");
            return;
        }

        string json = File.ReadAllText(path);
        MapData data = JsonUtility.FromJson<MapData>(json);

        // Clear + reconstruction depuis data.cells...
     */
}

