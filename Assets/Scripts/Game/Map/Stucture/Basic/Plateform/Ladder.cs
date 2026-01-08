using System;
using UnityEngine.Tilemaps;
class Ladder : Structure
{
    new protected string _name = "Limit";
    new public static string TileAssetReference = "Limit";


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

