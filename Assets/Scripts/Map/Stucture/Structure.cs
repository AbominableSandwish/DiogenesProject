using System.Collections.Generic;
using UnityEngine;



public class Structure
{
    public enum StructureMap
    {
        None,
        Basic,
        Utility,
        Decoration
    }

    public enum StructureType
    {
        None,
        Stair,
        Ladder,
        Ground,
        Coil,
        Generator,
        Engine,
        Storage
    }

    #region Private Method
    protected bool _isEnabled = false;

    protected StructureType _type = StructureType.None;
    protected StructureMap _map = StructureMap.None;
    private string _name = "";
    protected Vector3Int _position;

    
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
    public StructureMap GetMap { get => _map; set => _map = value; }
    public StructureType Type { get => _type; }
    public Vector3Int Position { get => _position; set => _position = value; }

    public virtual bool ToPlace(Vector3Int pos)
    {
        switch (_map)
        {
            case StructureMap.Basic:
                break;
            case StructureMap.Utility: 
                break;
            case StructureMap.Decoration:
                break;
        }
        return false;
    }

    public virtual bool ToRemove(Vector3Int pos)
    {
        switch (_map)
        {
            case StructureMap.Basic:
                break;
            case StructureMap.Utility:
                break;
            case StructureMap.Decoration:
                break;
        }
        return false;
    }
    #endregion


}