using System;
using UnityEngine;

[Serializable]
public class Generator : Structure
{
    protected float _electricProduction = 0.0f;
    protected float _electricPower;

    public override StructureLayer Layer => StructureLayer.Utility;

    public Generator(Vector3Int position = new Vector3Int())
    {
        this._type = StructureType.Generator;
        
        this._position = position;
    }

    public virtual float Output()
    {
        return _electricProduction;
    }
}
