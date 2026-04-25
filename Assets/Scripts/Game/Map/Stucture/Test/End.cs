public class End : Structure
{
    new protected string _name = "End";
    public override StructureLayer Layer => StructureLayer.Basic;
    public override string TileAssetReference => "End";
    public override StructureType Type => StructureType.End;

    public End()
    {
        _name = "End";
        IsWalkable = true;
        IsClimbable = false;
    }
}