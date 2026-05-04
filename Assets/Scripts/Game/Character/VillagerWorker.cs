/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using UnityEngine;

public class VillagerWorker : MonoBehaviour
{
    [SerializeField] private Villager villager;
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private MapManager mapManager;

    [SerializeField] private float workSpeed = 10f;

    private Task currentTask;
    private Coroutine workRoutine;

    private void Awake()
    {
        villager = UnityResolver.Resolve(villager, this, nameof(Villager));
        taskManager = UnityResolver.Resolve(taskManager, this, nameof(TaskManager));
        mapManager = UnityResolver.Resolve(mapManager, this, nameof(MapManager));
    }

    private void Update()
    {
        if (currentTask != null)
            return;

        Task task = taskManager.GetBestTask();
        if (task == null)
            return;

        AssignTask(task);
    }

    private void AssignTask(Task task)
    {
        currentTask = task;
        currentTask.AssignedWorkers++;

        villager.MoveTo(currentTask.WorkPosition, OnArrived);
    }

    public void OnArrived()
    {
        if (currentTask == null)
            return;

        if (currentTask.Type == TaskType.Build)
        {
            Structure structure = mapManager.GetStructure(
                currentTask.TargetPosition,
                currentTask.StructureToBuild.Layer
            );

            if (structure is ConstructionSite site)
            {
                workRoutine = StartCoroutine(WorkOnConstruction(site));
            }
        }
    }

    private IEnumerator WorkOnConstruction(ConstructionSite site)
    {
        while (currentTask != null && !site.IsCompleted)
        {
            site.AddWork(workSpeed * Time.deltaTime);
            yield return null;
        }

        if (currentTask != null && site.IsCompleted)
            FinishConstruction(site);
    }

    private void FinishConstruction(ConstructionSite site)
    {
        taskManager.CompleteTask(currentTask);

        currentTask = null;
        workRoutine = null;
    }

#if UNITY_INCLUDE_TESTS
    public void InitForTests(MapManager manager)
    {
        mapManager = manager;
    }

    public void SetCurrentTaskForTests(Task task)
    {
        currentTask = task;
    }
#endif
}