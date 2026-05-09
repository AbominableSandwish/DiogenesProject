/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine.Tilemaps;

public class Limit : Structure
{
    new protected string _name = "Limit";
    public override string TileAssetReference => "Limit";
    new public bool IsWalkable = false;
    new public bool IsClimbable = true;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Limit;

    #region Constructor
    public Limit(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
    }

    #endregion
}