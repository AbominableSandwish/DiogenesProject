/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine.Tilemaps;
public class Ladder : Structure
{
    new protected string _name = "Ladder";
    public override string TileAssetReference => "Ladder";
    new public bool IsWalkable = false;
    public override bool IsTraversable => true;
    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Ladder;

    #region Constructor
    public Ladder(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
    }
    #endregion
}

