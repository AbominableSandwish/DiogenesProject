using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private int maxRequestsPerFrame = 2;

    private readonly Queue<PathRequest> requests = new();

    private void Update()
    {
        int count = 0;

        while (requests.Count > 0 && count < maxRequestsPerFrame)
        {
            PathRequest request = requests.Dequeue();

            List<Vector3Int> path = request.Pathfinder.FindPath(
                request.Start,
                request.End
            );

            request.Callback?.Invoke(path);

            count++;
        }
    }

    public void RequestPath(
        Pathfinder pathfinder,
        Vector3Int start,
        Vector3Int end,
        Action<List<Vector3Int>> callback)
    {
        requests.Enqueue(new PathRequest(pathfinder, start, end, callback));
    }

    private readonly struct PathRequest
    {
        public readonly Pathfinder Pathfinder;
        public readonly Vector3Int Start;
        public readonly Vector3Int End;
        public readonly Action<List<Vector3Int>> Callback;

        public PathRequest(
            Pathfinder pathfinder,
            Vector3Int start,
            Vector3Int end,
            Action<List<Vector3Int>> callback)
        {
            Pathfinder = pathfinder;
            Start = start;
            End = end;
            Callback = callback;
        }
    }
}