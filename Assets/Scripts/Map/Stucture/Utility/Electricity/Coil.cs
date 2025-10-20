using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;

    #region Public Method
    public Coil()
    {
        this._type = StructureType.Coil;
    }
    public bool isConnect()
    {
        return StructureConnect != null;
    }
    public override bool ToPlace(Vector3Int pos)
    {
        return Map.AddStructure<Coil>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return Map.RemoveStructure<Coil>(pos);
    }
    #endregion
}