using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

class WoodPlateform : Structure
{
    new protected string _name = "WoodPlateform";
    public override string TileAssetReference => "WoodPlateform";
    new public bool IsWalkable = true;

    new protected StructureType _type = StructureType.WoodPlateform;

    #region Constructor
    public WoodPlateform(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {

        if (tilemap != null)
        {

        }
    }
    #endregion

    public override StructureLayer Layer => StructureLayer.Utility;

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new WoodPlateform(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new WoodPlateform(), pos);
    }
}