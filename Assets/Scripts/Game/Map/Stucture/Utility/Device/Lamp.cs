/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Lamp : Engine
{
    new protected string _name = "Lamp";
    public override string TileAssetReference => "Lamp";
    public override StructureType Type => StructureType.Lamp;


    #region Private data
    private Light2D _light;
    private float _outerRadius;

    public override StructureLayer Layer => StructureLayer.Utility;
    #endregion

    #region Constructor
    public Lamp(Tilemap tilemap = null, Vector3Int position = new Vector3Int()) : base(tilemap, position)
    {
        _electricityConsumption = 50.0f;
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

    public override void OnTilePlaced()
    {
        if (_tilemap == null)
            return;

        _object = _tilemap.GetInstantiatedObject(_position);

        if (_object == null)
        {
            Debug.LogWarning($"Lamp.OnTilePlaced: no instantiated object at {_position}");
            return;
        }

        _light = _object.GetComponent<Light2D>();

        if (_light == null)
        {
            Debug.LogWarning($"Lamp.OnTilePlaced: no Light2D found at {_position}");
            return;
        }

        _light.intensity = 0;
        _outerRadius = _light.pointLightOuterRadius;
    }


    public override void SetPowered(bool powered)
    {
        base.SetPowered(powered);

        if (_light == null)
            return;

        _light.intensity = powered ? 1f : 0f;
    }
    #endregion
}