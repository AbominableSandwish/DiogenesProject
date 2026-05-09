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

    private MapManager mapManager; 
    private Pathfinder pathFinder;

    [SetUp]
    public void SetUp()
    {
        GameObject go = new GameObject("MapManager");
        mapManager = go.AddComponent<MapManager>();

         go = new GameObject("PathFinder");
        pathFinder = go.AddComponent<Pathfinder>();

        // À adapter selon ton init réelle
        mapManager.InitForTests();
    }

    [Test]
    public void Path_Exists_OnFlatGround()
    {
        mapManager.InitForTests(5, 5);
        pathFinder.Init(mapManager);

        List<Vector3Int> path = pathFinder.FindPath(
            new Vector3Int(0, 0, 0),
            new Vector3Int(4, 0, 0)
        );

        Assert.IsNotNull(path);
        Assert.Greater(path.Count, 0);
    }

    [Test]
    public void Path_IsBlocked_ByObstacle()
    {
        mapManager.InitForTests(3, 1);
        mapManager.SetBlocked(new Vector3Int(1, 0, 0));
        pathFinder.Init(mapManager);

        List<Vector3Int> path = pathFinder.FindPath(
            new Vector3Int(0, 0, 0),
            new Vector3Int(2, 0, 0)
        );

        Assert.IsNull(path);
    }

    //[Test]
    //public void Path_CanGoAroundObstacle()
    //{
    //    mapManager.InitForTests(3, 3);
    //    mapManager.SetBlocked(new Vector3Int(1, 1));

    //    mapManager.AddStructure(new Ground(), new Vector3Int(0, 0, 0));  // support pour (0,1)
    //    mapManager.AddStructure(new Ground(), new Vector3Int(0, -1, 0)); // support pour (0,0)
    //    mapManager.AddStructure(new Ground(), new Vector3Int(1, -1, 0)); // support pour (1,0)
    //    mapManager.AddStructure(new Ground(), new Vector3Int(2, -1, 0)); // support pour (2,0)
    //    mapManager.AddStructure(new Ground(), new Vector3Int(2, 0, 0));  // support pour (2,1)

    //    pathFinder.Init(mapManager);

    //    List<Vector3Int> path = pathFinder.FindPath(
    //        new Vector3Int(0, 1),
    //        new Vector3Int(2, 1)
    //    );

    //    Assert.IsNotNull(path);
    //    Assert.Greater(path.Count, 0);
    //    Assert.IsFalse(path.Contains(new Vector3Int(1, 1)));
    //}
}
#endif