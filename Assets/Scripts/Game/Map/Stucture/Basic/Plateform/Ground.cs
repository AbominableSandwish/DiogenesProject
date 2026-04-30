/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */
class Ground : Structure
{
    new protected string _name = "Ground";
    public override string TileAssetReference => "Ground";
    new public bool IsWalkable = false;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Ground;

}



