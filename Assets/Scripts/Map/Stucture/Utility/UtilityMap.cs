using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Burst.Intrinsics.X86.Avx;

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
    [SerializeField] private Tilemap _electric;
    [SerializeField] private Tilemap _liquid;
    [SerializeField] private Tilemap _gas;
    [SerializeField] private List<Circuit> _circuits;
    public Dictionary<Vector3Int, Circuit> OwnerAt = new();

    private int _counterCoil = 0;
    private int _counterGenerator = 0;
    private int _counterStorage = 0;
    private int _counterEngine = 0;

    //Coil
    private Sprite _zeroConnect;
    private List<Sprite> _oneConnect;
    private List<Sprite> _twoConnect;
    private List<Sprite> _threeConnect;
    private Sprite _fourConnect;

    //Solar Panel
    private Sprite _solarPanel;

    //Storage Battery
    private TileBase _storageBattery;

    //Storage Lamp
    private TileBase _lamp;
    #endregion

    #region Mono

    private void Start()
    {
        _game = GameManager.Instance;
        _map = Map.Instance;

        _circuits = new List<Circuit>();

        LoadSprite();
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

    private void LoadSprite()
    {
        _zeroConnect = Resources.LoadAll<Sprite>("Sprite/Coils/0_connect")[0];

        _oneConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/1_connect"))
        {
            _oneConnect.Add(sprite);
        }

        _twoConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/2_connect"))
        {
            _twoConnect.Add(sprite);
        }

        _threeConnect = new List<Sprite>();
        foreach (Sprite sprite in Resources.LoadAll<Sprite>("Sprite/Coils/3_connect"))
        {
            _threeConnect.Add(sprite);
        }

        _fourConnect = Resources.LoadAll<Sprite>("Sprite/Coils/4_connect")[0];
        _solarPanel = Resources.LoadAll<Sprite>("Sprite/Generator/SolarPanel")[0];
        _lamp = Resources.LoadAll<TileBase>("Sprite/Engine/Lamp")[0];
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

                Sprite sprite = _zeroConnect;

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
                        sprite = _oneConnect[0];
                    }

                    if (upConnect)
                    {
                        sprite = _oneConnect[1];
                    }

                    if (rightConnect)
                    {
                        sprite = _oneConnect[2];
                    }

                    if (downConnect)
                    {
                        sprite = _oneConnect[3];
                    }
                }

                if (connectCounter == 2)
                {
                    if (leftConnect && upConnect)
                    {
                        sprite = _twoConnect[0];
                    }

                    if (leftConnect && rightConnect)
                    {
                        sprite = _twoConnect[1];
                    }

                    if (leftConnect && downConnect)
                    {
                        sprite = _twoConnect[2];
                    }

                    if (upConnect && downConnect)
                    {
                        sprite = _twoConnect[3];
                    }

                    if (upConnect && rightConnect)
                    {
                        sprite = _twoConnect[4];
                    }

                    if (rightConnect && downConnect)
                    {
                        sprite = _twoConnect[5];
                    }
                }

                if (connectCounter == 3)
                {
                    if (leftConnect && upConnect && rightConnect)
                    {
                        sprite = _threeConnect[0];
                    }

                    if (upConnect && rightConnect && downConnect)
                    {
                        sprite = _threeConnect[1];
                    }

                    if (leftConnect && downConnect && rightConnect)
                    {
                        sprite = _threeConnect[2];
                    }

                    if (downConnect && leftConnect && upConnect)
                    {
                        sprite = _threeConnect[3];
                    }
                }

                if (connectCounter == 4)
                {
                    sprite = _fourConnect;
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

    public bool AddEngine<T>(Vector3Int position)
    {
        Tile tile = null;

        if ((position.x > -1 || position.x < _map.Height) 
            || (position.y > -1 || position.y < _map.Width))
        {

            if (_tilemap.GetTile(new Vector3Int(position.x, position.y)) != null) return false; // END


            TileBase tileBase = null;
            switch (typeof(T))
            {
                case var cls when cls == typeof(Lamp):
                    tileBase = _lamp;
                    break;
            }

            //Self
            _tilemap.SetTile(new Vector3Int(position.x, position.y), tileBase);
            tile = (Tile)_tilemap.GetTile(new Vector3Int(position.x, position.y));
            tile.name = typeof(T) + _counterEngine.ToString();
            tile.colliderType = Tile.ColliderType.Grid;
           
            //neighboor
            Dictionary<Vector3Int, Tile> neighboors = GetConnectedNeighborsIgnoring(position);
            foreach (var neighboor in neighboors)
            {
                RefreshTile(neighboor.Key);
            }

            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);

            object[] args = { _tilemap, position.x, position.y};
            object instance = typeof(T).Instantiate(true, args);

            // Cas A: Aucun voisin → nouveau circuit indépendant
            if (neighborCircuits.Count == 0)
            {
                Dictionary<Vector3Int, Tile> path = new Dictionary<Vector3Int, Tile>();

                Circuit circuit = new Circuit(path);
                foreach (var key in neighboors.Keys)
                {
                    circuit.AddCable(key, neighboors[key]);
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
        }

        return true; // END
    }

    public bool AddGenerator<T>(Vector3Int position)
    {
        Tile tile = null;

        if ((position.x > -1 || position.x < _map.Height)
            || (position.y > -1 || position.y < _map.Width))
        {

            if (_tilemap.GetTile(position) != null) return false; // END

            _counterGenerator++;

            //Self
            tile = new Tile();
            tile.name = typeof(T) + _counterGenerator.ToString();
            tile.sprite = _solarPanel;
            tile.colliderType = Tile.ColliderType.Grid;
            _electric.SetTile(new Vector3Int(position.x, position.y), tile);

            //neighboor
            Dictionary<Vector3Int, Tile> neighboors = GetTileNeighbor(position);
            foreach (var neighboor in neighboors)
            {
                RefreshTile(neighboor.Key);
            }

            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);

            // Cas A: Aucun voisin → nouveau circuit indépendant
            if (neighborCircuits.Count == 0)
            {
                Dictionary<Vector3Int, Tile> path = new Dictionary<Vector3Int, Tile>();

                Circuit circuit = new Circuit(path);
                foreach (var key in neighboors.Keys)
                {
                    circuit.AddCable(key, neighboors[key]);
                }

                circuit.AddGenerator(position, (Generator)typeof(T).Instantiate());
                _circuits.Add(circuit);
            }

            // Cas B: Un seul circuit → ajouter la tuile au même circuit
            if (neighborCircuits.Count == 1)
            {
                Circuit circuit = neighborCircuits.Dequeue();
                circuit.AddGenerator(position, (Generator)typeof(T).Instantiate());
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

                newCircuit.AddGenerator(position, (Generator)typeof(T).Instantiate());
                _circuits.Add(newCircuit);
            }
        }

        return true; // END
    }

    List<Circuit> Split(Vector3Int position)
    {
        //neighboor
        Dictionary<Vector3Int, Tile> neighboors = GetTileNeighbor(position);
        foreach (var neighboor in neighboors)
        {
            RefreshTile(neighboor.Key);
        }

        Queue<Circuit> neighborCircuitsQ = GetCircuitNeighbors(neighboors);

        Circuit old = null;
        while (neighborCircuitsQ.Count > 0 && old == null)
            old = neighborCircuitsQ.Dequeue();

        if (old == null) return null; // rien à splitter

        // 2) Demander au circuit ses composantes restantes
        // Exemple rapide d'usage
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
        foreach (var p in c._path.Keys)
            OwnerAt[p] = c;
    }

    static readonly Vector3Int[] DIRS = {
    new(0, 1, 0),  // Up
    new(1, 0, 0),  // Right
    new(0,-1, 0),  // Down
    new(-1,0, 0),  // Left
    };

    Dictionary<Vector3Int ,Tile> GetConnectedNeighborsIgnoring(Vector3Int center)
    {
        var result = new Dictionary<Vector3Int, Tile>();
        for (int d = 0; d < 4; d++)
        {
            var n = center + DIRS[d];
            if (_electric.GetTile(n) != null)
                result.Add(n ,(Tile)_electric.GetTile(n));
        }
        return result;
    }

    private Queue<Circuit> GetCircuitNeighbors(Dictionary<Vector3Int, Tile> tileNeighbor)
    {
        Queue<Circuit> neighborCircuits = new Queue<Circuit>();
        foreach (Circuit circuit in _circuits)
        {
            foreach (Tile neighboor in tileNeighbor.Values)
            {
                if (circuit.Contains(neighboor))
                {
                    neighborCircuits.Enqueue(circuit);
                    break;
                }
            }
        }
        return neighborCircuits;
    }

    private Dictionary<Vector3Int, Tile> GetTileNeighbor(Vector3Int position)
    {
        //neighboor
        Dictionary<Vector3Int, Tile> neighboors = GetConnectedNeighborsIgnoring(position);
        return neighboors;
    }

    public bool AddCoil(Vector3Int position)
    {
        Tile tile = null;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {

            if (_electric.GetTile(position) != null) return false; // END 1

            //Self
            tile = new Tile();
            tile.name = "Coil_" + _counterCoil.ToString();
            tile.sprite = _zeroConnect;
            tile.colliderType = Tile.ColliderType.Grid;
            _electric.SetTile(new Vector3Int(position.x, position.y), tile);
            RefreshTile(position);
            _counterCoil++;

            //neighboor
            Dictionary<Vector3Int, Tile> neighboors = GetTileNeighbor(position);
            foreach (var neighboor in neighboors)
            {
                RefreshTile(neighboor.Key);
            }

            if (neighboors.Count == 0) return true; // END 2

            Queue<Circuit> neighborCircuits = GetCircuitNeighbors(neighboors);

            // Cas A: Aucun voisin → nouveau circuit indépendant
            if (neighborCircuits.Count == 0)
            {
                Dictionary<Vector3Int, Tile> path = new Dictionary<Vector3Int, Tile>();
                path.Add(position, tile);

                Circuit circuit = new Circuit(path);
                foreach(var key in neighboors.Keys)
                {
                    circuit.AddCable(key, neighboors[key]);
                }

                _circuits.Add(circuit);
            }

            // Cas B: Un seul circuit → ajouter la tuile au même circuit
            if (neighborCircuits.Count == 1)
            {
                Circuit circuit = neighborCircuits.Dequeue();
                circuit.AddCable(position, tile);
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

                newCircuit.AddCable(position, tile);
                _circuits.Add(newCircuit);
            }
        }

        return true; // END
    }

    public bool RemoveCoil(Vector3Int position)
    {
        Tile self = null;
        Dictionary<Vector3Int, Tile> neighboors = new Dictionary<Vector3Int, Tile>();
        bool canRemove = false;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {
            if (_electric.GetTile(new Vector3Int(position.x, position.y)) != null)
            {
                self = (Tile)_electric.GetTile(new Vector3Int(position.x, position.y));

                Circuit target = null;
                foreach (Circuit circuit in _circuits)
                {
                    if (circuit.Contains(self))
                    {
                        target = circuit;
                        break;
                    }
                }

                _electric.SetTile(new Vector3Int(position.x, position.y), null);
                RefreshTile(position);

                if (target != null)
                {
                    //Self
                    target.RemoveCable(position);
                    //neighboor
                    neighboors = GetConnectedNeighborsIgnoring(position);

                    if(neighboors.Count > 1)
                    {
                        Split(position);
                    }

                    foreach (var neighboor in neighboors)
                    {
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
        Tile self = null;
        Dictionary<Vector3Int, Tile> neighboors = new Dictionary<Vector3Int, Tile>();
        bool canRemove = false;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {
            if (_tilemap.GetTile(new Vector3Int(position.x, position.y)) != null)
            {
                self = (Tile)_tilemap.GetTile(new Vector3Int(position.x, position.y));

                Circuit target = null;
                foreach (Circuit circuit in _circuits)
                {
                    if (circuit.ContainsGenerator(position))
                    {
                        target = circuit;
                        break;
                    }
                }

                //Self
                _tilemap.SetTile(new Vector3Int(position.x, position.y), null);
                RefreshTile(position);

                if (target != null)
                {
                    target.RemoveGenerator(position);
                    //neighboor
                    neighboors = GetConnectedNeighborsIgnoring(position);
                  

                    foreach (var neighboor in neighboors)
                    {
                        RefreshTile(neighboor.Key);
                    }                 
                } 

                canRemove = true;
            }
        }
        return canRemove;
    }

    public bool RemoveEngine(Vector3Int position)
    {
        Tile self = null;
        Dictionary<Vector3Int, Tile> neighboors = new Dictionary<Vector3Int, Tile>();
        bool canRemove = false;

        if ((position.x > -1 || position.x < _map.Height) || (position.y > -1 || position.y < _map.Width))
        {
            if (_tilemap.GetTile(new Vector3Int(position.x, position.y)) != null)
            {
                self = (Tile)_tilemap.GetTile(new Vector3Int(position.x, position.y));

                Circuit target = null;
                foreach (Circuit circuit in _circuits)
                {
                    if (circuit.ContainsEngine(position))
                    {
                        target = circuit;
                        break;
                    }
                }

                //Self
                _tilemap.SetTile(new Vector3Int(position.x, position.y), null);
                RefreshTile(position);

                if (target != null)
                {
                    target.RemoveEngine(position);
                    //neighboor
                    neighboors = GetConnectedNeighborsIgnoring(position);
                
                    foreach (var neighboor in neighboors)
                    {
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

    void MoveSubset(Circuit from, Circuit to, List<Vector3Int> subset)
    {
        foreach (var p in subset)
        {
            if (from._path.TryGetValue(p, out var t)) { to._path[p] = t; from._path.Remove(p); OwnerAt[p] = to; }
            if (from._connMask.TryGetValue(p, out var m)) { to._connMask[p] = m; from._connMask.Remove(p); }
            if (from._generators.TryGetValue(p, out var g)) { to._generators[p] = g; from._generators.Remove(p); }
            if (from._engines.TryGetValue(p, out var e)) { to._engines[p] = e; from._engines.Remove(p); }
            if (from._storages.TryGetValue(p, out var s)) { to._storages[p] = s; from._storages.Remove(p); }
        }
    }

    void RetainSubset(Circuit circuit, HashSet<Vector3Int> keep)
    {
        // --- PATH (tuiles câble) ---
        foreach (var pos in circuit._path.Keys.Where(p => !keep.Contains(p)).ToList())
            circuit._path.Remove(pos);

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
