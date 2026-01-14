using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

class WoodPlateform : Structure
{
    new protected string _name = "WoodPlateform";
    new public static string TileAssetReference = "WoodPlateform";
    new public bool IsWalkable = true;

    #region Constructor
    public WoodPlateform(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        this._type = StructureType.WoodPlateform;

        if (tilemap != null)
        {

        }
    }
    #endregion

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<WoodPlateform>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<WoodPlateform>(pos);
    }
}