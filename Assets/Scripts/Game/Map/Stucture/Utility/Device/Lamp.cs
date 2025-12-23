using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lamp : Engine
{
    #region Private data
    private Light2D _light;
    private float _outerRadius;

    protected new StructureMap _map = StructureMap.Utility;
    #endregion

    #region Constructor
    public Lamp(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        this._type = StructureType.Lamp;

        if (tilemap != null) {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
            _light = _object.GetComponent<Light2D>();
            _light.intensity = 0;
            _outerRadius = _light.pointLightOuterRadius;
            _electricityConsumption = 50.0f;
        }
    }
    #endregion

    #region Public method
    public override float Input(float _electricity)
    {
        float electricity = _electricity;
        electricity = base.Input(electricity);
        
        if (_light != null)
        {
            _light.intensity = EnginePerformance;
            _light.pointLightOuterRadius = _outerRadius * EnginePerformance;
        }

        return electricity;
    }

    public override bool ToPlace(Vector3Int pos)
    {
        return MapManager.AddStructure<Lamp>(pos);
    }
    public override bool ToRemove(Vector3Int pos)
    {
        return MapManager.RemoveStructure<Lamp>(pos);
    }
    #endregion
}