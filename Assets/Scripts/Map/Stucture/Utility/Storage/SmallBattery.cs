using UnityEngine;
using UnityEngine.Tilemaps;
public class SmallBattery : Storage
{
    public SmallBattery(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
          
        }
    }
}


