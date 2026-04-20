using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Storage : Structure
{
    protected GameObject _object;
    protected SpriteRenderer _renderer;
    protected float _capacity = 0.0f;

    public const float OUT_MAX = 0.0f;
    public const float CAPACITY_MAX = 10000.0f;

    public override string TileAssetReference => "Coil";
    public override StructureLayer Layer => StructureLayer.Utility;
    new protected StructureType _type = StructureType.Storage;

    #region Nested Method
    public Storage(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
            _renderer = _object.GetComponent<SpriteRenderer>();
        }
    }
    #endregion

    #region Public Method
    public virtual void Input(float _electricity)
    {
        _capacity += _electricity;
        if (_capacity > CAPACITY_MAX)
            _capacity = CAPACITY_MAX;
    }

    public virtual float Output(float required = 0)
    {
        float collect = 0.0f;
        if (_capacity != 0)
        {
           
            if (_capacity >= OUT_MAX)
            {

                if (required != 0)
                {
                    if (required > OUT_MAX)
                        required = OUT_MAX;
                    collect = required;
                    _capacity -= required;
                }
                else
                {
                    collect = OUT_MAX;
                    _capacity -= OUT_MAX;
                }
            }

            if (_capacity < OUT_MAX)
            {
                if (required != 0)
                {
                    if (required > _capacity)
                        required = _capacity;
                    collect = required;
                    _capacity -= required;
                }
                else
                {
                    collect = _capacity;
                    _capacity -= _capacity;
                }
            } 
        }
        return collect;
    }
    #endregion
}
