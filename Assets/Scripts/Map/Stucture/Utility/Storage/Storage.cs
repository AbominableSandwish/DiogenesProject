using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Storage : Structure
{
    protected GameObject _object;
    protected SpriteRenderer _renderer;

    public float _Capacity = 0.0f;
    public float OUT_MAX = 0.0f;
    public const float CAPACITY_MAX = 10000.0f;

    public Storage(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
            _renderer = _object.GetComponent<SpriteRenderer>();
        }
    }

    public virtual void Input(float _electricity)
    {
        _Capacity += _electricity;
        if (_Capacity > CAPACITY_MAX)
            _Capacity = CAPACITY_MAX;
    }

    public virtual float Output(float required = 0)
    {
        float collect = 0.0f;
        if (_Capacity != 0)
        {
           
            if (_Capacity >= OUT_MAX)
            {

                if (required != 0)
                {
                    if (required > OUT_MAX)
                        required = OUT_MAX;
                    collect = required;
                    _Capacity -= required;
                }
                else
                {
                    collect = OUT_MAX;
                    _Capacity -= OUT_MAX;
                }
            }

            if (_Capacity < OUT_MAX)
            {
                if (required != 0)
                {
                    if (required > _Capacity)
                        required = _Capacity;
                    collect = required;
                    _Capacity -= required;
                }
                else
                {
                    collect = _Capacity;
                    _Capacity -= _Capacity;
                }

            } 
        }
        return collect;
    }
}
