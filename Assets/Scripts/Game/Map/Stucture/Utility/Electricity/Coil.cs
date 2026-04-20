using System.Collections.Generic;
using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;
    new protected  string _name = "Coil";
    public override string TileAssetReference => "Coil";
    new protected StructureType _type = StructureType.Coil;

    #region Public Method
    public override StructureLayer Layer => StructureLayer.Utility;
    public Coil(){}

    public Coil(Vector3Int position)
    {
        this._position = position;
    }
    public bool isConnect()
    {
        return StructureConnect != null;
    }
    #endregion
}