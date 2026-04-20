using System;
using UnityEngine;

[Serializable]
public class Generator : Structure
{
    protected float _electricProduction = 0.0f;
    protected float _electricPower;
    public override string TileAssetReference => "Generator";
    public override StructureType Type => StructureType.Generator;

    public override StructureLayer Layer => StructureLayer.Utility;

    public Generator(Vector3Int position = new Vector3Int())
    {    
        this._position = position;
    }

    public virtual float Output()
    {
        return _electricProduction;
    }
}
