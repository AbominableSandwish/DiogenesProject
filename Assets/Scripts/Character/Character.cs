using System;
using System.Collections.Generic;

public class Character : Element
{
    protected int _health = 100;
    protected int _hunger = 0;

    private List<Structure> _structures;
    private Structure _select;

    #region Nested Method
    public Character(string name)
    {
        this.Name = name;
        _structures = new List<Structure>();
    }
    #endregion

    #region Public Method
    public List<Structure> Structures { get => _structures; set => _structures = value; }
    public Structure Select { get => _select; set => _select = value; }
    #endregion

}
