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
    public static string TILE_ASSET_REFERENCE = "";
    public bool IsWalkable = false;
    public bool IsClimbable = true;
    #endregion

    #region Public Method
    public string Name { get => _name; }

    public StructureType Type { get => _type; }
    public Vector3Int Position { get => _position; set => _position = value; }

    public virtual string TileAssetReference()
    {
        return TILE_ASSET_REFERENCE;
    }

    public abstract StructureLayer Layer { get; }

    public virtual bool ToPlace(Vector3Int pos)
    {
        switch (Layer)
        {
            case StructureLayer.Basic:
                break;
            case StructureLayer.Utility: 
                break;
            case StructureLayer.Decoration:
                break;
        }
        return false;
    }

    public virtual bool ToRemove(Vector3Int pos)
    {
        switch (Layer)
        {
            case StructureLayer.Basic:
                break;
            case StructureLayer.Utility:
                break;
            case StructureLayer.Decoration:
                break;
        }
        return false;
    }
    #endregion
}