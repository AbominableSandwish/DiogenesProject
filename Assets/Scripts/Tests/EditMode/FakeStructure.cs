#if UNITY_INCLUDE_TESTS
public class FakeStructure : Structure
{
    public override string TileAssetReference => "TestTile";
    public override StructureType Type => StructureType.FakeStructure;
    public override StructureLayer Layer => StructureLayer.Basic;
}
#endif