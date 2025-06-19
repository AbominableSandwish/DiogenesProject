using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile
{
    private int _floor = 0;
    private List<Structure> _items;
   
    void SetFloor(int floor)
    {
        this._floor = floor;
    }
} 

public class Map
{
    private int _height = 40;
    private int _width = 40;

    private List<Tile> _tilesGround;
    private List<Tile> _tilesWall;
    public TileBase ground;
    public TileBase wall;

    private Tilemap _tilemapWall;

    public int Height { get => _height; set => _height = value; }
    public int Width { get => _width; set => _width = value; }

    public Map()
    {
        ground = Resources.Load<TileBase>("Sprite/Ground");
        _tilesGround = new List<Tile>();
        
        for(int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                _tilesGround.Add(new Tile());
            }
        }
    }

    public Map(Tilemap tilemapGround, Tilemap tilemapWall)
    {
        _tilemapWall = tilemapWall;

        ground = Resources.Load<TileBase>("Sprite/Ground");
        wall = Resources.Load<TileBase>("Sprite/Wall");
        _tilesGround = new List<Tile>();

        for (int i = -1; i <= _height; i++)
        {
            for (int j = -1; j <= _width; j++)
            {
                if ((i < 0 || i > _height - 1) || (j < 0 || j > _width - 1))
                {
                    _tilemapWall.SetTile(new Vector3Int(i, j), wall);
                }
            }
        }

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                tilemapGround.SetTile(new Vector3Int(i, j), ground);
            }
        }
    }

    public TileBase GetTile(Vector2Int position)
    {
        return _tilemapWall.GetTile(new Vector3Int(position.x, position.y, 0));
    }
}
