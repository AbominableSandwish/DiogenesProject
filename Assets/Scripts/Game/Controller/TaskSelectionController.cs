/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TaskSelectionController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private TaskManager taskManager;

    [Header("Preview")]
    [SerializeField] private TileBase previewTile;
    [SerializeField] private Color previewColor = new(1f, 1f, 1f, 0.5f);

    [Header("Occupation Check")]
    [SerializeField] private List<Tilemap> tilemapsToCheck;
    [SerializeField] private bool blockIfOccupied = true;

    private Vector3Int _lastCell = new(int.MinValue, int.MinValue, 0);
    public TaskType SelectedTaskType { get; private set; } = TaskType.None;
    public StructureType SelectedStructureType { get; private set; } = StructureType.NONE;

    private bool _isOccupied;

    private void Awake()
    {
        previewTilemap = UnityResolver.Resolve(previewTilemap, this, "Preview Tilemap");
        mapManager = UnityResolver.Resolve(mapManager, this, "MapManager");
        taskManager = UnityResolver.Resolve(taskManager, this, "TaskManager");
    }

    private void Update()
    {
        if (SelectedStructureType == StructureType.NONE || previewTilemap == null)
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

            TryPlanBuild(SelectedStructureType, cell);
        }

        if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
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


    public void SelectTask(TaskType type)
    {
        SelectedTaskType = type;
        SelectedStructureType = StructureType.NONE;
    }

    public void SelectStructure(StructureType type)
    {
        previewTile = TileRegistry.Instance.Get(type.ToString());

        SelectedStructureType = type;
        SelectedTaskType = TaskType.Build;
    }


    public void ClearSelection()
    {
        if (previewTilemap != null)
            previewTilemap.SetTile(_lastCell, null);

        SelectedTaskType = TaskType.None;
        SelectedStructureType = StructureType.NONE;
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



    private void TryPlanBuild(StructureType type, Vector3Int cell)
    {
        Structure plannedStructure = StructureFactory.Create(type);
        if (plannedStructure == null)
        {
            Debug.LogError($"No structure mapped for type {type}", this);
            return;
        }

        ConstructionSite site = new ConstructionSite(plannedStructure);

        bool placed = mapManager.AddStructure(site, cell);
        if (!placed)
        {
            Debug.LogWarning($"Cannot plan build {type} at {cell}", this);
            return;
        }

        Task task = new Task
        {
            Type = TaskType.Build,
            Priority = TaskPriority.Normal,
            TargetPosition = cell,
            WorkPosition = FindWorkPositionNear(cell),
            StructureToBuild = plannedStructure,
            MaxWorkers = 3
        };

        taskManager.AddTask(task);

        Debug.Log($"Build task planned: {type} at {cell}");
    }

    private Vector3Int FindWorkPositionNear(Vector3Int target)
    {
        Vector3Int[] dirs =
        {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };

        foreach (Vector3Int dir in dirs)
        {
            Vector3Int pos = target + dir;

            if (!mapManager.IsInBounds(pos))
                continue;

            if (mapManager.IsWalkable(pos, StructureLayer.Basic))
                return pos;
        }

        return target;
    }
}