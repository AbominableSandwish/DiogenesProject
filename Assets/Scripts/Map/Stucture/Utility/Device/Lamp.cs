using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lamp : Engine
{
    protected new float _electricityConsumption = 50.0f;  
    private Light2D _light;

    public Lamp()
    {
        Debug.Log("hello");
    }

    public Lamp(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        Debug.Log("Holla");
        if (tilemap != null) {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
            _light = _object.GetComponent<Light2D>();
        }
    }

    public override void Consumption(float _electricity)
    {
        _electricityCurrent = _electricity;
        EnginePerformance = _electricityCurrent / _electricityConsumption;

        if (EnginePerformance > 1.0f)
            EnginePerformance = 1.0f;
        _light.intensity = EnginePerformance;
    }

    public override bool ToPlace(Vector2Int pos)
    {
        return Map.AddStructure<Lamp>(pos);

    }
}

