using System;

[Serializable]
public class Generator : Structure
{
    public float _electricProduction = 0.0f;
    protected float _electricPower;

    public Generator()
    {

    }

    public virtual float Production()
    {
        return _electricProduction;
    }
}
