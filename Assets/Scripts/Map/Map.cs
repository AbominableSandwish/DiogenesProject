using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

struct DTile
{
    public Structure structure;
}

public class Map : MonoBehaviour
{
    #region Private Data
    private static Map _instance = null;
    private DTile[,] _tileMap;
    private int _height = 40;
    private int _width = 40;
    private TileBase _ground;
    private Tilemap _tilemap;
    #endregion

    #region Public Data
    public static Map Instance { get => _instance; protected set => _instance = value; }
    public int Height { get => _height; set => _height = value; }
    public int Width { get => _width; set => _width = value; }
    #endregion

    #region Mono
    public void Awake()
    {
        Instance = this;
        _tileMap = new DTile[Width, Height];
        _ground = Resources.Load<TileBase>("Tile/Ground");

        _tilemap = GetComponent<Tilemap>();

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                TileBase tile = _ground;
                _tileMap[i, j].structure = null;
                _tilemap.SetTile(new Vector3Int(i, j), _ground);
            }
        }
    }
    #endregion

    #region Public Method
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
    #endregion
}
