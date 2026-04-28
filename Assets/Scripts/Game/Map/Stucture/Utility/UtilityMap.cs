using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UtilityMap : StructureMap<UtilityMap>
{
    private string[] OneConnect = { "CableCoil_1", "CableCoil_2", "CableCoil_3", "CableCoil_4" };
    private string[] TwoConnect = { "CableCoil_5", "CableCoil_6", "CableCoil_7", "CableCoil_8", "CableCoil_9", "CableCoil__10" };
    private string[] ThreeConnect = { "CableCoil_11", "CableCoil_12", "CableCoil_13", "CableCoil_14" };
    private string FourConnect = "CableCoil_15";

    static readonly Vector3Int[] DIRS = {
    new(0, 1, 0),  // Up
    new(1, 0, 0),  // Right
    new(0,-1, 0),  // Down
    new(-1,0, 0),  // Left
    };

    public enum UtilityType
    {
        Electricity,
        Gas,
        Liquid,
        Generator,
        Accumulator,
        Device,
        LENGHT
    }


    #region Private Data

    private TileRegistry _tileRegistry;

    [SerializeField] private Tilemap _electric;
    [SerializeField] private List<Circuit> _circuits;
    public Dictionary<Vector3Int, Circuit> OwnerAt = new();

    private int _counterCoil = 0;
    private int _counterGenerator = 0;
    private int _counterStorage = 0;
    private int _counterEngine = 0;

    [SerializeField] private CircuitDebugRenderer _circuitDebugRenderer;
    #endregion

    #region Mono

    private void Start()
    {
        _circuitDebugRenderer = UnityResolver.Resolve(_circuitDebugRenderer, this, "CircuitDebugRenderer");
    }

    public void Init(bool Generation = false)
    {
        _game = GameManager.Instance;
        _map = MapManager.Instance;

        structures = new Dictionary<Vector3Int, Structure>();
        _circuits = new List<Circuit>();

        _tileRegistry = TileRegistry.Instance;
    }

    public void FixedUpdate()
    {
        foreach (Circuit circuit in _circuits)
        {
            circuit.Update();
        }
    }
    #endregion

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
               belowStruct.Type == StructureType.Ladder;
    }

    #region Private Method

 

    private void RefreshTile(Vector3Int pos)
    {
        bool rightConnect = false;
        bool leftConnect = false;
        bool upConnect = false;
        bool downConnect = false;

        int connectCounter = 0;
        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {
            if (_electric.GetTile(pos) != null)
            {
                if (_electric.GetTile(pos) == null)
                    return;

                
                Sprite sprite = TileRegistry.Instance.Get(new Coil().TileAssetReference).sprite;

                if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null  || Tilemap.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
                {
                    connectCounter++;
                    rightConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null || Tilemap.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
                {
                    connectCounter++;
                    leftConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null || Tilemap.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
                {
                    connectCounter++;
                    upConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null || Tilemap.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
                {
                    connectCounter++;
                    downConnect = true;
                }

                if (connectCounter == 1)
                {
                    if (leftConnect)
                    {
                        sprite = _tileRegistry.Get(OneConnect[0]).sprite;
                    }

                    if (upConnect)
                    {
                        sprite = _tileRegistry.Get(OneConnect[1]).sprite;
                    }

                    if (rightConnect)
                    {
                        sprite = _tileRegistry.Get(OneConnect[2]).sprite;
                    }

                    if (downConnect)
                    {
                        sprite = _tileRegistry.Get(OneConnect[3]).sprite;
                    }
                }

                if (connectCounter == 2)
                {
                    if (leftConnect && upConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[0]).sprite;
                    }

                    if (leftConnect && rightConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[1]).sprite;
                    }

                    if (leftConnect && downConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[2]).sprite;
                    }

                    if (upConnect && downConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[3]).sprite;
                    }

                    if (upConnect && rightConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[4]).sprite;
                    }

                    if (rightConnect && downConnect)
                    {
                        sprite = _tileRegistry.Get(TwoConnect[5]).sprite;
                    }   
                }

                if (connectCounter == 3)
                {
                    if (leftConnect && upConnect && rightConnect)
                    {
                        sprite = _tileRegistry.Get(ThreeConnect[0]).sprite;
                    }

                    if (upConnect && rightConnect && downConnect)
                    {
                        sprite = _tileRegistry.Get(ThreeConnect[1]).sprite;
                    }

                    if (leftConnect && downConnect && rightConnect)
                    {
                        sprite = _tileRegistry.Get(ThreeConnect[2]).sprite;
                    }

                    if (downConnect && leftConnect && upConnect)
                    {
                        sprite = _tileRegistry.Get(ThreeConnect[3]).sprite;
                    }
                }

                if (connectCounter == 4)
                {
                    sprite = _tileRegistry.Get(FourConnect).sprite;
                }

                Tile tile = (Tile)_electric.GetTile(pos);
                tile.sprite = sprite;
                tile.colliderType = Tile.ColliderType.Grid;
                _electric.SetTile(pos, tile);
                _electric.RefreshTile(pos);
            }
        }
    }
    #endregion


    #region Public Method

    #region Add
    public override bool AddStructure(Structure structure,Vector3Int position)
    {
        if (HasStructure(position))
            return false;

        bool canAdd = false;
        switch (structure.GetType())
        {
            case var cls when cls == typeof(Coil):
                AddCoil(position);
                canAdd = true;
                break;
            case var cls when cls == typeof(SolarPanel):
                AddGenerator<SolarPanel>(position);
                canAdd = true;
                break;
            case var cls when cls == typeof(Lamp):
                AddEngine<Lamp>(position);
                canAdd = true;
                break;
            case var cls when cls == typeof(ConstructionSite):
                // View
                structures.Add(position, structure);
                
                Tile tile = new Tile
                {
                    name = typeof(ConstructionSite) + _counterGenerator.ToString(),
                    sprite = _tileRegistry.Get(structure.TileAssetReference).sprite,
                    colliderType = Tile.ColliderType.Grid
                };
                Tilemap.SetTile(position, tile);
                Refresh();

                SpawnConstructionView(position, (ConstructionSite)structure);

                canAdd = true;
                break;
        }
        return canAdd;
    }

    public bool AddCoil(Vector3Int position)
    {
        if (Tilemap.GetTile(position) != null || _electric.GetTile(position) != null)
            return false;

        // =====================
        // Self
        // =====================
        Tile tile = new Tile
        {
            name = "Coil_" + _counterCoil.ToString(),
            sprite = _tileRegistry.Get(new Coil().TileAssetReference).sprite,
            colliderType = Tile.ColliderType.Grid
        };

        _electric.SetTile(position, tile);

        Coil coil = new Coil(position);
        structures.Add(position, coil);

        RefreshTile(position);
        _counterCoil++;

        // =====================
        // Neighbors
        // =====================
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        if (neighboors.Count == 0)
        {
            // Aucun voisin → circuit local unique
            Dictionary<Vector3Int, Coil> path = new();
            path.Add(position, coil);

            Circuit circuit = new Circuit(path);
            _circuits.Add(circuit);
            OwnerAt[position] = circuit;

            return true;
        }

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // =====================
        // Cas A: aucun circuit voisin
        // =====================
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new();
            path.Add(position, coil);

            Circuit circuit = new Circuit(path);

            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }

            _circuits.Add(circuit);
            RebuildOwnerMap(circuit);

            return true;
        }

        // =====================
        // Cas B: un seul circuit voisin
        // =====================
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddStructure(position, coil);

            OwnerAt[position] = circuit;
            return true;
        }

        // =====================
        // Cas C: plusieurs circuits voisins → merge
        // =====================
        if (neighborCircuits.Count > 1)
        {
            Circuit newCircuit = new Circuit();
            HashSet<Circuit> mergedCircuits = new();

            while (neighborCircuits.Count > 0)
            {
                Circuit toMerge = neighborCircuits.Dequeue();

                if (!mergedCircuits.Add(toMerge))
                    continue;

                _circuits.Remove(toMerge);
                newCircuit.Merge(toMerge);
            }

            newCircuit.AddStructure(position, coil);
            _circuits.Add(newCircuit);

            RebuildOwnerMap(newCircuit);

            return true;
        }


        ValidateCircuits("AddCoil");
        return true;
    }

    public bool AddEngine<T>(Vector3Int position)
    {
        if (structures.ContainsKey(position))
            return false;

        // Self
        object[] args = { Tilemap, position };
        Engine instance = (Engine)typeof(T).Instantiate(true, args);
        structures.Add(position, instance);


        Tile tile = new Tile
        {
            name = typeof(T) + _counterEngine.ToString(),
            colliderType = Tile.ColliderType.Grid
        };

        Tilemap.SetTile(position, _tileRegistry.Get(instance.TileAssetReference));

        instance.OnTilePlaced();
        _counterEngine++;

        // Neighbors
        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        // Cas 0 : engine seul => il crée son propre circuit
        if (neighboors.Count == 0)
        {
            Circuit circuit = new Circuit();
            circuit.AddEngine(position, instance);

            _circuits.Add(circuit);
            OwnerAt[position] = circuit;

            ValidateCircuits("AddEngine");
            return true;
        }

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // Cas A : voisins présents mais aucun circuit owner valide
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new();
            Circuit circuit = new Circuit(path);

            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }

            circuit.AddEngine(position, instance);
            _circuits.Add(circuit);
            RebuildOwnerMap(circuit);
        }

        // Cas B : un seul circuit voisin
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddEngine(position, instance);
            OwnerAt[position] = circuit;
        }

        // Cas C : plusieurs circuits voisins => merge
        if (neighborCircuits.Count > 1)
        {
            Circuit newCircuit = new Circuit();

            while (neighborCircuits.Count != 0)
            {
                Circuit toMerge = neighborCircuits.Dequeue();
                _circuits.Remove(toMerge);
                newCircuit.Merge(toMerge);
            }

            newCircuit.AddEngine(position, instance);
            _circuits.Add(newCircuit);
            RebuildOwnerMap(newCircuit);
        }

        ValidateCircuits("AddEngine");
        return true;
    }

    public bool AddGenerator<T>(Vector3Int position)
    {
        if (structures.ContainsKey(position))
            return false;

        // Self
        object[] args = { Tilemap, position };
        Generator generator = (Generator)typeof(T).Instantiate(true, args);
        structures.Add(position, generator);

        // View
        Tile tile = new Tile
        {
            name = typeof(T) + _counterGenerator.ToString(),
            sprite = _tileRegistry.Get(generator.TileAssetReference).sprite,
            colliderType = Tile.ColliderType.Grid
        };
        Tilemap.SetTile(position, tile);

        generator.OnTilePlaced();
        _counterGenerator++;

        // Neighbors
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        // Cas 0 : generator seul => il crée son propre circuit
        if (neighboors.Count == 0)
        {
            Circuit circuit = new Circuit();
            circuit.AddGenerator(position, generator);

            _circuits.Add(circuit);
            OwnerAt[position] = circuit;

            ValidateCircuits("AddGenerator");
            return true;
        }

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // Cas A : voisins présents mais aucun circuit owner valide
        if (neighborCircuits.Count == 0)
        {
            Circuit circuit = new Circuit();

            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }

            circuit.AddGenerator(position, generator);
            _circuits.Add(circuit);
            RebuildOwnerMap(circuit);
        }

        // Cas B : un seul circuit voisin
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddGenerator(position, generator);
            OwnerAt[position] = circuit;
        }

        // Cas C : plusieurs circuits voisins => merge
        if (neighborCircuits.Count > 1)
        {
            Circuit newCircuit = new Circuit();

            while (neighborCircuits.Count != 0)
            {
                Circuit toMerge = neighborCircuits.Dequeue();
                _circuits.Remove(toMerge);
                newCircuit.Merge(toMerge);
            }

            newCircuit.AddGenerator(position, generator);
            _circuits.Add(newCircuit);
            RebuildOwnerMap(newCircuit);
        }

        ValidateCircuits("AddGenerator");
        return true;
    }

    public bool AddStorage<T>(Vector3Int position)
    {
        _counterStorage++;
        ValidateCircuits("AddStorage");
        return false;
    }

    #endregion

    #region Remove
    public bool RemoveStructure(Structure structure, Vector3Int position)
    {
        bool canRemove = false;
        switch (structure.GetType())
        {
            case var cls when cls == typeof(Coil):
                RemoveCoil(position);
                break;
            case var cls when cls == typeof(SolarPanel):
                RemoveGenerator(position);
                break;
            case var cls when cls == typeof(Lamp):
                RemoveEngine(position);
                break;
            case var cls when cls == typeof(ConstructionSite):
                structures.Remove(position);
                // Supprimer le rendu
                Tilemap.SetTile(position, null);
                Tilemap.RefreshTile(position);
                break;
        }
        return canRemove;
    }

    // Remove Coil
    public bool RemoveCoil(Vector3Int position)
    {
        if (!structures.ContainsKey(position))
            return false;

        if (structures[position].Type != StructureType.Coil)
            return false;

        if (!OwnerAt.TryGetValue(position, out Circuit target))
            return false;

        _electric.SetTile(position, null);
        RefreshTile(position);

        structures.Remove(position);
        OwnerAt.Remove(position);

        target.RemoveCable(position);

        if (target.Count() <= 0)
        {
            _circuits.Remove(target);
            return true;
        }

        SplitFromCircuit(target, position);

        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        ValidateCircuits("RemoveCoil");
        return true;
    }

    public bool RemoveGenerator(Vector3Int position)
    {
        if (!structures.ContainsKey(position))
            return false;

        if (structures[position].Type != StructureType.Generator)
            return false;

        if (Tilemap.GetTile(position) == null)
            return false;

        if (!OwnerAt.TryGetValue(position, out Circuit target))
            return false;

        // Supprimer le rendu
        Tilemap.SetTile(position, null);
        Tilemap.RefreshTile(position);

        // Supprimer de la map globale
        structures.Remove(position);
        OwnerAt.Remove(position);

        // Supprimer du circuit
        target.RemoveGenerator(position);

        // Si le circuit est vide, on l’enlève
        if (target.Count() <= 0)
        {
            _circuits.Remove(target);
            return true;
        }

        // Recalcul du circuit restant
        target.RecomputeStates();

        // Rafraîchir les coils voisins si besoin
        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        ValidateCircuits("RemoveGenerator");
        return true;
    }

    public bool RemoveEngine(Vector3Int position)
    {
        if (!structures.ContainsKey(position))
            return false;

        if (structures[position].Type != StructureType.Engine)
            return false;

        if (Tilemap.GetTile(position) == null)
            return false;

        if (!OwnerAt.TryGetValue(position, out Circuit target))
            return false;

        // Supprimer le rendu
        Tilemap.SetTile(position, null);
        Tilemap.RefreshTile(position);

        // Supprimer de la map globale
        structures.Remove(position);
        OwnerAt.Remove(position);

        // Supprimer du circuit
        target.RemoveEngine(position);

        // Si le circuit est vide, on l’enlève
        if (target.Count() <= 0)
        {
            _circuits.Remove(target);
            return true;
        }

        // Recalcul du circuit restant
        target.RecomputeStates();

        // Rafraîchir les coils voisins si besoin
        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        ValidateCircuits("RemoveEngine");
        return true;
    }

    public bool RemoveStructure(Vector3Int pos, UtilityType type)
    {
        bool canRemove = false;
        switch (type)
        {
            case UtilityType.Electricity:
                canRemove = RemoveCoil(pos);
                break;
        }

        return canRemove;
    }
    #endregion

    private List<Circuit> SplitFromCircuit(Circuit oldCircuit, Vector3Int removedPosition)
    {
        List<Circuit> result = new();

        if (oldCircuit == null)
            return result;

        List<Circuit.ComponentData> components = oldCircuit.ComputeComponentsAfterChangeData(removedPosition);

        if (components == null || components.Count == 0)
        {
            _circuits.Remove(oldCircuit);
            return result;
        }

        // Trier du plus grand au plus petit
        components.Sort((a, b) => b.Tiles.Count.CompareTo(a.Tiles.Count));

        // Supprimer les anciens owners du circuit
        RemoveOwner(oldCircuit.GetAllPositions());

        // =====================
        // Circuit principal
        // =====================
        Circuit.ComponentData main = components[0];

        oldCircuit.Clear();

        foreach (Vector3Int pos in main.Tiles)
        {
            if (structures.TryGetValue(pos, out Structure structure) && structure is Coil coil)
                oldCircuit.AddStructure(pos, coil);
        }

        foreach (var kv in main.Generators)
            oldCircuit.AddGenerator(kv.Key, kv.Value);

        foreach (var kv in main.Engines)
            oldCircuit.AddEngine(kv.Key, kv.Value);

        foreach (var kv in main.Storages)
            oldCircuit.AddStorage(kv.Key, kv.Value);

        oldCircuit.RecomputeStates();
        RebuildOwnerMap(oldCircuit);
        result.Add(oldCircuit);

        // =====================
        // Nouveaux circuits
        // =====================
        for (int i = 1; i < components.Count; i++)
        {
            Circuit newCircuit = BuildCircuitFromComponent(components[i]);
            _circuits.Add(newCircuit);
            RebuildOwnerMap(newCircuit);
            result.Add(newCircuit);
        }

        return result;
    }

    Dictionary<Vector3Int , Structure> GetConnectedNeighborsIgnoring(Vector3Int center)
    {
        var result = new Dictionary<Vector3Int, Structure>();
        for (int d = 0; d < 4; d++)
        {
            var n = center + DIRS[d];
            if (_electric.GetTile(n) != null || Tilemap.GetTile(n) != null)
                result.Add(n, structures[n]);
        }
        return result;
    }

    private Queue<Circuit> GetCircuitNeighbors(IEnumerable<Vector3Int> neighborPositions)
    {
        HashSet<Circuit> uniqueCircuits = new();

        foreach (Vector3Int pos in neighborPositions)
        {
            if (OwnerAt.TryGetValue(pos, out Circuit circuit))
            {
                uniqueCircuits.Add(circuit);
            }
        }

        Queue<Circuit> result = new();

        foreach (Circuit circuit in uniqueCircuits)
        {
            result.Enqueue(circuit);
        }

        return result;
    }

    private Dictionary<Vector3Int, Structure> GetTileNeighbor(Vector3Int position)
    {
        //neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);
        return neighboors;
    }
    #endregion

    public CircuitData ExportCircuits()
    {
        CircuitData saveData = new CircuitData();

        foreach (var circuit in _circuits)
        {
            var cdata = new CircuitData();

            // --- Tuiles du circuit
            foreach (var tile in circuit._coils)
                cdata.coils.Add(tile.Key);

            // --- Masques de connexion
            foreach (var conn in circuit._connMask)
            {
                cdata.connections.Add(new ConnMaskData
                {
                    position = conn.Key,
                    mask = (byte)conn.Value
                });
            }

            // --- Générateurs
            foreach (var g in circuit._generators)
            {
                cdata.generators.Add(new EntityData
                {
                    position = g.Key,
                    id = g.Value.GetType().Name // ou un identifiant unique si tu en as un
                });
            }

            // --- Moteurs
            foreach (var e in circuit._engines)
            {
                cdata.engines.Add(new EntityData
                {
                    position = e.Key,
                    id = e.Value.GetType().Name
                });
            }

            // --- Stockages
            foreach (var s in circuit._storages)
            {
                cdata.storages.Add(new EntityData
                {
                    position = s.Key,
                    id = s.Value.GetType().Name
                });
            }
        }

        return saveData;
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
                    if(structure != null)
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

    private Circuit BuildCircuitFromComponent(Circuit.ComponentData component)
    {
        Circuit circuit = new Circuit();

        foreach (Vector3Int pos in component.Tiles)
        {
            if (structures.TryGetValue(pos, out Structure structure) && structure is Coil coil)
            {
                circuit.AddStructure(pos, coil);
            }
        }

        foreach (var kv in component.Generators)
            circuit.AddGenerator(kv.Key, kv.Value);

        foreach (var kv in component.Engines)
            circuit.AddEngine(kv.Key, kv.Value);

        foreach (var kv in component.Storages)
            circuit.AddStorage(kv.Key, kv.Value);

        circuit.RecomputeStates();
        return circuit;
    }

    public override Structure GetStructure(Vector3Int pos)
    {
        if (structures == null)
            return null;

        Structure structure = null;
        if(!structures.TryGetValue(pos, out structure))
        {
            structure = null;
        }
        return structure;
    }

    public override MapData Capture()
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
        _electric.ClearAllTiles();
        Tilemap.ClearAllTiles();
        structures.Clear();
        Circuit circuit = new Circuit();

        foreach (MapCellData cdata in data.cells)
        {
            switch ((StructureType)cdata.type) {
                case StructureType.SolarPanel:
                    AddStructure(new SolarPanel(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Coil:
                    AddStructure(new Coil(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
                case StructureType.Lamp:
                    AddStructure(new Lamp(), new Vector3Int(cdata.x, cdata.y, cdata.z));
                    break;
            }
        }
    }

    public override void Refresh()
    {
        TileBase tileBase = null;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector3Int key = new Vector3Int(i, j, 0);
                tileBase = _tileRegistry.Get(structures[key].TileAssetReference);
                object[] args = { Tilemap, i, j };
                Tilemap.SetTile(key, tileBase);
            }
        }
    }
    private void RemoveOwner(IEnumerable<Vector3Int> positions)
    {
        foreach (Vector3Int pos in positions)
        {
            OwnerAt.Remove(pos);
        }
    }

    private void RebuildOwnerMap(Circuit circuit)
    {
        foreach (Vector3Int pos in circuit.GetAllPositions())
        {
            OwnerAt[pos] = circuit;
        }
    }

    #region DEBUG

    private void ValidateCircuits(string context)
    {
        foreach (Circuit circuit in _circuits)
        {
            foreach (Vector3Int pos in circuit.GetAllPositions())
            {
                if (!OwnerAt.TryGetValue(pos, out Circuit owner))
                {
                    Debug.LogWarning($"[{context}] Missing owner for {pos} in circuit {circuit.Id}");
                    continue;
                }

                if (owner != circuit)
                {
                    Debug.LogWarning($"[{context}] Wrong owner for {pos}. Expected circuit {circuit.Id}, got {owner.Id}");
                }

                if (!structures.ContainsKey(pos))
                {
                    Debug.LogWarning($"[{context}] Circuit {circuit.Id} contains {pos} but structures does not.");
                }
            }
        }

        foreach (var kv in OwnerAt)
        {
            Vector3Int pos = kv.Key;
            Circuit circuit = kv.Value;

            if (!structures.ContainsKey(pos))
            {
                Debug.LogWarning($"[{context}] OwnerAt has {pos} -> circuit {circuit.Id}, but no structure exists there.");
            }

            if (!_circuits.Contains(circuit))
            {
                Debug.LogWarning($"[{context}] OwnerAt has {pos} -> missing circuit {circuit.Id} not in _circuits.");
            }
        }
    }

    #endregion
}
