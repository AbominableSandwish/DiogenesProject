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

    public Circuit()
    {
        _path = new List<TileBase>();
        _idStructures = new List<int>();
        _generators = new List<Generator>();
        _engines = new List<Engine>();
    }
    public Circuit(List<TileBase> path, List<int> structures = null, List<Generator> generators = null, List<Engine> engines = null)
    {
        _path = new List<TileBase>();
        _idStructures = new List<int>();
        _generators = new List<Generator>();
        _engines = new List<Engine>();

        _path = path;
        _idStructures = structures;
        _generators = generators;
        _engines = engines;
    }

    public void Update()
    {
        float Watt = 0;
        if (_generators != null)
        {
            foreach (Generator generator in _generators)
            {
                Watt += generator.Production();
            }
        }

        if (_engines != null)
        {
            foreach (Engine engine in _engines)
            {
                engine.Consumption(Watt / _engines.Count);
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
