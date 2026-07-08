/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Engine : Structure
{
    #region Public data
    public float EnginePerformance = 0.0f;
    public override string TileAssetReference => "Engine";
    public override StructureLayer Layer => StructureLayer.Basic;
    public override StructureType Type => StructureType.Engine;
    #endregion



    #region Private data
    protected GameObject _object;
    protected float _electricityConsumption = 0.0f;
    protected bool _isPowered;
    #endregion

    #region Constructor
    public Engine(Tilemap tilemap = null, Vector3Int position = new Vector3Int())
    {
        this._tilemap = tilemap;
        this._position = position;

        if(tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(position);
        }
    }
    #endregion

    #region Public method
    public float ElectricityConsumption { get => _electricityConsumption; set => _electricityConsumption = value; }

    public virtual float Input(float electricity)
    { 
        EnginePerformance = electricity / _electricityConsumption;
        if (EnginePerformance > 1.0f)
            EnginePerformance = 1.0f;   
        return electricity - _electricityConsumption;
    }

    public bool IsPowered => _isPowered;

    public virtual void SetPowered(bool powered)
    {
        _isPowered = powered;
    }
    #endregion
}