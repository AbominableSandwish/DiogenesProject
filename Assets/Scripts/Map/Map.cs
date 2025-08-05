using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

struct DTile
{
    public Structure structure;
}

public class Map
{
    private static Map _instance = null;
    private DTile[,] _tileMap;
    private int _height = 40;
    private int _width = 40;
    public TileBase ground;
    private Tilemap _tilemap;

    public static Map Instance { get => _instance; protected set => _instance = value; }
    public int Height { get => _height; set => _height = value; }
    public int Width { get => _width; set => _width = value; }

    public Map()
    {
        Instance = this;
        _tileMap = new DTile[Width, Height];
        ground = Resources.Load<TileBase>("Sprite/Ground");
    }

    public Map(Tilemap tilemap)
    {
        _instance = this;
        _tileMap = new DTile[Width, Height];
        TileBase tileBase = Resources.Load<TileBase>("Sprite/Ground");
        ground = tileBase;
        _tilemap = tilemap;

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                TileBase tile = ground;
                _tileMap[i, j].structure = null;
                _tilemap.SetTile(new Vector3Int(i, j), ground);
            }
        }
    }

    public Structure GetStructure(int x, int y)
    {
        Structure structure = null;
        if((x > -1 && x < Width) && (y > -1 && y < Height))
            structure = _tileMap[x, y].structure;
        return structure;
    } 

    public TileBase GetTile(Vector2Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
}
