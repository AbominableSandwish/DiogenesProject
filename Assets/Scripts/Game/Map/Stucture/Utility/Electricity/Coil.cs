using System.Collections.Generic;
using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;
    new protected  string _name = "Coil";
    new public static string TileAssetReference = "Coil";

    #region Public Method
    public Coil()
    {
       
        this._type = StructureType.Coil;
    }

    public Coil(Vector3Int position)
    {
        this._type = StructureType.Coil;
        this._position = position;
    }
    public bool isConnect()
    {
        return StructureConnect != null;
    }
    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Coil>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Coil>(pos);
    }
    #endregion
}