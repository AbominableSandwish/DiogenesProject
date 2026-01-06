using UnityEngine;

class Limit : Structure
{
    new protected string _name = "Limit";
    new public static string TileAssetReference = "Limit";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Limit>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Limit>(pos);
    }
}