using UnityEngine;
using UnityEngine.Tilemaps;

class Limit : Structure
{
    new protected string _name = "Limit";
    new public static string TileAssetReference = "Limit";

    #region Constructor
    public Limit(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        this._type = StructureType.Limit;

        if (tilemap != null)
        {

        }
    }
    #endregion

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Limit>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Limit>(pos);
    }
}