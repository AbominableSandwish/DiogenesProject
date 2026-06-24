/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.Tilemaps;
public class SmallBattery : Storage
{

    #region Nested Method
    public SmallBattery(Tilemap tilemap = null, int pos_x = 0, int pos_y = 0)
    {
        if (tilemap != null)
        {
            _object = tilemap.GetInstantiatedObject(new Vector3Int(pos_x, pos_y));
          
        }
    }
    #endregion
}


