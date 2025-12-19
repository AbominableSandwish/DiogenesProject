using System;
using System.Collections.Generic;

public class Character : Element
{
    protected int _health = 100;
    protected int _hunger = 0;

    private List<Structure> _structures;

    #region Nested Method
    public Character(string name)
    {
        this.Name = name;
    }
    #endregion

    #region Public Method
    public List<Structure> Structures { get => _structures; set => _structures = value; }
    #endregion

}
