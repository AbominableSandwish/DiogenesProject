using Unity.VisualScripting;

public class ConstructionSite : Structure
{
    public override string TileAssetReference => "ConstructionSite";
    public override StructureType Type => StructureType.ConstructionSite;
    public float Progress { get; private set; }
    public float RequiredWork { get; private set; } = 100f;
    public override bool IsTraversable => true;
    public bool IsCompleted => Progress >= RequiredWork;

    public Structure TargetStructure;

    public ConstructionSite(Structure targetStructure)
    {
        TargetStructure = targetStructure;
        Progress = 0f;
    }

    public override StructureLayer Layer => TargetStructure.Layer;

    public void AddWork(float amount)
    {
        Progress += amount;
    }
}