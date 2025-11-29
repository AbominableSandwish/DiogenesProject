using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureMap<T> : MonoBehaviour
{
    #region Private Data
    [SerializeField] protected Tilemap _tilemap;

    protected static T _instance;
    protected GameManager _game;
    protected GridManager _map;
    #endregion

    #region Public Method
    virtual public bool AddStructure<T>(Vector3Int pos)
    {
        return false;
    }

    virtual public bool RemoveStructure<T>(Vector3Int pos)
    {
        return false;
    }

    virtual public Structure GetStructure(Vector3Int pos)
    {
        return null;
    }

    virtual public TileBase GetTile(Vector3Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
    #endregion

    public virtual void SaveMap() { }
    public virtual void LoadMap() { }
}

