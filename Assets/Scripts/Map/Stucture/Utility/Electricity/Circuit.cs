using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class Circuit
{
    public List<TileBase> _path;
    public List<int> _idStructures;
    public List<Generator> _generators;
    public List<Engine> _engines;
    public List<Storage> _storages;

    public Circuit()
    {
        _path = new List<TileBase>();
        _idStructures = new List<int>();
        _generators = new List<Generator>();
        _engines = new List<Engine>();
        _storages = new List<Storage>();
    }
    public Circuit(List<TileBase> path, List<int> structures = null, List<Generator> generators = null, List<Engine> engines = null, List<Storage> storages = null)
    {
        _path = new List<TileBase>();
        _idStructures = new List<int>();
        _generators = new List<Generator>();
        _engines = new List<Engine>();
        _storages = new List<Storage>();

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

    public void Update()
    {

        //Connaitre la quantité d'energie demandé
        float total = 0.0f;
        if (_engines != null && _generators.Count != 0)
        {
            foreach (Engine engine in _engines)
            {
                total += engine._electricityConsumption;
            }
        }


        //Récupéré la production des générateur
        float Watt = 0;
        if (_generators != null && _generators.Count != 0)
        {
            foreach (Generator generator in _generators)
            {
                Watt += generator.Production();
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
                foreach (Storage storage in _storages) {
                    Watt += storage.Output(wattNeeded/_storages.Count);
                }
            }
        }

        if (_engines != null)
        {
            foreach (Engine engine in _engines)
            {
                Watt = engine.Consumption(Watt);
            }
        }
    }

    public bool Contains(Tile tile)
    {
        return _path.Contains(tile);
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

    //TODO
    //public Circuit Split()
    //{
    //    Circuit circuit;
    //    _path.AddRange(circuit._path);
    //    _structures.AddRange(circuit._structures);
    //    _generators.AddRange(circuit._generators);
    //    Enginecounter += circuit.Enginecounter;
    //    circuit = null;
    //}

    public void AddEngine(Engine engine)
    {
        _engines.Add(engine);
    }

    public void RemoveEngine(Engine engine)
    {
        _engines.Remove(engine);
    }

    public void AddGenerator(Generator generator)
    {
        _generators.Add(generator);
    }

    public void RemoveGenerator(Generator generator)
    {
        _generators.Remove(generator);
    }

    public void AddTile(TileBase tile)
    {
        _path.Add(tile);
    }
    public void RemoveTile(TileBase tile)
    {
        _path.Remove(tile);
    }
}
