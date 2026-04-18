using UnityEngine;

public class Resource : Structure
{
    new protected string _name = "Resource";
    new public static string TILE_ASSET_REFERENCE = "Resource";
    new public bool IsWalkable = true;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }
    public override string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }
}

public class Earth  : Resource
{
    new protected string _name = "Earth";
    new public static string TILE_ASSET_REFERENCE = "Earth";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }

    public override string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }
}

public class Stone : Resource
{
    new protected string _name = "Stone";
    new public static string TILE_ASSET_REFERENCE = "Stone";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }

    public override string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }
}


public class Roots : Resource
{
    new protected string _name = "Roots";
    new public static string TILE_ASSET_REFERENCE = "Roots";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }

    public override string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }
}

public class WaterSource : Resource
{
    new protected string _name = "WaterSource";
    new public static string TILE_ASSET_REFERENCE = "WaterSource";

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Ground>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Ground>(pos);
    }

    public override string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }
}