using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Structure;
using static UnityEngine.Rendering.GPUSort;


class BasicMap : StructureMap<BasicMap>
{
    private TileRegistry _tileRegistry;
    private TileBase _ground;
    private TileBase _limit;

    Dictionary<Vector3Int, Structure> structures;

    #region Mono
    public void Init(bool Generation = false)
    {
        _game = GameManager.Instance;
        _map = MapManager.Instance;
        _tileRegistry = TileRegistry.Instance;

        structures = new Dictionary<Vector3Int, Structure>();

        if (Generation)
            NewMap();
    }

    public void NewMap()
    {
        _limit = _tileRegistry.Get(Limit.TileAssetReference);
        _ground = _tileRegistry.Get(Ground.TileAssetReference);

        for (int i = -1; i <= _map.Width; i++)
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
    public override bool AddStructure<T>(Vector3Int position)
    {
        TileBase tileBase = null;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Ground):
                tileBase = _tileRegistry.Get(Ground.TileAssetReference);
              
                break;
            case var cls when cls == typeof(Limit):
                tileBase = _tileRegistry.Get(Limit.TileAssetReference);
                break;
            case var cls when cls == typeof(WoodPlateform):
                tileBase = _tileRegistry.Get(WoodPlateform.TileAssetReference);
                break;
            case var cls when cls == typeof(Ladder):
                tileBase = _tileRegistry.Get(Ladder.TileAssetReference);
                break;
            case var cls when cls == typeof(Door):
                tileBase = _tileRegistry.Get(Door.TileAssetReference);
                break;
            case var cls when cls == typeof(Glass):
                tileBase = _tileRegistry.Get(Glass.TileAssetReference);
                break;
        }

        object[] args = { _tilemap, position.x, position.y };
        Structure instance = (Structure)typeof(T).Instantiate(true, args);
        structures.Add(position, instance);
        _tilemap.SetTile(position, tileBase);

        return false;
    }

    public override bool RemoveStructure<T>(Vector3Int pos)
    {
        bool canRemove = structures.ContainsKey(pos);
        if (canRemove)
        {
            structures.Remove(pos);   
        }  
        return canRemove;
    }

    override public Structure GetStructure(Vector3Int pos)
    {
        if (structures == null)
            return null;

        Structure structure = null;
        if (!structures.TryGetValue(pos, out structure))
        {
            structure = null;
        }
        return structure;
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
        return belowStruct.Type == StructureType.WoodPlateform ||
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
        Tilemap tilemap = GetComponent<Tilemap>();
        _tilemap.ClearAllTiles();
        Circuit circuit = new Circuit();
        NewMap();

        foreach (MapCellData cdata in data.cells)
        {
            switch ((StructureType)cdata.type)
            {
                case StructureType.WoodPlateform:
                    AddStructure<WoodPlateform>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Ladder:
                    AddStructure<Ladder>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Door:
                    AddStructure<Door>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Glass:
                    AddStructure<Glass>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Limit:
                    AddStructure<Limit>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Ground:
                    AddStructure<Ground>(new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
            }
        }
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
