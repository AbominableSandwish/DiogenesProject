using System;

[Serializable]
public class Generator : Structure
{
    protected float _electricProduction = 0.0f;
    protected float _electricPower;

    public Generator()
    {
        this._type = StructureType.Generator;
    }

    public virtual float Output()
    {
        return _electricProduction;
    }
}
