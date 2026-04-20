public static class StructureFactory
{
    public static Structure Create(StructureType type)
    {
        switch (type)
        {
            case StructureType.Ground:
                return new Ground();

            case StructureType.Ladder:
                return new Ladder();

            case StructureType.Door:
                return new Door();

            case StructureType.Coil:
                return new Coil();

            case StructureType.Generator:
                return new Generator();

            default:
                return null;
        }
    }
}