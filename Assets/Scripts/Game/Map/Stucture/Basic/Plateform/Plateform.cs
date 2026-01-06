using UnityEngine;

class WoodPlateform : Structure
{
    new protected string _name = "WoodPlateform";
    new public static string TileAssetReference = "WoodPlateform";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<WoodPlateform>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<WoodPlateform>(pos);
    }
}