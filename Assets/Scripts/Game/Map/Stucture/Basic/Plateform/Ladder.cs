using System;
using UnityEngine.Tilemaps;
class Ladder : Structure
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
        if (tilemap != null)
        {

        }
    }
    #endregion
}

