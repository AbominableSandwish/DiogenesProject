/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

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

    public int AssignedWorkers;
    public int MaxWorkers = 3;
    public bool IsCompleted;

    public bool CanAssignWorker()
    {
        return AssignedWorkers < MaxWorkers;
    }
}

