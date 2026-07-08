using UnityEngine;
using UnityEngine.Tilemaps;
public abstract  class Structure
{
    #region Private Data
    protected bool _isEnabled = false;
    protected Tilemap _tilemap;
    protected string _name = "";
    protected Vector3Int _position;
    #endregion

    #region Public Data
    public abstract string TileAssetReference { get; }
    public bool IsWalkable = false;
    public bool IsClimbable = true;
    public virtual bool IsTraversable => false;
    #endregion

    #region Public Method
    public string Name { get => _name; }
    public Vector3Int Position { get => _position; set => _position = value; }
    public abstract StructureLayer Layer { get; }
    public abstract StructureType Type { get; }

    public virtual void Init()
    {
    }

    public virtual void OnTilePlaced()
    {
    }

    #endregion
}