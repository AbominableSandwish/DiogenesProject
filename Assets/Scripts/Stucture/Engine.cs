

class Engine : Structure
{
    protected float _electricityConsumption = 0.0f;
    protected float _electricityCurrent = 0.0f;
    public float EnginePerformance = 0.0f;
    public virtual void Consumption(float _electricity)
    {
        _electricityCurrent = _electricity;
    }
}
