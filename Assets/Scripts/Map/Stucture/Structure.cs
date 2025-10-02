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
    #region Private Method
    protected bool _isEnabled = false;

    protected StructureType _type = StructureType.None;
    private string _name = "";
    private List<Element> _elements = null;
    #endregion

    #region Nested Method
    public Structure()
    {
        return;
    }
    #endregion

    #region Public Method
    public string Name { get => _name; }
    public StructureType Type { get => _type; set => _type = value; }

    public virtual bool ToPlace(Vector3Int pos)
    {
        switch (_type)
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

    public virtual bool ToRemove(Vector3Int pos)
    {
        switch (_type)
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
    #endregion
}