using UnityEngine;

class Ground : Structure
{
    new protected string _name = "Ground";
    public override string TileAssetReference => "Ground";
    new public bool IsWalkable = false;

    public override StructureLayer Layer => StructureLayer.Utility;

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }
}



