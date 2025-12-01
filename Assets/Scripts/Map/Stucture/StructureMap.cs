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

    public virtual void SaveMap() { }
    public virtual void LoadMap() { }
}

