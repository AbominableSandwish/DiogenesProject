using System;
using UnityEngine.Tilemaps;
class Ladder : Structure
{
    new protected string _name = "Ladder";
    new public static string TILE_ASSET_REFERENCE = "Ladder";
    new public bool IsWalkable = false;

    public override StructureLayer Layer => StructureLayer.Utility;
    #region Constructor
    public Ladder(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        this._type = StructureType.Ladder;

        if (tilemap != null)
        {

        }
    }
    #endregion
}

