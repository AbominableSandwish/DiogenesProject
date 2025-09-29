using UnityEngine;
using UnityEngine.Tilemaps;
class DecorationMap : StructureMap<DecorationMap>
{
    #region Nested Method
    public DecorationMap(int height, int width)
    {
    }
    #endregion

    #region Public Method
    public override bool AddStructure<T>(Vector3Int pos)
    {
        return false;
    }

    public override bool RemoveStructure<T>(Vector3Int pos)
    {
        return false;
    }

    override public Structure GetStructure(Vector3Int pos)
    {
        return null;
    }

    override public TileBase GetTile(Vector3Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
    #endregion
}

