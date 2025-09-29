using System;

[Serializable]
public class Generator : Structure
{
    protected float _electricProduction = 0.0f;
    protected float _electricPower;

    public Generator()
    {

    }

    public virtual float Output()
    {
        return _electricProduction;
    }
}
