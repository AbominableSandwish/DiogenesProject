/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System;
using System.Collections.Generic;


[Serializable]
public class MapCellData
{
    public int x, y, z;
    public StructureType type; // "Ground", "Wall", "Ladder", etc.

    public MapCellData(int x = 0, int y = 0, int z = 0, StructureType type = StructureType.NONE)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.type = type;
    }
}

[Serializable]
public class MapData
{
    public int width;
    public int height;
    public List<MapCellData> cells = new List<MapCellData>();
}

/// <summary>
/// Wrapper (au lieu d'un Dictionary<string, MapData>) pour JsonUtility
/// </summary>
[Serializable]
public class NamedMapData
{
    public string name;   // "Basic", "Utility", "Decoration"
    public MapData data;
}

[Serializable]
public class WorldData
{
    public string version = "1.0";
    public string timestamp; // string pour JsonUtility
    public int width;
    public int height;
    public List<NamedMapData> maps = new List<NamedMapData>();
}