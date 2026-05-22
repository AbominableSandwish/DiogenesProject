using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    //Debug
    [SerializeField] private bool debugPaths = true;
    [SerializeField] private float debugDuration = 2f;

    private readonly List<PathDebugData> debugPathsData = new();
    public IReadOnlyList<PathDebugData> DebugPaths => debugPathsData;

    //Params
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

            if (debugPaths)
            {
                debugPathsData.Add(new PathDebugData
                {
                    Start = request.Start,
                    End = request.End,
                    Path = path,
                    Success = path != null && path.Count > 0,
                    ExpireTime = Time.time + debugDuration
                });
            }
        }

        debugPathsData.RemoveAll(d => Time.time >= d.ExpireTime);
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