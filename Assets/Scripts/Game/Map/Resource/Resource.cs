using UnityEngine;

public class Resource : Structure
{
    new protected string _name = "Resource";
    public override string TileAssetReference => "Resource";
    new public bool IsWalkable = true;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }
}

public class Earth  : Resource
{
    new protected string _name = "Earth";
    public override string TileAssetReference => "Earth";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }

}

public class Stone : Resource
{
    new protected string _name = "Stone";
    public override string TileAssetReference => "Stone";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }

}


public class Roots : Resource
{
    new protected string _name = "Roots";
    public override string TileAssetReference => "Roots";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }

}

public class WaterSource : Resource
{
    new protected string _name = "WaterSource";
    public override string TileAssetReference => "WaterSource";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Ground(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Ground(), pos);
    }


}