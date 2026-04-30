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
public enum TaskPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

public class Task
{
    public TaskType Type;
    public TaskPriority Priority;

    public Vector3Int TargetPosition;
    public Vector3Int WorkPosition;

    public Structure StructureToBuild;

    public int AssignedWorkers;
    public int MaxWorkers = 3;
    public bool IsCompleted;

    public bool CanAssignWorker()
    {
        return AssignedWorkers < MaxWorkers;
    }
}