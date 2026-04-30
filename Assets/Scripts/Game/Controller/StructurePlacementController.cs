/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StructurePlacementController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private MapManager mapManager;

    [Header("Preview")]
    [SerializeField] private TileBase previewTile;
    [SerializeField] private Color previewColor = new(1f, 1f, 1f, 0.5f);

    [Header("Occupation Check")]
    [SerializeField] private List<Tilemap> tilemapsToCheck;
    [SerializeField] private bool blockIfOccupied = true;

    private Vector3Int _lastCell = new(int.MinValue, int.MinValue, 0);
    private StructureType? _selectedType;
    private bool _isOccupied;

    private void Awake()
    {
        previewTilemap = UnityResolver.Resolve(previewTilemap, this, "Preview Tilemap");
        mapManager = UnityResolver.Resolve(mapManager, this, "MapManager");
    }

    private void OnEnable()
    {
        if (previewTilemap == null)
            return;

        previewTilemap.ClearAllTiles();
        previewTilemap.color = previewColor;
    }

    private void OnDisable()
    {
        if (previewTilemap != null)
            previewTilemap.ClearAllTiles();
    }

    public void SetSelectedType(StructureType type)
    {
        _selectedType = type;
        previewTile = TileRegistry.Instance.Get(type.ToString());

        if (previewTile == null)
        {
            Debug.LogWarning($"No preview tile found for type {type}", this);
        }
    }

    public void ClearSelection()
    {
        _selectedType = null;

        if (previewTilemap != null)
            previewTilemap.SetTile(_lastCell, null);

        _lastCell = new Vector3Int(int.MinValue, int.MinValue, 0);
    }

    private void Update()
    {
        if (_selectedType == null || previewTilemap == null)
            return;

        Vector3Int cell = GetMouseCellPosition();

        if (cell != _lastCell)
        {
            UpdatePreview(cell);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (blockIfOccupied && _isOccupied)
                return;

            TryPlaceStructure(_selectedType.Value, cell);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    private Vector3Int GetMouseCellPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found.", this);
            return _lastCell;
        }

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -cam.transform.position.z;

        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        return previewTilemap.WorldToCell(worldPosition);
    }

    private void UpdatePreview(Vector3Int cell)
    {
        previewTilemap.SetTile(_lastCell, null);

        if (previewTile != null)
            previewTilemap.SetTile(cell, previewTile);

        _lastCell = cell;
        _isOccupied = CheckOccupied(cell);
    }

    private bool CheckOccupied(Vector3Int cell)
    {
        if (tilemapsToCheck == null || tilemapsToCheck.Count == 0)
            return false;

        foreach (Tilemap tilemap in tilemapsToCheck)
        {
            if (tilemap == null)
                continue;

            if (tilemap.GetTile(cell) != null)
                return true;
        }

        return false;
    }



    private void TryPlaceStructure(StructureType type, Vector3Int cell)
    {
        Structure structure = StructureFactory.Create(type);
        if (structure == null)
        {
            Debug.LogError($"No structure mapped for type {type}", this);
            return;
        }

        bool placed = mapManager.AddStructure(structure, cell);

        if (!placed)
        {
            Debug.LogWarning($"cannot to place {type} at {cell}", this);
            return;
        }

        // Optionnel : vider la preview après placement
        // ClearSelection();
    }
}