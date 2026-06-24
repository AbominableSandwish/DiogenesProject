/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine.Tilemaps;

class WoodPlateform : Structure
{
    new protected string _name = "WoodPlateform";
    public override string TileAssetReference => "WoodPlateform";
    new public bool IsWalkable = true;

    #region Constructor
    public WoodPlateform(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {

        if (tilemap != null)
        {

        }
    }
    #endregion

    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Door;

}