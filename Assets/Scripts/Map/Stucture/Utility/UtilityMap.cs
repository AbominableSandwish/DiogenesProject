using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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

    private List<TileBase> GetAllNeighboors(Vector2Int origin)
    {
        Queue<Vector3Int> toSearch = new Queue<Vector3Int>();
        List<TileBase> neighboors = new List<TileBase>();
        toSearch.Enqueue(new Vector3Int(origin.x, origin.y));

        while (toSearch.Count != 0)
        {
            Vector3Int pos = toSearch.Dequeue();
            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y) + Vector3Int.right) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.right)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.right));
                    toSearch.Enqueue(pos + Vector3Int.right);
                }

            }
            if (_tilemap.GetTile(pos + Vector3Int.left) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.left)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.left));
                    toSearch.Enqueue(pos + Vector3Int.left);
                }
            }
            if (_tilemap.GetTile(pos + Vector3Int.up) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.up)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.up));
                    toSearch.Enqueue(pos + Vector3Int.up);
                }
            }
            if (_tilemap.GetTile(pos + Vector3Int.down) != null)
            {
                if (!neighboors.Contains(_tilemap.GetTile(pos + Vector3Int.down)))
                {
                    neighboors.Add(_tilemap.GetTile(pos + Vector3Int.down));
                    toSearch.Enqueue(pos + Vector3Int.down);
                }
            }
        }
        return neighboors;
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
    public bool AddEngine<T>(Vector3Int pos)
    {
        Tile tile = null;
        Dictionary<Vector3Int, Tile> neighboors = new Dictionary<Vector3Int, Tile>();

        if ((pos.x > -1 || pos.x < _map.Height) || (pos.y > -1 || pos.y < _map.Width))
        {

            if (_tilemap.GetTile(new Vector3Int(pos.x, pos.y)) != null) return false; // END



            TileBase tileBase = null;
            switch (typeof(T))
            {
                case var cls when cls == typeof(Lamp):
                    tileBase = _lamp;
                    break;
            }

            //Self
            _tilemap.SetTile(new Vector3Int(pos.x, pos.y), tileBase);
            tile = (Tile)_tilemap.GetTile(new Vector3Int(pos.x, pos.y));
            tile.name = typeof(T) + "_" + _counterEngine;

            //neighboor
            neighboors = GetConnectedNeighborsIgnoring(pos);

            foreach (var neighboor in neighboors)
            {
                RefreshTile(neighboor.Key);
            }

            object[] args = { _tilemap, pos.x, pos.y };
            object instance = typeof(T).Instantiate(true, args);
            Engine engine = (Engine)instance;

            _counterEngine++;

            if (neighboors.Count == 0) return true; // END

            Queue<Circuit> targets = new Queue<Circuit>();
            foreach (Circuit circuit in _circuits)
            {
                foreach (Tile neighboor in neighboors.Values)
                {
                    if (circuit.Contains(neighboor))
                    {
                        targets.Enqueue(circuit);
                        break;
                    }
                }
            }

            if (targets.Count == 0)
            {
                Dictionary<Vector3Int, Tile> path = new();
                path.AddRange(neighboors);

                Circuit circuit = new Circuit(path);
                circuit.AddEngine(pos, engine);
                _circuits.Add(circuit);
            }
            else
            {
                Circuit newCircuit = new Circuit();
               
                newCircuit.AddEngine(pos, engine);
                while (targets.Count != 0)
                {
                    Circuit toMerge = targets.Dequeue();
                    _circuits.Remove(toMerge);
                    newCircuit.Merge(toMerge);
                }
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

    Tuple<Circuit> Split(Vector3Int position)
    {
        //neighboor
        Dictionary<Vector3Int, Tile> neighboors = GetTileNeighbor(position);
        foreach (var neighboor in neighboors)
        {
            RefreshTile(neighboor.Key);
        }

        Queue<Circuit> neighborCircuitsQ = GetCircuitNeighbors(neighboors);

        Tuple<Circuit> circuits = new Tuple<Circuit>(new Circuit());
        // TODO
        // 3) Dédoublonner la queue -> ensemble ordonné
        var neighborCircuits = new List<Circuit>();
        var seen = new HashSet<Circuit>();
        while (neighborCircuitsQ.Count > 0)
        {
            var c = neighborCircuitsQ.Dequeue();
            if (c != null && seen.Add(c)) neighborCircuits.Add(c);
        }

        // 4) Aucun voisin : nouveau circuit (si tu ne le gères pas déjà)
        if (neighborCircuits.Count == 0)
        {
            // soit tu laisses la tuile orpheline en attendant d'autres poses
            // soit tu crées un circuit ici
            return null;
        }

        // 5) Un seul voisin : on rattache la tuile à ce circuit
        if (neighborCircuits.Count == 1)
        {
            var target = neighborCircuits[0];
            // Ajoute pos à target (si pas déjà fait par AddCable) :
            target._path[position] = target._path.ContainsKey(position) ? target._path[position] : /* ta Tile */ null;
            target._connMask[position] = target._connMask.TryGetValue(position, out var m) ? m : Conn.None;
            // (si tu maintiens un index OwnerAt, mets-le à jour)
            return null;
        }

        // 6) Plusieurs circuits : MERGE → choisir un "principal" et y fusionner les autres
        // Politique simple : garder le plus gros
        neighborCircuits = neighborCircuits
            .OrderByDescending(c => c._path.Count)
            .ToList();

        var primary = neighborCircuits[0];
        for (int i = 1; i < neighborCircuits.Count; i++)
        {
            var from = neighborCircuits[i];
            MoveAll(from, primary);
            _circuits.Remove(from); // si tu tiens une liste globale
        }

        // S’assurer que la tuile posée est bien dans le primary
        if (!primary._path.ContainsKey(position))
            primary._path[position] = /* ta Tile */ null;
        if (!primary._connMask.ContainsKey(position))
            primary._connMask[position] = Conn.None;

        ReindexCircuit(primary);   // si tu as OwnerAt
        primary.RecomputeStates();  // optionnel (powered, capacité, etc.)


        return circuits;
    }

    void ReindexCircuit(Circuit c)
    {
        foreach (var p in c._path.Keys)
            OwnerAt[p] = c;
    }

    // Déplace *tout* le contenu d’un circuit vers un autre
    void MoveAll(Circuit from, Circuit to)
    {
        foreach (var kv in from._path) to._path[kv.Key] = kv.Value;
        foreach (var kv in from._connMask) to._connMask[kv.Key] = kv.Value;
        foreach (var kv in from._generators) to._generators[kv.Key] = kv.Value;
        foreach (var kv in from._engines) to._engines[kv.Key] = kv.Value;
        foreach (var kv in from._storages) to._storages[kv.Key] = kv.Value;

        // Si tu as un index inverse
        foreach (var p in from._path.Keys) OwnerAt[p] = to;

        // Clear "from" si nécessaire
        from._path.Clear();
        from._connMask.Clear();
        from._generators.Clear();
        from._engines.Clear();
        from._storages.Clear();
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

        if ((position.x > -1 || position.x < _map.Height) 
           || (position.y > -1 || position.y < _map.Width))
        {

            if (_electric.GetTile(position) != null) return false; // END

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

            if (neighboors.Count == 0) return true; // END

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


    Circuit FindNeighborCircuit(Vector3Int posChanged)
    {
        // On inspecte les 4 voisins pour retrouver le circuit impacté
        var dirs = new Vector3Int[] { new(0, 1, 0), new(1, 0, 0), new(0, -1, 0), new(-1, 0, 0) };
        foreach (var d in dirs)
        {
            var n = posChanged + d;
            if (OwnerAt.TryGetValue(n, out var c))
                return c;
        }
        return null;
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
}
