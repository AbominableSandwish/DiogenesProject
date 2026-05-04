using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VillagerWorkerConstructionTests
{
    private MapManager mapManager;
    private VillagerWorker worker;

    [SetUp]
    public void SetUp()
    {
        GameObject mapGo = new GameObject("MapManager");
        mapManager = mapGo.AddComponent<MapManager>();
        mapManager.InitForTests();

        GameObject workerGo = new GameObject("VillagerWorker");
        worker = workerGo.AddComponent<VillagerWorker>();

        worker.InitForTests(mapManager);
    }

    [Test]
    public IEnumerator OnArrived_WithConstructionSite_ShouldIncreaseProgress()
    {
        Vector3Int position = new Vector3Int(2, 2, 0);

        Structure targetStructure = new FakeStructure();
        ConstructionSite site = new ConstructionSite(targetStructure);

        mapManager.AddStructure(site, position);

        Task buildTask = new Task
        {
            Type = TaskType.Build,
            TargetPosition = position,
            StructureToBuild = targetStructure
        };

        worker.SetCurrentTaskForTests(buildTask);

        worker.OnArrived();

        yield return new WaitForSeconds(0.2f);

        Assert.Greater(site.Progress, 0f);
    }

    [Test]
    public void IsInBuildRange_Should_ReturnTrue_WhenTargetIsAdjacent()
    {
        worker.SetBuildRangeForTests(1);

        Vector3Int workerPosition = new Vector3Int(2, 2, 0);
        Vector3Int targetPosition = new Vector3Int(3, 2, 0);

        Assert.IsTrue(worker.IsInBuildRangeForTests(workerPosition, targetPosition));
    }
}