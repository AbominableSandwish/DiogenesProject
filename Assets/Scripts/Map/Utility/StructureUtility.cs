using UnityEngine;

public static class StructureFactory
{
    public static Structure Create(StructureType type)
    {
        switch (type)
        {
            case StructureType.Ground:
                return new Ground();

            case StructureType.WoodPlateform:
                return new WoodPlateform();

            case StructureType.Limit:
                return new Limit();

            case StructureType.Door:
                return new Door();

            case StructureType.Glass:
                return new Glass();

            case StructureType.Ladder:
                return new Ladder();

            case StructureType.Coil:
                return new Coil();

            case StructureType.Generator:
                return new Generator();

            case StructureType.Engine:
                return new Engine();

            case StructureType.SolarPanel:
                return new SolarPanel();

            case StructureType.Lamp:
                return new Lamp();

            case StructureType.Begin:
                return new Begin();

            case StructureType.End:
                return new End();

            default:
                Debug.LogWarning($"No factory entry for structure type {type}");
                return null;
        }
    }
}