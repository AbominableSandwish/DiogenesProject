using UnityEngine;
using UnityEngine.Tilemaps;
class DecorationMap : StructureMap<DecorationMap>
{
    public DecorationMap(int height, int width)
    {
    }
    public override bool AddStructure<T>(Vector2Int pos)
    {
        return false;
    }

    public override bool RemoveStructure<T>(Vector2Int pos)
    {
        return false;
    }

    override public Structure GetStructure(Vector2Int pos)
    {
        return null;
    }

    override public TileBase GetTile(Vector2Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
}

