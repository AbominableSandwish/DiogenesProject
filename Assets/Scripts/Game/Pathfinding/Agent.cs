/*
 * Copyright (c) 2026 Dylan von Arx
 * Licensed under CC BY-NC 4.0
 *
 * You may use, modify, and share this code for non-commercial purposes only.
 */

using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private float moveSpeed = 3f;

    private List<Vector3Int> path;
    private int currentIndex;
    private Vector3Int target;
    private System.Action _onMoveFinished;

    private void Awake()
    {
        pathfinder = UnityResolver.Resolve(pathfinder, this, nameof(Pathfinder));
    }

    public void SetTarget(Vector3Int newTarget)
    {
        target = newTarget;

        Vector3Int currentPosition = GetCurrentGridPosition();

        path = pathfinder.FindPath(currentPosition, target);
        currentIndex = 0;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"No path found from {currentPosition} to {target}", this);
        }
    }

    public void MoveTo(Vector3Int newTarget, System.Action onFinished = null)
    {
        _onMoveFinished = onFinished;

        Vector3Int currentPosition = GetCurrentGridPosition();

        path = pathfinder.FindPath(currentPosition, newTarget);
        currentIndex = 0;

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"No path found from {currentPosition} to {newTarget}", this);
            FinishMove();
        }
    }

    private void FinishMove()
    {
        path = null;
        currentIndex = 0;

        _onMoveFinished?.Invoke();
        _onMoveFinished = null;
    }

    private void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (path == null)
            return;

        if (currentIndex >= path.Count)
        {
            FinishMove();
            return;
        }

        Vector3 targetWorld = new Vector3(
            path[currentIndex].x,
            path[currentIndex].y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorld,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetWorld) < 0.05f)
        {
            transform.position = targetWorld;
            currentIndex++;
        }
    }

    private Vector3Int GetCurrentGridPosition()
    {
        return new Vector3Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            0
        );
    }
}