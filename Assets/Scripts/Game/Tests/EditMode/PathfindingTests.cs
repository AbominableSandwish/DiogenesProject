#if UNITY_INCLUDE_TESTS
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class TestGrid
{
    private HashSet<Vector3Int> _blocked = new();

    private int _width;
    private int _height;

    public TestGrid(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public void SetBlocked(Vector3Int pos)
    {
        _blocked.Add(pos);
    }

    public bool IsWalkable(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= _width) return false;
        if (pos.y < 0 || pos.y >= _height) return false;

        return !_blocked.Contains(pos);
    }
}

public class PathfindingTests
{
    [Test]
    public void Path_Exists_OnFlatGround()
    {
        MapManager grid = new MapManager(5, 5);
        Pathfinder pathfinder = new Pathfinder(grid);

        List<Vector3Int> path = pathfinder.FindPath(
            new Vector3Int(0, 0, 0),
            new Vector3Int(4, 0, 0)
        );

        Assert.IsNotNull(path);
        Assert.Greater(path.Count, 0);
    }

    [Test]
    public void Path_IsBlocked_ByObstacle()
    {
        MapManager grid = new MapManager(3, 1);
        grid.SetBlocked(new Vector3Int(1, 0, 0));

        Pathfinder pathfinder = new Pathfinder(grid);

        List<Vector3Int> path = pathfinder.FindPath(
            new Vector3Int(0, 0, 0),
            new Vector3Int(2, 0, 0)
        );

        Assert.IsNull(path);
    }

    [Test]
    public void Path_CanGoAroundObstacle()
    {
        MapManager grid = new MapManager(3, 3);
        grid.SetBlocked(new Vector3Int(1, 1, 0));

        Pathfinder pathfinder = new Pathfinder(grid);

        List<Vector3Int> path = pathfinder.FindPath(
            new Vector3Int(0, 1, 0),
            new Vector3Int(2, 1, 0)
        );

        Assert.IsNotNull(path);
        Assert.Greater(path.Count, 0);
        Assert.IsFalse(path.Contains(new Vector3Int(1, 1, 0)));
    }
}
#endif