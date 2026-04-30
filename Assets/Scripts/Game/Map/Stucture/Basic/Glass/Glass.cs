/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

class Glass : Structure
{
    public override string TileAssetReference => "Glass";
    public override StructureLayer Layer => StructureLayer.Utility;
    public override StructureType Type => StructureType.Glass;
}