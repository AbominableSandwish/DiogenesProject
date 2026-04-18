using UnityEngine;
public abstract  class Structure
{
    #region Private Data
    protected bool _isEnabled = false;
    protected StructureType _type = StructureType.NONE;
    protected string _name = "";
    protected Vector3Int _position;
    #endregion

    #region Public Data
    public abstract string TileAssetReference { get; }
    public bool IsWalkable = false;
    public bool IsClimbable = true;
    #endregion

    #region Public Method
    public string Name { get => _name; }

    public StructureType Type { get => _type; }
    public Vector3Int Position { get => _position; set => _position = value; }
    public abstract StructureLayer Layer { get; }

    public virtual bool ToPlace(Vector3Int pos)
    {
        return false;
    }

    public virtual bool ToRemove(Vector3Int pos)
    {
        return false;
    }
    #endregion
}