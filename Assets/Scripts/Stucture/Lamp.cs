using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;

class Lamp : Engine
{
    protected new float _electricityConsumption = 50.0f;  
    private Light2D light;

    public void Start()
    {
        //light = this.AddComponent<Light2D>();
    }

    public override void Consumption(float _electricity)
    {
        _electricityCurrent = _electricity;
        EnginePerformance = _electricityCurrent / _electricityConsumption;

        if (EnginePerformance > 1.0f)
            EnginePerformance = 1.0f;
    }

    private void Update()
    {
        light.intensity = EnginePerformance;
    }
}

