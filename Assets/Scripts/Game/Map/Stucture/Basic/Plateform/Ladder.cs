using System;
using UnityEngine.Tilemaps;
class Ladder : Structure
{
    new protected string _name = "Ladder";
    public override string TileAssetReference => "Ladder";
    new public bool IsWalkable = false;
    new protected StructureType _type = StructureType.Ladder;

    public override StructureLayer Layer => StructureLayer.Basic;
    #region Constructor
    public Ladder(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null)
        {

        }
    }
    #endregion
}

