/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;

public class Coil : Structure
{
    Structure StructureConnect;
    new protected  string _name = "Coil";
    public override string TileAssetReference => "Coil";

    #region Public Method
    public override StructureLayer Layer => StructureLayer.Utility;
    public override StructureType Type => StructureType.Coil;
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