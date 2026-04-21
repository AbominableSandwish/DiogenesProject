using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Circuit;
using static Structure;

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
    #endregion

    #region Mono

    public void Init(bool Generation = false)
    {
        _game = GameManager.Instance;
        _map = MapManager.Instance;

        structures = new Dictionary<Vector3Int, Structure>();
        _circuits = new List<Circuit>();

        _tileRegistry = TileRegistry.Instance;
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

    public void FixedUpdate()
    {
        foreach(Circuit circuit in _circuits)
        {
            circuit.Update();
        }
    }

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

                if (_electric.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null  || _tilemap.GetTile(new Vector3Int(pos.x + 1, pos.y)) != null)
                {
                    connectCounter++;
                    rightConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null || _tilemap.GetTile(new Vector3Int(pos.x - 1, pos.y)) != null)
                {
                    connectCounter++;
                    leftConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null || _tilemap.GetTile(new Vector3Int(pos.x, pos.y + 1)) != null)
                {
                    connectCounter++;
                    upConnect = true;
                }

                if (_electric.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null || _tilemap.GetTile(new Vector3Int(pos.x, pos.y - 1)) != null)
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
                AddGenerator((Generator)structure, position);
                canAdd = true;
                break;
            case var cls when cls == typeof(Lamp):
                AddEngine<Lamp>(position);
                canAdd = true;
                break;
        }
        return canAdd;
    }

    public bool AddCoil(Vector3Int position)
    {
        if (_tilemap.GetTile(position) != null || _electric.GetTile(position)) return false; // END

        // Self
        Tile tile = null;
        tile = new Tile();
        tile.name = "Coil_" + _counterCoil.ToString();
        tile.sprite = _tileRegistry.Get(new Coil().TileAssetReference).sprite;
        tile.colliderType = Tile.ColliderType.Grid;
        _electric.SetTile(new Vector3Int(position.x, position.y), tile);    

        // Coil
        Coil coil = new Coil(position);
        structures.Add(position, coil);
        RefreshTile(position);
        _counterCoil++;

        // Neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);

        foreach (var neighboor in neighboors)
        {
            if (neighboor.Value.Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        if (neighboors.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new();
            path.Add(position, coil);

            Circuit circuit = new Circuit(path);
            _circuits.Add(circuit);
            OwnerAt[position] = circuit;

            return true;
        }

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // Cas A: Aucun voisin de circuit → nouveau circuit indépendant
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
            OwnerAt[position] = circuit;
        }

        // Cas B: Un seul circuit → ajouter la tuile au même circuit
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddStructure(position, coil);
            OwnerAt[position] = circuit;
        }

        // Cas C: Plusieurs circuits connectés → fusion
        if (neighborCircuits.Count > 1)
        {
            Circuit newCircuit = new Circuit();

            while (neighborCircuits.Count != 0)
            {
                Circuit toMerge = neighborCircuits.Dequeue();
                _circuits.Remove(toMerge);
                newCircuit.Merge(toMerge);
            }

            newCircuit.AddStructure(position, coil);
            _circuits.Add(newCircuit);

            // minimum immédiat
            OwnerAt[position] = newCircuit;

            // idéal ensuite : rebuild complet des owners du circuit fusionné
        }

        return true; // END
    }

    public bool AddEngine<T>(Vector3Int position)
    {
        if (structures.ContainsKey(position))
            return false; // END

        Tile tile = null;
        tile = new Tile();
        TileBase tileBase = null;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Lamp):
                tileBase = _tileRegistry.Get(Lamp.TileAssetReference); ;
                break;
        }

        //Self
        tile.name = typeof(T) + _counterEngine.ToString();
        _tilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
        tile = (Tile)_tilemap.GetTile(new Vector3Int(position.x, position.y));
        tile.colliderType = Tile.ColliderType.Grid;
        _tilemap.SetTile(new Vector3Int(position.x, position.y), tile);

        object[] args = { _tilemap, position.x, position.y };
        Engine instance = (Engine)typeof(T).Instantiate(true, args);
        structures.Add(position, instance);

        _counterEngine++;

        //neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetConnectedNeighborsIgnoring(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        if (neighboors.Count == 0)
            return true;

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // Cas A
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
            OwnerAt[position] = circuit;
        }

        // Cas B
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddEngine(position, instance);
            OwnerAt[position] = circuit;
        }

        // Cas C
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
            OwnerAt[position] = newCircuit;
        }

        return true; // END
    }

    

    public bool AddGenerator(Generator generator, Vector3Int position)
    {
        if (structures.ContainsKey(position))
            return false; // END

        //Self
        Tile tile = null;
        tile = new Tile();
       
        tile.name = generator.GetType() + _counterGenerator.ToString();
        tile.sprite = _tileRegistry.Get(generator.TileAssetReference).sprite;
        tile.colliderType = Tile.ColliderType.Grid;
        _tilemap.SetTile(new Vector3Int(position.x, position.y), tile);

        object[] args = { position.x, position.y };
        structures.Add(position, generator);
        _counterGenerator++;

        //neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);

        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        if (neighboors.Count == 0)
            return true;

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors.Keys);

        // Cas A
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new();
            Circuit circuit = new Circuit(path);

            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }

            circuit.AddStructure(position, generator);
            _circuits.Add(circuit);
            OwnerAt[position] = circuit;
        }

        // Cas B
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddStructure(position, generator);
            OwnerAt[position] = circuit;
        }

        // Cas C
        if (neighborCircuits.Count > 1)
        {
            Circuit newCircuit = new Circuit();

            while (neighborCircuits.Count != 0)
            {
                Circuit toMerge = neighborCircuits.Dequeue();
                _circuits.Remove(toMerge);
                newCircuit.Merge(toMerge);
            }

            newCircuit.AddStructure(position, generator);
            _circuits.Add(newCircuit);
            OwnerAt[position] = newCircuit;
        }

        return true; // END
    }

    public bool AddStorage<T>(Vector3Int position)
    {
        _counterStorage++;
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

        return true;
    }

    public bool RemoveGenerator(Vector3Int position)
    {
        if (!structures.ContainsKey(position))
            return false;

        if (structures[position].Type != StructureType.Generator)
            return false;

        if (_tilemap.GetTile(position) == null)
            return false;

        if (!OwnerAt.TryGetValue(position, out Circuit target))
            return false;

        // Supprimer le rendu
        _tilemap.SetTile(position, null);
        _tilemap.RefreshTile(position);

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

        return true;
    }

    public bool RemoveEngine(Vector3Int position)
    {
        if (!structures.ContainsKey(position))
            return false;

        if (structures[position].Type != StructureType.Engine)
            return false;

        if (_tilemap.GetTile(position) == null)
            return false;

        if (!OwnerAt.TryGetValue(position, out Circuit target))
            return false;

        // Supprimer le rendu
        _tilemap.SetTile(position, null);
        _tilemap.RefreshTile(position);

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

    void ReindexCircuit(Circuit c)
    {
        foreach (var p in c._coils.Keys)
            OwnerAt[p] = c;
    }



    Dictionary<Vector3Int , Structure> GetConnectedNeighborsIgnoring(Vector3Int center)
    {
        var result = new Dictionary<Vector3Int, Structure>();
        for (int d = 0; d < 4; d++)
        {
            var n = center + DIRS[d];
            if (_electric.GetTile(n) != null || _tilemap.GetTile(n) != null)
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

    void MoveSubset(Circuit from, Circuit to, List<Vector3Int> subset)
    {
        foreach (var p in subset)
        {
            if (from._coils.TryGetValue(p, out var t)) { to._coils[p] = t; from._coils.Remove(p); OwnerAt[p] = to; }
            if (from._connMask.TryGetValue(p, out var m)) { to._connMask[p] = m; from._connMask.Remove(p); }
            if (from._generators.TryGetValue(p, out var g)) { to._generators[p] = g; from._generators.Remove(p); }
            if (from._engines.TryGetValue(p, out var e)) { to._engines[p] = e; from._engines.Remove(p); }
            if (from._storages.TryGetValue(p, out var s)) { to._storages[p] = s; from._storages.Remove(p); }
        }
    }

    void MoveSubset(Circuit from, Circuit to, ComponentData comp)
    {
        foreach (var p in comp.Tiles)
        {
            if (from._coils.TryGetValue(p, out var t))
            {
                to._coils[p] = t;
                from._coils.Remove(p);
            }

            if (from._connMask.TryGetValue(p, out var m))
            {
                to._connMask[p] = m;
                from._connMask.Remove(p);
            }
        }


        foreach (var kv in comp.Generators)
        {
            to._generators[kv.Key] = kv.Value;
            from._generators.Remove(kv.Key);
        }

        foreach (var kv in comp.Engines)
        {
            to._engines[kv.Key] = kv.Value;
            from._engines.Remove(kv.Key);
        }

        foreach (var kv in comp.Storages)
        {
            to._storages[kv.Key] = kv.Value;
            from._storages.Remove(kv.Key);
        }
    }

    void RetainSubset(Circuit circuit, ComponentData keep)
    {
        var keepSet = new HashSet<Vector3Int>(keep.Tiles);

        circuit._coils = circuit._coils
            .Where(kvp => keepSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        circuit._connMask = circuit._connMask
            .Where(kvp => keepSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        circuit._generators = keep.Generators
            .Where(kvp => keepSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        circuit._engines = keep.Engines
            .Where(kvp => keepSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        circuit._storages = keep.Storages
            .Where(kvp => keepSet.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

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
        _tilemap.ClearAllTiles();
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
                object[] args = { _tilemap, i, j };
                _tilemap.SetTile(key, tileBase);
            }
        }
    }

    private HashSet<Circuit> GetNeighborCircuits(IEnumerable<Vector3Int> neighborPositions)
    {
        HashSet<Circuit> result = new();

        foreach (var pos in neighborPositions)
        {
            if (OwnerAt.TryGetValue(pos, out Circuit circuit))
                result.Add(circuit);
        }

        return result;
    }

    private void AssignOwner(Circuit circuit, IEnumerable<Vector3Int> positions)
    {
        foreach (Vector3Int pos in positions)
        {
            OwnerAt[pos] = circuit;
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
}
