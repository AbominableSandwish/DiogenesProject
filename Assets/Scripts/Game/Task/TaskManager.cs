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
    private readonly List<Task> _tasks = new();

    public void AddTask(Task task)
    {
        _tasks.Add(task);
    }

    public Task GetBestTask()
    {
        return _tasks
            .Where(t => !t.IsCompleted && t.CanAssignWorker())
            .OrderByDescending(t => t.Priority)
            .FirstOrDefault();
    }

    public void CompleteTask(Task task)
    {
        task.IsCompleted = true;
        _tasks.Remove(task);
    }
}