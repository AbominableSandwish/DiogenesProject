using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class Circuit
{
    private List<TileBase> _path;
    private List<Structure> _structures;
    private List<Generator> _generators;
    int Enginecounter;

    public Circuit()
    {
        _path = new List<TileBase>();
        _structures = new List<Structure>();
        _generators = new List<Generator>();
    }
    public Circuit(List<TileBase> path, List<Structure> structures = null, List<Generator> generators = null)
    {
        _path = new List<TileBase>();
        _structures = new List<Structure>();
        _generators = new List<Generator>();

        _path = path;
        _structures = structures;
        _generators = generators;

        if (_structures != null)
        {
            foreach (Structure structure in _structures)
            {
                if (structure is Engine)
                {
                    Enginecounter++;
                }
            }
        }

    }

    public void Update()
    {
        float Watt = 0;
        foreach (Generator generator in _generators)
        {
            Watt = generator.Production();
        }



        foreach (Structure structure in _structures)
        {
            if (structure is Engine)
            {
                ((Engine)structure).Consumption(Watt / Enginecounter);
            }
        }
    }

    public bool Contains(TileBase tile)
    {
        return _path.Contains(tile);
    }

    public void Merge(Circuit circuit)
    {
        _path.AddRange(circuit._path);
        if (circuit._structures != null)
            _structures.AddRange(circuit._structures);
        if (circuit._generators != null)
            _generators.AddRange(circuit._generators);
        Enginecounter += circuit.Enginecounter;
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

    public void AddTile(TileBase tile)
    {
        _path.Add(tile);
    }
    public void RemoveTile(TileBase tile)
    {
        _path.Remove(tile);
    }
}
