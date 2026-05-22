/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;

public enum TaskType
{
    None,
    Build,
    Gather,
    Repair,
    Transport
}

public class Task
{
    public TaskType Type;

    [Range(1, 9)]
    public int Priority;

    public Vector3Int TargetPosition; 
    public Structure StructureToBuild;

    public int MaxWorkers = 3;
    public bool IsCompleted;

    public bool IsAssigned { get; private set; }
    private readonly List<VillagerWorker> assignedWorkers = new();

    public float RetryAfterTime { get; private set; }
    public bool CanBeRetried => Time.time >= RetryAfterTime;

    public void MarkTemporaryUnreachable(float delay)
    {
        RetryAfterTime = Time.time + delay;
    }

    public bool CanAssignWorker()
    {
        return assignedWorkers.Count < MaxWorkers;
    }

    public readonly Dictionary<VillagerWorker, Vector3Int> reservedWorkPositions = new();

    public bool TryReserveWorkPosition(
        VillagerWorker worker,
        Vector3Int taskPosition,
        int range,
        MapManager mapManager,
        out Vector3Int workPosition)
    {
        workPosition = default;

        if (reservedWorkPositions.ContainsKey(worker))
        {
            workPosition = reservedWorkPositions[worker];
            return true;
        }

        List<Vector3Int> candidates = new();

        for (int x = -range; x <= range; x++)
{
    for (int y = -range; y <= range; y++)
    {
        Vector3Int pos = taskPosition + new Vector3Int(x, y, 0);

        if (reservedWorkPositions.ContainsValue(pos))
            continue;

        if (pos != taskPosition && !mapManager.IsWalkable(pos, StructureLayer.Basic))
            continue;

        candidates.Add(pos);
    }
}

        if (candidates.Count == 0)
            return false;

        candidates.Sort((a, b) =>
            Vector3Int.Distance(a, new Vector3Int((int)worker.transform.position.x, (int)worker.transform.position.y))
                .CompareTo(Vector3Int.Distance(b, new Vector3Int((int)worker.transform.position.x, (int)worker.transform.position.y)))
        );

        workPosition = candidates[0];
        reservedWorkPositions[worker] = workPosition;
        return true;
    }

    public Vector3Int GetPositionReserved(VillagerWorker worker)
    {
        return reservedWorkPositions[worker];
    }
    public void ReleaseWorkPosition(VillagerWorker worker)
    {
        reservedWorkPositions.Remove(worker);
    }

    private bool IsReserved(Vector3Int position)
    {
        return reservedWorkPositions.ContainsValue(position);
    }
    public void AssignTo(VillagerWorker worker)
    {
        if (worker == null)
            return;

        if (assignedWorkers.Contains(worker))
            return;

        if (!CanAssignWorker())
            return;

        assignedWorkers.Add(worker);
    }
    public void Release(VillagerWorker worker)
    {
        if (worker == null)
            return;

        assignedWorkers.Remove(worker);
    }
}

