using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Engine : Structure
{
    #region Public data
    public float EnginePerformance = 0.0f;
    #endregion

    #region Private data
    protected GameObject _object;
    protected float _electricityConsumption = 0.0f;
    #endregion

    #region Constructor
    public Engine(Tilemap tilemap = null, Vector3Int position = new Vector3Int())
    {
        this._type = StructureType.Engine;
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
    #endregion
}