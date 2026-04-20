using UnityEngine;

class Ground : Structure
{
    new protected string _name = "Ground";
    public override string TileAssetReference => "Ground";
    new public bool IsWalkable = false;

    public override StructureLayer Layer => StructureLayer.Basic;

}



