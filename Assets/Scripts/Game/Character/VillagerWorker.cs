/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using UnityEngine;

public class VillagerWorker : Villager
{
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private MapManager mapManager;

    [SerializeField] private float workSpeed = 10f;
    [SerializeField] private int buildRange = 1;

    private Task currentTask;
    private Coroutine workRoutine;

    protected void Awake()
    {
        base.Awake();
        taskManager = UnityResolver.Resolve(taskManager, this, nameof(TaskManager));
        mapManager = UnityResolver.Resolve(mapManager, this, nameof(MapManager));
    }

    private void Update()
    {
        base.Update();
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

        MoveTo(currentTask.WorkPosition, OnArrived);
    }

    public void OnArrived()
    {
        if (currentTask == null)
            return;

        if (currentTask.Type == TaskType.Build) 
        {

            Vector3Int workerPosition = new Vector3Int((int)this.transform.position.x, (int)this.transform.position.y);

            if (!IsInBuildRange(workerPosition, currentTask.TargetPosition))
            {
                Debug.LogWarning("Construction site is out of build range.");
                return;
            }

            Structure structure = mapManager.GetStructure(
                currentTask.TargetPosition,
                currentTask.StructureToBuild.Layer
            );

            if (structure is ConstructionSite site)
            {
                animator.SetBool("IsWorking", true);
                workRoutine = StartCoroutine(WorkOnConstruction(site));
            }
        }
    }

    private bool IsInBuildRange(Vector3Int workerPosition, Vector3Int targetPosition)
    {
        int dx = Mathf.Abs(workerPosition.x - targetPosition.x);
        int dy = Mathf.Abs(workerPosition.y - targetPosition.y);

        return Mathf.Max(dx, dy) <= buildRange;
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
        animator.SetBool("IsWorking", false);

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

    public void SetBuildRangeForTests(int range)
    {
        buildRange = range;
    }

    public bool IsInBuildRangeForTests(Vector3Int workerPosition, Vector3Int targetPosition)
    {
        return IsInBuildRange(workerPosition, targetPosition);
    }
#endif
}