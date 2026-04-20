using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructurePlacementController : MonoBehaviour
{
    [Header("Preview")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase previewTile;
    [SerializeField] private Color previewColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("Occupation Check")]
    [SerializeField] private List<Tilemap> tilemapsToCheck;
    [SerializeField] private bool blockIfOccupied = true;

    [Header("Dependencies")]
    [SerializeField] private MapManager mapManager;

    private Vector3Int lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    private StructureType? selectedType;
    private bool isOccupied;

    private void Awake()
    {
        previewTilemap = UnityResolver.Resolve(previewTilemap, this, "Preview Tilemap");
        mapManager = UnityResolver.Resolve(mapManager, this, "MapManager");
    }

    private void OnEnable()
    {
        if (previewTilemap != null)
        {
            previewTilemap.ClearAllTiles();
            previewTilemap.color = previewColor;
        }
    }

    private void OnDisable()
    {
        if (previewTilemap != null)
            previewTilemap.ClearAllTiles();
    }

    public void SetSelectedType(StructureType type)
    {
        selectedType = type;
        previewTile = TileRegistry.Instance.Get(type.ToString());
    }

    public void ClearSelection()
    {
        selectedType = null;

        if (previewTilemap != null)
            previewTilemap.SetTile(lastCell, null);

        lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    }

    private void Update()
    {
        if (selectedType == null || previewTilemap == null)
            return;

        Vector3Int cell = GetMouseCellPosition();

        if (cell != lastCell)
        {
            previewTilemap.SetTile(lastCell, null);
            previewTilemap.SetTile(cell, previewTile);
            lastCell = cell;

            isOccupied = CheckOccupied(cell);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (blockIfOccupied && isOccupied)
                return;

            TryPlaceStructure(selectedType.Value, cell);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    private Vector3Int GetMouseCellPosition()
    {
        Camera cam = Camera.main;
        Vector3 mp = Input.mousePosition;
        mp.z = -cam.transform.position.z;
        Vector3 world = cam.ScreenToWorldPoint(mp);

        return previewTilemap.WorldToCell(world);
    }

    private bool CheckOccupied(Vector3Int cell)
    {
        foreach (Tilemap tilemap in tilemapsToCheck)
        {
            if (tilemap != null && tilemap.GetTile(cell))
                return true;
        }

        return false;
    }

    private void TryPlaceStructure(StructureType type, Vector3Int cell)
    {
        Structure structure = StructureFactory.Create(type);
        if (structure == null)
        {
            Debug.LogError($"No structure found for type {type}", this);
            return;
        }

        if (!structure.ToPlace(cell))
        {
            Debug.Log($"Cannot place {type} at {cell}", this);
            return;
        }

        mapManager.AddStructure(structure, cell);
    }
}