using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lamp : Engine
{
    new protected string _name = "Lamp";
    new public static string TileAssetReference = "Lamp";
    public override StructureType Type => StructureType.Lamp;


    #region Private data
    private Light2D _light;
    private float _outerRadius;

    public override StructureLayer Layer => StructureLayer.Utility;
    #endregion

    #region Constructor
    public Lamp(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
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

    #endregion
}