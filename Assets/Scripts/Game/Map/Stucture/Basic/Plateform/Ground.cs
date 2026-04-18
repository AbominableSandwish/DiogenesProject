using UnityEngine;

class Ground : Structure
{
    new protected string _name = "Ground";
    new public static string TILE_ASSET_REFERENCE = "Ground";
    new public bool IsWalkable = false;

    public override StructureLayer Layer => StructureLayer.Utility;

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }
}



