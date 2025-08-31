using System.Collections.Generic;
using UnityEngine;

public enum StructureType
{
    None,
    Basic,
    Utility,
    Decoration
}
public class Structure
{
    StructureType type = StructureType.None;
    private string _name = "";
    List<Element> _elements = null;    
    protected bool _isEnabled = false;

    public string Name { get => _name; }

    public Structure()
    {
        return;
    }

    public virtual bool ToPlace(Vector2Int pos)
    {
        switch (type)
        {
            case StructureType.Basic:
                break;
            case StructureType.Utility: 
                break;
            case StructureType.Decoration:
                break;
        }
        return false;
    }
}