/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

public class Door : Structure
{
    public override string TileAssetReference => "Door";
    public override StructureLayer Layer => StructureLayer.Utility;
    public override StructureType Type => StructureType.Door;
}

