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

