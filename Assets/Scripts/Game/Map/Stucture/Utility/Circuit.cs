using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;




[Serializable]
public class Circuit
{

    [Flags]
    public enum Conn : byte { None = 0, Up = 1, Right = 2, Down = 4, Left = 8 }

    static readonly Vector3Int[] DIRS = {
    new(0, 1, 0),  // Up
    new(1, 0, 0),  // Right
    new(0,-1, 0),  // Down
    new(-1,0, 0),  // Left
    };


    public float Production;
    public float Consumption;
    public float TotalCapacity;
    public float Capacity;

    #region Public Data
    public Dictionary<Vector3Int, Coil> _coils;
    public HashSet<int> _idStructures;
    public Dictionary<Vector3Int, Generator> _generators;
    public Dictionary<Vector3Int, Engine> _engines;
    public Dictionary<Vector3Int, Storage> _storages;

    public Dictionary<int, Vector3Int> _position; // circuitId -> position  
    public Dictionary<Vector3Int, Conn> _connMask = new(); // position -> bitmask connexions


    #endregion

    #region Nested Method
    public Circuit()
    {
        _coils = new Dictionary<Vector3Int, Coil>();
        _idStructures = new HashSet<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();

        _connMask = new Dictionary<Vector3Int, Conn>();
    }
    public Circuit(Dictionary<Vector3Int, Coil> coils, HashSet<int> structures = null, Dictionary<Vector3Int, Generator> generators = null, Dictionary<Vector3Int, Engine> engines = null, Dictionary<Vector3Int, Storage> storages = null, Dictionary<Vector3Int, Conn> connMask = null)
    {
        _coils = new Dictionary<Vector3Int, Coil>();
        _idStructures = new HashSet<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();
        _connMask = new Dictionary<Vector3Int, Conn>();

        _coils = coils;
        if(structures != null)
            _idStructures = structures;
        if(generators != null)
            _generators = generators;
        if(engines != null)
            _engines = engines;
        if(storages != null)
            _storages = storages;

        if(connMask != null)
            _connMask = connMask;
    }
    #endregion

    #region Public Method

    public int Count()
    {
        int count = 0;
        count += _coils.Count;
        count += _engines.Count;
        count += _generators.Count;
        count += _storages.Count;
        return count;
    }

    public void Update()
    {
        float Power = Production;
        //Si quantité total d'energie insuffisant
        if (Production < Consumption)
        {
            //Si il y a du stockage
            if (_storages != null && _storages.Count != 0)
            {
                //Calculer le manquant d'energie
                Power = Production - Consumption;
                foreach (Storage storage in _storages.Values)
                {
                    Power += storage.Output(Power / _storages.Count);
                }
            }
        }

        if (_engines != null)
        {
            foreach (Engine engine in _engines.Values)
            {
                engine.Input(Power / _engines.Count);
            }
        }
    }
    public bool ContainsValue(Structure structure)
    {
        bool isContain = false;

        switch (structure.Type)
        {
            case Structure.StructureType.Coil:
                
                isContain = _coils.ContainsValue((Coil)structure);
                break;
            case Structure.StructureType.Engine:
                isContain = _engines.ContainsValue((Engine)structure);
                break;
            case Structure.StructureType.Generator:
                isContain = _generators.ContainsValue((Generator)structure);
                break;
            case Structure.StructureType.Storage:
                isContain = _storages.ContainsValue((Storage)structure);
                break;
        }
        return isContain;
    }
    public void Merge(Circuit circuit)
    {
        _coils.AddRange(circuit._coils);
        if (circuit._idStructures != null)
            _idStructures.AddRange(circuit._idStructures);
        if (circuit._generators != null)
            _generators.AddRange(circuit._generators);
        if (circuit._engines != null)
            _engines.AddRange(circuit._engines);
        if (circuit._connMask != null)
            _connMask.AddRange(circuit._connMask);

        circuit = null;
        RecomputeStates();
    }

    

    #region ADD
    public void AddCable(Vector3Int position, Coil coil)
    {
        _connMask[position] = NewConnection(position);
        _coils.Add(position, coil);
        RecomputeStates();
    }
    public void AddEngine(Vector3Int position, Engine engine)
    {      
        _engines.Add(position, engine);
        _connMask[position] = NewConnection(position);
        RecomputeStates();
    }

    public void AddGenerator(Vector3Int position, Generator generator)
    {
        _generators.Add(position, generator);
        _connMask[position] = NewConnection(position);
        RecomputeStates();
    }

    public void AddStructure(Vector3Int position, Structure structure)
    {
        switch (structure)
        {
            case Coil c:
                _coils.Add(position, c);
                break;

            case Generator g:
                _generators.Add(position, g);
                break;

            case Engine e:
                _engines.Add(position, e);
                break;

            case Storage st:
                _storages.Add(position, st);
                break;

            default:
                Debug.LogWarning($"Structure de type {structure.GetType().Name} non reconnue.");
                break;
        }

        _connMask[position] = NewConnection(position);
        RecomputeStates();
    }

    #endregion

    #region REMOVE
    public void RemoveEngine(Vector3Int position)
    {
        _engines.Remove(position);
        // Retirer la tuile
        _connMask.Remove(position);
        RecomputeStates();
    }

    public void RemoveGenerator(Vector3Int position)
    {
        _generators.Remove(position);
        // Retirer la tuile
        _connMask.Remove(position);
        RecomputeStates();
    }

    public void RemoveCable(Vector3Int position)
    {
        if (!_connMask.ContainsKey(position)) return;

        // Couper les connexions chez les voisins
        for (int d = 0; d < 4; d++)
        {
            var n = position + DIRS[d];
            if (_connMask.TryGetValue(n, out var nMask))
            {
                // Supprime la direction opposée
                Conn opposite = (Conn)(1 << ((d + 2) % 4));
                _connMask[n] = nMask & ~opposite;
            }
        }

        // Retirer la tuile
        _connMask.Remove(position);

        // Puis effectuer le split éventuel
        //SplitCircuitAfterChange(position);
        _coils.Remove(position);
        _connMask.Remove(position);
        RecomputeStates();
    }
    #endregion

    public void RecomputeStates()
    {
        Consumption = 0;
        //Connaitre la quantité d'energie demandé
        if (_engines != null && _generators.Count != 0)
        {
            foreach (Engine engine in _engines.Values)
            {
                Consumption += engine.ElectricityConsumption;
            }
        }

        Production = 0;
        //Récupéré la production des générateur
        if (_generators != null && _generators.Count != 0)
        {
            foreach (Generator generator in _generators.Values)
            {
                Production += generator.Output();
            }
        }
    }

    bool AreNeighborsConnected(Vector3Int a, int d, Vector3Int b)
    {
        if (!_connMask.TryGetValue(a, out var aMask)) return false;
        if (!_connMask.TryGetValue(b, out var bMask)) return false;

        Conn aNeed = (Conn)(1 << d);
        Conn bNeed = (Conn)(1 << ((d + 2) % 4));
        return (aMask & aNeed) != 0 && (bMask & bNeed) != 0;
    }

    Conn NewConnection(Vector3Int position)
    {
        Conn mask = Conn.None;

        for (int d = 0; d < 4; d++)
        {
            Vector3Int n = position + DIRS[d];
            if (_coils.ContainsKey(n))
            {
                // Si le voisin existe, on connecte dans les deux sens
                mask |= (Conn)(1 << d);

                Conn oppDir = (Conn)(1 << ((d + 2) % 4));
                if (_connMask.ContainsKey(n))
                    _connMask[n] |= oppDir;
                else
                    _connMask[n] = oppDir;
            }
        }
        return mask;
    }
    #endregion

    List<Vector3Int> GetConnectedNeighborsIgnoring(Vector3Int center)
    {
        // On ignore le centre (retiré/masqué), on ne fait que lister
        // les voisins qui sont encore des câbles.
        var result = new List<Vector3Int>(4);
        for (int d = 0; d < 4; d++)
        {
            var n = center + DIRS[d];
            if (_coils.ContainsKey(n)) result.Add(n);
        }
        return result;
    }

    // Dans class Circuit
    public List<ComponentData> ComputeComponentsAfterChangeData(Vector3Int posChanged)
    {
        var starts = GetConnectedNeighborsIgnoring(posChanged);
        var comps = new List<ComponentData>();
        if (starts.Count == 0) return comps;

        var visited = new HashSet<Vector3Int>();
        foreach (var s in starts)
        {
            if (visited.Contains(s)) continue;
            var comp = FloodFillComponentData(s, visited);
            if (comp.Tiles.Count > 0) comps.Add(comp);
        }
        return comps;
    }

    // ----- Résultat complet d'une composante -----
    public class ComponentData
    {
        public readonly List<Vector3Int> Tiles = new();
        public readonly Dictionary<Vector3Int, Generator> Generators = new();
        public readonly Dictionary<Vector3Int, Engine> Engines = new();
        public readonly Dictionary<Vector3Int, Storage> Storages = new();
    }

    // ----- Flood-fill qui récolte aussi les entités -----
    ComponentData FloodFillComponentData(Vector3Int start, HashSet<Vector3Int> visited)
    {
        var data = new ComponentData();
        var q = new Queue<Vector3Int>();
        visited.Add(start);
        q.Enqueue(start);

        while (q.Count > 0)
        {
            var p = q.Dequeue();
            data.Tiles.Add(p);

            // Si des entités sont posées sur cette tuile, on les ajoute
            if (_generators.TryGetValue(p, out var g)) data.Generators[p] = g;
            if (_engines.TryGetValue(p, out var e)) data.Engines[p] = e;
            if (_storages.TryGetValue(p, out var s)) data.Storages[p] = s;

            // Parcours des voisins réellement connectés
            for (int d = 0; d < 4; d++)
            {
                var n = p + DIRS[d];
                if (visited.Contains(n)) continue;
                if (!AreNeighborsConnected(p, d, n)) continue;

                visited.Add(n);
                q.Enqueue(n);
            }
        }
        return data;
    }
}
