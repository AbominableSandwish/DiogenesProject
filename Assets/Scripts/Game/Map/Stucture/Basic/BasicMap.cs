using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Structure;


class BasicMap : StructureMap<BasicMap>
{
    private TileBase _ground;
    private TileBase _limit;

    #region Mono
    private void Start()
    {
        _game = GameManager.Instance;
        _map = MapManager.Instance;

        _limit = Resources.Load<TileBase>("Tile/Basic/Limit");
        _ground = Resources.Load<TileBase>("Tile/Basic/Ground");

        for (int i = -1 ; i <= _map.Width;  i++)
        {
            for (int j = -1; j <= _map.Height; j++)
            {
                if (i == -1 || j == -1 || i == _map.Width)
                {
                    TileBase tile = _limit;
                    _tilemap.SetTile(new Vector3Int(i, j), _limit);
                }
                else
                {
                    TileBase tile = _ground;
                    _tilemap.SetTile(new Vector3Int(i, j), _ground);
                }
            }
        }
    }
    #endregion

    #region Public Method
    public static BasicMap Instance { get => _instance; protected set => _instance = value; }
    public override bool AddStructure<T>(Vector3Int pos     )
    {
        return false;
    }

    public override bool RemoveStructure<T>(Vector3Int pos)
    {
        return false;
    }

    override public Structure GetStructure(Vector3Int pos)
    {
        return null;
    }

    override public TileBase GetTile(Vector3Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }

    public bool IsWalkable(Vector3Int gridPos)
    {
        // Vérifie que le sol du dessous est solide
        Vector3Int below = new Vector3Int(gridPos.x, gridPos.y, gridPos.z - 1);

        // Si on est au niveau du sol (z == 0), c’est automatiquement praticable
        if (gridPos.z == 0)
            return true;

        // Structure sur la cellule du dessous
        Structure belowStruct = GetStructure(below);

        if (belowStruct == null)
            return false;

        // On peut marcher uniquement sur certains types de structure
        return belowStruct.Type == StructureType.Ground ||
               belowStruct.Type == StructureType.Stair ||
               belowStruct.Type == StructureType.Ladder;
    }
    #endregion

    public override MapData Capture()  // si tu as virtual dans la base
    {
        MapData data = new MapData
        {
            width = _map.Width,
            height = _map.Height,
            cells = CollectCells() // -> fabrique la liste MapCellData depuis ta grille/structures
        };

        return data;
    }

    public override void Restore(MapData data)
    {
       
    }

    private List<MapCellData> CollectCells()
    {
        var list = new List<MapCellData>();
        // Parcours ta tilemap/structures et remplis list
        for (int x = -1; x <= _map.Width; x++)
        {
            for (int y = -1; y <= _map.Height; y++)
            {
                if (x == -1 || y == -1 || x == _map.Width)
                {
                    Structure structure = GetStructure(new Vector3Int(x, y));
                    if (structure != null)
                        list.Add(new MapCellData(x, y, 0, (int)structure.Type));
                }
                else
                {
                   Structure structure = GetStructure(new Vector3Int(x, y));
                    if (structure != null)
                        list.Add(new MapCellData(x, y, 0, (int)structure.Type));
                }
            }
        }
       
        return list;
    }
}
