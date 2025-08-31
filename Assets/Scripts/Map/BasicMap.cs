using UnityEngine;
using UnityEngine.Tilemaps;

class BasicMap : StructureMap<BasicMap>
{
    enum BasicType
    {
        Ground,
        Wall,
        Window,
        Door
    }

    private TileBase _ground;

    private void Start()
    {
        _game = GameManager.Instance;
        _map = Map.Instance;

        _ground = Resources.Load<TileBase>("Tile/Ground");

        for (int i = 0; i < _map.Height; i++)
        {
            for (int j = 0; j < _map.Width; j++)
            {
                TileBase tile = _ground;
                _tilemap.SetTile(new Vector3Int(i, j), _ground);
            }
        }
    }

    public static BasicMap Instance { get => _instance; protected set => _instance = value; }
    public override bool AddStructure<T>(Vector2Int pos     )
    {
        return false;
    }

    public override bool RemoveStructure<T>(Vector2Int pos)
    {
        return false;
    }

    override public Structure GetStructure(Vector2Int pos)
    {
        return null;
    }

    override public TileBase GetTile(Vector2Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }
}
