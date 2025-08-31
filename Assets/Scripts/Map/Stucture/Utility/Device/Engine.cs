using System;

[Serializable]
public class Engine : Structure
{
    public float _electricityConsumption = 0.0f;
    public float _electricityCurrent = 0.0f;
    public float EnginePerformance = 0.0f;
    public virtual void Consumption(float _electricity)
    {
        _electricityCurrent = _electricity;
    }
}
