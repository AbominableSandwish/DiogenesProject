using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

[System.Serializable]
public class MapCellData
{
    public int x;
    public int y;
    public int z;
    public string type;

    public MapCellData(int x = 0, int y = 0, int z = 0 , string type = "")
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.type = type;
    }
}

[System.Serializable]
public class MapData
{
    public int width;
    public int height;
    public List<MapCellData> cells;
}