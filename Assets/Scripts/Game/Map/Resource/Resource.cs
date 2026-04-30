/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

public class Resource : Structure
{
    new protected string _name = "Resource";
    public override string TileAssetReference => "Resource";
    new public bool IsWalkable = true;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Resource;

}

public class Earth  : Resource
{
    new protected string _name = "Earth";
    public override string TileAssetReference => "Earth";

}

public class Stone : Resource
{
    new protected string _name = "Stone";
    public override string TileAssetReference => "Stone";

}


public class Roots : Resource
{
    new protected string _name = "Roots";
    public override string TileAssetReference => "Roots";

}

public class WaterSource : Resource
{
    new protected string _name = "WaterSource";
    public override string TileAssetReference => "WaterSource";

}