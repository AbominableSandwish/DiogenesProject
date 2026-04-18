using System.Collections.Generic;
using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;
    new protected  string _name = "Coil";
    public override string TileAssetReference => "Coil";

    #region Public Method
    public override StructureLayer Layer => StructureLayer.Utility;
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
        return MapManager.Instance.AddStructure(new Coil(), pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.Instance.AddStructure(new Coil(), pos);
    }
    #endregion
}