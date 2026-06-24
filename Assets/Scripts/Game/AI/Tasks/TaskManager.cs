/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private readonly List<VillagerTask> _tasks = new();

    public List<VillagerTask> GetTasks()
    {
        return _tasks;
    }

    public void AddTask(VillagerTask task)
    {
        _tasks.Add(task);
    }


    public VillagerTask GetBestTask(Vector3Int workerPosition, HashSet<VillagerTask> ignoredTasks = null)
    {
        return _tasks
            .Where(t => t.CanAssignWorker())
            .Where(t => ignoredTasks == null || !ignoredTasks.Contains(t))
            .Where(t => t.CanAssignWorker())
            .Where(t => t.CanBeRetried)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => GridDistance(workerPosition, t.TargetPosition))
            .FirstOrDefault();
    }

    private int GridDistance(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return Mathf.Max(dx, dy);
    }

    public void CompleteTask(VillagerTask task)
    {
        task.IsCompleted = true;
        _tasks.Remove(task);
    }

}