/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

public class End : Structure
{
    new protected string _name = "End";
    public override StructureLayer Layer => StructureLayer.Basic;
    public override string TileAssetReference => "End";
    public override bool IsTraversable => true;
    public override StructureType Type => StructureType.End;

    public End()
    {
        _name = "End";
        IsWalkable = true;
        IsClimbable = false;
    }
}