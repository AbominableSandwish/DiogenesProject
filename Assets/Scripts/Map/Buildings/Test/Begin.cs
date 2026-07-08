/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

public class Begin : Structure
{
    new protected string _name = "Begin";
    public override StructureLayer Layer => StructureLayer.Basic;
    public override string TileAssetReference => "Begin";
    public override bool IsTraversable => true;
    public override StructureType Type => StructureType.Begin;

    public Begin()
    {
        _name = "Begin";
        IsWalkable = true;
        IsClimbable = false;
    }
}