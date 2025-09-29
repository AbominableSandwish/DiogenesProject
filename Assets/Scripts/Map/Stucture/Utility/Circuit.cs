using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Circuit
{

    #region Public Data
    public Dictionary<Vector3Int,  Tile> _path;
    public List<int> _idStructures;
    public Dictionary<Vector3Int, Generator> _generators;
    public Dictionary<Vector3Int, Engine> _engines;
    public Dictionary<Vector3Int, Storage> _storages;
    #endregion

    #region Nested Method
    public Circuit()
    {
        _path = new Dictionary<Vector3Int, Tile>();
        _idStructures = new List<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();
    }
    public Circuit(Dictionary<Vector3Int, Tile> path, List<int> structures = null, Dictionary<Vector3Int, Generator> generators = null, Dictionary<Vector3Int, Engine> engines = null, Dictionary<Vector3Int, Storage> storages = null)
    {
        _path = new Dictionary<Vector3Int, Tile>();
        _idStructures = new List<int>();
        _generators = new Dictionary<Vector3Int, Generator>();
        _engines = new Dictionary<Vector3Int, Engine>();
        _storages = new Dictionary<Vector3Int, Storage>();

        _path = path;
        if(structures != null)
            _idStructures = structures;
        if(generators != null)
            _generators = generators;
        if(engines != null)
            _engines = engines;
        if(storages != null)
            _storages = storages;
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

    public void Merge(Circuit circuit)
    {
        _path.AddRange(circuit._path);
        if (circuit._idStructures != null)
            _idStructures.AddRange(circuit._idStructures);
        if (circuit._generators != null)
            _generators.AddRange(circuit._generators);
        if (circuit._engines != null)
            _engines.AddRange(circuit._engines);
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

    public void AddTile(Vector3Int position, Tile tile)
    {
        _path.Add(position, tile);
    }
    public void RemoveTile(Vector3Int position)
    {
        _path.Remove(position);
    }
    #endregion
}
