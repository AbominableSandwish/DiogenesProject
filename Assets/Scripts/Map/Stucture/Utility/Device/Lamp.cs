using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lamp : Engine
{
    protected new float _electricityConsumption = 50.0f;  
    private Light2D _light;

    float OuterRadius;
    public Lamp(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null) {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
            _light = _object.GetComponent<Light2D>();
            _light.intensity = 0;
            OuterRadius = _light.pointLightOuterRadius;
        }
    }

    public override float Consumption(float _electricity)
    {
        _electricity = base.Consumption(_electricity);
        
        if (_light != null)
        {
            _light.intensity = EnginePerformance;
            _light.pointLightOuterRadius = OuterRadius * EnginePerformance;
        }

        return _electricity;
    }

    public override bool ToPlace(Vector2Int pos)
    {
        return Map.AddStructure<Lamp>(pos);

    }
}

