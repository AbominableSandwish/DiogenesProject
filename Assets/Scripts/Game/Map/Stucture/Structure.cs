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

    #region Private Data
    protected bool _isEnabled = false;
    protected StructureType _type = StructureType.NONE;
    protected StructureMap _map = StructureMap.None;
    protected string _name = "";
    protected Vector3Int _position;


    #endregion

    #region Public Data
    public static string TileAssetReference = "";
    public bool IsWalkable = false;
    public bool IsClimbable = true;
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