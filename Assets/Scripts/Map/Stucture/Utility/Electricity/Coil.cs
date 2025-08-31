using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;

    public bool isConnect()
    {
        return StructureConnect != null;
    }

    public override bool ToPlace(Vector2Int pos)
    {
        return Map.AddStructure<Coil>(pos);
    }
}

