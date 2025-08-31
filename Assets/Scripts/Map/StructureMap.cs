using UnityEngine;
using UnityEngine.Tilemaps;

public class StructureMap<T> : MonoBehaviour
{
    protected static T _instance;
    protected GameManager _game;
    protected Map _map;

    [SerializeField] protected Tilemap _tilemap;

    virtual public bool AddStructure<T>(Vector2Int pos)
    {
        return false;
    }

    virtual public bool RemoveStructure<T>(Vector2Int pos)
    {
        return false;
    }

    virtual public Structure GetStructure(Vector2Int pos)
    {
        return null;
    }

    virtual public TileBase GetTile(Vector2Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
}

