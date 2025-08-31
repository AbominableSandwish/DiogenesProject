using UnityEngine;
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

    public override bool ToPlace(Vector2Int pos)
    {
        return Map.AddStructure<Lamp>(pos);
    }

    private void Update()
    {
        light.intensity = EnginePerformance;
    }
}

