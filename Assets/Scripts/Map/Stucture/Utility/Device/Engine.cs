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

    public virtual void Consumption(float _electricity)
    {
        _electricityCurrent = _electricity;
    }
}
