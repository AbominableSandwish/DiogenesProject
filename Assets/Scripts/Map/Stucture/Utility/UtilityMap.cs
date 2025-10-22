using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UtilityMap : StructureMap<UtilityMap>
{
    
    public enum UtilityType
    {
        Electricity,
        Gas,
        Liquid,
        Generator,
        Accumulator,
        Device
    }

    #region Private Data

    [SerializeField] private SpriteSystem _spriteSystem;
    Dictionary<Vector3Int, Structure> structures;

    [SerializeField] private Tilemap _electric;
    [SerializeField] private List<Circuit> _circuits;
    public Dictionary<Vector3Int, Circuit> OwnerAt = new();

    private int _counterCoil = 0;
    private int _counterGenerator = 0;
    private int _counterStorage = 0;
    private int _counterEngine = 0;


    #endregion

    #region Mono

    private void Start()
    {
        _game = GameManager.Instance;
        _map = Map.Instance;

        structures = new Dictionary<Vector3Int, Structure>();
        _circuits = new List<Circuit>();

        _spriteSystem.LoadSprite();
    }
    #endregion

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

                Sprite sprite = _spriteSystem.ZeroConnect;

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
                        sprite = _spriteSystem.OneConnect[0];
                    }

                    if (upConnect)
                    {
                        sprite = _spriteSystem.OneConnect[1];
                    }

                    if (rightConnect)
                    {
                        sprite = _spriteSystem.OneConnect[2];
                    }

                    if (downConnect)
                    {
                        sprite = _spriteSystem.OneConnect[3];
                    }
                }

                if (connectCounter == 2)
                {
                    if (leftConnect && upConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[0];
                    }

                    if (leftConnect && rightConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[1];
                    }

                    if (leftConnect && downConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[2];
                    }

                    if (upConnect && downConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[3];
                    }

                    if (upConnect && rightConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[4];
                    }

                    if (rightConnect && downConnect)
                    {
                        sprite = _spriteSystem.TwoConnect[5];
                    }
                }

                if (connectCounter == 3)
                {
                    if (leftConnect && upConnect && rightConnect)
                    {
                        sprite = _spriteSystem.ThreeConnect[0];
                    }

                    if (upConnect && rightConnect && downConnect)
                    {
                        sprite = _spriteSystem.ThreeConnect[1];
                    }

                    if (leftConnect && downConnect && rightConnect)
                    {
                        sprite = _spriteSystem.ThreeConnect[2];
                    }

                    if (downConnect && leftConnect && upConnect)
                    {
                        sprite = _spriteSystem.ThreeConnect[3];
                    }
                }

                if (connectCounter == 4)
                {
                    sprite = _spriteSystem.FourConnect;
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
    public override bool AddStructure<T>(Vector3Int pos)
    {
        bool canAdd = false;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Coil):
                AddCoil(pos);
                break;
            case var cls when cls == typeof(SolarPanel):
                AddGenerator<T>(pos);
                break;
            case var cls when cls == typeof(Lamp):
                AddEngine<Lamp>(pos);
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
        tile.sprite = _spriteSystem.ZeroConnect;
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
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }

        if (neighboors.Count == 0) return true; // END 2

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);

        // Cas A: Aucun voisin → nouveau circuit indépendant
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new Dictionary<Vector3Int, Coil>();
            path.Add(position, coil);

            Circuit circuit = new Circuit(path);

            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }
            _circuits.Add(circuit);
        }

        // Cas B: Un seul circuit → ajouter la tuile au même circuit
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddStructure(position, coil);
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
        }

        return true; // END
    }

    public bool AddEngine<T>(Vector3Int position)
    {
        if (_tilemap.GetTile(position) != null || _electric.GetTile(position)) return false; // END

        Tile tile = null;
        TileBase tileBase = null;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Lamp):
                tileBase = _spriteSystem.Lamp;
                break;
        }

        //Self
        _tilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
        tile = (Tile)_tilemap.GetTile(new Vector3Int(position.x, position.y));
        tile.name = typeof(T) + _counterEngine.ToString();
        tile.colliderType = Tile.ColliderType.Grid;
        _electric.SetTile(new Vector3Int(position.x, position.y), tile);

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

        if (neighboors.Count == 0) return true; // END

        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);

       

        // Cas A: Aucun voisin → nouveau circuit indépendant
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new Dictionary<Vector3Int, Coil>();

            Circuit circuit = new Circuit(path);
            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }

            circuit.AddEngine(position, (Engine)instance);
            _circuits.Add(circuit);
        }

        // Cas B: Un seul circuit → ajouter la tuile au même circuit
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddEngine(position, (Engine)instance);
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

            newCircuit.AddEngine(position, (Engine)instance);
            _circuits.Add(newCircuit);
        }

        return true; // END
    }

    

    public bool AddGenerator<T>(Vector3Int position)
    {
        if (_tilemap.GetTile(position) != null || _electric.GetTile(position)) return false; // END

        //Self
        Tile tile = null;
        tile = new Tile();
        tile.name = typeof(T) + _counterGenerator.ToString();
        tile.sprite = _spriteSystem.SolarPanel;
        tile.colliderType = Tile.ColliderType.Grid;
        _electric.SetTile(new Vector3Int(position.x, position.y), tile);

        object[] args = { position.x, position.y };
        Generator generator = (Generator)typeof(T).Instantiate(true, args);
        structures.Add(position, generator);
        _counterGenerator++;

        //neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);
        foreach (var neighboor in neighboors)
        {
            if (structures[neighboor.Key].Type == StructureType.Coil)
                RefreshTile(neighboor.Key);
        }
        
        if (neighboors.Count == 0) return true; // END
        Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);
        
        // Cas A: Aucun voisin → nouveau circuit indépendant
        if (neighborCircuits.Count == 0)
        {
            Dictionary<Vector3Int, Coil> path = new Dictionary<Vector3Int, Coil>();
        
            Circuit circuit = new Circuit(path);
            foreach (var key in neighboors.Keys)
            {
                circuit.AddStructure(key, neighboors[key]);
            }
        
        
            circuit.AddStructure(position, generator);
            _circuits.Add(circuit);
        }
        
        // Cas B: Un seul circuit → ajouter la tuile au même circuit
        if (neighborCircuits.Count == 1)
        {
            Circuit circuit = neighborCircuits.Dequeue();
            circuit.AddStructure(position, generator);
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
        
            newCircuit.AddStructure(position, generator);
        
            _circuits.Add(newCircuit);
        }

        return true; // END
    }

    #endregion

    #region Remove
    public override bool RemoveStructure<T>(Vector3Int pos)
    {
        bool canAdd = false;
        switch (typeof(T))
        {
            case var cls when cls == typeof(Coil):
                RemoveCoil(pos);
                break;
            case var cls when cls == typeof(SolarPanel):
                RemoveGenerator(pos);
                break;
            case var cls when cls == typeof(Lamp):
                RemoveEngine(pos);
                break;
        }
        return canAdd;
    }

    public bool RemoveCoil(Vector3Int position)
    {
        if (structures[position].Type != StructureType.Coil) return false;

        Structure self = null;
        Dictionary<Vector3Int, Structure> neighboors = new Dictionary<Vector3Int, Structure>();
        bool canRemove = false;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {
            if (_electric.GetTile(new Vector3Int(position.x, position.y)) != null)
            {
                self = structures[position];

                Circuit target = null;
                foreach (Circuit circuit in _circuits)
                {
                    if (circuit.ContainsValue((Coil)self))
                    {
                        target = circuit;
                        break;
                    }
                }

                _electric.SetTile(new Vector3Int(position.x, position.y), null);
                structures.Remove(position);
                RefreshTile(position);

                if (target != null)
                {
                    //Self
                    target.RemoveCable(position);
                    //neighboor
                    neighboors = GetConnectedNeighborsIgnoring(position);

                    if (neighboors.Count > 1)
                    {
                        Split(position);
                    }

                    foreach (var neighboor in neighboors)
                    {
                        if (structures[neighboor.Key].Type == StructureType.Coil)
                            RefreshTile(neighboor.Key);
                    }

                }
                canRemove = true;
            }
        }
        return canRemove;
    }

    public bool RemoveGenerator(Vector3Int position)
    {
        if (structures[position].Type != StructureType.Generator)
            return false;

        Generator self = (Generator)structures[position];
        Dictionary<Vector3Int, Structure> neighboors = new Dictionary<Vector3Int, Structure>();
        bool canRemove = false;

        if (_electric.GetTile(new Vector3Int(position.x, position.y)) != null)
        {
            Circuit target = null;
            foreach (Circuit circuit in _circuits)
            {
                if (circuit.ContainsValue(self))
                {
                    target = circuit;
                    break;
                }
            }

            //Self
            _electric.SetTile(new Vector3Int(position.x, position.y), null);
            RefreshTile(position);
            structures.Remove(position);

            if (target != null)
            {
                target.RemoveGenerator(position);
                //neighboor
                neighboors = GetConnectedNeighborsIgnoring(position);


                foreach (var neighboor in neighboors)
                {
                    if (structures[neighboor.Key].Type == StructureType.Coil)
                        RefreshTile(neighboor.Key);
                }
            }

            canRemove = true;
        }
        return canRemove;
    }

    public bool RemoveEngine(Vector3Int position)
    {
        if (structures[position].Type != StructureType.Engine)
            return false;

        Engine self = null;
        Dictionary<Vector3Int, Structure> neighboors = new Dictionary<Vector3Int, Structure>();
        bool canRemove = false;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {
            if (_tilemap.GetTile(new Vector3Int(position.x, position.y)) != null)
            {
                self = (Engine)structures[position];

                Circuit target = null;
                foreach (Circuit circuit in _circuits)
                {
                    if (circuit.ContainsValue(self))
                    {
                        target = circuit;
                        break;
                    }
                }

                //Self
                _tilemap.SetTile(new Vector3Int(position.x, position.y), null);
                RefreshTile(position);
                structures.Remove(position);

                if (target != null)
                {
                    target.RemoveEngine(position);
                    //neighboor
                    neighboors = GetConnectedNeighborsIgnoring(position);

                    foreach (var neighboor in neighboors)
                    {
                        if (structures[neighboor.Key].Type == StructureType.Coil)
                            RefreshTile(neighboor.Key);
                    }
                }
                canRemove = true;
            }
        }
        return canRemove;
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

    List<Circuit> Split(Vector3Int position)
    {
        //neighboor
        Dictionary<Vector3Int, Structure> neighboors = GetTileNeighbor(position);
        foreach (var neighboor in neighboors)
        {
            RefreshTile(neighboor.Key);
        }

        Queue<Circuit> neighborCircuitsQ = GetCircuitNeighbors(neighboors);

        Circuit old = null;
        while (neighborCircuitsQ.Count > 0 && old == null)
            old = neighborCircuitsQ.Dequeue();

        if (old == null) return null; // rien à splitter

        // 2) Demander au circuit ses composant
        var components = old.ComputeComponentsAfterChangeData(position)
                       .OrderByDescending(c => c.Tiles.Count) // garder la plus grande
                       .ToList();

        if (components == null || components.Count <= 1)
        {
            ReindexCircuit(old);
            old.RecomputeStates();
            return null;
        }

        for (int i = 1; i < components.Count; i++)
        {
            var neo = new Circuit();
            _circuits.Add(neo);
            MoveSubset(old, neo, components[i].Tiles); // déplace tuiles +, et tes entités suivent
            neo.RecomputeStates();
            // (Tu peux aussi exploiter comps[i].Generators/Engines/Storages si tu veux optimiser)
        }

        var keep = components[0];
        RetainSubset(old, new HashSet<Vector3Int>(keep.Tiles)); // conserve la composante principale
        old.RecomputeStates();

        // 4) Ne garder dans 'old' que la composante principale
        ReindexCircuit(old);
        old.RecomputeStates();
        return _circuits;
    }

    void ReindexCircuit(Circuit c)
    {
        foreach (var p in c._coils.Keys)
            OwnerAt[p] = c;
    }

    static readonly Vector3Int[] DIRS = {
    new(0, 1, 0),  // Up
    new(1, 0, 0),  // Right
    new(0,-1, 0),  // Down
    new(-1,0, 0),  // Left
    };

    Dictionary<Vector3Int , Structure> GetConnectedNeighborsIgnoring(Vector3Int center)
    {
        var result = new Dictionary<Vector3Int, Structure>();
        for (int d = 0; d < 4; d++)
        {
            var n = center + DIRS[d];
            if (_electric.GetTile(n) != null)
                result.Add(n, structures[n]);
        }
        return result;
    }

    private Queue<Circuit> GetCircuitNeighbors(Dictionary<Vector3Int, Structure> tileNeighbor)
    {
        Queue<Circuit> neighborCircuits = new Queue<Circuit>();
        foreach (Circuit circuit in _circuits)
        {
            foreach (Structure neighboor in tileNeighbor.Values)
            {
                if (circuit.ContainsValue(neighboor))
                {
                    neighborCircuits.Enqueue(circuit);
                    break;
                }
            }
        }
        return neighborCircuits;
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

    void RetainSubset(Circuit circuit, HashSet<Vector3Int> keep)
    {
        // --- PATH (tuiles câble) ---
        foreach (var pos in circuit._coils.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._coils.Remove(pos);

        // --- CONNEXIONS ---
        foreach (var pos in circuit._connMask.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._connMask.Remove(pos);

        // --- GÉNÉRATEURS ---
        foreach (var pos in circuit._generators.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._generators.Remove(pos);

        // --- MOTEURS ---
        foreach (var pos in circuit._engines.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._engines.Remove(pos);

        // --- STOCKAGES ---
        foreach (var pos in circuit._storages.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._storages.Remove(pos);

        // --- (Optionnel) Structures globales ---
        // Si tu as _idStructures non spatialisé, tu peux soit les garder,
        // soit recalcule leur appartenance à partir des positions restantes.
        // Exemple (si tu peux les mapper à des positions connues) :
        // circuit._idStructures.RemoveWhere(id => !keep.Contains(structurePos[id]));
    }
}
