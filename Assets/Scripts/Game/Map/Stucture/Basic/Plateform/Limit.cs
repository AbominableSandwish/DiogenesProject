using UnityEngine;
using UnityEngine.Tilemaps;

class Limit : Structure
{
    new protected string _name = "Limit";
    public override string TileAssetReference => "Limit";
    new public bool IsWalkable = false;
    new public bool IsClimbable = true;

    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Door;

    #region Constructor
    public Limit(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
    }

    #endregion

}