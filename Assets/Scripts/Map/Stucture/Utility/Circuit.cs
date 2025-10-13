using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

[Flags]
public enum Conn : byte { None = 0, Up = 1, Right = 2, Down = 4, Left = 8 }



[Serializable]
public class Circuit
{
    #region Public Data
    public Dictionary<Vector3Int,  Tile> _path;
    public HashSet<int> _idStructures;
    public Dictionary<Vector3Int, Generator> _generators;
    public Dictionary<Vector3Int, Engine> _engines;
    public Dictionary<Vector3Int, Storage> _storages;

    static readonly Vector3Int[] DIRS = {
    new(0, 1, 0),  // Up
    new(1, 0, 0),  // Right
    new(0,-1, 0),  // Down
    new(-1,0, 0),  // Left
    };

    public Dictionary<int, Vector3Int> _position; // circuitId -> position  
    private Dictionary<Vector3Int, Conn> _connMask = new(); // position -> bitmask connexions
    #endregion

    #region Nested Method
    public Circuit()
    {
        _path = new Dictionary<Vector3Int, Tile>();
        _idStructures = new HashSet<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();

        _connMask = new Dictionary<Vector3Int, Conn>();
    }
    public Circuit(Dictionary<Vector3Int, Tile> path, HashSet<int> structures = null, Dictionary<Vector3Int, Generator> generators = null, Dictionary<Vector3Int, Engine> engines = null, Dictionary<Vector3Int, Storage> storages = null, Dictionary<Vector3Int, Conn> connMask = null)
    {
        _path = new Dictionary<Vector3Int, Tile>();
        _idStructures = new HashSet<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();

        _connMask = new Dictionary<Vector3Int, Conn>();

        _path = path;
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
    public void Update()
    {
        //Connaitre la quantité d'energie demandé
        float total = 0.0f;
        if (_engines != null && _generators.Count != 0)
        {
            foreach (Engine engine in _engines.Values)
            {
                total += engine.ElectricityConsumption;
            }
        }

        //Récupéré la production des générateur
        float Watt = 0;
        if (_generators != null && _generators.Count != 0)
        {
            foreach (Generator generator in _generators.Values)
            {
                Watt += generator.Output();
            }
        }

        //Si quantité total d'energie insuffisant
        if(Watt < total)
        {
            //Si il y a du stockage
            if(_storages != null && _storages.Count != 0)
            {
                //Calculer le manquant d'energie
                float wattNeeded = total - Watt;
                foreach (Storage storage in _storages.Values) {
                    Watt += storage.Output(wattNeeded / _storages.Count);
                }
            }
        }

        if (_engines != null)
        {
            foreach (Engine engine in _engines.Values)
            {
                engine.Input(Watt / _engines.Count);
            }
        }
    }
    public bool Contains(Tile tile)
    {
        return _path.ContainsValue(tile);
    }
    public bool ContainsEngine(Vector3Int position)
    {
        return _engines.ContainsKey(position);
    }
    public bool ContainsGenerator(Vector3Int position)
    {
        return _generators.ContainsKey(position);
    }
    public void Merge(Circuit circuit)
    {
        _path.AddRange(circuit._path);
        if (circuit._idStructures != null)
            _idStructures.AddRange(circuit._idStructures);
        if (circuit._generators != null)
            _generators.AddRange(circuit._generators);
        if (circuit._engines != null)
            _engines.AddRange(circuit._engines);
        if (circuit._connMask != null)
            _connMask.AddRange(circuit._connMask);

        circuit = null;
    }
    public Tuple<Circuit> Split()
    {
        Tuple<Circuit> circuits = new Tuple<Circuit>(new Circuit());
        // TODO
        return circuits;
    }

    public void AddEngine(Vector3Int position, Engine engine)
    {
        _engines.Add(position, engine);
    }
    public void RemoveEngine(Vector3Int position)
    {
        _engines.Remove(position);
    }
    public void AddGenerator(Vector3Int position, Generator generator)
    {
        _generators.Add(position, generator);
    }
    public void RemoveGenerator(Vector3Int position)
    {
        _generators.Remove(position);
    }
    static bool AreNeighborsConnected(Conn aMask, int aDirIndex, Conn bMask)
    {
        // A doit sortir vers dir, B doit sortir vers la dir opposée
        Conn aNeed = (Conn)(1 << aDirIndex);
        Conn bNeed = (Conn)(1 << ((aDirIndex + 2) % 4));
        return (aMask & aNeed) != 0 && (bMask & bNeed) != 0;
    }
    public void AddCable(Vector3Int position, Tile tile)
    {
        Conn mask = Conn.None;

        for (int d = 0; d < 4; d++)
        {
            Vector3Int n = position + DIRS[d];
            if (_path.ContainsKey(n))
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

        _connMask[position] = mask;
        _path.Add(position, tile);
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
        _path.Remove(position);
    }
    #endregion
}
