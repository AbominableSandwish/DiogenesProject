using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicMap : StructureMap<BasicMap>
{
    private TileRegistry _tileRegistry;
    private TileBase _ground;
    private TileBase _limit;

    #region Mono
    public override void Init(int width, int height)
    {
         Width = width; 
         Height = height;
        _game = GameManager.Instance;
        _map = MapManager.Instance;
        _tileRegistry = TileRegistry.Instance;

        structures = new Dictionary<Vector3Int, Structure>();
    }

  

    #endregion

    #region Public Method
    public Dictionary<Vector3Int, Structure> Structures { get => structures; set => structures = value; }
    public Dictionary<Vector3Int, Structure> Structures1 { get => structures; set => structures = value; }

    public override bool AddStructure(Structure structure, Vector3Int position)
    {
        if (HasStructure(position))
            return false;

        TileBase tileBase = _tileRegistry.Get(structure.TileAssetReference);
        object[] args = { Tilemap, position.x, position.y };
        structures.Add(position, structure);
        Tilemap.SetTile(position, tileBase);

        if(structures.GetType() == typeof(ConstructionSite))
        {
            SpawnConstructionView(position, (ConstructionSite)structure);
        }
        return true;
    }

    public override bool RemoveStructure(Vector3Int pos)
    {
        bool canRemove = structures.ContainsKey(pos);
        if (canRemove)
        {
            structures[pos] = null;
            Tilemap.SetTile(pos, null);
        }  
        return canRemove;
    }

    public override void Refresh()
    {
        TileBase tileBase = null;
        for (int i = -1; i <= Width - 1 ; i++)
        {
            for (int j = -1; j <= Height - 1; j++)
            {
                Vector3Int key = new Vector3Int(i, j, 0);
                structures.TryGetValue(key, out Structure value);
                if (value != null)
                {
                    string name = value.TileAssetReference;
                    tileBase = _tileRegistry.Get(name); 
                    object[] args = { Tilemap, i, j };
                    Tilemap.SetTile(key, tileBase);
                }
            }
        }
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
        return Tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }

    public bool IsWalkable(Vector3Int gridPos)
    {
        // Vérifie que le sol du dessous est solide
        Vector3Int below = new Vector3Int(gridPos.x, gridPos.y - 1, gridPos.z);

        // Structure sur la cellule du dessous
        Structure belowStruct = GetStructure(below);

        if (belowStruct == null)
            return false;

        // On peut marcher uniquement sur certains types de structure
        return belowStruct.Type == StructureType.WoodPlateform ||
               belowStruct.Type == StructureType.Ladder ||
               belowStruct.Type == StructureType.Limit;
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
        Tilemap.ClearAllTiles();
        structures.Clear();
        Circuit circuit = new Circuit();

        foreach (MapCellData cdata in data.cells)
        {
            switch ((StructureType)cdata.type)
            {
                case StructureType.WoodPlateform:
                    AddStructure(new WoodPlateform(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Ladder:
                    AddStructure(new Ladder(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Door:
                    AddStructure(new Door(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Glass:
                    AddStructure(new Glass(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Limit:
                    AddStructure(new Limit(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Ground:
                    AddStructure(new Ground(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Begin:
                    AddStructure(new Begin(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.End:
                    AddStructure(new End(), new Vector3Int(cdata.x, cdata.y, cdata.z));
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
                        list.Add(new MapCellData(x, y, 0, structure.Type));
                }
                else
                {
                   Structure structure = GetStructure(new Vector3Int(x, y));
                    if (structure != null)
                        list.Add(new MapCellData(x, y, 0, structure.Type));
                }
            }
        }
       
        return list;
    }
}