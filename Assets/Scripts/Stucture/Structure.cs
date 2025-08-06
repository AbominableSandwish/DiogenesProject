using System.Collections.Generic;
using UnityEngine;

public class Structure : Element
{
    private string _name = "";
    List<Element> _elements = null;    
    protected bool _isEnabled = false;

    public virtual bool ToPlace(Vector2Int pos)
    {
        return false;
    }
}