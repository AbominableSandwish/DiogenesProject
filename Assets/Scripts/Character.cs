using System.Collections.Generic;

public class Character : Element
{
    protected int _health = 100;
    protected int _hunger = 0;

    private List<Element> _elements;
    private Element _select;

    public List<Element> Elements { get => _elements; set => _elements = value; }
    public Element Select { get => _select; set => _select = value; }

    public Character(string name)
    {
        this.Name = name;
        _elements = new List<Element>();

    }
}
