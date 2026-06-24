/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VillagerWorker : Villager
{
    [SerializeField] private TaskManager taskManager;
    [SerializeField] private MapManager mapManager;

    [SerializeField] private float workSpeed = 10f;
    [SerializeField] private int buildRange = 1;

    private VillagerTask currentTask;
    private Coroutine workRoutine;

    private readonly HashSet<VillagerTask> _failedTasksThisSearch = new();

    [SerializeField] private float taskSearchCooldown = 1f;
    private float _nextTaskSearchTime;

    private VillagerTask rememberedTask;

    protected void Awake()
    {
        base.Awake();
        taskManager = UnityResolver.Resolve(taskManager, this, nameof(TaskManager));
        mapManager = UnityResolver.Resolve(mapManager, this, nameof(MapManager));

        agent.OnMoveFailed += HandleMoveFailed;
    }

    private void Update()
    {
        base.Update();
        switch (CurrentActivity)
        {
            case VillagerActivity.Work:
                if (currentState == State.Sleeping)
                    currentState = State.Idle;

                if (currentState == State.Idle)
                {
                    if (currentTask != null)
                        return;
                    if (TryResumeRememberedTask())
                        return;
                    TryFindNewTask();
                }

                break;

            case VillagerActivity.Leisure:
                if (currentState == State.Working || currentState == State.Moving)
                    StopWorking();
                break;

            case VillagerActivity.Sleep:
                if (currentState != State.Sleeping)
                    GoSleep();
                
                break;
        }
    }

    private void StopWorking()
    {
        if (workRoutine != null)
        {
            StopCoroutine(workRoutine);
            workRoutine = null;
        }

        if (currentTask != null)
        {
            if (!currentTask.IsCompleted)
                rememberedTask = currentTask;

            currentTask.ReleaseWorkPosition(this);
            currentTask.Release(this);

            Structure structure = mapManager.GetStructure(
                currentTask.TargetPosition,
                currentTask.StructureToBuild.Layer
            );

            if (structure is ConstructionSite site)
            {
                site.SetBeingWorked(false);
            }

            currentTask = null;
        }

        currentState = State.Idle;
    }
    private void GoSleep()
    {
        StopWorking();

        currentState = State.Sleeping;

        animator.SetFloat("Velocity", 0f);
    }

    private bool TryResumeRememberedTask()
    {
        if (rememberedTask == null)
            return false;

        if (rememberedTask.IsCompleted)
        {
            rememberedTask = null;
            return false;
        }

        //if (!rememberedTask.IsRetryAllowed)
        //    return false;

        if (!rememberedTask.CanAssignWorker())
            return false;

        VillagerTask taskToResume = rememberedTask;
        rememberedTask = null;

        TryAssignTask(taskToResume);
        return true;
    }

    private void TryFindNewTask()
    {
        if (Time.time < _nextTaskSearchTime)
            return;

        _nextTaskSearchTime = Time.time + taskSearchCooldown;

        _failedTasksThisSearch.Clear();

        while (true)
        {
            VillagerTask task = taskManager.GetBestTask(new Vector3Int((int)transform.position.x, (int)transform.position.y), _failedTasksThisSearch);

            if (task == null)
            {
                currentState = State.Idle;
                return;
            }

            bool assigned = TryAssignTask(task);

            if (assigned)
                return;

            _failedTasksThisSearch.Add(task);
        }
    }
    private void AbandonCurrentTask()
    {
        if (currentTask != null)
        {
            currentTask.ReleaseWorkPosition(this);
            currentTask.Release(this);

            Structure structure = mapManager.GetStructure(
                currentTask.TargetPosition,
                currentTask.StructureToBuild.Layer
            );

            if (structure is ConstructionSite site)
            {
                site.SetBeingWorked(false);
            }

            currentTask = null;
        }

        currentState = State.Idle;
    }

    private bool TryAssignTask(VillagerTask task)
    {
        if (task == null || !task.CanAssignWorker())
            return false;
    
        currentTask = task;
        currentTask.AssignTo(this);

        if (transform.position == currentTask.TargetPosition ||
        IsInBuildRange(new Vector3Int((int)transform.position.x, (int)transform.position.y), currentTask.TargetPosition))
        {
            OnArrived();
            return true;
        }

        if (!currentTask.TryReserveWorkPosition(
        this,
        currentTask.TargetPosition,
        buildRange,
        mapManager,
        out Vector3Int workPosition))
        {
            AbandonCurrentTask();
            TryFindNewTask();
            return false;
        }

        if (!TryMoveToTask(workPosition, OnArrived))
        {
            currentTask.MarkTemporaryUnreachable(3f);
            AbandonCurrentTask();
            return false;
        }

        return true;
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
                animator?.SetBool("IsWorking", true);
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
            site.SetBeingWorked(true);
            yield return null;
        }

        if (currentTask != null && site.IsCompleted)
        {
            CompleteCurrentTask();
            site.SetBeingWorked(false);
        }
    }

    private void CompleteCurrentTask()
    {
        if (currentTask != null)
        {
            animator.SetBool("IsWorking", false);
            taskManager.CompleteTask(currentTask);

            currentTask.ReleaseWorkPosition(this);
            currentTask.Release(this);
            currentTask = null;
              workRoutine = null;
        }

        currentState = State.Idle;
    }

    private void HandleMoveFailed()
    {
        if (currentTask == null)
            return;

        currentTask.MarkTemporaryUnreachable(5f);
        AbandonCurrentTask();
    }

    #region Test
#if UNITY_INCLUDE_TESTS
    public void InitForTests(MapManager manager)
    {
        mapManager = manager;
    }

    public void SetCurrentTaskForTests(VillagerTask task)
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

    public void SetCurrentGridPositionForTests(Vector3Int position)
    {
        transform.position = position; // adapte au nom réel de ta variable
    }
#endif
    #endregion
}