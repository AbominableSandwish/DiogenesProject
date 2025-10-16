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
    public Engine(Tilemap tilemap = null, int pos_x = 0 , int pos_y = 0)
    {
        if(tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
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