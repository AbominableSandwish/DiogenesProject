public class Begin : Structure
{
    new protected string _name = "Begin";
    public override StructureLayer Layer => StructureLayer.Basic;
    public override string TileAssetReference => "Begin";
    public override bool IsTraversable => true;
    public override StructureType Type => StructureType.Begin;

    public Begin()
    {
        _name = "Begin";
        IsWalkable = true;
        IsClimbable = false;
    }
}