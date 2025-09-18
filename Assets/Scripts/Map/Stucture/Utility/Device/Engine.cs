using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Engine : Structure
{
    protected GameObject _object;

    public float _electricityConsumption = 0.0f;
    public float _electricityCurrent = 0.0f;
    public float EnginePerformance = 0.0f;

    public Engine(Tilemap tilemap = null, int pos_x = 0 , int pos_y = 0)
    {
        if(tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
        }
    }

    public virtual float Consumption(float electricity)
    {
        float _electricity = electricity;
        if(_electricity >= _electricityConsumption)
        {
            _electricityCurrent = _electricityConsumption;
            _electricity = _electricity - _electricityConsumption;
            EnginePerformance = 1.0f;
        }

        if(_electricity < _electricityConsumption)
        {
            _electricityCurrent = _electricity;
            _electricity = 0;
            EnginePerformance = _electricityCurrent / _electricityConsumption;
        }

        return _electricity;
    }
}
