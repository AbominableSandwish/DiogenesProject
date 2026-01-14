using UnityEngine;
using UnityEngine.Tilemaps;
using static Structure;
class DecorationMap : StructureMap<DecorationMap>
{
    #region Nested Method
    public DecorationMap(int height, int width)
    {
    }
    #endregion

    #region Public Method
    public override bool AddStructure<T>(Vector3Int pos)
    {
        return false;
    }

    public override bool RemoveStructure<T>(Vector3Int pos)
    {
        return false;
    }

    override public Structure GetStructure(Vector3Int pos)
    {
        return null;
    }

    override public TileBase GetTile(Vector3Int position)
    {
        return _tilemap.GetTile(new Vector3Int(position.x, position.y, 0));
    }

    public bool IsWalkable(Vector3Int gridPos)
    {
        // Vérifie que le sol du dessous est solide
        Vector3Int below = new Vector3Int(gridPos.x, gridPos.y, gridPos.z - 1);

        // Si on est au niveau du sol (z == 0), c’est automatiquement praticable
        if (gridPos.z == 0)
            return true;

        // Structure sur la cellule du dessous
        Structure belowStruct = GetStructure(below);

        if (belowStruct == null)
            return false;

        // On peut marcher uniquement sur certains types de structure
        return belowStruct.Type == StructureType.WoodPlateform ||
               belowStruct.Type == StructureType.Ladder;
    }
    #endregion
}

