using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;

    #region Public Method
    public bool isConnect()
    {
        return StructureConnect != null;
    }

    public override bool ToPlace(Vector3Int pos)
    {
        return Map.AddStructure<Coil>(pos);
    }
    #endregion
}