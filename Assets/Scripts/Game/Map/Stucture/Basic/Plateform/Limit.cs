using UnityEngine;
using UnityEngine.Tilemaps;

class Limit : Structure
{
    new protected string _name = "Limit";
    public override string TileAssetReference => "Limit";
    new public bool IsWalkable = false;
    new public bool IsClimbable = true;

    public override StructureLayer Layer => StructureLayer.Basic;

    #region Constructor
    public Limit(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        this._type = StructureType.Limit;
    }

    #endregion



    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Limit(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.RemoveStructure(new Limit(), pos);
    }
}